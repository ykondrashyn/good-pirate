[global::UnityEngine.RequireComponent(typeof(ShipBase))]
public class ShipController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	private ShipBase myShip;

	private ShipState state;

	public global::System.Collections.Generic.List<ShipSubController> subShipList = new global::System.Collections.Generic.List<ShipSubController>();

	public int PrevIndexX = -1;

	public int PrevIndexZ = -1;

	private Island nearIsland;

	private global::System.Collections.Generic.List<global::UnityEngine.Transform> lineList;

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<FormationPos> formationList = new global::System.Collections.Generic.List<FormationPos>();

	private global::UnityEngine.GameObject formationbackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform autoDriveArrowTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform destinationArrowTrans;

	private UI_WorldMapSameScale worldMapIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform birdPivotTrans;

	private Pirrot myPirrot;

	private ChestBird myChestBird;

	private Eagle myEagle;

	[global::UnityEngine.Header("audioClip")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip steerSmallAudioClip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip steerMiddleAudioClip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip steerBigAudioClip;

	private global::UnityEngine.AudioSource audioSource;

	private global::UnityEngine.Animator modelAnim;

	private bool isPaused;

	private float birdTime = 300f;

	private bool steerSound;

	private global::UnityEngine.Coroutine autoDriveCoroutine;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject autodirveUIObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup autoCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text precentText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image autoBoostGaugeImage;

	public global::UnityEngine.Coroutine destinationCoroutine;

	private int fogOverlapCount;

	private int bushOverlapCount;

	private int sunkenOverlapCount;

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
		transform = GetComponent<global::UnityEngine.Transform>();
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		audioSource = GetComponent<global::UnityEngine.AudioSource>();
		RefreshModel();
		global::UnityEngine.GameObject original = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Formation");
		lineList = ShipLineManager.Instance.GetLineData(ELine.LineType_01);
		for (int i = 0; i < lineList.Count; i++)
		{
			global::UnityEngine.Vector3 position = transform.TransformVector(lineList[i].localPosition) + transform.position;
			FormationPos component = global::UnityEngine.Object.Instantiate(original, position, global::UnityEngine.Quaternion.identity, transform).GetComponent<FormationPos>();
			component.index = i;
			formationList.Add(component);
		}
		if (lineList != null)
		{
			GenerateSubShip();
		}
		int level = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		ShipDataManager.Instance.GetMainData((EShipMain)level);
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Main);
		myShip.SetBaseData(shipBaseData);
		myShip.SetLevel(PlayerInfo.Instance.myData.mainShipLevel);
		foreach (FormationPos formation in formationList)
		{
			formation.SetLayer();
			formation.gameObject.SetActive(value: false);
		}
		if ((bool)GameManager.Instance)
		{
			GameManager.Instance.SetFollowTarget(transform);
		}
		autoDriveArrowTrans.gameObject.SetActive(value: false);
		destinationArrowTrans.gameObject.SetActive(value: false);
		myShip.myHealth.AddHitAction(HitHpBar);
		myShip.myHealth.AddDieAction(delegate
		{
			UI_Manager.Instance.ui_mainGame.HpBar(0f);
		});
		myShip.myHealth.AddHealAction(delegate
		{
			UI_Manager.Instance.ui_mainGame.HpBar(myShip.myHealth.HpRatio);
		});
		myShip.myHealth.AddHitAction(delegate
		{
			UI_Manager.Instance.ui_mainGame.HpBar(myShip.myHealth.HpRatio);
		});
		myShip.myHealth.AddHpSetAction(delegate
		{
			UI_Manager.Instance.ui_mainGame.HpBar(myShip.myHealth.HpRatio);
		});
		GenerateCannonRange();
		BirdCheck();
	}

	public void RefreshModel()
	{
		int level = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData((EShipMain)level);
		ShipDataManager.Instance.GetShipBaseData(EShipType.Main);
		global::UnityEngine.GameObject gameObject = null;
		if (PlayerInfo.Instance.myData.selectedSkin == eShipSkin.Normal)
		{
			switch ((int)PlayerInfo.Instance.myData.mainShipLevel % 100 / 25)
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
			if ((int)PlayerInfo.Instance.myData.mainShipLevel >= ConstValue.ShipLevelMax)
			{
				gameObject = mainData.playerModelType03;
			}
			if (gameObject != null)
			{
				myShip.SpawnShipModel(gameObject, ELayerType.Player);
			}
			else
			{
				myShip.SpawnShipModel(mainData.modelSrc, ELayerType.Player);
			}
		}
		else
		{
			ShipSkinData skinData = SkinDataManager.Instance.GetSkinData(PlayerInfo.Instance.myData.selectedSkin);
			myShip.SpawnShipModel(skinData.modelSrc, ELayerType.Player);
			CannonManager[] componentsInChildren = GetComponentsInChildren<CannonManager>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].MakeCannon(mainData.cannonCount);
			}
			int num = (int)PlayerInfo.Instance.myData.mainShipLevel / 100 - skinData.levelLimit / 100;
			myShip.modelTrans.localScale = global::UnityEngine.Vector3.one + 0.14f * (float)num * global::UnityEngine.Vector3.one;
		}
		int flagId = PlayerInfo.Instance.myData.selectedFlagId;
		if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
		{
			GetComponentInChildren<Flag>().SetFlag(info.flagMaterial);
		}
		modelAnim = GetComponentInChildren<global::UnityEngine.Animator>();
		SkinDataManager.Instance.RefreshAbility();
	}

	public void BirdCheck()
	{
		if ((bool)PlayerInfo.Instance.myData.isHaveEagle)
		{
			GenerateEagle();
		}
		if ((bool)PlayerInfo.Instance.myData.isHavePirrot)
		{
			GeneratePirrot();
		}
		if (PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot)
		{
			GenerateChestBird();
		}
	}

	private void Start()
	{
		TouchPad instance = TouchPad.Instance;
		instance.dragAction = (TouchPad.PointerAction)global::System.Delegate.Combine(instance.dragAction, new TouchPad.PointerAction(TouchDrag));
		TouchPad instance2 = TouchPad.Instance;
		instance2.pointerDownAction = (TouchPad.PointerAction)global::System.Delegate.Combine(instance2.pointerDownAction, new TouchPad.PointerAction(TouchDown));
		TouchPad instance3 = TouchPad.Instance;
		instance3.pointerUpAction = (TouchPad.PointerAction)global::System.Delegate.Combine(instance3.pointerUpAction, new TouchPad.PointerAction(TouchUp));
		TouchPad instance4 = TouchPad.Instance;
		instance4.touchingFixedUpdateAction = (TouchPad.PointerAction)global::System.Delegate.Combine(instance4.touchingFixedUpdateAction, new TouchPad.PointerAction(Acceleration));
		TouchPad instance5 = TouchPad.Instance;
		instance5.doubleTouch = (global::System.Action<global::UnityEngine.Vector2>)global::System.Delegate.Combine(instance5.doubleTouch, new global::System.Action<global::UnityEngine.Vector2>(DoubleTouch));
		if (UI_Manager.Instance != null && (bool)UI_Manager.Instance.ui_worldMap)
		{
			worldMapIcon = UI_Manager.Instance.ui_worldMap.GenerateSameScale();
			worldMapIcon.SetPosition(transform.position);
		}
		FollowTarget.Instance.SetTarget(transform);
		_ = (int)PlayerInfo.Instance.myData.selectedCubeNum;
		_ = (int)PlayerInfo.Instance.myData.selectedWorldNum;
		myShip.myHealth.AddHitAction(UI_Manager.Instance.Hit);
		ShipBase shipBase = myShip;
		shipBase.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase.beforeDestroyAction, new global::System.Action(GameManager.Instance.PlayerDie));
		ShipBase shipBase2 = myShip;
		shipBase2.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase2.beforeDestroyAction, (global::System.Action)delegate
		{
			UI_Manager.Instance.ui_mainGame.ui_islandStatus.gameObject.SetActive(value: false);
		});
		if (PlayerInfo.Instance.myData.lastLeftHp > 0)
		{
			if (PlayerInfo.Instance.myData.lastQuitTimeTick != 0L)
			{
				global::System.DateTime value = new global::System.DateTime(PlayerInfo.Instance.myData.lastQuitTimeTick);
				float num = global::UnityEngine.Mathf.Clamp01((float)global::System.DateTime.Now.Subtract(value).TotalSeconds / (float)ConstValue.HealthRegenerationTime);
				int health = global::UnityEngine.Mathf.Min((int)((float)PlayerInfo.Instance.myData.lastLeftHp + (float)PlayerInfo.Instance.PlayerShipHpMax * num), PlayerInfo.Instance.PlayerShipHpMax);
				myShip.myHealth.SetHealthPoint(health, PlayerInfo.Instance.PlayerShipHpMax);
				PlayerInfo.Instance.myData.lastQuitTimeTick = 0L;
				PlayerInfo.Instance.SaveData();
			}
			else
			{
				myShip.myHealth.SetHealthPoint(PlayerInfo.Instance.myData.lastLeftHp, PlayerInfo.Instance.PlayerShipHpMax);
			}
		}
		if (PlayerInfo.Instance.myData.destinationIslandId != -1)
		{
			Island islandData = GameManager.Instance.worldManager.GetIslandData(PlayerInfo.Instance.myData.destinationIslandId);
			SetDestination(islandData);
		}
		StartCoroutine(NearIslandCombatCheck());
		StartCoroutine("CheckNearIslandSpawnEnemyLevel");
		CheckFog();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			isPaused = true;
			SaveLastHpAndPushNotification();
		}
		else if (isPaused)
		{
			global::SA.CrossPlatform.Notifications.UM_NotificationCenter.Client.RemoveAllDeliveredNotifications();
			isPaused = false;
		}
	}

	private void OnApplicationQuit()
	{
		SaveLastHpAndPushNotification();
	}

	private void SaveLastHpAndPushNotification()
	{
		PlayerInfo.Instance.myData.lastLeftHp = myShip.myHealth.HP;
		PlayerInfo.Instance.myData.lastQuitTimeTick = global::System.DateTime.Now.Ticks;
		PlayerInfo.Instance.SaveData();
		float num = (1f - myShip.myHealth.HpRatio) * (float)ConstValue.HealthRegenerationTime;
		if (!(num > 10f))
		{
			return;
		}
		int num2 = (int)num + 3;
		global::SA.CrossPlatform.Notifications.UM_iNotificationsClient client = global::SA.CrossPlatform.Notifications.UM_NotificationCenter.Client;
		global::SA.CrossPlatform.Notifications.UM_Notification uM_Notification = new global::SA.CrossPlatform.Notifications.UM_Notification();
		uM_Notification.SetTitle(LanguageManager.Instance.translate("key_pushnoti_hp_title"));
		uM_Notification.SetBody(LanguageManager.Instance.translate("key_pushnoti_hp_body"));
		global::SA.CrossPlatform.Notifications.UM_TimeIntervalNotificationTrigger trigger = new global::SA.CrossPlatform.Notifications.UM_TimeIntervalNotificationTrigger(num2);
		global::SA.CrossPlatform.Notifications.UM_NotificationRequest request = new global::SA.CrossPlatform.Notifications.UM_NotificationRequest(1, uM_Notification, trigger);
		client.AddNotificationRequest(request, delegate(global::SA.Foundation.Templates.SA_Result result)
		{
			if (result.IsSucceeded)
			{
				global::UnityEngine.Debug.Log(result);
			}
			else
			{
				global::UnityEngine.Debug.Log(result.Error.FullMessage);
			}
		});
	}

	public void GenerateEagle()
	{
		if (myEagle != null)
		{
			return;
		}
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Eagle");
		if (!(gameObject != null))
		{
			return;
		}
		global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, birdPivotTrans);
		gameObject2.transform.localPosition = global::UnityEngine.Vector3.zero;
		gameObject2.transform.localScale = global::UnityEngine.Vector3.one;
		myEagle = gameObject2.GetComponent<Eagle>();
		int num = 0;
		if (myChestBird != null)
		{
			num++;
		}
		if (myPirrot != null)
		{
			num++;
		}
		switch (num)
		{
		case 0:
			myEagle.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
			break;
		case 1:
			if ((bool)myChestBird)
			{
				myEagle.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, myChestBird.modelTrans.localRotation.eulerAngles.y - 180f, 0f);
			}
			else
			{
				myEagle.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, myPirrot.modelTrans.localRotation.eulerAngles.y - 180f, 0f);
			}
			break;
		case 2:
			myChestBird.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 120f, 0f);
			myPirrot.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 240f, 0f);
			myEagle.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
			break;
		}
	}

	public void GeneratePirrot()
	{
		if (myPirrot != null)
		{
			return;
		}
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Pirrot");
		if (!(gameObject != null))
		{
			return;
		}
		global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, birdPivotTrans);
		gameObject2.transform.localPosition = global::UnityEngine.Vector3.zero;
		gameObject2.transform.localScale = global::UnityEngine.Vector3.one;
		myPirrot = gameObject2.GetComponent<Pirrot>();
		int num = 0;
		if (myEagle != null)
		{
			num++;
		}
		if (myChestBird != null)
		{
			num++;
		}
		switch (num)
		{
		case 0:
			myPirrot.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
			break;
		case 1:
			if ((bool)myEagle)
			{
				myPirrot.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, myEagle.modelTrans.localRotation.eulerAngles.y - 180f, 0f);
			}
			else
			{
				myPirrot.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, myChestBird.modelTrans.localRotation.eulerAngles.y - 180f, 0f);
			}
			break;
		case 2:
			myChestBird.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 120f, 0f);
			myPirrot.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 240f, 0f);
			myEagle.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
			break;
		}
	}

	public void GenerateChestBird()
	{
		if (myChestBird != null)
		{
			return;
		}
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/ChestBird");
		if (!(gameObject != null))
		{
			return;
		}
		global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, birdPivotTrans);
		gameObject2.transform.localPosition = global::UnityEngine.Vector3.zero;
		gameObject2.transform.localScale = global::UnityEngine.Vector3.one;
		myChestBird = gameObject2.GetComponent<ChestBird>();
		int num = 0;
		if (myEagle != null)
		{
			num++;
		}
		if (myPirrot != null)
		{
			num++;
		}
		switch (num)
		{
		case 0:
			myChestBird.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
			break;
		case 1:
			if ((bool)myEagle)
			{
				myChestBird.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, myEagle.modelTrans.localRotation.eulerAngles.y - 180f, 0f);
			}
			else
			{
				myChestBird.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, myPirrot.modelTrans.localRotation.eulerAngles.y - 180f, 0f);
			}
			break;
		case 2:
			myChestBird.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 120f, 0f);
			myPirrot.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 240f, 0f);
			myEagle.modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
			break;
		}
	}

	private global::System.Collections.IEnumerator BirdChanceCheck()
	{
		yield return new global::UnityEngine.WaitForSeconds(birdTime);
		BirdChanceOver();
	}

	public void BirdChanceOver()
	{
		if (!PlayerInfo.Instance.myData.isHavePirrot && myPirrot != null)
		{
			global::UnityEngine.Object.Destroy(myPirrot.gameObject);
		}
		if (!PlayerInfo.Instance.myData.isHaveEagle && myEagle != null)
		{
			global::UnityEngine.Object.Destroy(myEagle.gameObject);
		}
	}

	public void GenerateCannonRange()
	{
		CannonManager[] componentsInChildren = GetComponentsInChildren<CannonManager>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].GenerateFireRange();
		}
	}

	public void AllBirdRemoveForce()
	{
		if (myChestBird != null)
		{
			global::UnityEngine.Object.Destroy(myChestBird.gameObject);
		}
		if (myPirrot != null)
		{
			global::UnityEngine.Object.Destroy(myPirrot.gameObject);
		}
		if (myEagle != null)
		{
			global::UnityEngine.Object.Destroy(myEagle.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (TouchPad.Instance != null)
		{
			TouchPad instance = TouchPad.Instance;
			instance.dragAction = (TouchPad.PointerAction)global::System.Delegate.Remove(instance.dragAction, new TouchPad.PointerAction(TouchDrag));
			TouchPad instance2 = TouchPad.Instance;
			instance2.pointerDownAction = (TouchPad.PointerAction)global::System.Delegate.Remove(instance2.pointerDownAction, new TouchPad.PointerAction(TouchDown));
			TouchPad instance3 = TouchPad.Instance;
			instance3.pointerUpAction = (TouchPad.PointerAction)global::System.Delegate.Remove(instance3.pointerUpAction, new TouchPad.PointerAction(TouchUp));
			TouchPad instance4 = TouchPad.Instance;
			instance4.touchingFixedUpdateAction = (TouchPad.PointerAction)global::System.Delegate.Remove(instance4.touchingFixedUpdateAction, new TouchPad.PointerAction(Acceleration));
			TouchPad instance5 = TouchPad.Instance;
			instance5.doubleTouch = (global::System.Action<global::UnityEngine.Vector2>)global::System.Delegate.Remove(instance5.doubleTouch, new global::System.Action<global::UnityEngine.Vector2>(DoubleTouch));
		}
		if (worldMapIcon != null)
		{
			global::UnityEngine.Object.Destroy(worldMapIcon.gameObject);
		}
		foreach (ShipSubController subShip in subShipList)
		{
			if (subShip != null)
			{
				global::UnityEngine.Object.Destroy(subShip.gameObject);
			}
		}
	}

	public void InIsland()
	{
		StopCoroutine("HealthRegeneration");
		StopCoroutine("SubShipHealthRegeneration");
		StartCoroutine("HealthRegeneration");
		StartCoroutine("SubShipHealthRegeneration");
	}

	public void OutIsland()
	{
		StopCoroutine("HealthRegeneration");
		StopCoroutine("SubShipHealthRegeneration");
		UI_Manager.Instance.ui_mainGame.healSpeech.gameObject.SetActive(value: false);
	}

	public void FullHealth()
	{
		StopCoroutine("HealthRegeneration");
		StopCoroutine("SubShipHealthRegeneration");
		int hP_MAX = myShip.myHealth.HP_MAX;
		myShip.myHealth.SetHealthPoint(hP_MAX, hP_MAX);
		UI_Manager.Instance.ui_mainGame.healSpeech.gameObject.SetActive(value: false);
		PlayerInfo.Instance.myData.lastLeftHp = myShip.myHealth.HP;
		foreach (ShipSubController subShip in subShipList)
		{
			hP_MAX = subShip.myShip.myHealth.HP_MAX;
			subShip.myShip.myHealth.SetHealthPoint(hP_MAX, hP_MAX);
		}
		PlayerInfo.Instance.SaveData();
	}

	private global::System.Collections.IEnumerator HealthRegeneration()
	{
		if (myShip.myHealth.HpRatio >= 1f)
		{
			yield break;
		}
		UI_Manager.Instance.ui_mainGame.healSpeech.gameObject.SetActive(value: true);
		float duration = ConstValue.HealthRegenerationTime;
		float totalTime = duration * myShip.myHealth.HpRatio;
		int prevHp = myShip.myHealth.HP;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			int hP_MAX = myShip.myHealth.HP_MAX;
			int num2 = (int)((float)hP_MAX * num);
			if (prevHp != num2)
			{
				myShip.myHealth.SetHealthPoint(num2, hP_MAX);
				PlayerInfo.Instance.myData.lastLeftHp = num2;
				PlayerInfo.Instance.SaveData();
			}
			prevHp = num2;
			if (num >= 1f)
			{
				break;
			}
			yield return null;
		}
		UI_Manager.Instance.ui_mainGame.healSpeech.gameObject.SetActive(value: false);
	}

	private global::System.Collections.IEnumerator SubShipHealthRegeneration()
	{
		float duration = ConstValue.HealthRegenerationTimeSub;
		global::System.Collections.Generic.List<float> totalTimeList = new global::System.Collections.Generic.List<float>();
		foreach (ShipSubController subShip in subShipList)
		{
			totalTimeList.Add(subShip.myShip.myHealth.HpRatio * duration);
		}
		while (true)
		{
			for (int i = 0; i < subShipList.Count; i++)
			{
				ShipSubController shipSubController = subShipList[i];
				if (shipSubController == null)
				{
					global::UnityEngine.Debug.Log("보조배가 Null 이에욧");
				}
				else if (!(totalTimeList[i] > duration))
				{
					totalTimeList[i] += global::UnityEngine.Time.deltaTime;
					float num = global::UnityEngine.Mathf.Clamp01(totalTimeList[i] / duration);
					int hP_MAX = shipSubController.myShip.myHealth.HP_MAX;
					int health = (int)((float)hP_MAX * num);
					shipSubController.myShip.myHealth.SetHealthPoint(health, hP_MAX);
				}
			}
			bool flag = true;
			foreach (float item in totalTimeList)
			{
				if (item < duration)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	private void FixedUpdate()
	{
		if (modelAnim != null)
		{
			float speed = global::UnityEngine.Mathf.Lerp(0.2f, 1.8f, global::UnityEngine.Mathf.InverseLerp(0f, 4.5f, rigidbody.velocity.magnitude));
			modelAnim.speed = speed;
		}
		if (myShip.targetFinder.HealthList.Count > 0)
		{
			if (state != ShipState.Combat)
			{
				state = ShipState.Combat;
				myShip.SetState(state);
				foreach (ShipSubController subShip in subShipList)
				{
					subShip.SetState(state);
				}
			}
		}
		else if (state != ShipState.Follow)
		{
			state = ShipState.Follow;
			myShip.SetState(state);
			foreach (ShipSubController subShip2 in subShipList)
			{
				subShip2.SetState(state);
			}
			myShip.SetState(state);
		}
		if (UI_Manager.Instance != null && UI_Manager.Instance.ui_worldMap != null && UI_Manager.Instance.ui_worldMap.gameObject.activeSelf && worldMapIcon != null)
		{
			worldMapIcon.SetPosition(transform.position);
			worldMapIcon.SetRotation(transform.rotation.eulerAngles.y);
		}
	}

	public void StopNearIslandCheck()
	{
		StopCoroutine("CheckNearIslandSpawnEnemyLevel");
	}

	public void RestartNearIslandCheck()
	{
		StopCoroutine("CheckNearIslandSpawnEnemyLevel");
		StartCoroutine("CheckNearIslandSpawnEnemyLevel");
	}

	private global::System.Collections.IEnumerator CheckNearIslandSpawnEnemyLevel()
	{
		if ((bool)GameManager.Instance)
		{
			GameManager.Instance.worldManager.GetIndex(transform.position, out PrevIndexX, out PrevIndexZ);
		}
		yield return null;
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.3f);
		while (true)
		{
			if (GameManager.Instance != null)
			{
				GameManager.Instance.worldManager.GetIndex(transform.position, out var indexX, out var indexZ);
				GameManager.Instance.worldManager.Check(PrevIndexX, PrevIndexZ, indexX, indexZ);
				nearIsland = GameManager.Instance.worldManager.GetNearIsland(transform.position);
				if (nearIsland == null)
				{
					yield return wait;
					continue;
				}
				int enemylevelmin = EnemySpawnManager.Instance.islandSheet.dataArray[nearIsland.status.islandId].Enemylevelmin;
				int enemylevelmax = EnemySpawnManager.Instance.islandSheet.dataArray[nearIsland.status.islandId].Enemylevelmax;
				int spawnLevel = global::UnityEngine.Mathf.Clamp(PlayerInfo.Instance.myData.mainShipLevel, enemylevelmin, enemylevelmax);
				_ = (int)PlayerInfo.Instance.myData.selectedCubeNum;
				_ = (int)PlayerInfo.Instance.myData.selectedWorldNum;
				EnemySpawnManager.Instance.SpawnLevel = spawnLevel;
				PrevIndexX = indexX;
				PrevIndexZ = indexZ;
			}
			yield return wait;
		}
	}

	public void GenerateSubShip()
	{
		foreach (global::System.Collections.Generic.KeyValuePair<int, MyShipInventory> item in MyInventoryDataManager.Instance.myShipDataDic)
		{
			AddSubShip(item.Value);
		}
	}

	public void AddSubShip(MyShipInventory data)
	{
		int num = PlayerInfo.Instance.myData.selectedFormationNum;
		if (data.lineIndex[num] == -1)
		{
			global::UnityEngine.Debug.Log($"{data.shipType} is not equipped : {data.id}");
			return;
		}
		global::UnityEngine.Vector3 position = formationList[data.lineIndex[num]].transform.position;
		ShipSubController subShip = ShipGenerator.Instance.SpawnSubShip(position, transform.rotation, myShip, formationList[data.lineIndex[num]].transform.localPosition);
		int shipLevel = PlayerInfo.Instance.GetShipLevel(data.shipType);
		int level = global::UnityEngine.Mathf.Clamp(shipLevel / 100 + 1, 1, 10);
		ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(data.shipType, (EShipSub)level);
		if (subShipData == null || subShipData.modelSrc == null)
		{
			subShipData = ShipDataManager.Instance.GetSubShipData(data.shipType, EShipSub.LEVEL_01);
		}
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(data.shipType);
		subShip.SetBaseData(shipBaseData);
		subShip.myShip.SpawnShipModel(subShipData.modelSrc, ELayerType.Player);
		subShip.myShip.InvenShipData = data;
		subShip.myShip.SetLevel(shipLevel);
		subShip.SetState(state);
		subShip.transform.localScale = global::UnityEngine.Vector3.one * 0.3f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(subShip.transform, 1.3f, 0.3f), delegate
		{
			global::DG.Tweening.ShortcutExtensions.DOScale(subShip.transform, 1f, 0.3f);
		});
		int flagId = PlayerInfo.Instance.myData.selectedFlagId;
		if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
		{
			subShip.GetComponentInChildren<Flag>().SetFlag(info.flagMaterial);
		}
		formationList[data.lineIndex[num]].SetStatus(subShip);
		subShipList.Add(subShip);
	}

	public void FormationMode(bool active)
	{
		rigidbody.velocity = global::UnityEngine.Vector3.zero;
		formationbackObj.SetActive(active);
		foreach (FormationPos formation in formationList)
		{
			formation.gameObject.SetActive(active);
		}
		foreach (ShipSubController subShip in subShipList)
		{
			subShip.isFollowPause = active;
		}
	}

	private void TouchDrag(float strength, global::UnityEngine.Vector3 vecNormal, global::UnityEngine.Vector3 vecControl)
	{
		vecNormal.Set(vecNormal.x, 0f, vecNormal.y);
		float angleForward = myMathf.GetAngleForward(vecNormal);
		angleForward += 10f;
		if (angleForward > 360f)
		{
			angleForward -= 360f;
		}
		else if (angleForward < 0f)
		{
			angleForward += 360f;
		}
		if (!steerSound)
		{
			float num = global::UnityEngine.Mathf.Abs(myMathf.GetAngleForward(transform.forward) - angleForward);
			bool flag = false;
			if (num > 10f && num < 30f)
			{
				audioSource.clip = steerSmallAudioClip;
				flag = true;
			}
			else if (num < 100f && num >= 30f)
			{
				audioSource.clip = steerMiddleAudioClip;
				flag = true;
			}
			else if (num >= 100f)
			{
				audioSource.clip = steerBigAudioClip;
				flag = true;
			}
			if (flag)
			{
				Invoke("SteerSoundCoolTimeOver", 6f);
				audioSource.Play();
				steerSound = true;
			}
		}
		myShip.SetTargetAngle(angleForward);
	}

	private void SteerSoundCoolTimeOver()
	{
		steerSound = false;
	}

	private void TouchDown(float strength, global::UnityEngine.Vector3 vecNormal, global::UnityEngine.Vector3 vecControl)
	{
		myShip.isMove = true;
		SetAutoDriveMode(isActive: false);
	}

	private void TouchUp(float strength, global::UnityEngine.Vector3 vecNormal, global::UnityEngine.Vector3 vecControl)
	{
		myShip.isMove = false;
		myShip.Reset();
	}

	private void DoubleTouch(global::UnityEngine.Vector2 screenPos)
	{
		global::UnityEngine.Ray ray = global::UnityEngine.Camera.main.ScreenPointToRay(screenPos);
		global::UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 1000f, global::UnityEngine.Color.red, 3f);
		if (global::UnityEngine.Physics.Raycast(ray, out var hitInfo, 1000f, global::UnityEngine.LayerMask.GetMask("Water")))
		{
			global::UnityEngine.Vector3 forward = hitInfo.point - transform.position;
			forward.y = 0f;
			forward.Normalize();
			float angleForward = myMathf.GetAngleForward(forward);
			SetAutoDriveMode(isActive: true, angleForward);
		}
	}

	private void Acceleration(float strength, global::UnityEngine.Vector3 vecNormal, global::UnityEngine.Vector3 vecControl)
	{
		myShip.Acceleration(strength * myShip.acceleration);
	}

	private global::System.Collections.IEnumerator NearIslandCombatCheck()
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.5f);
		while (true)
		{
			if (ClaimManager.Instance.IsPlaying || GameManager.Instance.IsShipPause)
			{
				yield return null;
				continue;
			}
			if (nearIsland == null)
			{
				yield return wait;
				continue;
			}
			if (PlayerInfo.Instance.GetIsClearIsland(nearIsland.status.islandId))
			{
				yield return wait;
				continue;
			}
			if (nearIsland.myDock == null)
			{
				yield return wait;
				continue;
			}
			if ((nearIsland.myDock.transform.position - transform.position).sqrMagnitude < 10000f)
			{
				UI_Manager.Instance.ui_mainGame.ui_islandStatus.SetIsland(nearIsland);
				while (!(nearIsland == null))
				{
					global::UnityEngine.Vector3 vector = nearIsland.myDock.transform.position - transform.position;
					if (!TutorialManager.Instance.IsHadTutorial("firstisland") && vector.sqrMagnitude < 8100f && !TutorialManager.Instance.IsPlaying("firstisland"))
					{
						TutorialManager.Instance.StartTutorial("firstisland");
					}
					if (!(vector.sqrMagnitude < 10000f))
					{
						UI_Manager.Instance.ui_mainGame.ui_islandStatus.gameObject.SetActive(value: false);
						break;
					}
					yield return wait;
				}
			}
			yield return wait;
		}
	}

	public void SubShipsTeleport(float scale = 1f)
	{
		state = ShipState.Follow;
		foreach (ShipSubController subShip in subShipList)
		{
			if (subShip != null)
			{
				global::UnityEngine.Vector3 position = subShip.targetTrans.TransformVector(subShip.offset * scale) + subShip.targetTrans.position;
				subShip.SetState(state);
				subShip.myShip.Reset();
				subShip.transform.position = position;
				subShip.transform.rotation = subShip.targetTrans.rotation;
				subShip.myShip.rigidbody.velocity = global::UnityEngine.Vector3.zero;
				subShip.myShip.rigidbody.angularVelocity = global::UnityEngine.Vector3.zero;
			}
		}
	}

	public void SetAutoDriveMode(bool isActive, float angle = 0f)
	{
		if (isActive)
		{
			if (autoDriveCoroutine == null)
			{
				autoDriveCoroutine = StartCoroutine(AutoDrive(angle));
				autoDriveArrowTrans.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f - angle + 90f, 0f);
				StopCoroutine("FollowingWindIncrease");
				StartCoroutine("FollowingWindIncrease");
			}
		}
		else
		{
			if (autoDriveCoroutine != null)
			{
				StopCoroutine(autoDriveCoroutine);
				autoDriveCoroutine = null;
				StopCoroutine("FollowingWindIncrease");
				if (autodirveUIObj.activeSelf)
				{
					autodirveUIObj.SetActive(value: false);
				}
			}
			myShip.followingWindFactor = 1f;
			myShip.Reset();
		}
		autoDriveArrowTrans.gameObject.SetActive(isActive);
	}

	private global::System.Collections.IEnumerator FollowingWindIncrease()
	{
		if (!autodirveUIObj.activeSelf)
		{
			autodirveUIObj.SetActive(value: true);
			if (!autoCanvas.gameObject.activeSelf)
			{
				autoCanvas.gameObject.SetActive(value: true);
			}
			global::DG.Tweening.ShortcutExtensions.DOKill(autoCanvas);
			autoCanvas.alpha = 0f;
			global::DG.Tweening.DOTweenModuleUI.DOFade(autoCanvas, 1f, 0.6f);
		}
		float totalTime = 0f;
		float duration = 6f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			autoBoostGaugeImage.fillAmount = num;
			float num2 = global::UnityEngine.Mathf.Lerp(1f, 1.5f, num);
			myShip.followingWindFactor = num2;
			if (num >= 1f)
			{
				break;
			}
			precentText.text = (int)(num2 * 100f) + "%";
			yield return null;
		}
		precentText.text = LanguageManager.Instance.translate("key_max");
		yield return new global::UnityEngine.WaitForSeconds(1.8f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(autoCanvas, 0f, 0.6f), delegate
		{
			autoCanvas.gameObject.SetActive(value: false);
		});
	}

	private global::System.Collections.IEnumerator AutoDrive(float angle)
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			myShip.SetTargetAngle(angle);
			myShip.Acceleration(myShip.acceleration);
			autoDriveArrowTrans.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f - angle + 90f, 0f);
			yield return wait;
		}
	}

	public void SetDestination(Island island)
	{
		if (destinationCoroutine != null)
		{
			StopCoroutine(destinationCoroutine);
			destinationCoroutine = null;
		}
		if (island == null)
		{
			destinationArrowTrans.gameObject.SetActive(value: false);
		}
		else
		{
			destinationCoroutine = StartCoroutine(DestinationCoroutine(island));
		}
	}

	private global::System.Collections.IEnumerator DestinationCoroutine(Island island)
	{
		global::UnityEngine.Vector3 pos = island.myDock.transform.position;
		destinationArrowTrans.gameObject.SetActive(value: true);
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			global::UnityEngine.Vector3 forward = pos - transform.position;
			forward.y = 0f;
			forward.Normalize();
			float angleForward = myMathf.GetAngleForward(forward);
			destinationArrowTrans.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f - angleForward + 90f, 0f);
			yield return wait;
		}
	}

	private void OnCollisionEnter(global::UnityEngine.Collision other)
	{
		if (!(other.relativeVelocity.magnitude < 3f) && !other.gameObject.CompareTag(base.gameObject.tag))
		{
			CameraShakeManager.Instance.CameraShakeRotate(0.4f, 0.4f);
		}
	}

	private void HitHpBar()
	{
		PlayerInfo.Instance.myData.lastLeftHp = myShip.myHealth.HP;
		PlayerInfo.Instance.SaveData();
	}

	public void ChangeFlag(int flagNum)
	{
		PlayerInfo.Instance.myData.selectedFlagId = flagNum;
		int flagId = PlayerInfo.Instance.myData.selectedFlagId;
		if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
		{
			GetComponentInChildren<Flag>().SetFlag(info.flagMaterial);
			foreach (ShipSubController subShip in subShipList)
			{
				subShip.GetComponentInChildren<Flag>().SetFlag(info.flagMaterial);
			}
		}
		PlayerInfo.Instance.SaveData();
	}

	public void SetSubShipActive(bool active)
	{
		foreach (ShipSubController subShip in subShipList)
		{
			if (subShip != null)
			{
				subShip.gameObject.SetActive(active);
			}
		}
	}

	public void DestroyAllSubShip()
	{
		foreach (ShipSubController subShip in subShipList)
		{
			if (subShip != null)
			{
				global::UnityEngine.Object.Destroy(subShip.gameObject);
			}
		}
		subShipList.Clear();
	}

	public void EnterFog()
	{
		fogOverlapCount++;
		CheckFog();
	}

	public void ExitFog()
	{
		fogOverlapCount--;
		CheckFog();
	}

	private void CheckFog()
	{
		bool isSkillActive = false;
		if (fogOverlapCount > 0)
		{
			if (!UI_Manager.Instance.smokeObj.gameObject.activeSelf)
			{
				UI_Manager.Instance.smokeObj.gameObject.SetActive(value: true);
			}
			UI_Manager.Instance.ui_passiveSkill.FogSetActive(active: true, out isSkillActive);
		}
		else
		{
			if (UI_Manager.Instance.smokeObj.gameObject.activeSelf)
			{
				UI_Manager.Instance.smokeObj.Off();
			}
			UI_Manager.Instance.ui_passiveSkill.FogSetActive(active: false, out isSkillActive);
		}
	}

	public void EnterBush()
	{
		bushOverlapCount++;
		CheckBush();
	}

	public void ExitBush()
	{
		bushOverlapCount--;
		CheckBush();
	}

	private void CheckBush()
	{
		bool isSkillActive = false;
		if (bushOverlapCount > 0)
		{
			UI_Manager.Instance.ui_passiveSkill.BushSetActive(active: true, out isSkillActive);
		}
		else
		{
			UI_Manager.Instance.ui_passiveSkill.BushSetActive(active: false, out isSkillActive);
		}
	}

	public void EnterSunken()
	{
		sunkenOverlapCount++;
		CheckSunken();
	}

	public void ExitSunken()
	{
		sunkenOverlapCount--;
		CheckSunken();
	}

	private void CheckSunken()
	{
		bool isSkillActive = false;
		if (sunkenOverlapCount > 0)
		{
			UI_Manager.Instance.ui_passiveSkill.RockSetActive(active: true, out isSkillActive);
		}
		else
		{
			UI_Manager.Instance.ui_passiveSkill.RockSetActive(active: false, out isSkillActive);
		}
	}
}
