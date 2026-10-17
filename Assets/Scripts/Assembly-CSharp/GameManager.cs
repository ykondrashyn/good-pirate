public class GameManager : global::UnityEngine.MonoBehaviour
{
	public static GameManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform cameraPivotTrans;

	public FollowTarget followTarget;

	public bool IsInIsland;

	public bool IsBossFighting;

	public Docking anchoredDock;

	public CameraMoveOpenMenu cameraMoveOpenMenu;

	public WorldMaker worldMaker;

	public WorldManager worldManager;

	public WeatherManager weatherManager;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject rootBoxSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject rewardBoxSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject healParticleSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lifeBeadSrc;

	[global::UnityEngine.SerializeField]
	private TaxShip taxShip;

	public bool IsShipPause;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> objectList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	public global::UnityEngine.Events.UnityAction PlayerDieAction;

	[global::UnityEngine.SerializeField]
	private IslandSheetData islandSheet;

	public ShipBase playerShip { get; private set; }

	public ShipController playerShipController { get; private set; }

	private void OnDestroy()
	{
		Instance = null;
	}

	private void OnEnable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Combine(instance.UpgradeCrewAction, new global::System.Action(CheckTimeIsGole));
	}

	private void OnDisable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Remove(instance.UpgradeCrewAction, new global::System.Action(CheckTimeIsGole));
	}

	private void Awake()
	{
		IsInIsland = false;
		Instance = this;
		SetNormalDieAction();
	}

	public void SetNormalDieAction()
	{
		PlayerDieAction = NormalDieAction;
	}

	public void SetPlayerShip(ShipBase ship)
	{
		playerShip = ship;
	}

	public void SetPlayerShipController(ShipController controller)
	{
		playerShipController = controller;
	}

	public void LoadWorld(int gateDirection, int worldNum)
	{
		int worldNum2 = worldManager.selectedWorldData.worldNum;
		worldManager.ClearWorld();
		UI_Manager.Instance.ui_worldMap.ClearWorldMap();
		worldManager.GenerateWorld(1, worldNum);
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (worldNum2 == worldManager.gates[i].gateStatus.worldNum)
			{
				num = i;
				break;
			}
		}
		Gate gate = worldManager.gates[num];
		playerShip.transform.position = gate.transform.position - gate.transform.forward * 30f;
		playerShip.transform.forward = -gate.transform.forward;
		global::DG.Tweening.ShortcutExtensions.DOKill(playerShip.modelTrans);
		playerShip.modelTrans.localPosition = global::UnityEngine.Vector3.zero;
		followTarget.isPause = false;
		followTarget.transform.position = playerShip.transform.position;
		followTarget.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, -10f, 0f);
		followTarget.CameraPivotReset();
		playerShipController.SubShipsTeleport();
		UI_Manager.Instance.TouchPanelActive(active: true);
		UI_Manager.Instance.BlackScreenOff(1f, delegate
		{
			playerShipController.RestartNearIslandCheck();
			EnemySpawnManager.Instance.isSpawnPause = false;
			playerShip.isBoundaryNoUse = false;
			StopCoroutine("ShipAcceleration");
			UI_Manager.Instance.SetTouchBlock(isActive: false);
			MoPubAdManager.Instance.ShowInterstitialAd();
		});
		StartCoroutine("ShipAcceleration");
	}

	private global::System.Collections.IEnumerator ShipAcceleration()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			playerShip.Acceleration(playerShip.acceleration);
			yield return wait;
		}
	}

	public void MusicOn()
	{
		BGMManager.Instance.masterVolume = 0.8f;
		BGMManager.Instance.PlayBGM("wave_loop_freesound", "bgm_Adventure_Theme");
	}

	private void Start()
	{
		if ((bool)PlayerInfo.Instance.myData.isMusicOn)
		{
			MusicOn();
		}
		else
		{
			BGMManager.Instance.StopBGM();
		}
		if ((bool)PlayerInfo.Instance.myData.isSoundOn)
		{
			AudioManager.Instance.SFXStart();
		}
		else
		{
			AudioManager.Instance.SFXStop();
		}
		if (worldMaker != null && worldManager != null)
		{
			worldManager.GenerateWorld(PlayerInfo.Instance.myData.selectedCubeNum, PlayerInfo.Instance.myData.selectedWorldNum);
		}
		ShipBase ship = null;
		ShipController shipController = null;
		ShipGenerator.Instance.SpawnMain(global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.Euler(0f, 135f, 0f), out ship, out shipController);
		if (ship != null)
		{
			playerShip = ship;
		}
		if (shipController != null)
		{
			playerShipController = shipController;
		}
		Island islandData = worldManager.GetIslandData(PlayerInfo.Instance.myData.selectedIslandId);
		if (islandData != null)
		{
			InIsland(islandData.myDock);
		}
		else
		{
			UI_Manager.Instance.TouchPanelActive(active: false);
			EnemySpawnManager.Instance.isSpawnPause = false;
			IsInIsland = false;
		}
		worldManager.GetIndex(playerShip.transform.position, out var indexX, out var indexZ);
		worldManager.Check(indexX, indexZ);
	}

	public void SetFollowTarget(global::UnityEngine.Transform target)
	{
		followTarget.SetTarget(target);
	}

	public void CameraShake(float duration = 0.2f, float strength = 0.3f, bool fadeOut = true)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(cameraPivotTrans);
		cameraPivotTrans.localPosition = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOShakePosition(cameraPivotTrans, duration, strength, 20, 360f, snapping: false, fadeOut), delegate
		{
			cameraPivotTrans.localPosition = global::UnityEngine.Vector3.zero;
		});
	}

	public void CameraShakeRotate(float duration = 0.2f, float strength = 0.3f, bool fadeOut = true)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(cameraPivotTrans);
		cameraPivotTrans.localRotation = global::UnityEngine.Quaternion.identity;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOShakeRotation(cameraPivotTrans, duration, strength, 20, 360f, fadeOut), delegate
		{
			cameraPivotTrans.localRotation = global::UnityEngine.Quaternion.identity;
		});
	}

	public void OutIsland()
	{
		StartCoroutine(OutIslandControlPlayerShip());
		playerShipController.OutIsland();
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DORotate(followTarget.transform, new global::UnityEngine.Vector3(0f, -10f, 0f), 5f), global::DG.Tweening.Ease.OutQuart), delegate
		{
			UI_Manager.Instance.TouchPanelActive(active: true);
		});
		followTarget.isSmooth = true;
		followTarget.isPause = false;
	}

	public void CheckTimeIsGole()
	{
		StopCoroutine("SailorSkillGoldCheck");
		StartCoroutine("SailorSkillGoldCheck");
	}

	private global::System.Collections.IEnumerator SailorSkillGoldCheck()
	{
		bool flag = false;
		float totalTime = 0f;
		float coolTime = 20f;
		float goldFactor = 0.2f;
		global::System.Collections.Generic.List<ESailor> skill_timeIsGold = SailorDataManager.Instance.skill_timeIsGold;
		if (skill_timeIsGold.Count > 0)
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_timeIsGold[0]);
			if (mySailorData.isHave)
			{
				switch (mySailorData.level)
				{
				case 1:
					goldFactor = 0.2f;
					break;
				case 2:
					goldFactor = 0.3f;
					break;
				case 3:
					goldFactor = 0.4f;
					break;
				}
				flag = true;
			}
		}
		if (!flag)
		{
			yield break;
		}
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime >= coolTime)
			{
				if (playerShip == null)
				{
					yield return null;
					continue;
				}
				totalTime -= coolTime;
				int num = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel - 1, 0, 999);
				long amount = (int)((float)ShipDataManager.Instance.enemyReward.dataArray[num].Normalgold * goldFactor);
				UI_Manager.Instance.AddReward(playerShip.transform.position, new Reward(EMonenyType.gold, amount));
			}
			yield return null;
		}
	}

	private global::System.Collections.IEnumerator OutIslandControlPlayerShip()
	{
		ShipController shipController = playerShipController;
		global::DG.Tweening.ShortcutExtensions.DOKill(shipController.transform);
		global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOMove(shipController.transform, playerShip.transform.forward * 28f, 6f), global::DG.Tweening.Ease.InOutQuad), isRelative: true);
		StartCoroutine("CheckFarToIsland");
		IsInIsland = false;
		yield return new global::UnityEngine.WaitForSeconds(6f);
		if (anchoredDock != null && TutorialManager.Instance.IsHadTutorial("secondbattle"))
		{
			anchoredDock.DockingActive(active: true);
		}
		UI_Manager.Instance.UIActiveFade(active: true);
		TutorialManager.Instance.StartTutorial("move_base");
		if (!TutorialManager.Instance.IsHadTutorial("firstbattle"))
		{
			StopCoroutine(BattleTutorialCheck());
			StartCoroutine(BattleTutorialCheck());
		}
		CheckTimeIsGole();
	}

	private global::System.Collections.IEnumerator BattleTutorialCheck()
	{
		global::UnityEngine.Vector3 pos = playerShip.transform.position;
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.3f);
		while (true)
		{
			if ((pos - playerShip.transform.position).magnitude > 60f)
			{
				yield return new global::UnityEngine.WaitForSeconds(4f);
				if (TutorialManager.Instance.IsHadTutorial("firstbattle") || TutorialManager.Instance.IsPlaying("firstbattle"))
				{
					break;
				}
				TutorialManager.Instance.StartTutorial("firstbattle");
			}
			yield return wait;
		}
	}

	private global::System.Collections.IEnumerator CheckFarToIsland()
	{
		while (true)
		{
			if (playerShipController == null)
			{
				yield return null;
				continue;
			}
			if ((anchoredDock.transform.position - playerShipController.transform.position).sqrMagnitude > 8100f)
			{
				break;
			}
			yield return null;
		}
		EnemySpawnManager.Instance.isSpawnPause = false;
	}

	private global::System.Collections.IEnumerator WaitCameraOn()
	{
		while (!(global::UnityEngine.Camera.main != null))
		{
			yield return null;
		}
		global::UnityEngine.Camera.main.orthographicSize = 55f;
	}

	public void InIsland(Docking dock)
	{
		UI_Manager.Instance.TouchPanelActive(active: false);
		PlayerInfo.Instance.myData.isWatchDieAskPopup = false;
		StopCoroutine("CheckFarToIsland");
		StopCoroutine("SailorSkillGoldCheck");
		if (global::UnityEngine.Camera.main != null)
		{
			global::UnityEngine.Camera.main.orthographicSize = 55f;
		}
		else
		{
			StartCoroutine(WaitCameraOn());
		}
		EnemySpawnManager.Instance.isSpawnPause = true;
		UI_Manager.Instance.UIActive(active: false);
		if (dock.myIsland.status.islandId == 0)
		{
			UI_Manager.Instance.ui_mainGame.SetClaimActive(active: false);
		}
		else
		{
			UI_Manager.Instance.ui_mainGame.SetClaimActive(active: true);
		}
		if (PlayerInfo.Instance.GetIsClearIslandClaim(dock.myIsland.status.islandId))
		{
			UI_Manager.Instance.ui_mainGame.conquestClearNeedMsg.SetActive(value: false);
			UI_Manager.Instance.ui_mainGame.conquestHaveTaxTime.gameObject.SetActive(value: true);
		}
		else if (dock.myIsland.status.islandId != 0)
		{
			int number = (int)islandSheet.dataArray[dock.myIsland.status.islandId].Gold / 10;
			UI_Manager.Instance.ui_mainGame.needClearTaxValueText.text = Numbering.Convert(number);
			UI_Manager.Instance.ui_mainGame.conquestClearNeedMsg.SetActive(value: true);
			UI_Manager.Instance.ui_mainGame.conquestHaveTaxTime.gameObject.SetActive(value: false);
		}
		else
		{
			UI_Manager.Instance.ui_mainGame.conquestClearNeedMsg.SetActive(value: false);
			UI_Manager.Instance.ui_mainGame.conquestHaveTaxTime.gameObject.SetActive(value: false);
		}
		UI_Manager.Instance.ui_mainGame.SetIslandName(string.Format("{0} {1}", LanguageManager.Instance.translate("key_island"), dock.myIsland.status.islandId + 1));
		anchoredDock = dock;
		anchoredDock.DockingActive(active: false);
		IsInIsland = true;
		followTarget.IslandCameraSetting(anchoredDock.transform);
		cameraMoveOpenMenu.CombackCameraPivot();
		ShipController playerController = playerShipController;
		playerController.SetAutoDriveMode(isActive: false);
		playerController.rigidbody.velocity = global::UnityEngine.Vector3.zero;
		playerController.rigidbody.angularVelocity = global::UnityEngine.Vector3.zero;
		playerController.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, anchoredDock.transform.rotation.eulerAngles.y + 180f, 0f);
		TouchPad.Instance.ForceTouchUp();
		global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(-22f, -2f, -22f);
		global::UnityEngine.Vector3 vector2 = anchoredDock.transform.TransformVector(vector) + anchoredDock.transform.position;
		global::UnityEngine.Vector3 position = vector2 + anchoredDock.transform.forward * 10f;
		playerController.transform.position = position;
		playerController.SubShipsTeleport(0.6f);
		foreach (ShipSubController subShip in playerController.subShipList)
		{
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOMove(subShip.transform, anchoredDock.transform.forward * -10f, 3f), isIndependentUpdate: true), isRelative: true), global::DG.Tweening.Ease.OutQuad);
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOMove(playerController.transform, vector2, 3f), isIndependentUpdate: true), global::DG.Tweening.Ease.OutQuad), delegate
		{
			playerController.InIsland();
		});
		TutorialManager.Instance.StartTutorial("start");
		UI_Manager.Instance.BlackScreenOff(0.6f, delegate
		{
			UI_Manager.Instance.UIActiveFade(active: true);
			UI_Manager.Instance.ui_mainGame.UIMode(EUIMode.InIsland);
		});
		PlayerInfo.Instance.myData.selectedCubeNum = worldManager.selectedWorldData.cubeNum;
		PlayerInfo.Instance.myData.selectedWorldNum = worldManager.selectedWorldData.worldNum;
		PlayerInfo.Instance.myData.selectedIslandId = dock.myIsland.status.islandId;
		PlayerInfo.Instance.SaveData();
		EnemySpawnManager.Instance.DestroyAllEnemies();
		foreach (global::UnityEngine.GameObject @object in objectList)
		{
			if (@object != null)
			{
				global::UnityEngine.Object.Destroy(@object);
			}
		}
		ObjectPoolManager.Instance.AllDisable();
		global::UnityEngine.Time.timeScale = 1f;
	}

	public void PlayerDie()
	{
		PlayerDieAction();
	}

	public void NormalDieAction()
	{
		if (!PlayerInfo.Instance.myData.isWatchDieAskPopup)
		{
			UI_Manager.Instance.ui_mainGame.deadAskPopupObj.gameObject.SetActive(value: true);
		}
		else
		{
			DieSure();
		}
	}

	public void DieShowAds(global::UnityEngine.Events.UnityAction action)
	{
		PlayerResurrectionNow(0.3f);
		action();
	}

	public void DieUseResurrection()
	{
		PlayerResurrectionNow(0.7f);
	}

	public void DieSure()
	{
		UI_Manager.Instance.BlackScreenOn(0.8f, PlayerReGenerate);
	}

	private void PlayerResurrectionNow(float hpPercent)
	{
		PlayerInfo.Instance.myData.lastQuitTimeTick = 0L;
		if (global::UnityEngine.Time.timeScale != 1f)
		{
			TimeScale(1f);
		}
		PlayerInfo.Instance.myData.lastLeftHp = (int)((float)playerShip.myHealth.HP_MAX * hpPercent);
		PlayerInfo.Instance.myData.isWatchDieAskPopup = true;
		ShipBase ship = null;
		ShipController shipController = null;
		ShipGenerator.Instance.SpawnMain(followTarget.transform.position, global::UnityEngine.Quaternion.Euler(0f, 45f, 0f), out ship, out shipController);
		if (ship != null)
		{
			playerShip = ship;
		}
		if (shipController != null)
		{
			playerShipController = shipController;
		}
		playerShip.myHealth.SetHealthPoint((int)((float)playerShip.myHealth.HP_MAX * hpPercent), playerShip.myHealth.HP_MAX);
		PlayerInfo.Instance.SaveData();
		Windblast component = global::UnityEngine.Object.Instantiate(global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/Windblast"), playerShip.transform.position, global::UnityEngine.Quaternion.identity, playerShip.transform).GetComponent<Windblast>();
		int num = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		float shieldScale = 1f;
		switch (num)
		{
		case 1:
			shieldScale = 0.57f;
			break;
		case 2:
			shieldScale = 0.68f;
			break;
		case 3:
			shieldScale = 0.74f;
			break;
		case 4:
		case 5:
			shieldScale = 0.8f;
			break;
		case 6:
		case 7:
		case 8:
			shieldScale = 1f;
			break;
		case 9:
			shieldScale = 1.15f;
			break;
		case 10:
			shieldScale = 1.26f;
			break;
		}
		EShipMain level = (EShipMain)num;
		int level2 = PlayerInfo.Instance.myData.mainShipLevel;
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(EShipType.Main, level2);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = (int)((float)(playerShip.Damage * mainData.cannonCount) / shipSheetData.fireCoolTime);
		float num3 = 6f;
		float num4 = 0.1f;
		component.damage = (int)((float)num2 * num3);
		component.SetShieldScale(shieldScale);
		playerShip.myHealth.AddShieldEnergy((int)((float)playerShip.myHealth.HP_MAX * num4));
		AudioManager.Instance.PlaySFX("skill_windblast");
	}

	private void PlayerReGenerate()
	{
		PlayerInfo.Instance.myData.lastQuitTimeTick = 0L;
		if ((int)PlayerInfo.Instance.myData.selectedCubeNum != worldManager.selectedWorldData.cubeNum || (int)PlayerInfo.Instance.myData.selectedWorldNum != worldManager.selectedWorldData.worldNum)
		{
			worldManager.ClearWorld();
			UI_Manager.Instance.ui_worldMap.ClearWorldMap();
			worldManager.GenerateWorld(PlayerInfo.Instance.myData.selectedCubeNum, PlayerInfo.Instance.myData.selectedWorldNum);
		}
		UI_PatrolCombatStatus ui_patrolCombatStatus = UI_Manager.Instance.ui_mainGame.ui_patrolCombatStatus;
		if (ui_patrolCombatStatus.gameObject.activeSelf)
		{
			ui_patrolCombatStatus.gameObject.SetActive(value: false);
		}
		foreach (Island island in worldManager.islandList)
		{
			island.TryRespawn();
		}
		PlayerInfo.Instance.myData.lastLeftHp = (int)((float)playerShip.myHealth.HP_MAX * 0.1f);
		MyInventoryDataManager.Instance.SpawnShipClear();
		Gate[] gates = worldManager.gates;
		for (int i = 0; i < gates.Length; i++)
		{
			gates[i].TryEnemyRespawn();
		}
		worldManager.PatrolEnemyReset();
		ShipBase ship = null;
		ShipController shipController = null;
		ShipGenerator.Instance.SpawnMain(global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.Euler(0f, 45f, 0f), out ship, out shipController);
		if (ship != null)
		{
			playerShip = ship;
		}
		if (shipController != null)
		{
			playerShipController = shipController;
		}
		playerShip.myHealth.SetHealthPoint((int)((float)playerShip.myHealth.HP_MAX * 0.1f), playerShip.myHealth.HP_MAX);
		Island islandData = worldManager.GetIslandData(PlayerInfo.Instance.myData.selectedIslandId);
		if (islandData != null)
		{
			InIsland(islandData.myDock);
		}
		worldManager.AllActive(active: false);
		worldManager.GetIndex(playerShip.transform.position, out var indexX, out var indexZ);
		worldManager.Check(indexX, indexZ);
		IsBossFighting = false;
	}

	public bool GenerateRootBox(global::UnityEngine.Vector3 spawnPos, out RootBox rootBox)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(rootBoxSrc, spawnPos, global::UnityEngine.Quaternion.identity);
		rootBox = gameObject.GetComponent<RootBox>();
		objectList.Add(gameObject);
		return rootBox != null;
	}

	public bool GenerateRewardBox(global::UnityEngine.Vector3 spawnPos, out RewardBox rewardBox)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(rewardBoxSrc, spawnPos, global::UnityEngine.Quaternion.identity);
		rewardBox = gameObject.GetComponent<RewardBox>();
		objectList.Add(gameObject);
		return rewardBox != null;
	}

	public bool GenerateLifeBead(global::UnityEngine.Vector3 spawnPos, out LifeBead lifeBead)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(lifeBeadSrc, spawnPos, global::UnityEngine.Quaternion.identity);
		lifeBead = gameObject.GetComponent<LifeBead>();
		objectList.Add(gameObject);
		return lifeBead != null;
	}

	public void AddObject(global::UnityEngine.GameObject obj)
	{
		objectList.Add(obj);
	}

	public void DockAndTaxShipFlag()
	{
		if (anchoredDock != null)
		{
			anchoredDock.RefreshFlag();
		}
		int flagId = PlayerInfo.Instance.myData.selectedFlagId;
		if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
		{
			taxShip.GetComponentInChildren<Flag>().SetFlag(info.flagMaterial);
		}
	}

	public void TaxSummon()
	{
		StartCoroutine(TaxRutine());
	}

	private global::System.Collections.IEnumerator TaxRutine()
	{
		ClaimTaxManager.Instance.RefreshTaxValue();
		if (IsInIsland && PlayerInfo.Instance.GetIsClearIslandClaim(anchoredDock.myIsland.status.islandId))
		{
			anchoredDock.TaxShip.MakeTaxBox(new Reward(EMonenyType.gold, ClaimTaxManager.Instance.TaxValue));
			yield return new global::UnityEngine.WaitForSeconds(2.5f);
			anchoredDock.TaxShip.Force();
			yield break;
		}
		if (!taxShip.gameObject.activeSelf)
		{
			taxShip.gameObject.SetActive(value: true);
		}
		taxShip.transform.SetParent(followTarget.transform);
		taxShip.transform.localPosition = new global::UnityEngine.Vector3(-60f, 0f, -60f);
		taxShip.MakeTaxBox(new Reward(EMonenyType.gold, ClaimTaxManager.Instance.TaxValue));
		taxShip.transform.SetParent(null);
	}

	public void GenerateTutorialRootBox()
	{
		if (GenerateRootBox(playerShip.transform.position + new global::UnityEngine.Vector3(-10f, 0f, 22f), out var rootBox))
		{
			rootBox.DontSelfDestroy();
			rootBox.AddReward(new Reward(EMonenyType.gold, 300L));
		}
	}

	public void AllTutorialOver()
	{
		if (!PlayerInfo.Instance.myData.isWatchedTutorial)
		{
			PlayerInfo.Instance.myData.isWatchedTutorial = true;
			PlayerInfo.Instance.SaveData();
			Appsflyer.Instance.SendEvent("tutorial_Finish");
		}
	}

	public void TimeScale(float factor)
	{
		global::UnityEngine.Time.timeScale = factor;
	}

	public void GenerateHealParticle(global::UnityEngine.Transform parentTrans)
	{
		global::UnityEngine.Transform obj = global::UnityEngine.Object.Instantiate(healParticleSrc, parentTrans).transform;
		obj.localPosition = new global::UnityEngine.Vector3(0f, 5f, 0f);
		obj.localScale = global::UnityEngine.Vector3.one * 10f;
	}

	public void FirstBattleOver()
	{
		Invoke("StartSecondBattle", 1f);
	}

	private void StartSecondBattle()
	{
		TutorialManager.Instance.StartTutorial("secondbattle");
	}

	public void TutorialIslandDockOpen()
	{
		Island islandData = worldManager.GetIslandData(0);
		if (islandData != null)
		{
			islandData.myDock.DockingActive(active: true);
		}
	}
}
