public class SkinDataManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSkinData> skinDataList = new global::System.Collections.Generic.List<ShipSkinData>();

	private global::System.Collections.Generic.Dictionary<eShipSkin, ShipSkinData> skinDataDic = new global::System.Collections.Generic.Dictionary<eShipSkin, ShipSkinData>(new eShipSkinComparer());

	public static SkinDataManager Instance;

	public float AbilityAttack { get; private set; }

	public float AbilityCoolTime { get; private set; }

	public float AbilityAcceleration { get; private set; }

	public float AbilityRotation { get; private set; }

	private void Start()
	{
		RefreshAbility();
	}

	public void RefreshAbility()
	{
		AbilityAttack = 0f;
		AbilityCoolTime = 0f;
		AbilityAcceleration = 0f;
		AbilityRotation = 0f;
		if (PlayerInfo.Instance.myData.selectedSkin != eShipSkin.Normal)
		{
			ShipSkinData skinData = GetSkinData(PlayerInfo.Instance.myData.selectedSkin);
			Set(skinData.abilityType, skinData.abilityValue);
			Set(skinData.abilityType2, skinData.abilityValue2);
		}
	}

	private void Set(ESailorAbility ability, float value)
	{
		switch (ability)
		{
		case ESailorAbility.ATTACK:
			AbilityAttack = value;
			break;
		case ESailorAbility.COOLTIME:
			AbilityCoolTime = value;
			break;
		case ESailorAbility.ACCELERATION:
			AbilityAcceleration = value;
			break;
		case ESailorAbility.ROTATION:
			AbilityRotation = value;
			break;
		}
	}

	private void Awake()
	{
		Instance = this;
		foreach (ShipSkinData skinData in skinDataList)
		{
			skinDataDic.Add(skinData.skin, skinData);
		}
	}

	public ShipSkinData GetSkinData(eShipSkin type)
	{
		if (skinDataDic.ContainsKey(type))
		{
			return skinDataDic[type];
		}
		return null;
	}
}
