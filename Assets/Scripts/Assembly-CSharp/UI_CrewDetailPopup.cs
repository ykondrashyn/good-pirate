public class UI_CrewDetailPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private UI_CrewSlot card;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewTypeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color textColorS;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color textColorA;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color textColorB;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color textColorC;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button upgradeBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image upgradeBtnImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] btnTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image priceIconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text abilityTextBefore;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text abilityTextAfter;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] abilityIconImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] maxLevelOnOffObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform abilityTextRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite attackSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite cooltimeSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite accelerationSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite rotationSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] inappOffObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject inappAbilityObj;

	private UI_CrewSlotManager mySlotMangaer;

	private void Awake()
	{
		mySlotMangaer = GetComponentInParent<UI_CrewSlotManager>();
		crewTypeText.font = LanguageManager.Instance.getLanguageFont();
	}

	public void SetData(UI_CrewSlot slot)
	{
		card.SetData(slot.myData, null);
		RefreshData();
	}

	public void Upgrade()
	{
		MyInventoryDataManager.Instance.UpgradeSailor(card.myData.sailorType);
		RefreshData();
		mySlotMangaer.AllRefresh();
		mySlotMangaer.TextRefresh();
	}

	private void RefreshData()
	{
		card.Refresh();
		switch (card.myData.baseData.grade)
		{
		case EGrade.cClass:
			crewTypeText.color = textColorC;
			break;
		case EGrade.bClass:
			crewTypeText.color = textColorB;
			break;
		case EGrade.aClass:
			crewTypeText.color = textColorA;
			break;
		case EGrade.sClass:
			crewTypeText.color = textColorS;
			break;
		}
		if (card.myData.level >= card.myData.baseData.maxCount)
		{
			global::UnityEngine.GameObject[] array = maxLevelOnOffObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			abilityTextRect.anchoredPosition = new global::UnityEngine.Vector2(91f, -261f);
		}
		else
		{
			global::UnityEngine.GameObject[] array = maxLevelOnOffObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			abilityTextRect.anchoredPosition = new global::UnityEngine.Vector2(-50f, -261f);
			if (card.myData.baseData.id == ESailor.boatswain_s_inappp)
			{
				array = inappOffObjs;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: false);
				}
				inappAbilityObj.SetActive(value: true);
			}
			else
			{
				array = inappOffObjs;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(value: true);
				}
				inappAbilityObj.SetActive(value: false);
			}
		}
		ESailorAbility abilityType = card.myData.baseData.abilityType;
		if ((uint)(abilityType - 4) <= 7u)
		{
			global::UnityEngine.GameObject[] array = inappOffObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			global::UnityEngine.Vector2 anchoredPosition = crewTypeText.rectTransform.anchoredPosition;
			anchoredPosition.y = -200f;
			crewTypeText.rectTransform.anchoredPosition = anchoredPosition;
		}
		else
		{
			global::UnityEngine.Vector2 anchoredPosition2 = crewTypeText.rectTransform.anchoredPosition;
			anchoredPosition2.y = -132f;
			crewTypeText.rectTransform.anchoredPosition = anchoredPosition2;
		}
		abilityTextBefore.text = MyInventoryDataManager.Instance.GetAbilityValue(card.myData.baseData.abilityType, card.myData.baseData.grade, card.myData.level).ToString("F1") + "%";
		abilityTextAfter.text = MyInventoryDataManager.Instance.GetAbilityValue(card.myData.baseData.abilityType, card.myData.baseData.grade, card.myData.level + 1).ToString("F1") + "%";
		switch (card.myData.baseData.abilityType)
		{
		case ESailorAbility.ATTACK:
		{
			crewTypeText.text = LanguageManager.Instance.translate("key_boatswain");
			global::UnityEngine.UI.Image[] array2 = abilityIconImages;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].sprite = attackSprite;
			}
			break;
		}
		case ESailorAbility.COOLTIME:
		{
			crewTypeText.text = LanguageManager.Instance.translate("key_gunner");
			global::UnityEngine.UI.Image[] array2 = abilityIconImages;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].sprite = cooltimeSprite;
			}
			break;
		}
		case ESailorAbility.ACCELERATION:
		{
			crewTypeText.text = LanguageManager.Instance.translate("key_sailingmaster");
			global::UnityEngine.UI.Image[] array2 = abilityIconImages;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].sprite = accelerationSprite;
			}
			break;
		}
		case ESailorAbility.ROTATION:
		{
			crewTypeText.text = LanguageManager.Instance.translate("key_steersman");
			global::UnityEngine.UI.Image[] array2 = abilityIconImages;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].sprite = rotationSprite;
			}
			break;
		}
		default:
			crewTypeText.text = LanguageManager.Instance.translate(card.myData.baseData.defaultName);
			break;
		}
		if (card.myData.haveCount >= card.myData.NextNeedCount)
		{
			upgradeBtn.enabled = true;
			upgradeBtnImage.color = myColor.GetColor(0, 188, 64);
			global::UnityEngine.UI.Text[] array3 = btnTexts;
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i].color = global::UnityEngine.Color.white;
			}
			priceIconImage.color = global::UnityEngine.Color.white;
		}
		else
		{
			upgradeBtn.enabled = false;
			upgradeBtnImage.color = myColor.GetColor(90, 90, 90);
			global::UnityEngine.UI.Text[] array3 = btnTexts;
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i].color = myColor.GetColor(184, 184, 184);
			}
			priceIconImage.color = myColor.GetColor(72, 72, 72);
		}
	}
}
