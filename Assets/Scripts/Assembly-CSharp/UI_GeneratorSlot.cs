public class UI_GeneratorSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image generateGauge;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text generateLeftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image priceButtonImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text spawnText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text spawnPriceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text diamondText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text needCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text generateTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject buyButtonObject;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject diaCompleteButtonObject;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lockObjectsObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text lockMessage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject newObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button spawnBtn;

	private global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();

	private bool isHave;

	private EShipType shipType;

	private ShipBaseData shipData;

	private CoolTimeData coolData;

	private SubShipUpgradePrice upgradePriceData;

	private bool isTutoSlot;

	private void Awake()
	{
		generateGauge.gameObject.SetActive(value: false);
		upgradePriceData = ShipDataManager.Instance.subShipUpgradePrice;
	}

	private void OnEnable()
	{
		if (coolData != null)
		{
			Check();
			PlayerInfo instance = PlayerInfo.Instance;
			instance.onMoneyModify = (PlayerInfo.eventCallBack)global::System.Delegate.Combine(instance.onMoneyModify, new PlayerInfo.eventCallBack(Check));
		}
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onMoneyModify = (PlayerInfo.eventCallBack)global::System.Delegate.Remove(instance.onMoneyModify, new PlayerInfo.eventCallBack(Check));
		if (PlayerInfo.Instance.GetIsNew(shipType))
		{
			PlayerInfo.Instance.SetIsNew(shipType, isNew: false);
		}
	}

	public void SetData(EShipType shipType)
	{
		this.shipType = shipType;
		shipData = ShipDataManager.Instance.GetShipBaseData(this.shipType);
		int num = shipData.generateTime;
		global::System.Collections.Generic.List<ESailor> skill_subshipSpawntime = SailorDataManager.Instance.skill_subshipSpawntime;
		if (skill_subshipSpawntime.Count > 0)
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_subshipSpawntime[0]);
			if (mySailorData.isHave)
			{
				switch (mySailorData.level)
				{
				case 1:
					num = global::UnityEngine.Mathf.Max(1, (int)((float)num - (float)num * 0.2f));
					break;
				case 2:
					num = global::UnityEngine.Mathf.Max(1, (int)((float)num - (float)num * 0.4f));
					break;
				}
			}
		}
		UI_GeneratorManager.Instance.AddGenerateAction(this.shipType, num, Generate, out coolData);
		EShipSub level = (EShipSub)global::UnityEngine.Mathf.Clamp(PlayerInfo.Instance.GetShipLevel(this.shipType) / 100 + 1, 1, 10);
		iconImage.sprite = ShipDataManager.Instance.GetSubShipData(this.shipType, level).icon;
		global::UnityEngine.UI.Text[] array = nameText;
		foreach (global::UnityEngine.UI.Text obj in array)
		{
			obj.font = LanguageManager.Instance.getLanguageFont();
			obj.text = LanguageManager.Instance.translate(shipData.shipName);
		}
		Check();
	}

	public void Refresh()
	{
		Check();
	}

	public void Generate()
	{
		CoolTimeManager.DeleteCoolTime(shipData.id.ToString());
		Invoke("RealGenerate", 0f);
	}

	private void RealGenerate()
	{
		AudioManager.Instance.PlaySFX("ship_spawn");
		MyData myData = PlayerInfo.Instance.myData;
		myData.reserveShipCount = (int)myData.reserveShipCount - shipData.useCount;
		PlayerInfo.Instance.myData.reserveShipCount = global::UnityEngine.Mathf.Max(0, PlayerInfo.Instance.myData.reserveShipCount);
		PlayerInfo.Instance.SaveData();
		MyShipInventory myShipInventory = MyInventoryDataManager.Instance.AddNewShip(shipType, autoEquip: true);
		if (myShipInventory != null)
		{
			if (!ClaimManager.Instance.IsPlaying)
			{
				GameManager.Instance.playerShipController.AddSubShip(myShipInventory);
			}
			generateGauge.gameObject.SetActive(value: false);
			diaCompleteButtonObject.SetActive(value: false);
			buyButtonObject.SetActive(value: true);
		}
		Check();
	}

	public void UseDiamondComplete()
	{
		int leftTotalSeconds = coolData.LeftTotalSeconds;
		int num = global::UnityEngine.Mathf.Max(1, leftTotalSeconds / 60 + 1);
		if ((long)PlayerInfo.Instance.myData.diamond >= num)
		{
			AudioManager.Instance.PlaySFX("use_diamond");
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -num);
			coolData.isActive = false;
			CoolTimeManager.DeleteCoolTime(shipType.ToString());
			Generate();
			buyButtonObject.SetActive(value: true);
			diaCompleteButtonObject.SetActive(value: false);
			GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "보조배 소환", "보조배 소환", 1L);
		}
		else
		{
			global::UnityEngine.Debug.Log("스토어 팝업");
		}
	}

	public void RequestGenerate()
	{
		if (PlayerInfo.Instance.UseShipCount + shipData.useCount > PlayerInfo.Instance.MaxSubShipCount)
		{
			UI_SpawnShipListManager componentInParent = GetComponentInParent<UI_SpawnShipListManager>();
			if (componentInParent != null)
			{
				componentInParent.Alarm();
			}
			AudioManager.Instance.PlaySFX("error");
		}
		else
		{
			if (!IsCanSpawn())
			{
				return;
			}
			global::UnityEngine.Mathf.Clamp(PlayerInfo.Instance.GetShipLevel(shipType) - 1, 0, 999);
			int num = shipData.generateTime;
			global::System.Collections.Generic.List<ESailor> skill_subshipSpawntime = SailorDataManager.Instance.skill_subshipSpawntime;
			if (skill_subshipSpawntime.Count > 0)
			{
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_subshipSpawntime[0]);
				if (mySailorData.isHave)
				{
					switch (mySailorData.level)
					{
					case 1:
						num = global::UnityEngine.Mathf.Max(1, (int)((float)num - (float)num * 0.2f));
						break;
					case 2:
						num = global::UnityEngine.Mathf.Max(1, (int)((float)num - (float)num * 0.4f));
						break;
					}
				}
			}
			if (isTutoSlot)
			{
				UI_GeneratorManager.Instance.AddGenerateAction(shipType, shipData.generateTime, null, out coolData);
			}
			else
			{
				UI_GeneratorManager.Instance.AddGenerateAction(shipType, num, Generate, out coolData);
			}
			generateGauge.gameObject.SetActive(value: true);
			generateGauge.fillAmount = 1f;
			buyButtonObject.SetActive(value: false);
			diaCompleteButtonObject.SetActive(value: true);
			PlayerInfo.Instance.SaveData();
			AudioManager.Instance.PlaySFX("ship_spawn_request");
		}
	}

	public void Check()
	{
		int num = shipData.generateTime;
		global::System.Collections.Generic.List<ESailor> skill_subshipSpawntime = SailorDataManager.Instance.skill_subshipSpawntime;
		if (skill_subshipSpawntime.Count > 0)
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_subshipSpawntime[0]);
			if (mySailorData.isHave)
			{
				switch (mySailorData.level)
				{
				case 1:
					num = global::UnityEngine.Mathf.Max(1, (int)((float)num - (float)num * 0.2f));
					break;
				case 2:
					num = global::UnityEngine.Mathf.Max(1, (int)((float)num - (float)num * 0.4f));
					break;
				}
			}
		}
		int shipLevel = PlayerInfo.Instance.GetShipLevel(shipType);
		levelText.text = "Lv. " + shipLevel;
		needCountText.text = shipData.useCount.ToString();
		newObj.SetActive(PlayerInfo.Instance.GetIsNew(shipType));
		generateTimeText.text = $"{num / 60}:{num % 60:00}";
		isHave = PlayerInfo.Instance.IsHaveSubShip(shipType);
		if (isHave)
		{
			int level = global::UnityEngine.Mathf.Clamp(shipLevel / 100 + 1, 1, 10);
			ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(shipData.id, (EShipSub)level);
			if (subShipData == null || subShipData.modelSrc == null)
			{
				subShipData = ShipDataManager.Instance.GetSubShipData(shipData.id, EShipSub.LEVEL_01);
			}
			shipLevel = global::UnityEngine.Mathf.Clamp(shipLevel - 1, 0, 999);
			ShipDataManager.Instance.GetShipSheetData(shipData.id, shipLevel);
			if (upgradePriceData == null)
			{
				upgradePriceData = ShipDataManager.Instance.subShipUpgradePrice;
			}
			lockObjectsObj.SetActive(value: false);
			if (coolData.isActive)
			{
				generateGauge.gameObject.SetActive(value: true);
				float leftTimeRatio = coolData.LeftTimeRatio;
				generateGauge.fillAmount = leftTimeRatio;
				buyButtonObject.SetActive(value: false);
				diaCompleteButtonObject.SetActive(value: true);
				return;
			}
			generateGauge.gameObject.SetActive(value: false);
			buyButtonObject.SetActive(value: true);
			diaCompleteButtonObject.SetActive(value: false);
			if (IsCanSpawn())
			{
				spawnText.color = global::UnityEngine.Color.white;
				priceButtonImage.color = myColor.GetColor(0, 188, 64);
				spawnBtn.enabled = true;
			}
			else
			{
				spawnText.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.4f);
				priceButtonImage.color = myColor.GetColor(118, 203, 161);
				spawnBtn.enabled = false;
			}
		}
		else
		{
			diaCompleteButtonObject.SetActive(value: false);
			buyButtonObject.SetActive(value: false);
			lockObjectsObj.SetActive(value: true);
			lockMessage.text = $"{PlayerInfo.Instance.ClearIslandCount} / {shipData.unlockIslandCount}";
		}
	}

	private void LateUpdate()
	{
		if (coolData == null)
		{
			return;
		}
		if (!coolData.isActive)
		{
			if (generateGauge.gameObject.activeSelf)
			{
				generateGauge.gameObject.SetActive(value: false);
			}
			return;
		}
		float leftTimeRatio = coolData.LeftTimeRatio;
		generateGauge.fillAmount = leftTimeRatio;
		int leftTotalSeconds = coolData.LeftTotalSeconds;
		diamondText.text = global::UnityEngine.Mathf.Max(1, leftTotalSeconds / 60 + 1).ToString();
		sb.Length = 0;
		sb.Append($"{leftTotalSeconds / 60:D2}:{leftTotalSeconds % 60:D2}");
		generateLeftTimeText.text = sb.ToString();
	}

	private bool IsCanSpawn()
	{
		return true;
	}

	private bool IsCanStudy()
	{
		return true;
	}

	public void TutoDataSet()
	{
		isTutoSlot = true;
		shipType = EShipType.Normal;
		shipData = ShipDataManager.Instance.GetShipBaseData(shipType);
		EShipSub level = (EShipSub)(PlayerInfo.Instance.GetShipLevel(shipType) / 100 + 1);
		iconImage.sprite = ShipDataManager.Instance.GetSubShipData(shipType, level).icon;
		global::UnityEngine.UI.Text[] array = nameText;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = shipData.shipName;
		}
		generateTimeText.text = $"{shipData.generateTime / 60}:{shipData.generateTime % 60:00}";
		coolData = UI_GeneratorManager.Instance.coolTimeData[shipType];
		Check();
	}
}
