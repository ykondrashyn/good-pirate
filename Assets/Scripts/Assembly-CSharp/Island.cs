public class Island : global::UnityEngine.MonoBehaviour
{
	private DockingPos[] dockingPos;

	private global::UnityEngine.GameObject dockingSrc;

	public IslandStatus status;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipBase> spawnEnemyList;

	private global::UnityEngine.GameObject islandSignSrc;

	public Docking myDock { get; private set; }

	public int GetCurrentEnemyCount
	{
		get
		{
			if (spawnEnemyList == null)
			{
				return 0;
			}
			return spawnEnemyList.Count;
		}
	}

	private void Awake()
	{
		dockingPos = GetComponentsInChildren<DockingPos>();
		dockingSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Docking");
		islandSignSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/IslandShipObject");
	}

	private void OnEnable()
	{
		if (myDock != null && PlayerInfo.Instance.GetIsClearIsland(status.islandId))
		{
			myDock.DockingActive(active: true);
		}
	}

	public void SetIslandData(IslandStatus data)
	{
		status = data;
		base.transform.position = data.islandPosition;
		base.transform.rotation = data.islandRotation;
		if (dockingPos == null)
		{
			dockingPos = GetComponentsInChildren<DockingPos>();
		}
		if (dockingSrc == null)
		{
			dockingSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Docking");
		}
		if (dockingPos.Length != 0 && status.selectedDockNumber != -1)
		{
			int num = global::UnityEngine.Mathf.Clamp(status.selectedDockNumber, 0, dockingPos.Length - 1);
			if (dockingSrc != null)
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(dockingSrc, dockingPos[num].transform);
				myDock = gameObject.GetComponent<Docking>();
				myDock.myIsland = this;
				global::UnityEngine.Transform obj = gameObject.transform;
				obj.localRotation = global::UnityEngine.Quaternion.identity;
				obj.localPosition = global::UnityEngine.Vector3.zero;
			}
		}
		if (!(myDock == null) && !(GameManager.Instance == null))
		{
			spawnEnemyList = new global::System.Collections.Generic.List<ShipBase>();
			if (!PlayerInfo.Instance.GetIsClearIsland(status.islandId))
			{
				myDock.DockingActive(active: false);
				Spawn();
			}
			else
			{
				myDock.DockingActive(active: true);
			}
		}
	}

	public void TryRespawn()
	{
		if (myDock == null || GameManager.Instance == null || PlayerInfo.Instance.GetIsClearIsland(status.islandId))
		{
			return;
		}
		int num = 0;
		foreach (IslandSpawnData spawn in status.spawnList)
		{
			num += spawn.count;
		}
		if (num != spawnEnemyList.Count)
		{
			AllEnemiesDestroy();
			Spawn();
		}
	}

	private void AllEnemiesDestroy()
	{
		for (int i = 0; i < spawnEnemyList.Count; i++)
		{
			global::UnityEngine.Object.Destroy(spawnEnemyList[i].gameObject);
		}
		spawnEnemyList.Clear();
	}

	private void Spawn()
	{
		ShipEnemyController shipEnemyController = null;
		float num = 25f;
		int level = EnemySpawnManager.Instance.islandSheet.dataArray[status.islandId].Enemylevelmax;
		for (int i = 0; i < status.spawnList.Count; i++)
		{
			global::UnityEngine.Vector3 vector = myDock.transform.position + myDock.transform.forward * -55f + new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(0f - num, num), 0f, global::UnityEngine.Random.Range(0f - num, num));
			vector.y = 0f;
			if (status.spawnList[i].shipType == EShipType.Main)
			{
				ShipBase shipBase = EnemySpawnManager.Instance.Spawn(status.spawnList[i].shipType, level);
				shipEnemyController = shipBase.GetComponent<ShipEnemyController>();
				shipEnemyController.SetGoBackPlace(vector);
				shipBase.transform.position = vector;
				shipBase.transform.SetParent(base.transform);
				shipBase.transform.forward = -myDock.transform.forward;
				shipBase.SetBaseData(ShipDataManager.Instance.GetShipBaseData(status.spawnList[i].shipType));
				shipBase.SetLevel(level);
				shipBase.checkDelegate = (ShipBase.EnemyIslandRemoveCheck)global::System.Delegate.Combine(shipBase.checkDelegate, new ShipBase.EnemyIslandRemoveCheck(RemoveEnemy));
				global::UnityEngine.Object.Instantiate(islandSignSrc, shipBase.transform);
				spawnEnemyList.Add(shipBase);
				continue;
			}
			if (shipEnemyController != null)
			{
				shipEnemyController.SpawnSubShip(status.spawnList[i].count, status.spawnList[i].shipType, delegate(ShipBase shipBase3)
				{
					spawnEnemyList.Add(shipBase3);
					shipBase3.checkDelegate = (ShipBase.EnemyIslandRemoveCheck)global::System.Delegate.Combine(shipBase3.checkDelegate, new ShipBase.EnemyIslandRemoveCheck(RemoveEnemy));
					shipBase3.SetLevel(level);
					global::UnityEngine.Object.Instantiate(islandSignSrc, shipBase3.transform);
				});
				continue;
			}
			for (int num2 = 0; num2 < status.spawnList[i].count; num2++)
			{
				ShipBase shipBase2 = EnemySpawnManager.Instance.Spawn(status.spawnList[i].shipType, level);
				shipBase2.transform.position = vector;
				shipBase2.transform.forward = -myDock.transform.forward;
				shipBase2.transform.SetParent(base.transform);
				shipBase2.SetBaseData(ShipDataManager.Instance.GetShipBaseData(status.spawnList[i].shipType));
				shipBase2.SetLevel(level);
				shipBase2.checkDelegate = (ShipBase.EnemyIslandRemoveCheck)global::System.Delegate.Combine(shipBase2.checkDelegate, new ShipBase.EnemyIslandRemoveCheck(RemoveEnemy));
				global::UnityEngine.Object.Instantiate(islandSignSrc, shipBase2.transform);
				global::UnityEngine.Transform obj = new global::UnityEngine.GameObject().transform;
				obj.parent = base.transform;
				obj.position = vector;
				shipBase2.GetComponent<ShipEnemyController>().SetGoBackPlace(vector);
				vector = myDock.transform.position + myDock.transform.forward * -55f + new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(0f - num, num), 0f, global::UnityEngine.Random.Range(0f - num, num));
				vector.y = 0f;
				spawnEnemyList.Add(shipBase2);
			}
		}
	}

	private void RemoveEnemy(ShipBase ship)
	{
		if (!spawnEnemyList.Contains(ship))
		{
			return;
		}
		spawnEnemyList.Remove(ship);
		if (UI_Manager.Instance.ui_mainGame.ui_islandStatus != null)
		{
			UI_Manager.Instance.ui_mainGame.ui_islandStatus.RefreshLeftEnemyCount();
		}
		if (spawnEnemyList.Count > 0 || GameManager.Instance.playerShip.IsDie)
		{
			return;
		}
		try
		{
			PlayerInfo.Instance.CheckSubShip();
			FlagManager.Instance.CheckIslandFlag();
			GameManager.Instance.AllTutorialOver();
			PlayerInfo.Instance.SetClearIsland(status.islandId);
			myDock.DockingActive(active: true);
			if (UI_Manager.Instance.ui_mainGame.ui_islandStatus != null)
			{
				UI_Manager.Instance.ui_mainGame.ui_islandStatus.gameObject.SetActive(value: false);
			}
			UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.gameObject.SetActive(value: true);
			UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetIslandData(status);
			if (UI_Manager.Instance.ui_worldMap.islandWorldmapDic.ContainsKey(status.islandId))
			{
				UI_Manager.Instance.ui_worldMap.islandWorldmapDic[status.islandId].SetData(this);
			}
			AudioManager.Instance.PlaySFX("island_win");
			global::UnityEngine.Time.timeScale = 0f;
			if (status.nameText.Equals(""))
			{
				Appsflyer.Instance.SendEvent("Island " + (status.islandId + 1));
				GoogleAnalyticsV3.instance.LogEvent("섬 클리어", "Island " + status.islandId, "Level: " + PlayerInfo.Instance.myData.mainShipLevel.ToString(), 1L);
			}
			else
			{
				Appsflyer.Instance.SendEvent(status.nameText);
				GoogleAnalyticsV3.instance.LogEvent("섬 클리어", status.nameText, "Level: " + PlayerInfo.Instance.myData.mainShipLevel.ToString(), 1L);
			}
			Appsflyer.Instance.SendEvent("Island_ClearCount " + PlayerInfo.Instance.ClearIslandCount);
			GoogleAnalyticsV3.instance.LogEvent("섬 클리어 갯수", "섬 클리어 갯수", PlayerInfo.Instance.ClearIslandCount.ToString(), 1L);
		}
		catch (global::System.Exception message)
		{
			global::UnityEngine.Debug.Log(message);
		}
	}
}
