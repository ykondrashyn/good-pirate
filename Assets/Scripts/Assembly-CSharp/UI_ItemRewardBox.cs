public class UI_ItemRewardBox : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.Header("images")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image boxImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image backgroundImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image fillImage;

	[global::UnityEngine.Header("texts")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text openNowDiaPriceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelText;

	[global::UnityEngine.Header("gameobjects")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject openObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject openNowObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject needClearObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject showAdsBtnObj;

	[global::UnityEngine.Header("sprites")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite normalSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite canOpenSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite bronzeSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite silverSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite goldSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite blackSprite;

	public int boxSlotNumber = 1;

	private int price;

	private bool isLeftCooltime;

	public RewardBoxStatus myData { get; private set; }

	public string MyCoolTimeKey => $"RewardBoxSlot_{boxSlotNumber}";

	private void Awake()
	{
		if (boxSlotNumber == 0)
		{
			myData = new RewardBoxStatus();
			myData.boxType = ERewardBox.NONE;
		}
		boxImage.color = global::UnityEngine.Color.white;
	}

	private void OnEnable()
	{
		RefreshStatus();
	}

	public void RefreshStatus()
	{
		if (myData != null)
		{
			global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(MyCoolTimeKey);
			openObj.SetActive(value: false);
			openNowObj.SetActive(value: false);
			needClearObj.SetActive(value: false);
			boxImage.gameObject.SetActive(value: true);
			if ((bool)levelText)
			{
				levelText.gameObject.SetActive(value: true);
			}
			if (leftCoolTime.TotalSeconds <= 0.0)
			{
				backgroundImage.sprite = canOpenSprite;
				openObj.SetActive(value: true);
				isLeftCooltime = false;
				fillImage.gameObject.SetActive(value: false);
				if ((bool)showAdsBtnObj)
				{
					showAdsBtnObj.SetActive(value: false);
				}
				return;
			}
			if (boxSlotNumber == 0)
			{
				if (PlayerInfo.Instance.myData.isWatchedFreeBoxAds)
				{
					showAdsBtnObj.SetActive(value: false);
				}
				else
				{
					showAdsBtnObj.SetActive(value: true);
				}
			}
			openNowObj.SetActive(value: true);
			isLeftCooltime = true;
			backgroundImage.sprite = normalSprite;
			fillImage.gameObject.SetActive(value: true);
		}
		else
		{
			if ((bool)levelText)
			{
				levelText.gameObject.SetActive(value: false);
			}
			fillImage.gameObject.SetActive(value: false);
			isLeftCooltime = false;
			backgroundImage.sprite = normalSprite;
			openObj.SetActive(value: false);
			openNowObj.SetActive(value: false);
			needClearObj.SetActive(value: true);
			boxImage.gameObject.SetActive(value: false);
		}
	}

	private void LateUpdate()
	{
		if (isLeftCooltime)
		{
			global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(MyCoolTimeKey);
			price = (int)(leftCoolTime.TotalSeconds / 60.0) + 1;
			if ((bool)openNowDiaPriceText)
			{
				openNowDiaPriceText.text = Numbering.Convert(price);
			}
			int num = 0;
			switch (myData.boxType)
			{
			case ERewardBox.NONE:
				num = ConstValue.rewardFreeBoxTime;
				break;
			case ERewardBox.BRONZE:
				num = ConstValue.rewardBoxBronzeTime;
				break;
			case ERewardBox.SILVER:
				num = ConstValue.rewardBoxSilverTime;
				break;
			case ERewardBox.GOLD:
				num = ConstValue.rewardBoxGoldTime;
				break;
			case ERewardBox.BLACK:
				num = ConstValue.rewardBoxBlackTime;
				break;
			}
			double num2 = leftCoolTime.TotalSeconds / (double)num;
			fillImage.fillAmount = (float)num2;
			if (leftCoolTime.Hours > 0)
			{
				leftTimeText.text = $"{leftCoolTime.Hours}:{leftCoolTime.Minutes}:{leftCoolTime.Seconds}";
			}
			else
			{
				leftTimeText.text = $"{leftCoolTime.Minutes:D2}:{leftCoolTime.Seconds:D2}";
			}
			if (leftCoolTime.TotalSeconds <= 0.0)
			{
				isLeftCooltime = false;
				RefreshStatus();
			}
		}
	}

	public void Open()
	{
		UI_Manager.Instance.ui_itemBoxPopup.gameObject.SetActive(value: true);
		if (boxSlotNumber == 0)
		{
			UI_Manager.Instance.ui_itemBoxPopup.SetIsFreeBox(isTrue: true);
		}
		else
		{
			UI_Manager.Instance.ui_itemBoxPopup.SetIsFreeBox(isTrue: false);
		}
		UI_Manager.Instance.ui_itemBoxPopup.SetItem(myData);
		UI_Manager.Instance.ui_itemBoxPopup.Open(myData.boxType);
		if (boxSlotNumber == 0)
		{
			int coolTime = 0;
			switch (myData.boxType)
			{
			case ERewardBox.NONE:
				coolTime = ConstValue.rewardFreeBoxTime;
				break;
			case ERewardBox.BRONZE:
				coolTime = ConstValue.rewardBoxBronzeTime;
				break;
			case ERewardBox.SILVER:
				coolTime = ConstValue.rewardBoxSilverTime;
				break;
			case ERewardBox.GOLD:
				coolTime = ConstValue.rewardBoxGoldTime;
				break;
			case ERewardBox.BLACK:
				coolTime = ConstValue.rewardBoxBlackTime;
				break;
			}
			CoolTimeManager.AddCooltimeSecondFromNow(MyCoolTimeKey, coolTime);
		}
		else
		{
			CoolTimeManager.DeleteCoolTime(MyCoolTimeKey);
			PlayerInfo.Instance.myData.rewardBoxStatus[boxSlotNumber - 1].boxType = ERewardBox.NONE;
			PlayerInfo.Instance.myData.rewardBoxStatus[boxSlotNumber - 1].level = 1;
			myData = null;
			PlayerInfo.Instance.SaveData();
		}
		RefreshStatus();
	}

	public void OpenNow()
	{
		CoolTimeManager.GetLeftCoolTime(MyCoolTimeKey);
		if ((long)PlayerInfo.Instance.myData.diamond >= price)
		{
			CoolTimeManager.DeleteCoolTime(MyCoolTimeKey);
			AudioManager.Instance.PlaySFX("use_diamond");
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -price);
			RefreshStatus();
			switch (myData.boxType)
			{
			case ERewardBox.BLACK:
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "상자 즉시오픈", "블랙", 1L);
				break;
			case ERewardBox.BRONZE:
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "상자 즉시오픈", "브론즈", 1L);
				break;
			case ERewardBox.GOLD:
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "상자 즉시오픈", "골드", 1L);
				break;
			case ERewardBox.SILVER:
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "상자 즉시오픈", "실버", 1L);
				break;
			}
		}
		else
		{
			UI_Manager.Instance.inappObj.gameObject.SetActive(value: true);
			UI_Manager.Instance.inappObj.TabOpen(2);
		}
	}

	public void ShowAdsMinus10Min()
	{
		if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
		{
			AdsSuccess();
			return;
		}
		MoPubAdManager.Instance.ShowRewardedVideo(AdsSuccess);
		GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "무료상자 단축", 1L);
	}

	private void AdsSuccess()
	{
		global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(MyCoolTimeKey);
		global::System.TimeSpan ts = new global::System.TimeSpan(0, 10, 0);
		global::System.TimeSpan timeSpan = leftCoolTime.Subtract(ts);
		CoolTimeManager.AddCooltimeSecondFromNow(MyCoolTimeKey, (int)timeSpan.TotalSeconds);
		PlayerInfo.Instance.myData.isWatchedFreeBoxAds = true;
		PlayerInfo.Instance.SaveData();
		RefreshStatus();
	}

	public void SetData(RewardBoxStatus boxData)
	{
		int coolTime = 0;
		myData = boxData;
		if (myData.boxType == ERewardBox.NONE)
		{
			myData = null;
			RefreshStatus();
			return;
		}
		levelText.text = "Lv." + myData.level;
		switch (boxData.boxType)
		{
		case ERewardBox.BRONZE:
			coolTime = ConstValue.rewardBoxBronzeTime;
			boxImage.sprite = bronzeSprite;
			break;
		case ERewardBox.SILVER:
			coolTime = ConstValue.rewardBoxSilverTime;
			boxImage.sprite = silverSprite;
			break;
		case ERewardBox.GOLD:
			coolTime = ConstValue.rewardBoxGoldTime;
			boxImage.sprite = goldSprite;
			break;
		case ERewardBox.BLACK:
			coolTime = ConstValue.rewardBoxBlackTime;
			boxImage.sprite = blackSprite;
			break;
		}
		if (!CoolTimeManager.HaveKey(MyCoolTimeKey))
		{
			CoolTimeManager.AddCooltimeSecondFromNow(MyCoolTimeKey, coolTime);
		}
	}
}
