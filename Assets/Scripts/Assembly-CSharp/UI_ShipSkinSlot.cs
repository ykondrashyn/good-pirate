public class UI_ShipSkinSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public UI_ShipSkinManager manager;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform buyTextRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnEquip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnUnequip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnBuy;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnLimit;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image firstAbilityIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text firstAbilityText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image secondAbilityIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text secondAbilityText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject raftPriceObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelLimitText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite accelSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite atkSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite coolSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite rotateSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image skinIcon;

	public bool isCanBuy;

	private ShipSkinData myData;

	public void SetData(ShipSkinData data)
	{
		myData = data;
		skinIcon.sprite = myData.icon;
		nameText.text = LanguageManager.Instance.translate(myData.nameId);
		AbilityIcon(myData.abilityType, firstAbilityIcon);
		AbilityIcon(myData.abilityType2, secondAbilityIcon);
		firstAbilityText.text = $"+{(int)data.abilityValue}%";
		secondAbilityText.text = $"+{(int)data.abilityValue2}%";
		levelLimitText.text = $"Lv.{myData.levelLimit}";
		Refresh();
	}

	public void ShowPreview()
	{
		manager.Preview(myData);
	}

	public void Refresh()
	{
		btnUnequip.gameObject.SetActive(value: false);
		btnLimit.gameObject.SetActive(value: false);
		btnEquip.gameObject.SetActive(value: false);
		btnBuy.gameObject.SetActive(value: false);
		if (PlayerInfo.Instance.GetIsHaveShipSkin(myData.skin))
		{
			if (PlayerInfo.Instance.myData.selectedSkin == myData.skin)
			{
				btnUnequip.gameObject.SetActive(value: true);
				btnEquip.gameObject.SetActive(value: false);
			}
			else
			{
				btnEquip.gameObject.SetActive(value: true);
				btnUnequip.gameObject.SetActive(value: false);
			}
			return;
		}
		btnBuy.gameObject.SetActive(value: true);
		if (myData.skin == eShipSkin.로빈슨의땟목)
		{
			buyTextRect.anchoredPosition = global::UnityEngine.Vector3.zero;
			priceText.gameObject.SetActive(value: false);
		}
		else
		{
			buyTextRect.anchoredPosition = new global::UnityEngine.Vector3(0f, 39.1f, 0f);
			raftPriceObj.SetActive(value: false);
			priceText.gameObject.SetActive(value: true);
			priceText.text = Numbering.Convert(myData.price);
		}
		if (myData.levelLimit <= (int)PlayerInfo.Instance.myData.mainShipLevel)
		{
			isCanBuy = true;
			btnBuy.enabled = true;
			btnLimit.gameObject.SetActive(value: false);
		}
		else
		{
			isCanBuy = false;
			btnBuy.enabled = false;
			btnLimit.gameObject.SetActive(value: true);
		}
	}

	public void Buy()
	{
		if (myData.skin == eShipSkin.로빈슨의땟목)
		{
			UI_Manager.Instance.inappObj.gameObject.SetActive(value: true);
			UI_Manager.Instance.inappObj.TabOpen(1);
			GetComponentInParent<PopupAction>().Close();
		}
		else if (isCanBuy)
		{
			if ((long)PlayerInfo.Instance.myData.diamond >= myData.price)
			{
				PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -myData.price);
				PlayerInfo.Instance.BuyShipSkin(myData.skin);
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "스킨", myData.skin.ToString(), 0L);
				Equip();
			}
			else
			{
				UI_Manager.Instance.inappObj.gameObject.SetActive(value: true);
				UI_Manager.Instance.inappObj.TabOpen(2);
				GetComponentInParent<PopupAction>().Close();
			}
		}
	}

	public void Equip()
	{
		PlayerInfo.Instance.myData.selectedSkin = myData.skin;
		PlayerInfo.Instance.SaveData();
		GameManager.Instance.playerShipController.RefreshModel();
		manager.AllRefresh();
		ShowPreview();
	}

	public void Unequip()
	{
		PlayerInfo.Instance.myData.selectedSkin = eShipSkin.Normal;
		PlayerInfo.Instance.SaveData();
		GameManager.Instance.playerShipController.RefreshModel();
		manager.AllRefresh();
		manager.ModelRefresh();
	}

	private void AbilityIcon(ESailorAbility type, global::UnityEngine.UI.Image img)
	{
		switch (type)
		{
		case ESailorAbility.ACCELERATION:
			img.sprite = accelSprite;
			break;
		case ESailorAbility.ATTACK:
			img.sprite = atkSprite;
			break;
		case ESailorAbility.COOLTIME:
			img.sprite = coolSprite;
			break;
		case ESailorAbility.ROTATION:
			img.sprite = rotateSprite;
			break;
		}
	}
}
