public class UI_CrewSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject skillSlotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform sGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform aGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform bGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform cGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject statCrewsPanelObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject skillCrewsPanelObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] buttonList;

	[global::UnityEngine.SerializeField]
	private UI_CrewDetailPopup crewDetailPopup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text typeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image typeBgIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite attackSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite cooltimeSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite accelerationSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite rotationSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] signObjs;

	private global::System.Collections.Generic.List<UI_CrewSlot> slotList = new global::System.Collections.Generic.List<UI_CrewSlot>();

	private global::System.Collections.Generic.List<UI_SkillCrewSlot> skillSlotList = new global::System.Collections.Generic.List<UI_SkillCrewSlot>();

	private global::System.Collections.Generic.List<UI_CrewSlot> attackTypeList = new global::System.Collections.Generic.List<UI_CrewSlot>();

	private global::System.Collections.Generic.List<UI_CrewSlot> cooltimeTypeList = new global::System.Collections.Generic.List<UI_CrewSlot>();

	private global::System.Collections.Generic.List<UI_CrewSlot> accelerationTypeList = new global::System.Collections.Generic.List<UI_CrewSlot>();

	private global::System.Collections.Generic.List<UI_CrewSlot> rotationTypeList = new global::System.Collections.Generic.List<UI_CrewSlot>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] attackTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] cooltimeTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] accelerationTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] rotationTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color activeColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color inactiveColor;

	[global::UnityEngine.SerializeField]
	private UI_LayoutHorizontal layoutHorizontal;

	[global::UnityEngine.Header("skill crews")]
	[global::UnityEngine.SerializeField]
	private UI_LayoutHorizontal skillCrewLayoutHorizontal;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform skillCrewContentTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform activeTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform passiveTrans;

	private int selectNum;

	private void Awake()
	{
		GetComponentInChildren<UI_LayoutHorizontal>();
	}

	private void OnEnable()
	{
		statCrewsPanelObj.SetActive(value: false);
		skillCrewsPanelObj.SetActive(value: true);
		CheckCanUpgradeIsHave();
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Combine(instance.UpgradeCrewAction, new global::System.Action(CheckCanUpgradeIsHave));
		TutorialManager.Instance.StartTutorial("crewpopup");
		foreach (global::System.Collections.Generic.KeyValuePair<ESailor, MySailorInventory> item in MyInventoryDataManager.Instance.mySailorDataDic)
		{
			MySailorInventory value = item.Value;
			if (value.baseData.abilityType == ESailorAbility.ATTACK || value.baseData.abilityType == ESailorAbility.ACCELERATION || value.baseData.abilityType == ESailorAbility.COOLTIME || value.baseData.abilityType == ESailorAbility.ROTATION)
			{
				global::UnityEngine.Transform parent = null;
				switch (value.baseData.grade)
				{
				case EGrade.cClass:
					parent = cGridTrans;
					break;
				case EGrade.bClass:
					parent = bGridTrans;
					break;
				case EGrade.aClass:
					parent = aGridTrans;
					break;
				case EGrade.sClass:
					parent = sGridTrans;
					break;
				}
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(slotSrc, parent);
				UI_CrewSlot component = gameObject.GetComponent<UI_CrewSlot>();
				component.SetData(value, this);
				gameObject.transform.localPosition = global::UnityEngine.Vector3.zero;
				gameObject.transform.localScale = global::UnityEngine.Vector3.one;
				switch (value.baseData.abilityType)
				{
				case ESailorAbility.ATTACK:
					attackTypeList.Add(component);
					break;
				case ESailorAbility.COOLTIME:
					cooltimeTypeList.Add(component);
					break;
				case ESailorAbility.ACCELERATION:
					accelerationTypeList.Add(component);
					break;
				case ESailorAbility.ROTATION:
					rotationTypeList.Add(component);
					break;
				}
				gameObject.SetActive(value: false);
				slotList.Add(component);
			}
			else
			{
				global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(skillSlotSrc, skillCrewContentTrans);
				global::UnityEngine.Transform transform = obj.transform;
				UI_SkillCrewSlot component2 = obj.GetComponent<UI_SkillCrewSlot>();
				ESailorAbility abilityType = value.baseData.abilityType;
				if ((uint)(abilityType - 4) <= 2u)
				{
					transform.SetSiblingIndex(activeTrans.GetSiblingIndex());
				}
				else
				{
					transform.SetSiblingIndex(passiveTrans.GetSiblingIndex());
				}
				component2.SetData(value, this);
				skillSlotList.Add(component2);
			}
		}
		OpenPanel(4);
		TextRefresh();
		StartCoroutine(EnableNext());
	}

	private global::System.Collections.IEnumerator EnableNext()
	{
		yield return new global::UnityEngine.WaitForEndOfFrame();
		if (statCrewsPanelObj.activeSelf)
		{
			layoutHorizontal.Refresh();
		}
		if (skillCrewsPanelObj.activeSelf)
		{
			skillCrewLayoutHorizontal.Refresh();
		}
	}

	public void CheckCanUpgradeIsHave()
	{
		global::System.Collections.Generic.Dictionary<ESailor, MySailorInventory>.ValueCollection values = MyInventoryDataManager.Instance.mySailorDataDic.Values;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		foreach (MySailorInventory item in values)
		{
			if (item.haveCount < item.NextNeedCount || item.level >= item.baseData.maxCount)
			{
				continue;
			}
			switch (item.baseData.abilityType)
			{
			case ESailorAbility.ATTACK:
				if (!flag)
				{
					flag = true;
				}
				break;
			case ESailorAbility.ACCELERATION:
				if (!flag3)
				{
					flag3 = true;
				}
				break;
			case ESailorAbility.COOLTIME:
				if (!flag2)
				{
					flag2 = true;
				}
				break;
			case ESailorAbility.ROTATION:
				if (!flag4)
				{
					flag4 = true;
				}
				break;
			}
		}
		if (flag)
		{
			signObjs[0].SetActive(value: true);
		}
		else
		{
			signObjs[0].SetActive(value: false);
		}
		if (flag2)
		{
			signObjs[1].SetActive(value: true);
		}
		else
		{
			signObjs[1].SetActive(value: false);
		}
		if (flag3)
		{
			signObjs[2].SetActive(value: true);
		}
		else
		{
			signObjs[2].SetActive(value: false);
		}
		if (flag4)
		{
			signObjs[3].SetActive(value: true);
		}
		else
		{
			signObjs[3].SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Remove(instance.UpgradeCrewAction, new global::System.Action(CheckCanUpgradeIsHave));
		foreach (UI_CrewSlot slot in slotList)
		{
			if (slot != null)
			{
				global::UnityEngine.Object.Destroy(slot.gameObject);
			}
		}
		attackTypeList.Clear();
		cooltimeTypeList.Clear();
		accelerationTypeList.Clear();
		rotationTypeList.Clear();
		slotList.Clear();
		foreach (UI_SkillCrewSlot skillSlot in skillSlotList)
		{
			if (skillSlot != null)
			{
				global::UnityEngine.Object.Destroy(skillSlot.gameObject);
			}
		}
		skillSlotList.Clear();
	}

	public void OpenSailorDetailPopup(UI_CrewSlot slot)
	{
		crewDetailPopup.gameObject.SetActive(value: true);
		crewDetailPopup.SetData(slot);
	}

	public void OpenPanel(int num)
	{
		if (selectNum == num)
		{
			return;
		}
		if (num == 4)
		{
			statCrewsPanelObj.SetActive(value: false);
			skillCrewsPanelObj.SetActive(value: true);
			skillCrewLayoutHorizontal.Refresh();
		}
		else
		{
			statCrewsPanelObj.SetActive(value: true);
			skillCrewsPanelObj.SetActive(value: false);
			global::System.Collections.Generic.List<UI_CrewSlot> list = null;
			typeText.font = LanguageManager.Instance.getLanguageFont();
			switch (num)
			{
			case 0:
				list = attackTypeList;
				typeBgIcon.sprite = attackSprite;
				typeText.text = LanguageManager.Instance.translate("key_boatswain");
				break;
			case 1:
				list = cooltimeTypeList;
				typeBgIcon.sprite = cooltimeSprite;
				typeText.text = LanguageManager.Instance.translate("key_gunner");
				break;
			case 2:
				list = accelerationTypeList;
				typeBgIcon.sprite = accelerationSprite;
				typeText.text = LanguageManager.Instance.translate("key_sailingmaster");
				break;
			case 3:
				list = rotationTypeList;
				typeBgIcon.sprite = rotationSprite;
				typeText.text = LanguageManager.Instance.translate("key_steersman");
				break;
			}
			foreach (UI_CrewSlot slot in slotList)
			{
				if (slot.gameObject.activeSelf)
				{
					slot.gameObject.SetActive(value: false);
				}
			}
			foreach (UI_CrewSlot item in list)
			{
				item.gameObject.SetActive(value: true);
			}
			layoutHorizontal.Refresh();
		}
		buttonList[selectNum].color = inactiveColor;
		buttonList[num].color = activeColor;
		selectNum = num;
		AudioManager.PlayButtonClick();
	}

	public void AllRefresh()
	{
		foreach (UI_CrewSlot slot in slotList)
		{
			slot.Refresh();
		}
		foreach (UI_SkillCrewSlot skillSlot in skillSlotList)
		{
			if (skillSlot != null)
			{
				skillSlot.Refresh();
			}
		}
	}

	public void TextRefresh()
	{
		for (int i = 0; i < attackTexts.Length; i++)
		{
			attackTexts[i].text = ((i == 1) ? "+ " : "") + MyInventoryDataManager.Instance.AbilityAttack.ToString("F1") + "%";
		}
		for (int j = 0; j < cooltimeTexts.Length; j++)
		{
			cooltimeTexts[j].text = ((j == 1) ? "+ " : "") + MyInventoryDataManager.Instance.AbilityCoolTime.ToString("F1") + "%";
		}
		for (int k = 0; k < accelerationTexts.Length; k++)
		{
			accelerationTexts[k].text = ((k == 1) ? "+ " : "") + MyInventoryDataManager.Instance.AbilityAcceleration.ToString("F1") + "%";
		}
		for (int l = 0; l < rotationTexts.Length; l++)
		{
			rotationTexts[l].text = ((l == 1) ? "+ " : "") + MyInventoryDataManager.Instance.AbilityRotation.ToString("F1") + "%";
		}
	}
}
