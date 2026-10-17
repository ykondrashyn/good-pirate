public class UI_CrewSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myBgImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myCharImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject haveObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text cardHaveCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image gradeBgImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text gradeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image gaugeImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite sRankSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite aRankSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite bRankSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite cRankSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite lRankRedSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite lRankGreenSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite lRankPurpleSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite haventSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject canUpgardeObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorS = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorA = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorB = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorC = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorLRed = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorLPurple = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorLGreen = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color gradeBgColorHavent = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private bool absoluteColor;

	private global::UnityEngine.UI.Button myButton;

	public UI_CrewSlotManager slotManager { get; private set; }

	public MySailorInventory myData { get; private set; }

	private void Awake()
	{
		myButton = GetComponent<global::UnityEngine.UI.Button>();
	}

	public void SetData(MySailorInventory data, UI_CrewSlotManager manager)
	{
		myData = data;
		slotManager = manager;
		Refresh();
	}

	public void Refresh()
	{
		if (myData.level >= myData.baseData.maxCount)
		{
			levelText.text = "Lv.MAX";
		}
		else
		{
			levelText.text = $"Lv.{myData.level}";
		}
		if (myData.baseData.id == ESailor.boatswain_s_inappp)
		{
			if (myData.isHave)
			{
				levelText.text = "Lv.MAX";
				cardHaveCountText.text = "MAX";
			}
			else
			{
				cardHaveCountText.text = "0/1";
			}
		}
		else
		{
			cardHaveCountText.text = $"{myData.haveCount}/{myData.NextNeedCount}";
		}
		if (myData.level >= myData.baseData.maxCount)
		{
			cardHaveCountText.text = "MAX";
		}
		float num = global::UnityEngine.Mathf.Clamp01((float)myData.haveCount / (float)myData.NextNeedCount);
		if (myData.level >= myData.baseData.maxCount)
		{
			num = 0f;
		}
		gaugeImage.fillAmount = num;
		if (num >= 1f)
		{
			if (myData.level < myData.baseData.maxCount)
			{
				if ((bool)canUpgardeObj)
				{
					canUpgardeObj.SetActive(value: true);
				}
			}
			else if ((bool)canUpgardeObj)
			{
				canUpgardeObj.SetActive(value: false);
			}
			cardHaveCountText.color = global::UnityEngine.Color.white;
			gaugeImage.color = myColor.GetColor(255, 0, 66);
		}
		else
		{
			if ((bool)canUpgardeObj)
			{
				canUpgardeObj.SetActive(value: false);
			}
			cardHaveCountText.color = global::UnityEngine.Color.black;
			gaugeImage.color = myColor.GetColor(0, 188, 64);
		}
		switch (myData.baseData.grade)
		{
		case EGrade.sClass:
			gradeText.text = "S";
			break;
		case EGrade.aClass:
			gradeText.text = "A";
			break;
		case EGrade.bClass:
			gradeText.text = "B";
			break;
		case EGrade.cClass:
			gradeText.text = "C";
			break;
		}
		if (myData.isHave || absoluteColor)
		{
			switch (myData.baseData.grade)
			{
			case EGrade.sClass:
				switch (myData.baseData.abilityType)
				{
				case ESailorAbility.Skill_Meteor:
				case ESailorAbility.Skill_AvoidEnvironment:
				case ESailorAbility.Skill_SubshipUnLimit:
				case ESailorAbility.Skill_EnemyHealthDown:
					gradeText.color = myColor.GetColor(255, 197, 0);
					myBgImage.sprite = lRankRedSprite;
					gradeBgImage.color = gradeBgColorLRed;
					gradeText.text = "L";
					break;
				case ESailorAbility.Skill_Spark:
				case ESailorAbility.Skill_TimeIsGold:
					gradeText.color = myColor.GetColor(255, 197, 0);
					myBgImage.sprite = aRankSprite;
					gradeBgImage.color = gradeBgColorA;
					gradeText.text = "L";
					break;
				case ESailorAbility.Skill_Windblast:
					gradeText.color = myColor.GetColor(255, 197, 0);
					myBgImage.sprite = lRankPurpleSprite;
					gradeBgImage.color = gradeBgColorLPurple;
					gradeText.text = "L";
					break;
				case ESailorAbility.Skill_SubshipSpawnTime:
					gradeText.color = myColor.GetColor(255, 197, 0);
					myBgImage.sprite = lRankGreenSprite;
					gradeBgImage.color = gradeBgColorLGreen;
					gradeText.text = "L";
					break;
				default:
					gradeText.color = myColor.GetColor(137, 134, 232);
					myBgImage.sprite = sRankSprite;
					gradeBgImage.color = gradeBgColorS;
					break;
				}
				break;
			case EGrade.aClass:
				gradeText.color = global::UnityEngine.Color.white;
				myBgImage.sprite = aRankSprite;
				gradeBgImage.color = gradeBgColorA;
				break;
			case EGrade.bClass:
				gradeText.color = global::UnityEngine.Color.white;
				myBgImage.sprite = bRankSprite;
				gradeBgImage.color = gradeBgColorB;
				break;
			case EGrade.cClass:
				gradeText.color = myColor.GetColor(179, 141, 111);
				myBgImage.sprite = cRankSprite;
				gradeBgImage.color = gradeBgColorC;
				break;
			}
			myCharImage.sprite = myData.baseData.characterImage;
			return;
		}
		gradeText.color = myColor.GetColor(106, 106, 106);
		myBgImage.sprite = haventSprite;
		myCharImage.sprite = myData.baseData.characterBlackImage;
		gradeBgImage.color = gradeBgColorHavent;
		EGrade grade = myData.baseData.grade;
		if (grade == EGrade.sClass)
		{
			ESailorAbility abilityType = myData.baseData.abilityType;
			if ((uint)(abilityType - 4) <= 7u)
			{
				gradeText.text = "L";
			}
		}
	}

	public void SelectThis()
	{
		slotManager.OpenSailorDetailPopup(this);
	}
}
