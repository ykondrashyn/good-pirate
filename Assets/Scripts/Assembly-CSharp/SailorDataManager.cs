public class SailorDataManager : global::UnityEngine.MonoBehaviour
{
	public static SailorDataManager Instance;

	public global::System.Collections.Generic.List<SailorBaseData> sailorDataList = new global::System.Collections.Generic.List<SailorBaseData>();

	private global::System.Collections.Generic.Dictionary<ESailor, SailorBaseData> sailorDataDic = new global::System.Collections.Generic.Dictionary<ESailor, SailorBaseData>(new ESailorComparer());

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<ESailor> cRankList = new global::System.Collections.Generic.List<ESailor>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<ESailor> bRankList = new global::System.Collections.Generic.List<ESailor>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<ESailor> aRankList = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> sRankList = new global::System.Collections.Generic.List<ESailor>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<ESailor> lRankList = new global::System.Collections.Generic.List<ESailor>();

	private global::System.Collections.Generic.List<ESailor> attackList = new global::System.Collections.Generic.List<ESailor>();

	private global::System.Collections.Generic.List<ESailor> cooltimeList = new global::System.Collections.Generic.List<ESailor>();

	private global::System.Collections.Generic.List<ESailor> accelerationList = new global::System.Collections.Generic.List<ESailor>();

	private global::System.Collections.Generic.List<ESailor> rotationList = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_meteor = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_spark = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_windblast = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_avoidEnvironment = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_subshipUnLimit = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_enemyHealthDown = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_subshipSpawntime = new global::System.Collections.Generic.List<ESailor>();

	public global::System.Collections.Generic.List<ESailor> skill_timeIsGold = new global::System.Collections.Generic.List<ESailor>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<ESailor> haveSkillList = new global::System.Collections.Generic.List<ESailor>();

	private void Awake()
	{
		Instance = this;
		foreach (SailorBaseData sailorData in sailorDataList)
		{
			if (sailorDataDic.ContainsKey(sailorData.id))
			{
				continue;
			}
			sailorDataDic.Add(sailorData.id, sailorData);
			if (sailorData.id != ESailor.boatswain_s_inappp)
			{
				switch (sailorData.grade)
				{
				case EGrade.cClass:
					cRankList.Add(sailorData.id);
					break;
				case EGrade.bClass:
					bRankList.Add(sailorData.id);
					break;
				case EGrade.aClass:
					aRankList.Add(sailorData.id);
					break;
				case EGrade.sClass:
					sRankList.Add(sailorData.id);
					break;
				default:
					lRankList.Add(sailorData.id);
					break;
				}
				switch (sailorData.abilityType)
				{
				case ESailorAbility.ATTACK:
					attackList.Add(sailorData.id);
					break;
				case ESailorAbility.COOLTIME:
					cooltimeList.Add(sailorData.id);
					break;
				case ESailorAbility.ACCELERATION:
					accelerationList.Add(sailorData.id);
					break;
				case ESailorAbility.ROTATION:
					rotationList.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_Meteor:
					skill_meteor.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_Spark:
					skill_spark.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_Windblast:
					skill_windblast.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_AvoidEnvironment:
					skill_avoidEnvironment.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_SubshipUnLimit:
					skill_subshipUnLimit.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_SubshipSpawnTime:
					skill_subshipSpawntime.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_EnemyHealthDown:
					skill_enemyHealthDown.Add(sailorData.id);
					break;
				case ESailorAbility.Skill_TimeIsGold:
					skill_timeIsGold.Add(sailorData.id);
					break;
				}
			}
		}
	}

	private void Start()
	{
		foreach (SailorBaseData sailorData in sailorDataList)
		{
			switch (sailorData.abilityType)
			{
			case ESailorAbility.Skill_Meteor:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_Spark:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_Windblast:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_AvoidEnvironment:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_SubshipUnLimit:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_SubshipSpawnTime:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_EnemyHealthDown:
				HaveCheck(sailorData.id);
				break;
			case ESailorAbility.Skill_TimeIsGold:
				HaveCheck(sailorData.id);
				break;
			}
		}
	}

	public void HaveCheck(ESailor sailor)
	{
		MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(sailor);
		bool flag = false;
		switch (mySailorData.baseData.abilityType)
		{
		case ESailorAbility.Skill_Meteor:
			flag = true;
			break;
		case ESailorAbility.Skill_Spark:
			flag = true;
			break;
		case ESailorAbility.Skill_Windblast:
			flag = true;
			break;
		case ESailorAbility.Skill_AvoidEnvironment:
			flag = true;
			break;
		case ESailorAbility.Skill_SubshipUnLimit:
			flag = true;
			break;
		case ESailorAbility.Skill_SubshipSpawnTime:
			flag = true;
			break;
		case ESailorAbility.Skill_EnemyHealthDown:
			flag = true;
			break;
		case ESailorAbility.Skill_TimeIsGold:
			flag = true;
			break;
		}
		if (flag)
		{
			if (sRankList.Contains(sailor))
			{
				sRankList.Remove(sailor);
			}
			if (!lRankList.Contains(sailor))
			{
				lRankList.Add(sailor);
			}
			if (mySailorData.isHave && !haveSkillList.Contains(sailor))
			{
				haveSkillList.Add(sailor);
			}
		}
	}

	public SailorBaseData GetSailorBaseData(ESailor id)
	{
		if (!sailorDataDic.ContainsKey(id))
		{
			return null;
		}
		return sailorDataDic[id];
	}

	public EGrade GetSailorGrade()
	{
		float num = global::UnityEngine.Random.Range(0f, 1000f);
		if (num <= 32f)
		{
			return EGrade.sClass;
		}
		if (num <= 137f)
		{
			return EGrade.aClass;
		}
		if (num <= 367f)
		{
			return EGrade.bClass;
		}
		return EGrade.cClass;
	}

	public ESailorAbility GetSailorAbility(out global::System.Collections.Generic.List<ESailor> abilityList)
	{
		bool flag = false;
		if (haveSkillList.Count > 0 && global::UnityEngine.Random.Range(0f, 100f) <= 0.5f)
		{
			flag = true;
		}
		ESailorAbility eSailorAbility;
		if (flag)
		{
			ESailor id = haveSkillList[global::UnityEngine.Random.Range(0, haveSkillList.Count)];
			eSailorAbility = GetSailorBaseData(id).abilityType;
			switch (eSailorAbility)
			{
			case ESailorAbility.Skill_Meteor:
				abilityList = skill_meteor;
				break;
			case ESailorAbility.Skill_Spark:
				abilityList = skill_spark;
				break;
			case ESailorAbility.Skill_Windblast:
				abilityList = skill_windblast;
				break;
			case ESailorAbility.Skill_AvoidEnvironment:
				abilityList = skill_avoidEnvironment;
				break;
			case ESailorAbility.Skill_SubshipUnLimit:
				abilityList = skill_subshipUnLimit;
				break;
			case ESailorAbility.Skill_SubshipSpawnTime:
				abilityList = skill_subshipSpawntime;
				break;
			case ESailorAbility.Skill_EnemyHealthDown:
				abilityList = skill_enemyHealthDown;
				break;
			case ESailorAbility.Skill_TimeIsGold:
				abilityList = skill_timeIsGold;
				break;
			default:
				global::UnityEngine.Debug.Log("ability list is null");
				abilityList = null;
				break;
			}
		}
		else
		{
			float num = global::UnityEngine.Random.Range(0f, 1000f);
			if (num <= 466f)
			{
				eSailorAbility = ESailorAbility.ATTACK;
				abilityList = attackList;
			}
			else if (num <= 732f)
			{
				eSailorAbility = ESailorAbility.COOLTIME;
				abilityList = cooltimeList;
			}
			else if (num <= 898f)
			{
				eSailorAbility = ESailorAbility.ACCELERATION;
				abilityList = accelerationList;
			}
			else
			{
				eSailorAbility = ESailorAbility.ROTATION;
				abilityList = rotationList;
			}
		}
		return eSailorAbility;
	}

	public global::System.Collections.Generic.List<ESailor> GetRewardSailorList(ERewardBox rewardBoxGrade)
	{
		global::System.Collections.Generic.List<ESailor> list = new global::System.Collections.Generic.List<ESailor>();
		int num = 0;
		switch (rewardBoxGrade)
		{
		case ERewardBox.NONE:
			num = 1;
			break;
		case ERewardBox.BRONZE:
			num = 1;
			break;
		case ERewardBox.SILVER:
			num = 2;
			break;
		case ERewardBox.GOLD:
			num = 2;
			break;
		case ERewardBox.BLACK:
			num = 3;
			break;
		}
		for (int i = 0; i < num; i++)
		{
			int num2 = 0;
			while (true)
			{
				EGrade sailorGrade = GetSailorGrade();
				GetSailorAbility(out var abilityList);
				if (GetSailor(sailorGrade, abilityList, out var getSailor, list))
				{
					list.Add(getSailor);
					break;
				}
				num2++;
				if (num2 >= 100)
				{
					getSailor = ESailor.END;
					list.Add(getSailor);
					break;
				}
			}
		}
		return list;
	}

	public bool GetSailor(EGrade sailorGrade, global::System.Collections.Generic.List<ESailor> abilityList, out ESailor getSailor, global::System.Collections.Generic.List<ESailor> overlapCheckList = null)
	{
		int num = 0;
		bool flag = true;
		getSailor = ESailor.END;
		while (true)
		{
			int index = global::UnityEngine.Random.Range(0, abilityList.Count);
			ESailor eSailor = abilityList[index];
			bool flag2 = false;
			if (haveSkillList.Contains(eSailor))
			{
				getSailor = eSailor;
				flag2 = true;
			}
			else
			{
				switch (sailorGrade)
				{
				case EGrade.cClass:
					if (cRankList.Contains(eSailor))
					{
						flag2 = true;
						getSailor = eSailor;
					}
					break;
				case EGrade.bClass:
					if (bRankList.Contains(eSailor))
					{
						flag2 = true;
						getSailor = eSailor;
					}
					break;
				case EGrade.aClass:
					if (aRankList.Contains(eSailor))
					{
						flag2 = true;
						getSailor = eSailor;
					}
					break;
				case EGrade.sClass:
					if (sRankList.Contains(eSailor))
					{
						flag2 = true;
						getSailor = eSailor;
					}
					break;
				}
			}
			if (flag2)
			{
				if (overlapCheckList != null && overlapCheckList.Contains(getSailor))
				{
					flag = false;
				}
				if (!IsCanGiveSailor(getSailor))
				{
					flag = false;
				}
				if (flag)
				{
					break;
				}
			}
			num++;
			if (num > 100)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public bool IsCanGiveSailor(ESailor id)
	{
		if (id == ESailor.END)
		{
			return false;
		}
		bool result = true;
		MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(id);
		if (mySailorData.level >= mySailorData.baseData.maxCount)
		{
			result = false;
		}
		return result;
	}
}
