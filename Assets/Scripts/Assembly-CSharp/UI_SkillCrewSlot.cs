public class UI_SkillCrewSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject crewSlotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform crewSlotPivotTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myContentText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] attackSkillIconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] buffSkillIconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image durationIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text damageText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text damageValueText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text attackCoolTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text buffNameValueText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text buffValueText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text durationTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text durationValueText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] passiveOffObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup avoidFirBuffCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup avoidSecBuffCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup avoidThridBuffCanvas;

	[global::UnityEngine.Header("sprites")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite meteorIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite sparkIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite windblastIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite powerIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite speedIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite shieldIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite avoidEnvironmentFog;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite unlimitSubship;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite cooldownGenerate;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite enemyhealthdown;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite timeisgold;

	private UI_CrewSlot myCrewSlot;

	private MySailorInventory selectedData;

	private void Awake()
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(crewSlotSrc, crewSlotPivotTrans.position, global::UnityEngine.Quaternion.identity, crewSlotPivotTrans);
		myCrewSlot = gameObject.GetComponent<UI_CrewSlot>();
		damageText.font = LanguageManager.Instance.getLanguageFont();
		buffValueText.font = LanguageManager.Instance.getLanguageFont();
		durationTimeText.font = LanguageManager.Instance.getLanguageFont();
		damageValueText.font = LanguageManager.Instance.getLanguageFont();
		durationValueText.font = LanguageManager.Instance.getLanguageFont();
		myContentText.font = LanguageManager.Instance.getLanguageFont();
		buffNameValueText.font = LanguageManager.Instance.getLanguageFont();
	}

	public void Refresh()
	{
		if (selectedData != null)
		{
			SetData(selectedData, myCrewSlot.slotManager);
		}
	}

	public void SetData(MySailorInventory mySailorData, UI_CrewSlotManager manager)
	{
		selectedData = mySailorData;
		myCrewSlot.SetData(selectedData, manager);
		myContentText.text = LanguageManager.Instance.translate(selectedData.baseData.contentId);
		buffNameValueText.text = LanguageManager.Instance.translate(selectedData.baseData.skillnameId);
		switch (selectedData.baseData.abilityType)
		{
		case ESailorAbility.Skill_Meteor:
		case ESailorAbility.Skill_Spark:
		case ESailorAbility.Skill_Windblast:
			damageText.text = LanguageManager.Instance.translate("key_power");
			break;
		case ESailorAbility.Skill_AvoidEnvironment:
			damageText.text = LanguageManager.Instance.translate("key_immune");
			buffNameValueText.transform.parent.gameObject.SetActive(value: false);
			durationTimeText.transform.parent.gameObject.SetActive(value: false);
			break;
		default:
			damageText.text = LanguageManager.Instance.translate(selectedData.baseData.skillnameId);
			buffNameValueText.transform.parent.gameObject.SetActive(value: false);
			durationTimeText.transform.parent.gameObject.SetActive(value: false);
			break;
		}
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.Sprite sprite = null;
		global::UnityEngine.Sprite sprite2 = null;
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		int level2 = PlayerInfo.Instance.myData.mainShipLevel;
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(EShipType.Main, level2);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num = (int)((float)(playerShip.Damage * mainData.cannonCount) / shipSheetData.fireCoolTime);
		float num2 = 1f;
		string text = "20";
		string text2 = "120%";
		switch (selectedData.baseData.abilityType)
		{
		case ESailorAbility.Skill_Meteor:
			sprite = meteorIcon;
			sprite2 = powerIcon;
			switch (selectedData.level)
			{
			case 0:
			case 1:
				num2 = 4f;
				text = "20s";
				text2 = "120%";
				break;
			case 2:
				num2 = 4.5f;
				text = "22s";
				text2 = "125%";
				break;
			case 3:
				num2 = 5f;
				text = "24s";
				text2 = "135%";
				break;
			case 4:
				num2 = 5.5f;
				text = "26s";
				text2 = "145%";
				break;
			case 5:
				num2 = 6f;
				text = "28s";
				text2 = "150%";
				break;
			}
			buffValueText.text = text2;
			damageValueText.text = Numbering.Convert((int)((float)num * num2));
			durationValueText.text = text;
			break;
		case ESailorAbility.Skill_Spark:
			sprite = sparkIcon;
			sprite2 = speedIcon;
			durationIcon.sprite = speedIcon;
			switch (selectedData.level)
			{
			case 0:
			case 1:
				num2 = 0.8f;
				text = "15s";
				text2 = "160%";
				break;
			case 2:
				num2 = 0.9f;
				text = "15s";
				text2 = "170%";
				break;
			case 3:
				num2 = 1f;
				text = "15s";
				text2 = "180%";
				break;
			case 4:
				num2 = 1.1f;
				text = "15s";
				text2 = "190%";
				break;
			case 5:
				num2 = 1.2f;
				text = "15s";
				text2 = "200%";
				break;
			}
			buffValueText.text = text2;
			damageValueText.text = Numbering.Convert((int)((float)num * num2));
			durationValueText.text = text;
			break;
		case ESailorAbility.Skill_Windblast:
			sprite = windblastIcon;
			sprite2 = shieldIcon;
			durationIcon.sprite = shieldIcon;
			switch (selectedData.level)
			{
			case 0:
			case 1:
				num2 = 3f;
				text = "30s";
				text2 = "10%";
				break;
			case 2:
				num2 = 3.25f;
				text = "30s";
				text2 = "15%";
				break;
			case 3:
				num2 = 3.5f;
				text = "30s";
				text2 = "20%";
				break;
			case 4:
				num2 = 3.75f;
				text = "30s";
				text2 = "25%";
				break;
			case 5:
				num2 = 4f;
				text = "30s";
				text2 = "30%";
				break;
			}
			buffValueText.text = text2;
			damageValueText.text = Numbering.Convert((int)((float)num * num2));
			durationValueText.text = text;
			break;
		case ESailorAbility.Skill_AvoidEnvironment:
			damageValueText.text = LanguageManager.Instance.translate("key_fog");
			avoidFirBuffCanvas.gameObject.SetActive(value: true);
			avoidSecBuffCanvas.gameObject.SetActive(value: true);
			avoidThridBuffCanvas.gameObject.SetActive(value: true);
			avoidFirBuffCanvas.alpha = 0.5f;
			avoidSecBuffCanvas.alpha = 0.5f;
			avoidThridBuffCanvas.alpha = 0.5f;
			sprite = avoidEnvironmentFog;
			switch (selectedData.level)
			{
			case 1:
				avoidFirBuffCanvas.alpha = 1f;
				break;
			case 2:
				avoidFirBuffCanvas.alpha = 1f;
				avoidSecBuffCanvas.alpha = 1f;
				break;
			case 3:
				avoidFirBuffCanvas.alpha = 1f;
				avoidSecBuffCanvas.alpha = 1f;
				avoidThridBuffCanvas.alpha = 1f;
				break;
			}
			break;
		case ESailorAbility.Skill_EnemyHealthDown:
			damageValueText.text = "";
			sprite = enemyhealthdown;
			break;
		case ESailorAbility.Skill_SubshipSpawnTime:
			damageValueText.text = "";
			sprite = cooldownGenerate;
			break;
		case ESailorAbility.Skill_SubshipUnLimit:
			damageValueText.text = "";
			sprite = unlimitSubship;
			break;
		case ESailorAbility.Skill_TimeIsGold:
			damageValueText.text = "";
			sprite = timeisgold;
			break;
		default:
			damageValueText.text = "";
			break;
		}
		global::UnityEngine.UI.Image[] array = attackSkillIconImage;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = sprite;
		}
		array = buffSkillIconImage;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = sprite2;
		}
	}
}
