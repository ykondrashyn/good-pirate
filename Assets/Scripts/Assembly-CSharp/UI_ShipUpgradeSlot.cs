public class UI_ShipUpgradeSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelBeforeAfterText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] priceIconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] priceCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject levelMaxObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject levelNotMaxObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text hpText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text attackText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text cannonText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text coolTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image buttonImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button upgradeBtn;

	[global::UnityEngine.SerializeField]
	private UI_CustomButton upgradeCustomBtn;

	private EShipType myType;

	private ShipBaseData myBaseData;

	private SubShipUpgradePrice upgradePriceData;

	private MainUpgradePrice upgradeMainPriceData;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup shipCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] haveObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] notHaveObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text unlockMessageText;

	public void SetData(EShipType type)
	{
		myType = type;
		if (myType == EShipType.Main)
		{
			upgradeMainPriceData = global::UnityEngine.Resources.Load<MainUpgradePrice>("Data/MainUpgradePrice");
		}
		else
		{
			upgradePriceData = ShipDataManager.Instance.subShipUpgradePrice;
		}
		DataRefresh();
	}

	public void DataRefresh()
	{
		EShipSub level = (EShipSub)global::UnityEngine.Mathf.Clamp(PlayerInfo.Instance.GetShipLevel(myType) / 100 + 1, 1, 10);
		iconImage.sprite = ShipDataManager.Instance.GetSubShipData(myType, level).icon;
		myBaseData = ShipDataManager.Instance.GetShipBaseData(myType);
		if (myType != EShipType.Main && !PlayerInfo.Instance.IsHaveSubShip(myType))
		{
			shipCanvasGroup.alpha = 0.4f;
			global::UnityEngine.GameObject[] array = haveObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = notHaveObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			unlockMessageText.text = $"{PlayerInfo.Instance.ClearIslandCount} / {myBaseData.unlockIslandCount}";
		}
		else
		{
			global::UnityEngine.GameObject[] array = haveObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			array = notHaveObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			shipCanvasGroup.alpha = 1f;
		}
		int num = global::UnityEngine.Mathf.Max(1, PlayerInfo.Instance.GetShipLevel(myType));
		if (myType == EShipType.Main)
		{
			num = PlayerInfo.Instance.myData.mainShipLevel;
		}
		nameText.font = LanguageManager.Instance.getLanguageFont();
		nameText.text = LanguageManager.Instance.translate(myBaseData.shipName);
		if (num >= 1000)
		{
			levelText.text = "Lv. MAX";
		}
		else
		{
			levelText.text = $"Lv. {num}";
		}
		ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(myBaseData.id, level);
		if (subShipData == null || subShipData.modelSrc == null)
		{
			subShipData = ShipDataManager.Instance.GetSubShipData(myBaseData.id, EShipSub.LEVEL_01);
		}
		num = global::UnityEngine.Mathf.Clamp(num - 1, 0, 999);
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(myBaseData.id, num);
		hpText.text = Numbering.Convert(shipSheetData.health);
		attackText.text = Numbering.Convert(shipSheetData.attack);
		if (myType == EShipType.Main)
		{
			cannonText.text = "x" + ShipDataManager.Instance.GetMainData((EShipMain)level).cannonCount;
		}
		else
		{
			cannonText.text = "x" + subShipData.cannonCount;
		}
		coolTimeText.text = string.Format("{0}{1}", shipSheetData.fireCoolTime.ToString("F1"), LanguageManager.Instance.translate("key_sec"));
		long num2 = ((myType != EShipType.Main) ? upgradePriceData.dataArray[num].Gold : upgradeMainPriceData.dataArray[num].Gold);
		switch (myBaseData.id)
		{
		case EShipType.Normal:
			num2 = (long)((float)num2 * 0.4f);
			break;
		case EShipType.Cannon:
			num2 = (long)((float)num2 * 0.5f);
			break;
		case EShipType.Rapid:
			num2 = (long)((float)num2 * 0.5f);
			break;
		case EShipType.Gas:
			num2 = (long)((float)num2 * 0.5f);
			break;
		case EShipType.Guide:
			num2 = (long)((float)num2 * 0.6f);
			break;
		case EShipType.Multi:
			num2 = (long)((float)num2 * 0.6f);
			break;
		}
		priceText[0].text = Numbering.Convert(num2);
		priceIconImage[0].sprite = UI_Manager.Instance.GetMoneyIcon(EMonenyType.gold);
		if (IsCanSpawn())
		{
			upgradeCustomBtn.enabled = true;
			priceCanvasGroup[0].alpha = 1f;
			return;
		}
		upgradeCustomBtn.enabled = false;
		if ((long)PlayerInfo.Instance.myData.gold < num2)
		{
			priceCanvasGroup[0].alpha = 0.4f;
		}
		else
		{
			priceCanvasGroup[0].alpha = 1f;
		}
	}

	public void Upgrade()
	{
		if (IsCanSpawn())
		{
			int num = global::UnityEngine.Mathf.Max(1, PlayerInfo.Instance.GetShipLevel(myType));
			if (myType == EShipType.Main)
			{
				num = PlayerInfo.Instance.myData.mainShipLevel;
			}
			global::System.Collections.Generic.List<ShipSubController> subShipList = GameManager.Instance.playerShipController.subShipList;
			EShipSub level = (EShipSub)global::UnityEngine.Mathf.Clamp((num + 1) / 100 + 1, 1, 10);
			ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(myType, level);
			foreach (ShipSubController item in subShipList)
			{
				if (item.myShip.baseData.id == myType)
				{
					float hpRatio = item.myShip.myHealth.HpRatio;
					item.myShip.SetLevel(num + 1);
					int hP_MAX = item.myShip.myHealth.HP_MAX;
					int health = (int)((float)hP_MAX * hpRatio);
					item.myShip.myHealth.SetHealthPoint(health, hP_MAX);
					if ((num + 1) % 100 == 0)
					{
						item.myShip.SpawnShipModel(subShipData.modelSrc, ELayerType.Player);
						DataRefresh();
					}
				}
			}
			PlayerInfo.Instance.SetShipLevel(myType, num + 1);
			num = global::UnityEngine.Mathf.Clamp(num - 1, 0, 999);
			long num2 = upgradePriceData.dataArray[num].Gold;
			switch (myBaseData.id)
			{
			case EShipType.Normal:
				num2 = (long)((float)num2 * 0.4f);
				break;
			case EShipType.Cannon:
				num2 = (long)((float)num2 * 0.5f);
				break;
			case EShipType.Rapid:
				num2 = (long)((float)num2 * 0.5f);
				break;
			case EShipType.Gas:
				num2 = (long)((float)num2 * 0.5f);
				break;
			case EShipType.Guide:
				num2 = (long)((float)num2 * 0.6f);
				break;
			case EShipType.Multi:
				num2 = (long)((float)num2 * 0.6f);
				break;
			}
			PlayerInfo.Instance.AddMoney(EMonenyType.gold, -num2, isSave: false);
			PlayerInfo.Instance.SaveData();
			DataRefresh();
			global::DG.Tweening.ShortcutExtensions.DOKill(iconImage.rectTransform);
			iconImage.rectTransform.localScale = global::UnityEngine.Vector3.one;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(iconImage.rectTransform, 1.2f, 0.1f), delegate
			{
				global::DG.Tweening.ShortcutExtensions.DOScale(iconImage.rectTransform, 1f, 0.1f);
			});
			AudioManager.Instance.PlaySFX("ui_upgrade");
		}
		else
		{
			global::UnityEngine.Debug.Log("돈부족");
		}
	}

	private bool IsCanSpawn()
	{
		bool result = true;
		int num = global::UnityEngine.Mathf.Max(1, PlayerInfo.Instance.GetShipLevel(myType));
		if (myType == EShipType.Main)
		{
			num = PlayerInfo.Instance.myData.mainShipLevel;
		}
		num = global::UnityEngine.Mathf.Clamp(num - 1, 0, 999);
		if (num >= 999)
		{
			levelMaxObj.SetActive(value: true);
			levelNotMaxObj.SetActive(value: false);
			return false;
		}
		long num2 = ((myType != EShipType.Main) ? upgradePriceData.dataArray[num].Gold : upgradeMainPriceData.dataArray[num].Gold);
		switch (myBaseData.id)
		{
		case EShipType.Normal:
			num2 = (long)((float)num2 * 0.4f);
			break;
		case EShipType.Cannon:
			num2 = (long)((float)num2 * 0.5f);
			break;
		case EShipType.Rapid:
			num2 = (long)((float)num2 * 0.5f);
			break;
		case EShipType.Gas:
			num2 = (long)((float)num2 * 0.5f);
			break;
		case EShipType.Guide:
			num2 = (long)((float)num2 * 0.6f);
			break;
		case EShipType.Multi:
			num2 = (long)((float)num2 * 0.6f);
			break;
		}
		if ((long)PlayerInfo.Instance.myData.gold < num2)
		{
			result = false;
		}
		return result;
	}
}
