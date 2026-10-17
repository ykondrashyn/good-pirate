public class UI_Store : global::UnityEngine.MonoBehaviour
{
	private enum EStoreItemType
	{
		BlackChest = 0,
		GoldChest = 1,
		Resurrection = 2,
		CardC = 3,
		CardB = 4,
		CardA = 5,
		CardS = 6
	}

	private struct BoxPackageInfo
	{
		public string goldStr;

		public string diaStr;

		public string crewStr;
	}

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] panelCanvsGroups;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] tabButtons;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject popupObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button buyButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform[] meshModelObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] popupObjects;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject goldboxPack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject blackboxPack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject goldbox5Pack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject blackbox5Pack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cardsPackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cardaPackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cardbPackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cardcPackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject resurrectionX5Obj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject resurrectionX10Obj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text diaPriceText;

	[global::UnityEngine.Header("===================경계선===================")]
	[global::UnityEngine.Header("=================다이아광고==================")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject freeDiaCanObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject freeDiaCantObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button freeDiaButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text timeText;

	[global::UnityEngine.Header("=================sold out==================")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject parrotObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject eagleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject blackBirdObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject parrotSoldObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject eagleSoldObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject blackBirdSoldObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject removeSoldObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject robinsonRaftSoldObj;

	[global::UnityEngine.Header("=================pack==================")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject birdPackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnToday;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lockToday;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnStarter;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject soldOutStarter;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnInter;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lockInter;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject soldOutInter;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnExpert;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lockExpert;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject soldOutExpert;

	[global::UnityEngine.Header("=================box data==================")]
	[global::UnityEngine.SerializeField]
	private StoreBoxData[] goldX5;

	[global::UnityEngine.SerializeField]
	private StoreBoxData[] blackboxX5;

	[global::UnityEngine.SerializeField]
	private StoreBoxData[] blackboxData;

	[global::UnityEngine.SerializeField]
	private StoreBoxData[] goldboxData;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject restoreBtn;

	private const string FREE_DIAMOND = "KEY_FREEDIA";

	private float beforeTimeScale;

	private int priceDiamond;

	private UI_Store.EStoreItemType itemType;

	private int itemCount;

	private RewardBoxData rewardData;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] todayLeftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text legendCharNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text legendCharContentText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image legendCharSkillIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] refreshPriceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image selectedCharImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image selectedCharBGImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite meteorSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite sparkSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite windSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite avoidSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite unlimitSubSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite fastSpawnSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite healthDownSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite timeIsGoldSprite;

	private int refreshPrice = 5;

	[global::UnityEngine.SerializeField]
	private CardGotchaAnim legendGotcha;

	[global::UnityEngine.ContextMenu("test")]
	private void timetest()
	{
		global::UnityEngine.Debug.Log(global::System.DateTime.Now.Month);
		global::UnityEngine.Debug.Log(global::System.DateTime.Now.Day);
		global::UnityEngine.Debug.Log(global::System.DateTime.Now.TimeOfDay);
		global::UnityEngine.Debug.Log(global::System.DateTime.Now.ToLocalTime());
		global::UnityEngine.Debug.Log(global::System.DateTime.Now.Date);
	}

	private void Awake()
	{
		RelabelOfflinePrices();
		diaPriceText.font = LanguageManager.Instance.getLanguageFont();
		if (rewardData == null)
		{
			rewardData = global::UnityEngine.Resources.Load<RewardBoxData>("Data/RewardBoxData");
		}
	}

	private void RelabelOfflinePrices()
	{
		foreach (global::UnityEngine.UI.Text label in GetComponentsInChildren<global::UnityEngine.UI.Text>(includeInactive: true))
		{
			string text = label.text == null ? string.Empty : label.text.Trim();
			if (text.EndsWith("USD", global::System.StringComparison.OrdinalIgnoreCase)
				|| text.EndsWith("$", global::System.StringComparison.Ordinal))
			{
				label.text = "FREE";
			}
		}
	}

	private void OnEnable()
	{
		restoreBtn.SetActive(value: true);
		beforeTimeScale = global::UnityEngine.Time.timeScale;
		global::UnityEngine.Time.timeScale = 0f;
		FreeDiamondCheck();
		Refresh();
		UI_Store.BoxPackageInfo boxData = GetBoxData(ERewardBox.GOLD, 5);
		StoreBoxData[] array = goldX5;
		foreach (StoreBoxData obj in array)
		{
			obj.crewText.text = boxData.crewStr;
			obj.diaText.text = boxData.diaStr;
			obj.goldText.text = boxData.goldStr;
		}
		UI_Store.BoxPackageInfo boxData2 = GetBoxData(ERewardBox.GOLD, 1);
		array = goldboxData;
		foreach (StoreBoxData obj2 in array)
		{
			obj2.crewText.text = boxData2.crewStr;
			obj2.diaText.text = boxData2.diaStr;
			obj2.goldText.text = boxData2.goldStr;
		}
		UI_Store.BoxPackageInfo boxData3 = GetBoxData(ERewardBox.BLACK, 5);
		array = blackboxX5;
		foreach (StoreBoxData obj3 in array)
		{
			obj3.crewText.text = boxData3.crewStr;
			obj3.diaText.text = boxData3.diaStr;
			obj3.goldText.text = boxData3.goldStr;
		}
		UI_Store.BoxPackageInfo boxData4 = GetBoxData(ERewardBox.BLACK, 1);
		array = blackboxData;
		foreach (StoreBoxData obj4 in array)
		{
			obj4.crewText.text = boxData4.crewStr;
			obj4.diaText.text = boxData4.diaStr;
			obj4.goldText.text = boxData4.goldStr;
		}
	}

	public void Refresh()
	{
		RefreshSoldOut();
		SetLegendPackRefreshPrice();
		RefreshLegendDataPack();
	}

	private void RefreshSoldOut()
	{
		if (PlayerInfo.Instance.myData.isBuyStarter)
		{
			btnStarter.gameObject.SetActive(value: false);
			soldOutStarter.SetActive(value: true);
			lockInter.SetActive(value: false);
			btnInter.enabled = true;
		}
		else
		{
			btnStarter.gameObject.SetActive(value: true);
			soldOutStarter.SetActive(value: false);
			lockInter.SetActive(value: false);
			btnInter.enabled = true;
		}
		if (PlayerInfo.Instance.myData.isBuyIntermediate)
		{
			btnInter.gameObject.SetActive(value: false);
			soldOutInter.SetActive(value: true);
			lockExpert.SetActive(value: false);
			btnExpert.enabled = true;
		}
		else
		{
			btnInter.gameObject.SetActive(value: true);
			soldOutInter.SetActive(value: false);
			lockExpert.SetActive(value: false);
			btnExpert.enabled = true;
		}
		if (PlayerInfo.Instance.myData.isBuyExpert)
		{
			btnExpert.gameObject.SetActive(value: false);
			soldOutExpert.SetActive(value: true);
		}
		else
		{
			btnExpert.gameObject.SetActive(value: true);
			soldOutExpert.SetActive(value: false);
		}
		robinsonRaftSoldObj.SetActive(PlayerInfo.Instance.myData.isBuyRaft);
		parrotObj.SetActive(!PlayerInfo.Instance.myData.isHavePirrot);
		eagleObj.SetActive(!PlayerInfo.Instance.myData.isHaveEagle);
		blackBirdObj.SetActive(!PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot);
		parrotSoldObj.SetActive(PlayerInfo.Instance.myData.isHavePirrot);
		eagleSoldObj.SetActive(PlayerInfo.Instance.myData.isHaveEagle);
		blackBirdSoldObj.SetActive(PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot);
		removeSoldObj.SetActive(PlayerInfo.Instance.myData.isRemoveADS);
		bool flag = (bool)PlayerInfo.Instance.myData.isHavePirrot && (bool)PlayerInfo.Instance.myData.isHaveEagle && PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot;
		birdPackObj.SetActive(!flag);
	}

	private void LateUpdate()
	{
		global::UnityEngine.Transform[] array = meshModelObjs;
		foreach (global::UnityEngine.Transform transform in array)
		{
			if (transform.position.y > 119.3f)
			{
				if (transform.gameObject.activeSelf)
				{
					transform.gameObject.SetActive(value: false);
				}
			}
			else if (!transform.gameObject.activeSelf)
			{
				transform.gameObject.SetActive(value: true);
			}
		}
		if (freeDiaCantObj.activeSelf)
		{
			double totalSeconds = CoolTimeManager.GetLeftCoolTime("KEY_FREEDIA").TotalSeconds;
			if (totalSeconds > 0.0)
			{
				int num = (int)(totalSeconds / 60.0);
				int num2 = (int)(totalSeconds % 60.0);
				timeText.text = $"{num:D2}:{num2:D2}";
			}
			else
			{
				FreeDiamondCheck();
			}
		}
		global::System.DateTime dateTime = global::System.DateTime.Now.AddDays(1.0);
		global::System.TimeSpan timeSpan = new global::System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day).Subtract(global::System.DateTime.Now);
		string text = $"{timeSpan.Hours}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		global::UnityEngine.UI.Text[] array2 = todayLeftTimeText;
		foreach (global::UnityEngine.UI.Text obj in array2)
		{
			obj.text = text;
			obj.font = LanguageManager.Instance.getLanguageFont();
		}
		if (timeSpan.TotalSeconds <= 0.0)
		{
			PlayerInfo.Instance.myData.isTodayPackCanBuy = true;
			PlayerInfo.Instance.myData.todayPackRefreshCount = 0;
			PlayerInfo.Instance.myData.isRefreshTodayPack = true;
			PlayerInfo.Instance.myData.todayPackRefreshCount = 0;
			PlayerInfo.Instance.myData.lastConnectedMonth = global::System.DateTime.Now.Month;
			PlayerInfo.Instance.myData.lastConnectedDay = global::System.DateTime.Now.Day;
			SetLegendPackRefreshPrice();
			RefreshLegendDataPack(change: true);
			PlayerInfo.Instance.SaveData();
		}
	}

	private void FreeDiamondCheck()
	{
		if (CoolTimeManager.GetLeftCoolTime("KEY_FREEDIA").TotalSeconds > 0.0)
		{
			freeDiaCanObj.SetActive(value: false);
			freeDiaCantObj.SetActive(value: true);
			freeDiaButton.enabled = false;
		}
		else
		{
			freeDiaCanObj.SetActive(value: true);
			freeDiaCantObj.SetActive(value: false);
			freeDiaButton.enabled = true;
		}
	}

	public void FreeDiamond()
	{
		if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
		{
			FreeDiaSuccess();
			return;
		}
		MoPubAdManager.Instance.ShowRewardedVideo(FreeDiaSuccess);
		GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "무료 다이아", 1L);
	}

	private void FreeDiaSuccess()
	{
		PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 5L);
		CoolTimeManager.AddCooltimeSecondFromNow("KEY_FREEDIA", 601);
		freeDiaCanObj.SetActive(value: false);
		freeDiaCantObj.SetActive(value: true);
		freeDiaButton.enabled = false;
	}

	public void Close()
	{
		global::UnityEngine.Time.timeScale = beforeTimeScale;
	}

	private void OpenPopup()
	{
		global::UnityEngine.GameObject[] array = popupObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		popupObj.SetActive(value: true);
	}

	public void SelectCardC()
	{
		OpenPopup();
		cardcPackObj.SetActive(value: true);
		priceDiamond = 5;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.CardC;
		itemCount = 1;
	}

	public void SelectCardB()
	{
		OpenPopup();
		cardbPackObj.SetActive(value: true);
		priceDiamond = 10;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.CardB;
		itemCount = 1;
	}

	public void SelectCardA()
	{
		OpenPopup();
		cardaPackObj.SetActive(value: true);
		priceDiamond = 20;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.CardA;
		itemCount = 1;
	}

	public void SelectCardS()
	{
		OpenPopup();
		cardsPackObj.SetActive(value: true);
		priceDiamond = 40;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.CardS;
		itemCount = 1;
	}

	public void SelectGoldBoxPack()
	{
		OpenPopup();
		goldboxPack.SetActive(value: true);
		priceDiamond = 80;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.GoldChest;
		itemCount = 1;
	}

	public void SelectGoldBoxPack5()
	{
		OpenPopup();
		goldbox5Pack.SetActive(value: true);
		priceDiamond = 360;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.GoldChest;
		itemCount = 5;
	}

	public void SelectBlackBoxPack()
	{
		OpenPopup();
		blackboxPack.SetActive(value: true);
		priceDiamond = 130;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.BlackChest;
		itemCount = 1;
	}

	public void SelectBlackBoxPack5()
	{
		OpenPopup();
		blackbox5Pack.SetActive(value: true);
		priceDiamond = 600;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.BlackChest;
		itemCount = 5;
	}

	public void SelectResurrection10()
	{
		OpenPopup();
		resurrectionX10Obj.SetActive(value: true);
		priceDiamond = 80;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.Resurrection;
		itemCount = 10;
	}

	public void SelectResurrection5()
	{
		OpenPopup();
		resurrectionX5Obj.SetActive(value: true);
		priceDiamond = 45;
		diaPriceText.text = priceDiamond.ToString();
		itemType = UI_Store.EStoreItemType.Resurrection;
		itemCount = 5;
	}

	private UI_Store.BoxPackageInfo GetBoxData(ERewardBox box, int count)
	{
		int num = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel - 1 + 25, 0, 999);
		long num2 = rewardData.dataArray[num].Gold;
		int num4;
		int num5;
		int num3 = (num4 = (num5 = 0));
		switch (box)
		{
		case ERewardBox.BLACK:
			num2 *= 12;
			num4 = 5;
			num5 = 10;
			num3 = 3;
			break;
		case ERewardBox.GOLD:
			num2 *= 8;
			num4 = 3;
			num5 = 6;
			num3 = 2;
			break;
		}
		num2 *= count;
		UI_Store.BoxPackageInfo result = default(UI_Store.BoxPackageInfo);
		result.goldStr = Numbering.Convert(num2);
		result.diaStr = $"{num4 * count}-{num5 * count}";
		result.crewStr = (num3 * 4 * count).ToString();
		return result;
	}

	public void Buy()
	{
		if (DiaHaveCheck())
		{
			switch (itemType)
			{
			case UI_Store.EStoreItemType.BlackChest:
			{
				for (int j = 0; j < itemCount; j++)
				{
					RewardBoxStatus rewardBoxStatus2 = new RewardBoxStatus();
					rewardBoxStatus2.boxType = ERewardBox.BLACK;
					rewardBoxStatus2.level = global::UnityEngine.Mathf.Min(1000, (int)PlayerInfo.Instance.myData.mainShipLevel + 25);
					PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, rewardBoxStatus2);
					PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.sClass);
				}
				UI_Manager.Instance.ui_storeGetCard.Refresh();
				UI_Manager.Instance.ui_storeGetRewardBox.Refresh();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사", "스토어", "블랙박스패키지 x" + itemCount, 1L);
				break;
			}
			case UI_Store.EStoreItemType.GoldChest:
			{
				for (int i = 0; i < itemCount; i++)
				{
					RewardBoxStatus rewardBoxStatus = new RewardBoxStatus();
					rewardBoxStatus.boxType = ERewardBox.GOLD;
					rewardBoxStatus.level = global::UnityEngine.Mathf.Min(1000, (int)PlayerInfo.Instance.myData.mainShipLevel + 25);
					PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, rewardBoxStatus);
					PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.aClass);
				}
				UI_Manager.Instance.ui_storeGetCard.Refresh();
				UI_Manager.Instance.ui_storeGetRewardBox.Refresh();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사", "스토어", "골드박스패키지 x" + itemCount, 1L);
				break;
			}
			case UI_Store.EStoreItemType.Resurrection:
			{
				MyData myData = PlayerInfo.Instance.myData;
				myData.resurrectionTicket = (int)myData.resurrectionTicket + itemCount;
				GoogleAnalyticsV3.instance.LogEvent("다이아 사", "스토어", "부활권 x" + itemCount, 1L);
				break;
			}
			case UI_Store.EStoreItemType.CardA:
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.aClass);
				UI_Manager.Instance.ui_storeGetCard.Refresh();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사", "스토어", "카드 A", 1L);
				break;
			case UI_Store.EStoreItemType.CardB:
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.bClass);
				UI_Manager.Instance.ui_storeGetCard.Refresh();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "스토어", "카드 B", 1L);
				break;
			case UI_Store.EStoreItemType.CardC:
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.cClass);
				UI_Manager.Instance.ui_storeGetCard.Refresh();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "스토어", "카드 C", 1L);
				break;
			case UI_Store.EStoreItemType.CardS:
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.sClass);
				UI_Manager.Instance.ui_storeGetCard.Refresh();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "스토어", "카드 S", 1L);
				break;
			}
			popupObj.GetComponent<PopupAction>().Close();
			AudioManager.Instance.PlaySFX("use_diamond");
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -priceDiamond);
		}
		else
		{
			popupObj.GetComponent<PopupAction>().Close();
			TabOpen(2);
		}
	}

	private bool DiaHaveCheck()
	{
		if ((long)PlayerInfo.Instance.myData.diamond >= priceDiamond)
		{
			return true;
		}
		return false;
	}

	public void TabOpen(int num)
	{
		for (int i = 0; i < panelCanvsGroups.Length; i++)
		{
			if (i == num)
			{
				if (!panelCanvsGroups[i].gameObject.activeSelf)
				{
					panelCanvsGroups[i].gameObject.SetActive(value: true);
					global::DG.Tweening.ShortcutExtensions.DOKill(panelCanvsGroups[i]);
					panelCanvsGroups[i].alpha = 0f;
					global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(panelCanvsGroups[i], 1f, 0.2f), isIndependentUpdate: true);
				}
			}
			else if (panelCanvsGroups[i].gameObject.activeSelf)
			{
				panelCanvsGroups[i].gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < tabButtons.Length; j++)
		{
			if (j == num)
			{
				tabButtons[j].color = global::UnityEngine.Color.white;
			}
			else
			{
				tabButtons[j].color = myColor.GetColor(255, 255, 255, 128);
			}
		}
		AudioManager.PlayButtonClick();
	}

	public void BuyPackStarter()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_pack_starter);
	}

	public void BuyPackToday()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_pack_today);
	}

	public void BuyPackBirds()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_pack_birds);
	}

	public void BuyPackIntermediate()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_pack_inter);
	}

	public void BuyPackExpert()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_pack_expert);
	}

	public void BuyParrot()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_parrot);
	}

	public void BuyEagle()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_eagle);
	}

	public void BuyBlackBird()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_blackbird);
	}

	public void BuyRemoveAds()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_removeads);
	}

	public void BuyCrystal00()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_crystal_00);
	}

	public void BuyCrystal01()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_crystal_01);
	}

	public void BuyCrystal02()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_crystal_02);
	}

	public void BuyCrystal03()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_crystal_03);
	}

	public void BuyCrystal04()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_crystal_04);
	}

	public void BuyCrystal05()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_crystal_05);
	}

	public void BuyShipSkinRaft()
	{
		IapManager.Instance.BuyProductID(IapManager.Instance.inapp_skin_raft);
	}

	private void SetLegendPackRefreshPrice()
	{
		refreshPrice = 5;
		for (int i = 0; i < PlayerInfo.Instance.myData.todayPackRefreshCount; i++)
		{
			refreshPrice *= 2;
		}
		global::UnityEngine.UI.Text[] array = refreshPriceText;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].text = refreshPrice.ToString();
		}
	}

	public void RefreshLegendaryPack()
	{
		if ((long)PlayerInfo.Instance.myData.diamond >= refreshPrice)
		{
			AudioManager.Instance.PlaySFX("use_diamond");
			if (!PlayerInfo.Instance.myData.isTodayPackCanBuy)
			{
				PlayerInfo.Instance.myData.isTodayPackCanBuy = true;
			}
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -refreshPrice);
			PlayerInfo.Instance.myData.todayPackRefreshCount++;
			SetLegendPackRefreshPrice();
			RefreshLegendDataPack(change: true);
			GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "스토어", "전설팩 새로고침", 1L);
		}
	}

	private void RefreshLegendDataPack(bool change = false)
	{
		if (PlayerInfo.Instance.myData.isTodayPackCanBuy)
		{
			btnToday.enabled = true;
			lockToday.SetActive(value: false);
		}
		else
		{
			btnToday.enabled = false;
			lockToday.SetActive(value: true);
		}
		if (!PlayerInfo.Instance.myData.isRefreshTodayPack || change)
		{
			int num = 0;
			int num2 = 100;
			do
			{
				ESailor eSailor = ESailor.END;
				float num3 = global::UnityEngine.Random.Range(0f, 100f);
				eSailor = ((num3 < 17.86f) ? ((ESailor)global::UnityEngine.Random.Range(31, 34)) : ((num3 < 28.57f) ? ESailor.skill_avoidEnvironment : ((ESailor)((!(num3 < 35.71f)) ? global::UnityEngine.Random.Range(37, 39) : global::UnityEngine.Random.Range(35, 37)))));
				if (eSailor != PlayerInfo.Instance.myData.todaySelectedLegendarySailor)
				{
					PlayerInfo.Instance.myData.todaySelectedLegendarySailor = eSailor;
					break;
				}
			}
			while (++num < num2);
			PlayerInfo.Instance.myData.isRefreshTodayPack = true;
		}
		PlayerInfo.Instance.SaveData();
		switch (PlayerInfo.Instance.myData.todaySelectedLegendarySailor)
		{
		case ESailor.skill_meteor:
			selectedCharImage.sprite = meteorSprite;
			break;
		case ESailor.skill_spark:
			selectedCharImage.sprite = sparkSprite;
			break;
		case ESailor.skill_windblast:
			selectedCharImage.sprite = windSprite;
			break;
		case ESailor.skill_avoidEnvironment:
			selectedCharImage.sprite = avoidSprite;
			break;
		case ESailor.skill_unlimitSubship:
			selectedCharImage.sprite = unlimitSubSprite;
			break;
		case ESailor.skill_SubshipGenerateFast:
			selectedCharImage.sprite = fastSpawnSprite;
			break;
		case ESailor.skill_healthDown:
			selectedCharImage.sprite = healthDownSprite;
			break;
		case ESailor.skill_timeIsGold:
			selectedCharImage.sprite = timeIsGoldSprite;
			break;
		}
		BlinkColorImage component = selectedCharBGImage.GetComponent<BlinkColorImage>();
		switch (PlayerInfo.Instance.myData.todaySelectedLegendarySailor)
		{
		case ESailor.skill_meteor:
		case ESailor.skill_avoidEnvironment:
		case ESailor.skill_unlimitSubship:
		case ESailor.skill_healthDown:
			selectedCharBGImage.color = myColor.GetColor(255, 0, 0);
			component.originalColor = myColor.GetColor(255, 0, 0);
			component.originalColor = myColor.GetColor(255, 0, 0, 70);
			break;
		case ESailor.skill_spark:
		case ESailor.skill_timeIsGold:
			selectedCharBGImage.color = myColor.GetColor(255, 255, 0);
			component.originalColor = myColor.GetColor(255, 255, 0);
			component.originalColor = myColor.GetColor(255, 255, 0, 70);
			break;
		case ESailor.skill_windblast:
			selectedCharBGImage.color = myColor.GetColor(186, 0, 255);
			component.originalColor = myColor.GetColor(186, 0, 255);
			component.originalColor = myColor.GetColor(186, 0, 255, 70);
			break;
		case ESailor.skill_SubshipGenerateFast:
			selectedCharBGImage.color = myColor.GetColor(0, 239, 255);
			component.originalColor = myColor.GetColor(0, 239, 255);
			component.originalColor = myColor.GetColor(0, 239, 255, 70);
			break;
		}
		SailorBaseData sailorBaseData = SailorDataManager.Instance.GetSailorBaseData(PlayerInfo.Instance.myData.todaySelectedLegendarySailor);
		legendCharNameText.text = LanguageManager.Instance.translate(sailorBaseData.defaultName);
		legendCharNameText.font = LanguageManager.Instance.getLanguageFont();
		legendCharContentText.text = LanguageManager.Instance.translate(sailorBaseData.contentId);
		legendCharContentText.font = LanguageManager.Instance.getLanguageFont();
	}

	public void OpenSelectedLegend()
	{
		legendGotcha.gameObject.SetActive(value: true);
		ESailor todaySelectedLegendarySailor = PlayerInfo.Instance.myData.todaySelectedLegendarySailor;
		MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(todaySelectedLegendarySailor);
		switch (SailorDataManager.Instance.GetSailorBaseData(todaySelectedLegendarySailor).abilityType)
		{
		case ESailorAbility.Skill_Meteor:
		case ESailorAbility.Skill_AvoidEnvironment:
		case ESailorAbility.Skill_SubshipUnLimit:
		case ESailorAbility.Skill_EnemyHealthDown:
			legendGotcha.SetRed();
			break;
		case ESailorAbility.Skill_Spark:
		case ESailorAbility.Skill_TimeIsGold:
			legendGotcha.SetYellow();
			break;
		case ESailorAbility.Skill_Windblast:
			legendGotcha.SetPurple();
			break;
		case ESailorAbility.Skill_SubshipSpawnTime:
			legendGotcha.SetGreen();
			break;
		}
		legendGotcha.SetPurple();
		legendGotcha.SetCardData(mySailorData);
		MyInventoryDataManager.Instance.AddSailor(todaySelectedLegendarySailor, 1);
	}

	public void Restore()
	{
		IapManager.Instance.RestorePurchases();
	}
}
