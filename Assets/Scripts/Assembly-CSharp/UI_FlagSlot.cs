public class UI_FlagSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.RawImage myFlagImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject selectObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject unSelectObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject priceObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image priceIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image abilityIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text abilityText;

	[global::UnityEngine.Header("sprites")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite diaSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite adsSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite islandSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite claimSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite accelerationSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite attackSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite cooltimeSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite rotationSprite;

	[global::UnityEngine.HideInInspector]
	public UI_FlagPopup popupManager;

	public FlagInfo myData { get; private set; }

	public void SetFlagData(FlagInfo info)
	{
		myData = info;
		myFlagImage.texture = info.flagTexture;
		Refresh();
		switch (info.abilityType)
		{
		case ESailorAbility.ACCELERATION:
			abilityIcon.sprite = accelerationSprite;
			break;
		case ESailorAbility.ATTACK:
			abilityIcon.sprite = attackSprite;
			break;
		case ESailorAbility.COOLTIME:
			abilityIcon.sprite = cooltimeSprite;
			break;
		case ESailorAbility.ROTATION:
			abilityIcon.sprite = rotationSprite;
			break;
		}
		abilityText.text = string.Format("+{0}%", info.abilityValue.ToString("F1"));
		if (myData.id == 0)
		{
			abilityIcon.gameObject.SetActive(value: false);
			abilityText.gameObject.SetActive(value: false);
		}
	}

	public void SelectThis()
	{
		if (popupManager == null)
		{
			global::UnityEngine.Debug.Log("FlagPopupManager is NULL");
		}
		else
		{
			popupManager.SelectedThis(this);
		}
	}

	public void SetSelect(bool active)
	{
		if ((bool)selectObj)
		{
			selectObj.SetActive(active);
		}
		if ((bool)unSelectObj)
		{
			unSelectObj.SetActive(!active);
		}
	}

	public void Refresh()
	{
		if (PlayerInfo.Instance.IsHaveFlag(myData.id))
		{
			myFlagImage.color = global::UnityEngine.Color.white;
			priceObj.SetActive(value: false);
		}
		else
		{
			priceObj.SetActive(value: true);
			switch (myData.priceType)
			{
			case EFlagPriceType.ADS:
				priceIcon.sprite = adsSprite;
				priceText.text = $"{PlayerInfo.Instance.GetFlagAdsCount(myData.id)}/{myData.price}";
				priceIcon.SetNativeSize();
				break;
			case EFlagPriceType.DIAMOND:
				priceIcon.sprite = diaSprite;
				priceText.text = myData.price.ToString();
				priceIcon.SetNativeSize();
				break;
			case EFlagPriceType.ISLAND:
				priceIcon.sprite = islandSprite;
				priceText.text = $"{PlayerInfo.Instance.ClearIslandCount}/{myData.price}";
				priceIcon.rectTransform.sizeDelta = global::UnityEngine.Vector2.one * 57f;
				break;
			case EFlagPriceType.CLAIM:
				priceIcon.sprite = claimSprite;
				priceText.text = $"{PlayerInfo.Instance.ClearIslandClaimCount}/{myData.price}";
				priceIcon.rectTransform.sizeDelta = global::UnityEngine.Vector2.one * 57f;
				break;
			}
			myFlagImage.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.3f);
		}
		if (myData.id == (int)PlayerInfo.Instance.myData.selectedFlagId)
		{
			SetSelect(active: true);
		}
		else
		{
			SetSelect(active: false);
		}
	}
}
