[global::System.Serializable]
public class MySailorInventory
{
	public ESailor sailorType;

	public ESailorStatus status;

	public SailorBaseData baseData;

	public bool isNew;

	public bool isHave;

	public int level;

	public int haveCount;

	public long exploreStartTick;

	public long exploreEndTick;

	public int exploreGrade;

	public int NextNeedCount
	{
		get
		{
			int result = 0;
			switch (baseData.grade)
			{
			case EGrade.sClass:
				if (baseData.abilityType == ESailorAbility.Skill_SubshipUnLimit || baseData.abilityType == ESailorAbility.Skill_SubshipUnLimit)
				{
					switch (level)
					{
					case 0:
						result = 2;
						break;
					case 1:
						result = 5;
						break;
					case 2:
						result = 5;
						break;
					case 3:
						result = 5;
						break;
					case 4:
						result = 5;
						break;
					}
				}
				else
				{
					switch (level)
					{
					case 0:
						result = 1;
						break;
					case 1:
						result = 3;
						break;
					case 2:
						result = 5;
						break;
					case 3:
						result = 7;
						break;
					case 4:
						result = 9;
						break;
					}
				}
				break;
			case EGrade.aClass:
				switch (level)
				{
				case 0:
					result = 2;
					break;
				case 1:
					result = 4;
					break;
				case 2:
					result = 6;
					break;
				case 3:
					result = 8;
					break;
				case 4:
					result = 10;
					break;
				}
				break;
			case EGrade.bClass:
				switch (level)
				{
				case 0:
					result = 3;
					break;
				case 1:
					result = 6;
					break;
				case 2:
					result = 9;
					break;
				case 3:
					result = 12;
					break;
				case 4:
					result = 15;
					break;
				}
				break;
			case EGrade.cClass:
				switch (level)
				{
				case 0:
					result = 5;
					break;
				case 1:
					result = 10;
					break;
				case 2:
					result = 15;
					break;
				case 3:
					result = 20;
					break;
				case 4:
					result = 25;
					break;
				}
				break;
			}
			return result;
		}
	}

	public long UpgradePrice
	{
		get
		{
			CrewUpgradePrice crewUpgradePrice = global::UnityEngine.Resources.Load<CrewUpgradePrice>("Data/CrewUpgradePrice");
			int num = global::UnityEngine.Mathf.Clamp(level, 0, crewUpgradePrice.dataArray.Length);
			long result = 0L;
			switch (baseData.grade)
			{
			case EGrade.aClass:
				result = crewUpgradePrice.dataArray[num].Ranka;
				break;
			case EGrade.bClass:
				result = crewUpgradePrice.dataArray[num].Rankb;
				break;
			case EGrade.cClass:
				result = crewUpgradePrice.dataArray[num].Rankc;
				break;
			case EGrade.sClass:
				result = crewUpgradePrice.dataArray[num].Ranks;
				break;
			}
			return result;
		}
	}
}
