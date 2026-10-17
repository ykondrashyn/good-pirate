public class Gate : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform leftDoor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform rightDoor;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.Collider doorCollider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] openedObjects;

	[global::UnityEngine.SerializeField]
	private int direction;

	private global::System.Collections.Generic.List<ShipBase> spawnEnemyList;

	public GateStatus gateStatus;

	private global::UnityEngine.GameObject GateShipObjectSrc;

	private float disappearCombatDist = 9025f;

	private float startCombatDist = 8100f;

	public int AliveEnemyCount => spawnEnemyList.Count;

	public int TotalEnemyCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < gateStatus.enemySpawnList.Count; i++)
			{
				num += gateStatus.enemySpawnList[i].count;
			}
			return num;
		}
	}

	public int Direction
	{
		get
		{
			return direction;
		}
		private set
		{
			direction = value;
		}
	}

	private void Awake()
	{
		GateShipObjectSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/GateShipObject");
	}

	public void SetDirection(int dir)
	{
		Direction = dir;
	}

	public void SetDoor(bool isOpen)
	{
		global::UnityEngine.GameObject[] array = openedObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(isOpen);
		}
		if (isOpen)
		{
			leftDoor.localRotation = global::UnityEngine.Quaternion.Euler(0f, 130f, 0f);
			rightDoor.localRotation = global::UnityEngine.Quaternion.Euler(0f, 50f, 0f);
			doorCollider.enabled = true;
		}
		else
		{
			leftDoor.localRotation = global::UnityEngine.Quaternion.identity;
			rightDoor.localRotation = global::UnityEngine.Quaternion.Euler(0f, 180f, 0f);
			doorCollider.enabled = false;
			EnemySpawn();
			StartCoroutine(NeerCheck());
		}
	}

	public void Open()
	{
		AudioManager.Instance.PlaySFX("opendoor");
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(leftDoor, new global::UnityEngine.Vector3(0f, 130f, 0f), 5f), global::DG.Tweening.Ease.Linear);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(rightDoor, new global::UnityEngine.Vector3(0f, 50f, 0f), 5f), global::DG.Tweening.Ease.Linear), delegate
		{
			global::UnityEngine.MonoBehaviour.print("문 다 열렸을때 철컥 비슷한소리");
			doorCollider.enabled = true;
			global::UnityEngine.GameObject[] array = openedObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		});
	}

	private global::System.Collections.IEnumerator NeerCheck()
	{
		if (spawnEnemyList.Count <= 0)
		{
			yield break;
		}
		GetComponentInChildren<global::UnityEngine.MeshRenderer>();
		UI_GateEnemyStatus gateStatus = UI_Manager.Instance.ui_mainGame.ui_gateEnemyStatus;
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.3f);
		while (true)
		{
			ShipBase player = GameManager.Instance.playerShip;
			if (player == null)
			{
				yield return null;
				continue;
			}
			if ((player.transform.position - base.transform.position).sqrMagnitude < startCombatDist && !gateStatus.gameObject.activeSelf)
			{
				gateStatus.gameObject.SetActive(value: true);
				gateStatus.SetGate(this);
				while (true)
				{
					if (player == null)
					{
						yield return null;
						continue;
					}
					if ((player.transform.position - base.transform.position).sqrMagnitude > disappearCombatDist)
					{
						break;
					}
					yield return wait;
				}
				gateStatus.gameObject.SetActive(value: false);
			}
			yield return wait;
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider col)
	{
		ShipController componentInParent = col.GetComponentInParent<ShipController>();
		if (componentInParent != null)
		{
			componentInParent.SetAutoDriveMode(isActive: false);
			PlayerInfo.Instance.myData.destinationIslandId = -1;
			GameManager.Instance.playerShipController.SetDestination(null);
			GameManager.Instance.worldManager.RefreshDestination(null);
			PlayerInfo.Instance.SaveData();
			if (UI_Manager.Instance.ui_worldMap.gameObject.activeSelf)
			{
				UI_Manager.Instance.WorldMapSetActive(active: false);
			}
			doorCollider.enabled = false;
			EnemySpawnManager.Instance.DestroyAllEnemies();
			UI_Manager.Instance.TouchPanelActive(active: false);
			UI_Manager.Instance.SetTouchBlock(isActive: true);
			EnemySpawnManager.Instance.isSpawnPause = true;
			componentInParent.StopNearIslandCheck();
			GameManager.Instance.playerShip.isBoundaryNoUse = true;
			GameManager.Instance.playerShip.rigidbody.velocity = global::UnityEngine.Vector3.zero;
			GameManager.Instance.playerShip.SetTargetAngle(-999f);
			global::DG.Tweening.ShortcutExtensions.DOMove(componentInParent.transform, base.transform.position - base.transform.forward * -2f, 2.5f);
			global::DG.Tweening.ShortcutExtensions.DORotate(componentInParent.transform, new global::UnityEngine.Vector3(0f, base.transform.eulerAngles.y, 0f), 2.5f);
			FollowTarget followTarget = GameManager.Instance.followTarget;
			followTarget.isPause = true;
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOMove(followTarget.transform, base.transform.position, 3f), global::DG.Tweening.Ease.InOutQuad);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DORotate(followTarget.transform, new global::UnityEngine.Vector3(0f, base.transform.eulerAngles.y, 0f), 3f), global::DG.Tweening.Ease.InOutQuad), delegate
			{
				EnterPlayerBefore();
				StartCoroutine(EnterPlayer());
			});
		}
	}

	private void EnterPlayerBefore()
	{
		_ = GameManager.Instance.playerShipController;
		FollowTarget followTarget = GameManager.Instance.followTarget;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalMove(followTarget.cameraPivot, new global::UnityEngine.Vector3(0f, 130f, -661f), 6f), global::DG.Tweening.Ease.InOutCubic);
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(followTarget.cameraPivot, new global::UnityEngine.Vector3(10f, 0f, 0f), 6f), global::DG.Tweening.Ease.InOutCubic);
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOOrthoSize(global::UnityEngine.Camera.main, 32f, 6f), global::DG.Tweening.Ease.InOutCubic);
	}

	public void TryEnemyRespawn()
	{
		if (spawnEnemyList == null || spawnEnemyList.Count <= 0)
		{
			return;
		}
		foreach (ShipBase spawnEnemy in spawnEnemyList)
		{
			if (spawnEnemy != null)
			{
				global::UnityEngine.Object.Destroy(spawnEnemy.gameObject);
			}
		}
		spawnEnemyList.Clear();
		EnemySpawn();
	}

	private global::System.Collections.IEnumerator EnterPlayer()
	{
		global::UnityEngine.WaitForFixedUpdate waitFixed = new global::UnityEngine.WaitForFixedUpdate();
		ShipBase player = GameManager.Instance.playerShip;
		_ = GameManager.Instance.playerShipController;
		global::UnityEngine.Vector3 startPos = player.transform.position;
		while (true)
		{
			player.Acceleration(6f);
			if ((startPos - player.transform.position).magnitude > 40f)
			{
				break;
			}
			yield return waitFixed;
		}
		UI_Manager.Instance.BlackScreenOn(2f, delegate
		{
			GameManager.Instance.LoadWorld(direction, gateStatus.worldNum);
		});
	}

	private void EnemySpawn()
	{
		spawnEnemyList = new global::System.Collections.Generic.List<ShipBase>();
		ShipEnemyController shipEnemyController = null;
		float num = 20f;
		int enemyLevel = gateStatus.enemyLevel;
		global::System.Collections.Generic.List<IslandSpawnData> enemySpawnList = gateStatus.enemySpawnList;
		for (int i = 0; i < gateStatus.enemySpawnList.Count; i++)
		{
			global::UnityEngine.Vector3 vector = base.transform.position + base.transform.forward * -25f;
			vector.y = 0f;
			if (enemySpawnList[i].shipType == EShipType.Main)
			{
				ShipBase shipBase = EnemySpawnManager.Instance.Spawn(enemySpawnList[i].shipType, enemyLevel);
				shipEnemyController = shipBase.GetComponent<ShipEnemyController>();
				shipEnemyController.SetGoBackPlace(vector);
				shipBase.transform.position = vector;
				shipBase.transform.SetParent(base.transform);
				shipBase.transform.forward = -base.transform.forward;
				shipBase.SetBaseData(ShipDataManager.Instance.GetShipBaseData(enemySpawnList[i].shipType));
				shipBase.SetLevel(enemyLevel);
				shipBase.checkDelegate = (ShipBase.EnemyIslandRemoveCheck)global::System.Delegate.Combine(shipBase.checkDelegate, new ShipBase.EnemyIslandRemoveCheck(RemoveEnemy));
				global::UnityEngine.Object.Instantiate(GateShipObjectSrc, shipBase.transform);
				spawnEnemyList.Add(shipBase);
				continue;
			}
			if (shipEnemyController != null)
			{
				shipEnemyController.SpawnSubShip(enemySpawnList[i].count, enemySpawnList[i].shipType, delegate(ShipBase shipbase)
				{
					spawnEnemyList.Add(shipbase);
					shipbase.checkDelegate = (ShipBase.EnemyIslandRemoveCheck)global::System.Delegate.Combine(shipbase.checkDelegate, new ShipBase.EnemyIslandRemoveCheck(RemoveEnemy));
					global::UnityEngine.Object.Instantiate(GateShipObjectSrc, shipbase.transform);
				});
				continue;
			}
			for (int num2 = 0; num2 < enemySpawnList[i].count; num2++)
			{
				ShipBase shipBase2 = EnemySpawnManager.Instance.Spawn(enemySpawnList[i].shipType, enemyLevel);
				shipBase2.transform.position = vector;
				shipBase2.transform.forward = -base.transform.forward;
				shipBase2.transform.SetParent(base.transform);
				shipBase2.SetBaseData(ShipDataManager.Instance.GetShipBaseData(enemySpawnList[i].shipType));
				shipBase2.SetLevel(enemyLevel);
				shipBase2.checkDelegate = (ShipBase.EnemyIslandRemoveCheck)global::System.Delegate.Combine(shipBase2.checkDelegate, new ShipBase.EnemyIslandRemoveCheck(RemoveEnemy));
				global::UnityEngine.Object.Instantiate(GateShipObjectSrc, shipBase2.transform);
				global::UnityEngine.Transform obj = new global::UnityEngine.GameObject().transform;
				obj.parent = base.transform;
				obj.position = vector;
				shipBase2.GetComponent<ShipEnemyController>().SetGoBackPlace(vector);
				vector = base.transform.position + base.transform.forward * -25f + new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(0f - num, num), 0f, global::UnityEngine.Random.Range(0f - num, num));
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
		if (UI_Manager.Instance.ui_mainGame.ui_gateEnemyStatus.gameObject.activeSelf)
		{
			UI_Manager.Instance.ui_mainGame.ui_gateEnemyStatus.TextRefresh();
		}
		if (spawnEnemyList.Count > 0)
		{
			return;
		}
		WorldData selectedWorldData = GameManager.Instance.worldManager.selectedWorldData;
		PlayerInfo.Instance.SetClearDoor(selectedWorldData.cubeNum, selectedWorldData.worldNum, Direction);
		for (int i = 1; i <= 6; i++)
		{
			if (i == selectedWorldData.worldNum)
			{
				continue;
			}
			WorldData worldData = GameManager.Instance.worldMaker.GetWorldData(1, i);
			for (int j = 0; j < worldData.gateList.Count; j++)
			{
				if (gateStatus.worldNum == worldData.gateList[j].worldNum)
				{
					PlayerInfo.Instance.SetClearDoor(1, i, j);
				}
			}
		}
		WorldData worldData2 = GameManager.Instance.worldMaker.GetWorldData(1, gateStatus.worldNum);
		for (int k = 0; k < 4; k++)
		{
			if (selectedWorldData.worldNum == worldData2.gateList[k].worldNum)
			{
				PlayerInfo.Instance.SetClearDoor(selectedWorldData.cubeNum, gateStatus.worldNum, k);
				break;
			}
		}
		Open();
	}
}
