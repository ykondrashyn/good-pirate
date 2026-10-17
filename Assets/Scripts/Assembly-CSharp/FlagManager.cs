public class FlagManager : global::UnityEngine.MonoBehaviour
{
	public static FlagManager Instance;

	public global::System.Collections.Generic.List<FlagInfo> flagList;

	private global::System.Collections.Generic.Dictionary<int, FlagInfo> flagDic = new global::System.Collections.Generic.Dictionary<int, FlagInfo>();

	public float AbilityAttack { get; private set; }

	public float AbilityCoolTime { get; private set; }

	public float AbilityAcceleration { get; private set; }

	public float AbilityRotation { get; private set; }

	private void Awake()
	{
		Instance = this;
		foreach (FlagInfo flag in flagList)
		{
			flagDic.Add(flag.id, flag);
		}
		CheckIslandFlag();
	}

	public bool GetFlagInfo(int flagId, out FlagInfo info)
	{
		if (flagDic.ContainsKey(flagId))
		{
			info = flagDic[flagId];
			return true;
		}
		info = null;
		return false;
	}

	public void RefreshFlagAbility()
	{
		AbilityAttack = 0f;
		AbilityCoolTime = 0f;
		AbilityAcceleration = 0f;
		AbilityRotation = 0f;
		foreach (FlagInfo flag in flagList)
		{
			if (PlayerInfo.Instance.IsHaveFlag(flag.id) && flag.id != 0)
			{
				switch (flag.abilityType)
				{
				case ESailorAbility.ATTACK:
					AbilityAttack += flag.abilityValue;
					break;
				case ESailorAbility.COOLTIME:
					AbilityCoolTime += flag.abilityValue;
					break;
				case ESailorAbility.ACCELERATION:
					AbilityAcceleration += flag.abilityValue;
					break;
				case ESailorAbility.ROTATION:
					AbilityRotation += flag.abilityValue;
					break;
				}
			}
		}
	}

	public void CheckIslandFlag()
	{
		int clearIslandCount = PlayerInfo.Instance.ClearIslandCount;
		foreach (FlagInfo flag in flagList)
		{
			if (flag.priceType == EFlagPriceType.ISLAND && !PlayerInfo.Instance.IsHaveFlag(flag.id) && clearIslandCount >= flag.price)
			{
				PlayerInfo.Instance.BuyFlag(flag.id);
			}
		}
	}

	public void CheckIslandClameFlag()
	{
		int clearIslandClaimCount = PlayerInfo.Instance.ClearIslandClaimCount;
		foreach (FlagInfo flag in flagList)
		{
			if (flag.priceType == EFlagPriceType.CLAIM && !PlayerInfo.Instance.IsHaveFlag(flag.id) && clearIslandClaimCount >= flag.price)
			{
				PlayerInfo.Instance.BuyFlag(flag.id);
			}
		}
	}
}
