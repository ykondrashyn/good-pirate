public class UI_Manager : global::UnityEngine.MonoBehaviour
{
	private struct FormationBeforeShipData
	{
		public global::UnityEngine.Vector3 pos;

		public global::UnityEngine.Quaternion rot;

		public global::UnityEngine.Vector3 velocity;
	}

	public static UI_Manager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.GraphicRaycaster canvasRaycaster;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup blackScreenImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup justBlackScreen;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject touchPanel;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup gameCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform topUIObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform bottomUIObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject downMenuObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject miniMapUIObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject rewardCanvasSrc;

	public UI_Store inappObj;

	public UI_WorldMap ui_worldMap;

	public UI_Formation ui_formation;

	public UI_MainMenu ui_mainMenu;

	public UI_InvenShipSlotManager shipPopupSlotManager;

	public UI_SpawnShipListManager ui_spawnShipManager;

	public UI_StoreGetRewardBox ui_storeGetRewardBox;

	public UI_StoreGetCard ui_storeGetCard;

	public UI_ChestPanel ui_chestPanel;

	public UI_ItemBoxPopup ui_itemBoxPopup;

	public CardGotchaAnim cardOpenPopup;

	public UI_PassiveSkill ui_passiveSkill;

	public UI_EnterIslandPopup ui_enterIslandPopup;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.GameObject inkObj;

	[global::UnityEngine.SerializeField]
	public UI_Fog smokeObj;

	public ClaimWorldMap claimWorldMap;

	public UI_Claim ui_claim;

	[global::UnityEngine.Header("money type icon sprite")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite iconDiamond;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite iconFish;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite iconGold;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite iconIron;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite iconWood;

	[global::UnityEngine.Header("DamageCanvas")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject damageNormalSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject damageHitSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject damageCriticalSrc;

	[global::UnityEngine.Header("etc")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image thunderImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Light directionalLight;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image hitImage;

	[global::UnityEngine.Header("tutorials")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] tutorialPlayingInActiveButtons;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform rewardChestUITrans;

	[global::UnityEngine.Header("store")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject rewardboxListObj;

	private global::UnityEngine.Camera mainCamera;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject loadingObj;

	private global::System.Collections.Generic.List<UI_Manager.FormationBeforeShipData> beforeData = new global::System.Collections.Generic.List<UI_Manager.FormationBeforeShipData>();

	private EUIMode prevUIMode;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject allRemoveAskPopup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cloudFailPop;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cloudSuccessPop;

	public UI_MainGame ui_mainGame { get; private set; }

	private void Start()
	{
		IapManager.Instance.loadingBlockOn = delegate
		{
			BlackScreenOn(0.3f);
		};
		IapManager.Instance.loadingBlockOff = delegate
		{
			BlackScreenOff(0.3f);
		};
	}

	public void Reset()
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.DeleteAll();
		global::UnityEngine.Application.Quit();
	}

	public void AddMoney(int amount)
	{
		PlayerInfo.Instance.AddMoney(EMonenyType.gold, amount);
	}

	public void AddDiamond(int amount)
	{
		PlayerInfo.Instance.AddMoney(EMonenyType.diamond, amount);
	}

	public void AddPirrot()
	{
		GameManager.Instance.playerShipController.GeneratePirrot();
	}

	public void AddEagle()
	{
		GameManager.Instance.playerShipController.GenerateEagle();
	}

	public void TestInterstitial()
	{
		MoPubAdManager.Instance.ShowInterstitialAd();
	}

	public void TestRewardedVideo()
	{
		MoPubAdManager.Instance.ShowRewardedVideo(null);
	}

	public void FrameRate(int rate)
	{
		global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(rate);
	}

	public void RateUs()
	{
		if (!PlayerInfo.Instance.myData.isRateUs)
		{
			PlayerInfo.Instance.myData.isRateUs = true;
			PlayerInfo.Instance.SaveData();
			SetCloudPopupTitle(cloudSuccessPop, "Good Pirate preview");
			SetCloudPopupMessage(cloudSuccessPop, "You can report problems through the project homepage. Thank you for playing.");
			cloudSuccessPop.SetActive(true);
		}
	}

	private void Awake()
	{
		Instance = this;
		blackScreenImage.gameObject.SetActive(value: true);
		blackScreenImage.alpha = 1f;
		ui_mainGame = GetComponentInChildren<UI_MainGame>();
		if ((bool)ui_worldMap)
		{
			ui_worldMap.gameObject.SetActive(value: false);
		}
		hitImage.gameObject.SetActive(value: false);
		mainCamera = global::UnityEngine.Camera.main;
		topUIObj.anchoredPosition = global::UnityEngine.Vector2.zero;
		bottomUIObj.anchoredPosition = global::UnityEngine.Vector2.zero;
		global::Percent.CrossPromotion.addShowAdsToQueue();
		global::Percent.CrossPromotion.showPrivacyWindow(isFirstScreen: true);
		global::UnityEngine.Debug.Log("showPrivacyWindow");
	}

	public void RewardBoxOn()
	{
		rewardboxListObj.SetActive(value: true);
		rewardboxListObj.SendMessage("Refresh");
	}

	[global::UnityEngine.ContextMenu("껄껄")]
	public void TestAddBlackBox()
	{
		for (int i = 0; i < 10; i++)
		{
			RewardBoxStatus rewardBoxStatus = new RewardBoxStatus();
			rewardBoxStatus.boxType = ERewardBox.BLACK;
			rewardBoxStatus.level = global::UnityEngine.Mathf.Min(999, (int)PlayerInfo.Instance.myData.mainShipLevel + 49);
			PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, rewardBoxStatus);
		}
		PlayerInfo.Instance.SaveData();
		RewardBoxOn();
	}

	private void Update()
	{
		if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Escape))
		{
			var popups = global::UnityEngine.Object.FindObjectsByType<PopupAction>(global::UnityEngine.FindObjectsSortMode.None);
			PopupAction top = null;
			foreach (var popup in popups)
			{
				if (popup.isActiveAndEnabled && (top == null || popup.transform.GetSiblingIndex() > top.transform.GetSiblingIndex())) top = popup;
			}
			if (top != null) top.Close();
		}
	}

	public void SetPortrait()
	{
		global::UnityEngine.Screen.orientation = global::UnityEngine.ScreenOrientation.Portrait;
		OrientationCheck.Instance.portraitEvent.Invoke();
	}

	public void SetLandscape()
	{
		global::UnityEngine.Screen.orientation = global::UnityEngine.ScreenOrientation.LandscapeLeft;
		OrientationCheck.Instance.landscapeEvent.Invoke();
	}

	public void TouchPanelActive(bool active)
	{
		touchPanel.SetActive(active);
	}

	public void BlackScreenOn(float time, global::UnityEngine.Events.UnityAction completedCallBack = null, bool isActiveLoading = true)
	{
		loadingObj.SetActive(isActiveLoading);
		global::DG.Tweening.ShortcutExtensions.DOKill(blackScreenImage);
		blackScreenImage.gameObject.SetActive(value: true);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(blackScreenImage, 1f, time), isIndependentUpdate: true), delegate
		{
			if (completedCallBack != null)
			{
				completedCallBack();
			}
		});
	}

	public void BlackScreenOff(float time, global::UnityEngine.Events.UnityAction completedCallBack = null, bool isHaveDelay = true)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(blackScreenImage);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOFade(blackScreenImage, 0f, time), isHaveDelay ? global::UnityEngine.Random.Range(1f, 2f) : 0f), isIndependentUpdate: true), delegate
		{
			if (completedCallBack != null)
			{
				completedCallBack();
			}
			blackScreenImage.gameObject.SetActive(value: false);
		});
	}

	public void RealJustBlackOn(float time, float alpha, global::UnityEngine.Events.UnityAction completedCallBack = null)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(justBlackScreen);
		justBlackScreen.gameObject.SetActive(value: true);
		justBlackScreen.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(justBlackScreen, alpha, time), isIndependentUpdate: true), delegate
		{
			if (completedCallBack != null)
			{
				completedCallBack();
			}
		});
	}

	public void RealJustBlackOff(float time, float delayTime, global::UnityEngine.Events.UnityAction completedCallBack = null)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(justBlackScreen);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOFade(justBlackScreen, 0f, time), delayTime), isIndependentUpdate: true), delegate
		{
			if (completedCallBack != null)
			{
				completedCallBack();
			}
			justBlackScreen.gameObject.SetActive(value: false);
		});
	}

	public void UIActive(bool active)
	{
		gameCanvas.gameObject.SetActive(active);
	}

	public void UIActiveFade(bool active, global::UnityEngine.Events.UnityAction completedCallback = null)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(gameCanvas);
		if (active)
		{
			gameCanvas.gameObject.SetActive(value: true);
			gameCanvas.alpha = 0f;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(gameCanvas, 1f, 1.2f), isIndependentUpdate: true), delegate
			{
				if (completedCallback != null)
				{
					completedCallback();
				}
			});
			return;
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(gameCanvas, 0f, 0.6f), isIndependentUpdate: true), delegate
		{
			if (completedCallback != null)
			{
				completedCallback();
			}
			gameCanvas.gameObject.SetActive(value: false);
		});
	}

	public void FormationMode(bool active)
	{
		StartCoroutine(FormationModeWaitFixedUpdate(active));
	}

	private global::System.Collections.IEnumerator FormationModeWaitFixedUpdate(bool active)
	{
		yield return new global::UnityEngine.WaitForFixedUpdate();
		ShipController shipController = null;
		if (GameManager.Instance != null)
		{
			shipController = GameManager.Instance.playerShipController;
		}
		TouchPanelActive(!active);
		ui_formation.gameObject.SetActive(active);
		shipController.FormationMode(active);
		miniMapUIObj.SetActive(!active);
		if (GameManager.Instance != null)
		{
			GameManager.Instance.worldManager.ObjSetActive(shipController.transform.position, !active);
			GameManager.Instance.followTarget.FormationCameraSetting(active);
		}
		else if (FollowTarget.Instance != null)
		{
			FollowTarget.Instance.FormationCameraSetting(active);
		}
		if (active)
		{
			FollowTarget.Instance.Offset = global::UnityEngine.Vector3.zero;
			if (GameManager.Instance != null && !GameManager.Instance.IsInIsland)
			{
				beforeData.Clear();
				for (int i = 0; i < shipController.subShipList.Count; i++)
				{
					UI_Manager.FormationBeforeShipData item = new UI_Manager.FormationBeforeShipData
					{
						pos = shipController.subShipList[i].transform.position,
						rot = shipController.subShipList[i].transform.rotation,
						velocity = shipController.subShipList[i].myShip.rigidbody.velocity
					};
					beforeData.Add(item);
				}
			}
			prevUIMode = ui_mainGame.uiMode;
			ui_mainGame.UIMode(EUIMode.Formation);
			shipController.rigidbody.velocity = global::UnityEngine.Vector3.zero;
			shipController.SubShipsTeleport();
			shipController.SetAutoDriveMode(isActive: false);
			ui_formation.Clean();
			yield break;
		}
		ui_mainGame.UIMode(prevUIMode);
		if (GameManager.Instance != null && !GameManager.Instance.IsInIsland)
		{
			FollowTarget.Instance.Offset = new global::UnityEngine.Vector3(0f, 0f, -42f);
			for (int j = 0; j < shipController.subShipList.Count; j++)
			{
				TouchPad.Instance.gameObject.SetActive(value: true);
				shipController.subShipList[j].transform.position = beforeData[j].pos;
				shipController.subShipList[j].transform.rotation = beforeData[j].rot;
				shipController.subShipList[j].myShip.rigidbody.velocity = beforeData[j].velocity;
			}
			beforeData.Clear();
		}
	}

	public void AudioClick()
	{
		AudioManager.PlayButtonClick();
	}

	public void WorldMapSetActive(bool active)
	{
		ui_worldMap.gameObject.SetActive(active);
		AudioManager.PlayButtonClick();
		mainCamera.enabled = !active;
		if (active)
		{
			ui_worldMap.OpenSetPosition(GameManager.Instance.playerShip.transform.position);
			return;
		}
		if (IsInvoking("DoubleTapTutorial"))
		{
			CancelInvoke("DoubleTapTutorial");
		}
		Invoke("DoubleTapTutorial", 1f);
	}

	private void DoubleTapTutorial()
	{
		TutorialManager.Instance.StartTutorial("doubletap");
	}

	public void Hit()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(hitImage);
		if (!hitImage.gameObject.activeSelf)
		{
			hitImage.gameObject.SetActive(value: true);
		}
		hitImage.color = global::UnityEngine.Color.white;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOColor(hitImage, new global::UnityEngine.Color(1f, 1f, 1f, 0f), 0.2f), 0.1f), delegate
		{
			hitImage.gameObject.SetActive(value: false);
		});
	}

	public void AddReward(global::UnityEngine.Vector3 pos, params Reward[] reward)
	{
		global::UnityEngine.Object.Instantiate(rewardCanvasSrc, pos, global::UnityEngine.Quaternion.identity).GetComponent<RewardCanvas>().AddReward(reward);
	}

	public global::UnityEngine.Sprite GetMoneyIcon(EMonenyType type)
	{
		global::UnityEngine.Sprite result = null;
		switch (type)
		{
		case EMonenyType.diamond:
			result = iconDiamond;
			break;
		case EMonenyType.fish:
			result = iconFish;
			break;
		case EMonenyType.gold:
			result = iconGold;
			break;
		case EMonenyType.iron:
			result = iconIron;
			break;
		case EMonenyType.wood:
			result = iconWood;
			break;
		}
		return result;
	}

	public void GoStageScene()
	{
		Instance = null;
		BlackScreenOn(0.5f, delegate
		{
			global::UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("StageScene");
		});
	}

	public void Thunder()
	{
		StartCoroutine(ThunderCoroutine());
	}

	private global::System.Collections.IEnumerator ThunderCoroutine()
	{
		thunderImage.gameObject.SetActive(value: true);
		thunderImage.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.745f);
		global::UnityEngine.Color prevColor = directionalLight.color;
		directionalLight.intensity = 60f;
		directionalLight.color = global::UnityEngine.Color.white;
		if (global::UnityEngine.Random.value <= 0.5f)
		{
			yield return new global::UnityEngine.WaitForSeconds(0.07f);
			directionalLight.intensity = 1.2f;
			directionalLight.color = prevColor;
			thunderImage.gameObject.SetActive(value: false);
			yield return new global::UnityEngine.WaitForSeconds(0.07f);
			thunderImage.gameObject.SetActive(value: true);
			directionalLight.intensity = 60f;
			directionalLight.color = global::UnityEngine.Color.white;
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOColor(thunderImage, new global::UnityEngine.Color(1f, 1f, 1f, 0f), 2f), 0.1f), delegate
		{
			thunderImage.gameObject.SetActive(value: false);
		});
		global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOColor(directionalLight, prevColor, 2f), 0.1f);
		yield return new global::UnityEngine.WaitForSeconds(0.1f);
		float totalTime = 0f;
		float duration = 2f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			directionalLight.intensity = global::UnityEngine.Mathf.Lerp(60f, 1.2f, num);
			if (!(num > 1f))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public void SetTouchBlock(bool isActive)
	{
		canvasRaycaster.enabled = !isActive;
	}

	public void SubShipAllRemovePopup()
	{
		allRemoveAskPopup.SetActive(value: true);
	}

	public void SubShipAllRemoveClose()
	{
		allRemoveAskPopup.GetComponent<PopupAction>().Close();
	}

	public void SubShipAllRemove()
	{
		MyInventoryDataManager.Instance.SpawnShipClear();
		MyInventoryDataManager.Instance.myShipDataDic.Clear();
		MyInventoryDataManager.Instance.myData.myShipDataList.Clear();
		UI_GeneratorManager.Instance.Reset();
		PlayerInfo.Instance.myData.reserveShipCount = 0;
		ui_spawnShipManager.Refresh();
		SubShipAllRemoveClose();
	}

	public void DamageUIGenerate(EDamageType dmgType, global::UnityEngine.Vector3 pos, int damage)
	{
	}

	public void TutorialPlayingButtonsSetActive(bool active)
	{
		global::UnityEngine.UI.Button[] array = tutorialPlayingInActiveButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = active;
		}
	}

	public void TutorialRewardBoxUIParent(global::UnityEngine.Transform trans)
	{
		rewardChestUITrans.parent = trans;
		global::UnityEngine.Vector3 anchoredPosition3D = rewardChestUITrans.anchoredPosition3D;
		anchoredPosition3D.z = 0f;
		rewardChestUITrans.anchoredPosition3D = anchoredPosition3D;
	}

	public void CloudFailPopup()
	{
		cloudFailPop.SetActive(value: true);
	}

	public void CloudFailPopup(string operation)
	{
		SetCloudPopupTitle(cloudFailPop, operation);
		SetCloudPopupMessage(
			cloudFailPop,
			"Save transfer failed\n\nThe local save was not changed");
		CloudFailPopup();
	}

	public void CloudSuccessPopup()
	{
		cloudSuccessPop.SetActive(value: true);
	}

	public void CloudSuccessPopup(string operation)
	{
		SetCloudPopupTitle(cloudSuccessPop, operation);
		CloudSuccessPopup();
	}

	private static void SetCloudPopupTitle(global::UnityEngine.GameObject popup, string operation)
	{
		if (popup == null || global::System.String.IsNullOrEmpty(operation))
		{
			return;
		}
		foreach (global::UnityEngine.UI.Text label in popup.GetComponentsInChildren<global::UnityEngine.UI.Text>(includeInactive: true))
		{
			if (label.text == "Copy Save" || label.text == "Paste Save")
			{
				label.text = operation;
			}
		}
	}

	private static void SetCloudPopupMessage(global::UnityEngine.GameObject popup, string message)
	{
		if (popup == null || global::System.String.IsNullOrEmpty(message))
		{
			return;
		}
		foreach (global::UnityEngine.UI.Text label in popup.GetComponentsInChildren<global::UnityEngine.UI.Text>(includeInactive: true))
		{
			if (label.text != "Copy Save" && label.text != "Paste Save")
			{
				label.text = message;
			}
		}
	}
}
