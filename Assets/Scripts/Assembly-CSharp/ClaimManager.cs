public class ClaimManager : global::UnityEngine.MonoBehaviour
{
	public static ClaimManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] claimModeOffBtns;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] claimModeOffCanvasGroups;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] claimModeOffObjects;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] claimModeOnObjects;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] raycastOffImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] darkModeImages;

	[global::UnityEngine.SerializeField]
	private UI_DownMenuDrag openBtn;

	public bool IsPlaying;

	private const int gridSizeX = 3;

	private const int gridSizeZ = 3;

	private GridInfo[,] gridList;

	private global::System.Collections.Generic.List<Health> enemyHealthList = new global::System.Collections.Generic.List<Health>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<ShipBase> allyShipList = new global::System.Collections.Generic.List<ShipBase>();

	private ClaimStatus selectedClaimData;

	private int selectedPlayerNumber;

	private GridInfo spawnGrid;

	private global::System.Collections.Generic.List<GridInfo> spawnEnemyGrid = new global::System.Collections.Generic.List<GridInfo>();

	private global::UnityEngine.Coroutine enemySpawnCoroutine;

	public bool IsGameWin;

	public bool IsGameLose;

	[global::UnityEngine.Header("Resources")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject protectedObjectSrc;

	private global::UnityEngine.GameObject protectedObj;

	[global::UnityEngine.SerializeField]
	private MainShipData mainSheetData;

	private int spectateNumber;

	private global::UnityEngine.Coroutine showMission;

	private global::UnityEngine.Coroutine gameLogic;

	private global::UnityEngine.Coroutine waitVictoryEmoji;

	private bool isEmojiActive;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void SetClaimStatus(ClaimStatus data, int playerNum)
	{
		selectedClaimData = data;
		selectedPlayerNumber = playerNum;
	}

	public void LoadClaimMap()
	{
		UI_Manager.Instance.ui_claim.SetMissionPercent(0f);
		IsPlaying = true;
		if (showMission != null)
		{
			StopCoroutine(showMission);
		}
		if (gameLogic != null)
		{
			StopCoroutine(gameLogic);
		}
		if (waitVictoryEmoji != null)
		{
			StopCoroutine(waitVictoryEmoji);
		}
		IsGameWin = false;
		IsGameLose = false;
		isEmojiActive = false;
		GameManager.Instance.IsShipPause = true;
		GameManager.Instance.worldManager.ClearWorld();
		UI_Manager.Instance.ui_worldMap.ClearWorldMap();
		GameManager.Instance.worldManager.GenerateWorld(2, global::UnityEngine.Random.Range(1, 11));
		GameManager.Instance.PlayerDieAction = PlayerDie;
		WorldData selectedWorldData = GameManager.Instance.worldManager.selectedWorldData;
		EnemySpawnManager.Instance.isSpawnPause = true;
		global::UnityEngine.Application.runInBackground = true;
		UI_Manager.Instance.TouchPanelActive(active: true);
		FollowTarget.Instance.isSmooth = true;
		FollowTarget.Instance.isPause = false;
		FollowTarget.Instance.transform.rotation = global::UnityEngine.Quaternion.Euler(new global::UnityEngine.Vector3(0f, -10f, 0f));
		UI_Manager.Instance.ui_mainGame.UIMode(EUIMode.OutIsland);
		UI_Manager.Instance.ui_mainGame.ui_mainUpgradeMenu.Close();
		UI_Manager.Instance.ui_claim.SetMissitonStatusActive(active: true);
		UI_Manager.Instance.claimWorldMap.SettingShowMission();
		if (UI_Manager.Instance.ui_mainGame.ui_limitShipBtn.IsActive)
		{
			UI_Manager.Instance.ui_mainGame.ui_limitShipBtn.Toggle();
		}
		SetDefaultUIActive(active: false);
		gridList = new GridInfo[3, 3];
		float num = selectedWorldData.worldSizeX / 3;
		float num2 = selectedWorldData.worldSizeZ / 3;
		float num3 = num / 2f;
		float num4 = num2 / 2f;
		float num5 = -selectedWorldData.worldSizeX / 2;
		float num6 = -selectedWorldData.worldSizeZ / 2;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				gridList[i, j] = new GridInfo();
				gridList[i, j].center = new global::UnityEngine.Vector3(num5 + (float)j * num + num3, 0f, num6 + (float)i * num2 + num4);
				gridList[i, j].left = gridList[i, j].center + new global::UnityEngine.Vector3(0f - num3, 0f, 0f);
				gridList[i, j].right = gridList[i, j].center + new global::UnityEngine.Vector3(num3, 0f, 0f);
				gridList[i, j].top = gridList[i, j].center + new global::UnityEngine.Vector3(0f, 0f, 0f - num4);
				gridList[i, j].bottom = gridList[i, j].center + new global::UnityEngine.Vector3(0f, 0f, num4);
			}
		}
		SpawnAlly();
		spawnGrid = gridList[1, 1];
		spawnEnemyGrid.Add(gridList[1, 0]);
		if ((bool)GameManager.Instance.playerShip)
		{
			ShipBase playerShip = GameManager.Instance.playerShip;
			playerShip.transform.position = spawnGrid.GetRandomPos();
			playerShip.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			GameManager.Instance.playerShipController.DestroyAllSubShip();
			playerShip.HPBAR.SetNameTextActive(active: true);
			playerShip.HPBAR.SetNameText("User " + (selectedPlayerNumber + 1));
			GameManager.Instance.playerShipController.AllBirdRemoveForce();
			GameManager.Instance.playerShipController.SetDestination(null);
		}
		showMission = StartCoroutine("ShowMission");
		gameLogic = StartCoroutine("GameLogic");
		waitVictoryEmoji = StartCoroutine("WaitVictoryEmoji");
		switch (selectedClaimData.claimType)
		{
		case eClaimMode.KillAllEnemy:
			UI_Manager.Instance.claimWorldMap.SetMissionContent(LanguageManager.Instance.translate("key_claim_message_killall"));
			SpawnEnemy();
			enemySpawnCoroutine = StartCoroutine(KillAllEnemies());
			break;
		case eClaimMode.KillTheBoss:
			UI_Manager.Instance.claimWorldMap.SetMissionContent(LanguageManager.Instance.translate("key_claim_message_killboss"));
			enemySpawnCoroutine = StartCoroutine(KillTheBoss());
			break;
		case eClaimMode.DefendTheSupplyShip:
			UI_Manager.Instance.claimWorldMap.SetMissionContent(LanguageManager.Instance.translate("key_claim_message_defend"));
			SpawnEnemy();
			enemySpawnCoroutine = StartCoroutine(DefendTheSupply());
			break;
		}
	}

	private global::System.Collections.IEnumerator WaitVictoryEmoji()
	{
		yield return new global::UnityEngine.WaitUntil(() => isEmojiActive);
		foreach (ShipBase allyShip in allyShipList)
		{
			ShipAllyController component = allyShip.GetComponent<ShipAllyController>();
			if (component != null)
			{
				component.VictoryEmoji();
			}
		}
	}

	private void EmojiCheck(float ratio)
	{
		if (!isEmojiActive && ratio >= 0.9f)
		{
			isEmojiActive = true;
		}
	}

	public void PlayerDie()
	{
		UI_Manager.Instance.RealJustBlackOn(1.2f, 1f, delegate
		{
			if (allyShipList.Count > 0)
			{
				int num = global::UnityEngine.Random.Range(0, allyShipList.Count);
				spectateNumber = num;
				FollowTarget.Instance.SetTarget(allyShipList[spectateNumber].transform);
				ShipAllyController component = allyShipList[spectateNumber].GetComponent<ShipAllyController>();
				UI_Manager.Instance.ui_claim.SetSpectateModeActive(active: true);
				UI_Manager.Instance.ui_claim.SetName(component.userName);
			}
			UI_Manager.Instance.RealJustBlackOff(1.2f, 0f);
		});
	}

	private global::System.Collections.IEnumerator SpectatingAllyCheck()
	{
		global::UnityEngine.WaitForSecondsRealtime wait = new global::UnityEngine.WaitForSecondsRealtime(0.3f);
		while (true)
		{
			if (FollowTarget.Instance.TargetTrans == null && allyShipList.Count > spectateNumber)
			{
				FollowTarget.Instance.SetTarget(allyShipList[spectateNumber].transform);
				ShipAllyController component = allyShipList[spectateNumber].GetComponent<ShipAllyController>();
				UI_Manager.Instance.ui_claim.SetName(component.userName);
			}
			yield return wait;
		}
	}

	public void SpectateNextShip()
	{
		spectateNumber++;
		SpectateCheckNum(ref spectateNumber);
		FollowTarget.Instance.SetTarget(allyShipList[spectateNumber].transform);
		ShipAllyController component = allyShipList[spectateNumber].GetComponent<ShipAllyController>();
		UI_Manager.Instance.ui_claim.SetName(component.userName);
	}

	public void SpectatePrevShip()
	{
		spectateNumber--;
		SpectateCheckNum(ref spectateNumber);
		FollowTarget.Instance.SetTarget(allyShipList[spectateNumber].transform);
		ShipAllyController component = allyShipList[spectateNumber].GetComponent<ShipAllyController>();
		UI_Manager.Instance.ui_claim.SetName(component.userName);
	}

	private void SpectateCheckNum(ref int number)
	{
		if (number < 0)
		{
			number = allyShipList.Count - 1;
		}
		if (number >= allyShipList.Count)
		{
			number = 0;
		}
	}

	private global::System.Collections.IEnumerator ShowMission()
	{
		yield return new global::UnityEngine.WaitForSecondsRealtime(3f);
		UI_Manager.Instance.claimWorldMap.SettingMinimapMode();
		yield return new global::UnityEngine.WaitForSecondsRealtime(3f);
		GameManager.Instance.IsInIsland = false;
		UI_Manager.Instance.claimWorldMap.Close();
		yield return new global::UnityEngine.WaitForSecondsRealtime(0.5f);
		GameManager.Instance.IsShipPause = false;
	}

	private global::System.Collections.IEnumerator GameLogic()
	{
		float totalTime = 0f;
		int prevSec = -1;
		int prevMin = -1;
		float timeLimit = selectedClaimData.limitTime;
		bool colorRed = false;
		int num = (int)(timeLimit / 60f);
		int num2 = (int)(timeLimit % 60f);
		string text = $"{num:00}:{num2:00}";
		UI_Manager.Instance.claimWorldMap.SetMissionTime(text);
		UI_Manager.Instance.ui_claim.SetLeftTimeColor(global::UnityEngine.Color.white);
		UI_Manager.Instance.ui_claim.SetLeftTime(text);
		while (true)
		{
			if (GameManager.Instance.IsInIsland || IsGameWin || IsGameLose)
			{
				yield return null;
				continue;
			}
			totalTime += global::UnityEngine.Time.deltaTime;
			float num3 = global::UnityEngine.Mathf.Max(0f, timeLimit - totalTime);
			int num4 = (int)(num3 / 60f);
			int num5 = (int)(num3 % 60f);
			if (!colorRed && selectedClaimData.claimType != eClaimMode.DefendTheSupplyShip && num4 == 0 && num5 <= 30)
			{
				UI_Manager.Instance.ui_claim.SetLeftTimeColor(global::UnityEngine.Color.red);
				colorRed = true;
			}
			string text2 = $"{num4:00}:{num5:00}";
			if (prevMin != num4 || prevSec != num5)
			{
				UI_Manager.Instance.claimWorldMap.SetMissionTime(text2);
				UI_Manager.Instance.ui_claim.SetLeftTime(text2);
			}
			if (selectedClaimData.claimType == eClaimMode.DefendTheSupplyShip)
			{
				float num6 = global::UnityEngine.Mathf.Clamp01(totalTime / timeLimit);
				UI_Manager.Instance.ui_claim.SetMissionPercent(num6);
				EmojiCheck(num6);
			}
			if (num3 <= 0f)
			{
				if (IsGameLose || IsGameWin)
				{
					yield return null;
					continue;
				}
				switch (selectedClaimData.claimType)
				{
				case eClaimMode.KillAllEnemy:
				case eClaimMode.KillTheBoss:
					UI_Manager.Instance.ui_claim.Lose(isTime: true);
					IsGameLose = true;
					yield break;
				case eClaimMode.DefendTheSupplyShip:
					UI_Manager.Instance.ui_claim.Win();
					FlagManager.Instance.CheckIslandClameFlag();
					if (!PlayerInfo.Instance.myData.isHadClaimClear)
					{
						PlayerInfo.Instance.myData.isHadClaimClear = true;
						PlayerInfo.Instance.SaveData();
					}
					PlayerInfo.Instance.SetClearIslandClaim(selectedClaimData.islandId);
					IsGameWin = true;
					yield break;
				}
			}
			if (allyShipList.Count <= 0 && GameManager.Instance.playerShip == null)
			{
				UI_Manager.Instance.ui_claim.Lose();
				IsGameLose = true;
			}
			prevMin = num4;
			prevSec = num5;
			yield return null;
		}
	}

	private global::System.Collections.IEnumerator DefendTheSupply()
	{
		float totalTime = -10f;
		float coolTime = selectedClaimData.spawnIntervalTime;
		new global::UnityEngine.WaitForSecondsRealtime(0.2f);
		protectedObj = global::UnityEngine.Object.Instantiate(protectedObjectSrc, global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.identity);
		Health component = protectedObj.GetComponent<Health>();
		global::UnityEngine.Resources.Load<MainShipData>("Data/MainShipData");
		if (mainSheetData != null)
		{
			int num = selectedClaimData.maxLevel - 1;
			int num2 = (int)((float)mainSheetData.dataArray[num].Health * 1.5f);
			component.SetHealthPoint(num2, num2);
		}
		yield return null;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime > coolTime)
			{
				totalTime = 0f;
				SpawnEnemy();
			}
			if (protectedObj == null && !IsGameWin && !IsGameLose)
			{
				break;
			}
			yield return null;
		}
		IsGameLose = true;
		UI_Manager.Instance.ui_claim.Lose();
	}

	private global::System.Collections.IEnumerator KillTheBoss()
	{
		int maxLevel = selectedClaimData.maxLevel;
		ShipBase shipBase = EnemySpawnManager.Instance.Spawn((selectedClaimData.bossType == 0) ? EShipType.Elite : EShipType.Bomber, maxLevel);
		int index = global::UnityEngine.Random.Range(0, spawnEnemyGrid.Count);
		shipBase.transform.position = spawnEnemyGrid[index].GetRandomPos();
		Health health = shipBase.myHealth;
		int num = health.HP_MAX * 2;
		health.SetHealthPoint(num, num);
		enemyHealthList.Add(health);
		shipBase.targetFinder.SetSize(200f);
		UI_Manager.Instance.ui_claim.SetMissionPercent(0f);
		shipBase.myHealth.AddHitAction(delegate
		{
			float num3 = 1f - shipBase.myHealth.HpRatio;
			UI_Manager.Instance.ui_claim.SetMissionPercent(num3);
			EmojiCheck(num3);
		});
		ShipEnemyController component = shipBase.gameObject.GetComponent<ShipEnemyController>();
		if ((bool)component)
		{
			component.isNeverGoBack = true;
		}
		for (int num2 = 0; num2 < selectedClaimData.bossSubCount; num2++)
		{
			component.SpawnSubShip(1, GetRandomShipType(), delegate(ShipBase subShip)
			{
				enemyHealthList.Add(subShip.myHealth);
				ShipBase shipBase3 = subShip;
				shipBase3.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase3.beforeDestroyAction, (global::System.Action)delegate
				{
					if (enemyHealthList.Contains(subShip.myHealth))
					{
						enemyHealthList.Remove(subShip.myHealth);
					}
				});
			});
		}
		ShipBase shipBase2 = shipBase;
		shipBase2.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase2.beforeDestroyAction, (global::System.Action)delegate
		{
			if (enemyHealthList.Contains(shipBase.myHealth))
			{
				enemyHealthList.Remove(shipBase.myHealth);
			}
		});
		float totalTime = -10f;
		float coolTime = selectedClaimData.spawnIntervalTime;
		while (true)
		{
			if (health == null || health.HpRatio <= 0f)
			{
				if (IsGameWin || IsGameLose)
				{
					break;
				}
				IsGameWin = true;
				UI_Manager.Instance.ui_claim.Win();
				TutorialManager.Instance.StartTutorial("winclaimfirst");
				FlagManager.Instance.CheckIslandClameFlag();
				if (!PlayerInfo.Instance.myData.isHadClaimClear)
				{
					PlayerInfo.Instance.myData.isHadClaimClear = true;
					PlayerInfo.Instance.SaveData();
				}
				PlayerInfo.Instance.SetClearIslandClaim(selectedClaimData.islandId);
			}
			totalTime += global::UnityEngine.Time.deltaTime;
			if (enemyHealthList.Count <= 1 || totalTime > coolTime)
			{
				totalTime = 0f;
				SpawnEnemy();
			}
			yield return null;
		}
	}

	private global::System.Collections.IEnumerator KillAllEnemies()
	{
		int nowSpawnCount = 1;
		float totalTime = -10f;
		float coolTime = selectedClaimData.spawnIntervalTime;
		int maxKillCount = selectedClaimData.spawnGroupCount * selectedClaimData.spawnCount;
		int prevKillCount = -1;
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.2f);
		while (true)
		{
			totalTime += 0.2f;
			int num = (nowSpawnCount - 1) * selectedClaimData.spawnGroupCount + (selectedClaimData.spawnGroupCount - enemyHealthList.Count);
			if (num != prevKillCount)
			{
				float num2 = (float)num / (float)maxKillCount;
				UI_Manager.Instance.ui_claim.SetMissionPercent(num2);
				EmojiCheck(num2);
			}
			prevKillCount = num;
			if (enemyHealthList.Count <= 0 || totalTime > coolTime)
			{
				if (nowSpawnCount < selectedClaimData.spawnCount)
				{
					totalTime = 0f;
					nowSpawnCount++;
					SpawnEnemy();
				}
				else if (nowSpawnCount == selectedClaimData.spawnCount && enemyHealthList.Count <= 0)
				{
					break;
				}
			}
			yield return wait;
		}
		if (!IsGameWin && !IsGameLose)
		{
			IsGameWin = true;
			UI_Manager.Instance.ui_claim.Win();
			FlagManager.Instance.CheckIslandClameFlag();
			if (!PlayerInfo.Instance.myData.isHadClaimClear)
			{
				PlayerInfo.Instance.myData.isHadClaimClear = true;
				PlayerInfo.Instance.SaveData();
			}
			PlayerInfo.Instance.SetClearIslandClaim(selectedClaimData.islandId);
		}
	}

	private void SpawnAlly()
	{
		spawnGrid = null;
		spawnEnemyGrid.Clear();
		switch (selectedClaimData.claimType)
		{
		case eClaimMode.KillTheBoss:
		{
			spawnEnemyGrid.Add(gridList[1, 1]);
			int num = global::UnityEngine.Random.Range(0, 3);
			int num2 = global::UnityEngine.Random.Range(0, 3);
			while (num == 1 && num2 == num)
			{
				num = global::UnityEngine.Random.Range(0, 3);
				num2 = global::UnityEngine.Random.Range(0, 3);
			}
			spawnGrid = gridList[num2, num];
			break;
		}
		case eClaimMode.KillAllEnemy:
		{
			int num3 = global::UnityEngine.Random.Range(0, 3);
			int num4 = global::UnityEngine.Random.Range(0, 3);
			while (num3 == 1 && num4 == num3)
			{
				num3 = global::UnityEngine.Random.Range(0, 3);
				num4 = global::UnityEngine.Random.Range(0, 3);
			}
			switch (num4)
			{
			case 0:
				switch (num3)
				{
				case 0:
					spawnEnemyGrid.Add(gridList[2, 2]);
					break;
				case 1:
					spawnEnemyGrid.Add(gridList[2, global::UnityEngine.Random.Range(0, 3)]);
					break;
				default:
					spawnEnemyGrid.Add(gridList[2, 0]);
					break;
				}
				break;
			case 1:
				if (num3 == 0)
				{
					spawnEnemyGrid.Add(gridList[global::UnityEngine.Random.Range(0, 3), 2]);
				}
				else
				{
					spawnEnemyGrid.Add(gridList[global::UnityEngine.Random.Range(0, 3), 0]);
				}
				break;
			default:
				switch (num3)
				{
				case 0:
					spawnEnemyGrid.Add(gridList[0, 2]);
					break;
				case 1:
					spawnEnemyGrid.Add(gridList[0, global::UnityEngine.Random.Range(0, 3)]);
					break;
				default:
					spawnEnemyGrid.Add(gridList[0, 0]);
					break;
				}
				break;
			}
			spawnGrid = gridList[num4, num3];
			break;
		}
		case eClaimMode.DefendTheSupplyShip:
		{
			spawnGrid = gridList[1, 1];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (i != 1 || j != 1)
					{
						spawnEnemyGrid.Add(gridList[j, i]);
					}
				}
			}
			break;
		}
		}
		if (spawnGrid == null)
		{
			return;
		}
		allyShipList.Clear();
		for (int k = 0; k < selectedClaimData.allyCount - 1; k++)
		{
			ShipBase shipBase = null;
			global::UnityEngine.Vector3 randomPos = spawnGrid.GetRandomPos();
			ShipGenerator.Instance.SpawnAlly(randomPos, global::UnityEngine.Quaternion.Euler(0f, 90f, 0f), out shipBase);
			int num5 = ((global::UnityEngine.Random.value <= 0.6f) ? global::UnityEngine.Mathf.Clamp(selectedClaimData.maxLevel + global::UnityEngine.Random.Range(0, 50), 10, 1000) : ((!(global::UnityEngine.Random.value <= 0.95f)) ? global::UnityEngine.Mathf.Clamp(selectedClaimData.maxLevel + global::UnityEngine.Random.Range(51, 110), 10, 1000) : global::UnityEngine.Mathf.Clamp(selectedClaimData.maxLevel - global::UnityEngine.Random.Range(0, 20), 10, 1000)));
			int level = global::UnityEngine.Mathf.Clamp(num5 / 100 + 1, 1, 10);
			ShipMainData mainData = ShipDataManager.Instance.GetMainData((EShipMain)level);
			ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Main);
			shipBase.SetBaseData(shipBaseData);
			if (global::UnityEngine.Random.value <= 0.3f)
			{
				global::System.Collections.Generic.List<eShipSkin> list = new global::System.Collections.Generic.List<eShipSkin>();
				for (int l = 0; l < 21; l++)
				{
					if (SkinDataManager.Instance.GetSkinData((eShipSkin)l).levelLimit <= num5)
					{
						list.Add((eShipSkin)l);
					}
				}
				int index = global::UnityEngine.Random.Range(0, list.Count);
				ShipSkinData skinData = SkinDataManager.Instance.GetSkinData(list[index]);
				shipBase.SpawnShipModel(skinData.modelSrc, ELayerType.Player);
				CannonManager[] componentsInChildren = shipBase.GetComponentsInChildren<CannonManager>();
				for (int m = 0; m < componentsInChildren.Length; m++)
				{
					componentsInChildren[m].MakeCannon(mainData.cannonCount);
				}
				int num6 = num5 / 100 - skinData.levelLimit / 100;
				shipBase.modelTrans.localScale = global::UnityEngine.Vector3.one + 0.14f * (float)num6 * global::UnityEngine.Vector3.one;
			}
			else
			{
				int num7 = num5 % 100 / 25;
				global::UnityEngine.GameObject gameObject = null;
				switch (num7)
				{
				case 0:
					gameObject = mainData.modelSrc;
					break;
				case 1:
					gameObject = mainData.playerModelType01;
					break;
				case 2:
					gameObject = mainData.playerModelType02;
					break;
				case 3:
					gameObject = mainData.playerModelType03;
					break;
				}
				if (num5 >= ConstValue.ShipLevelMax)
				{
					gameObject = mainData.playerModelType03;
				}
				if (gameObject != null)
				{
					shipBase.SpawnShipModel(gameObject, ELayerType.Player);
				}
				else
				{
					shipBase.SpawnShipModel(mainData.modelSrc, ELayerType.Player);
				}
			}
			int flagId = global::UnityEngine.Random.Range(0, FlagManager.Instance.flagList.Count);
			if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
			{
				Flag[] componentsInChildren2 = shipBase.GetComponentsInChildren<Flag>();
				for (int m = 0; m < componentsInChildren2.Length; m++)
				{
					componentsInChildren2[m].SetFlag(info.flagMaterial);
				}
			}
			shipBase.SetLevel(num5);
			shipBase.targetFinder.SetSize(200f);
			ShipBase shipBase2 = shipBase;
			shipBase2.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase2.beforeDestroyAction, (global::System.Action)delegate
			{
				if (allyShipList.Contains(shipBase))
				{
					allyShipList.Remove(shipBase);
				}
			});
			ShipAllyController component = shipBase.gameObject.GetComponent<ShipAllyController>();
			if ((bool)component)
			{
				component.SetMoveType(eAllyMoveType.SameEnemy);
				component.GoCenter();
			}
			allyShipList.Add(shipBase);
		}
		StartCoroutine(AllyShipSetName());
	}

	private global::System.Collections.IEnumerator AllyShipSetName()
	{
		yield return null;
		for (int i = 0; i < allyShipList.Count; i++)
		{
			ShipBase shipBase = allyShipList[i];
			ShipAllyController component = shipBase.GetComponent<ShipAllyController>();
			shipBase.HPBAR.SetNameTextActive(active: true);
			string text = ((i >= selectedPlayerNumber) ? ("User " + (i + 2)) : ("User " + (i + 1)));
			shipBase.HPBAR.SetNameText(text);
			component.userName = text;
		}
	}

	private EShipType GetRandomShipType()
	{
		float num = 10f;
		float num2 = 15f;
		float num3 = 15f;
		float num4 = 15f;
		float num5 = 15f;
		float num6 = 15f;
		float num7 = global::UnityEngine.Random.Range(0f, 100f);
		if (num7 <= num + num2)
		{
			return EShipType.Normal;
		}
		if (num7 <= num + num2 + num3)
		{
			return EShipType.Rapid;
		}
		if (num7 <= num + num2 + num3 + num4)
		{
			return EShipType.Cannon;
		}
		if (num7 <= num + num2 + num3 + num4 + num5)
		{
			return EShipType.Gas;
		}
		if (num7 <= num + num2 + num3 + num4 + num5 + num6)
		{
			return EShipType.Guide;
		}
		return EShipType.Multi;
	}

	private void SpawnEnemy()
	{
		int index = global::UnityEngine.Random.Range(0, spawnEnemyGrid.Count);
		for (int i = 0; i < selectedClaimData.spawnGroupCount; i++)
		{
			EShipType randomShipType = GetRandomShipType();
			ShipBase shipBase = EnemySpawnManager.Instance.Spawn(randomShipType, selectedClaimData.maxLevel);
			shipBase.transform.position = spawnEnemyGrid[index].GetRandomPos();
			shipBase.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			ShipGenerator.Instance.JustSpawnParticle(shipBase.transform.position);
			shipBase.targetFinder.SetSize(200f);
			ShipEnemyController component = shipBase.gameObject.GetComponent<ShipEnemyController>();
			if ((bool)component)
			{
				component.isNeverGoBack = true;
			}
			ShipBase shipBase2 = shipBase;
			shipBase2.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase2.beforeDestroyAction, (global::System.Action)delegate
			{
				if (enemyHealthList.Contains(shipBase.myHealth))
				{
					enemyHealthList.Remove(shipBase.myHealth);
				}
			});
			enemyHealthList.Add(shipBase.myHealth);
		}
	}

	public void AddEnemyHealth(Health health)
	{
		if (!enemyHealthList.Contains(health))
		{
			enemyHealthList.Add(health);
		}
	}

	private void SetDefaultUIActive(bool active)
	{
		openBtn.isPause = !active;
		global::UnityEngine.UI.Button[] array = claimModeOffBtns;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = active;
		}
		global::UnityEngine.CanvasGroup[] array2 = claimModeOffCanvasGroups;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].alpha = (active ? 1f : 0.3f);
		}
		global::UnityEngine.GameObject[] array3 = claimModeOffObjects;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].SetActive(active);
		}
		array3 = claimModeOnObjects;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].SetActive(!active);
		}
		if (active)
		{
			global::UnityEngine.UI.Graphic[] array4 = darkModeImages;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].color = global::UnityEngine.Color.white;
			}
			global::UnityEngine.UI.Image[] array5 = raycastOffImages;
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i].raycastTarget = true;
			}
		}
		else
		{
			global::UnityEngine.UI.Graphic[] array4 = darkModeImages;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].color = global::UnityEngine.Color.black;
			}
			global::UnityEngine.UI.Image[] array5 = raycastOffImages;
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i].raycastTarget = false;
			}
		}
	}

	private void DestroyAlly()
	{
		foreach (ShipBase allyShip in allyShipList)
		{
			if (allyShip != null)
			{
				global::UnityEngine.Object.Destroy(allyShip.gameObject);
			}
		}
		allyShipList.Clear();
	}

	private void DestroyEnemies()
	{
		foreach (Health enemyHealth in enemyHealthList)
		{
			if (enemyHealth != null && enemyHealth.myShip != null)
			{
				global::UnityEngine.Object.Destroy(enemyHealth.myShip.gameObject);
			}
		}
		enemyHealthList.Clear();
	}

	public void Exit()
	{
		UI_Manager.Instance.BlackScreenOn(0.8f, GiveUp);
	}

	private void GiveUp()
	{
		IsPlaying = false;
		if (enemySpawnCoroutine != null)
		{
			StopCoroutine(enemySpawnCoroutine);
		}
		if (showMission != null)
		{
			StopCoroutine(showMission);
		}
		if (gameLogic != null)
		{
			StopCoroutine(gameLogic);
		}
		if (waitVictoryEmoji != null)
		{
			StopCoroutine(waitVictoryEmoji);
		}
		UI_Manager.Instance.ui_claim.Reset();
		DestroyAlly();
		DestroyEnemies();
		if (protectedObj != null)
		{
			global::UnityEngine.Object.Destroy(protectedObj);
		}
		WorldManager worldManager = GameManager.Instance.worldManager;
		worldManager.ClearWorld();
		UI_Manager.Instance.ui_worldMap.ClearWorldMap();
		worldManager.GenerateWorld(PlayerInfo.Instance.myData.selectedCubeNum, PlayerInfo.Instance.myData.selectedWorldNum);
		UI_PatrolCombatStatus ui_patrolCombatStatus = UI_Manager.Instance.ui_mainGame.ui_patrolCombatStatus;
		if (ui_patrolCombatStatus.gameObject.activeSelf)
		{
			ui_patrolCombatStatus.gameObject.SetActive(value: false);
		}
		MyInventoryDataManager.Instance.SpawnShipClear();
		if ((bool)GameManager.Instance.playerShip)
		{
			ShipBase playerShip = GameManager.Instance.playerShip;
			playerShip.transform.position = spawnGrid.GetRandomPos();
			playerShip.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			GameManager.Instance.playerShipController.DestroyAllSubShip();
			GameManager.Instance.playerShipController.GenerateSubShip();
			playerShip.HPBAR.SetNameTextActive(active: false);
		}
		else
		{
			ShipBase ship = null;
			ShipController shipController = null;
			ShipGenerator.Instance.SpawnMain(global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.Euler(0f, 45f, 0f), out ship, out shipController);
			if (ship != null)
			{
				GameManager.Instance.SetPlayerShip(ship);
			}
			if (shipController != null)
			{
				GameManager.Instance.SetPlayerShipController(shipController);
			}
		}
		ShipBase playerShip2 = GameManager.Instance.playerShip;
		ShipController playerShipController = GameManager.Instance.playerShipController;
		Island islandData = worldManager.GetIslandData(PlayerInfo.Instance.myData.selectedIslandId);
		if (islandData != null)
		{
			GameManager.Instance.InIsland(islandData.myDock);
		}
		playerShipController.BirdCheck();
		SetDefaultUIActive(active: true);
		GameManager.Instance.worldManager.AllActive(active: false);
		GameManager.Instance.worldManager.GetIndex(playerShip2.transform.position, out var indexX, out var indexZ);
		GameManager.Instance.worldManager.Check(indexX, indexZ);
		global::UnityEngine.Application.runInBackground = false;
		GameManager.Instance.SetNormalDieAction();
	}
}
