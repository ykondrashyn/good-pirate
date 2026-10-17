public class ShipDataManager : global::UnityEngine.MonoBehaviour
{
	public static ShipDataManager Instance;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipMainData> shipMainDataList = new global::System.Collections.Generic.List<ShipMainData>();

	private global::System.Collections.Generic.Dictionary<EShipMain, ShipMainData> shipMainDic = new global::System.Collections.Generic.Dictionary<EShipMain, ShipMainData>(new EShipMainComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipBaseData> shipBaseDataList = new global::System.Collections.Generic.List<ShipBaseData>();

	private global::System.Collections.Generic.Dictionary<EShipType, ShipBaseData> shipBaseDataDic = new global::System.Collections.Generic.Dictionary<EShipType, ShipBaseData>(new EShipTypeComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subNormalList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subSplashList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subRapidList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subRepairList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subFishingList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subSucideBombList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subGuideList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subHarpoonList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subMultiList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> subGasList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipSubData> patrolBomberList = new global::System.Collections.Generic.List<ShipSubData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subNormalDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subSplashDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subRapidDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subRepairDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subFishingDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subSucideBombDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subGuideDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subHarpoonDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subMultiDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> subGasDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData> patrolBomberDic = new global::System.Collections.Generic.Dictionary<EShipSub, ShipSubData>(new EShipSubComparer());

	public MainUpgradePrice mainUpgradePriceData;

	public SubShipUpgradePrice subShipUpgradePrice { get; private set; }

	public EnemyReward enemyReward { get; private set; }

	public MainShipData mainShipData { get; set; }

	private SubCannonShipData cannonShipData { get; set; }

	private SubNormalShipData normalShipData { get; set; }

	private SubRapidShipData rapidShipData { get; set; }

	private SubSucideShipData sucideShipData { get; set; }

	private SubFishShipData fishShipData { get; set; }

	private SubHarpoonShipData harpoonShipData { get; set; }

	private SubGasShipData gasShipData { get; set; }

	private SubRepairShipData repairShipData { get; set; }

	private SubMultiShipData multiShipData { get; set; }

	private SubGuideShipData guideShipData { get; set; }

	private void Awake()
	{
		Instance = this;
		SheetsInit();
		foreach (ShipMainData shipMainData in shipMainDataList)
		{
			shipMainDic.Add(shipMainData.id, shipMainData);
		}
		foreach (ShipBaseData shipBaseData in shipBaseDataList)
		{
			shipBaseDataDic.Add(shipBaseData.id, shipBaseData);
		}
		foreach (ShipSubData subNormal in subNormalList)
		{
			subNormalDic.Add(subNormal.id, subNormal);
		}
		foreach (ShipSubData subSplash in subSplashList)
		{
			subSplashDic.Add(subSplash.id, subSplash);
		}
		foreach (ShipSubData subRapid in subRapidList)
		{
			subRapidDic.Add(subRapid.id, subRapid);
		}
		foreach (ShipSubData subRepair in subRepairList)
		{
			subRepairDic.Add(subRepair.id, subRepair);
		}
		foreach (ShipSubData subFishing in subFishingList)
		{
			subFishingDic.Add(subFishing.id, subFishing);
		}
		foreach (ShipSubData subSucideBomb in subSucideBombList)
		{
			subSucideBombDic.Add(subSucideBomb.id, subSucideBomb);
		}
		foreach (ShipSubData subGuide in subGuideList)
		{
			subGuideDic.Add(subGuide.id, subGuide);
		}
		foreach (ShipSubData subHarpoon in subHarpoonList)
		{
			subHarpoonDic.Add(subHarpoon.id, subHarpoon);
		}
		foreach (ShipSubData subMulti in subMultiList)
		{
			subMultiDic.Add(subMulti.id, subMulti);
		}
		foreach (ShipSubData subGas in subGasList)
		{
			subGasDic.Add(subGas.id, subGas);
		}
		foreach (ShipSubData patrolBomber in patrolBomberList)
		{
			patrolBomberDic.Add(patrolBomber.id, patrolBomber);
		}
	}

	public ShipMainData GetMainData(EShipMain level)
	{
		if (shipMainDic.ContainsKey(level))
		{
			return shipMainDic[level];
		}
		return null;
	}

	public ShipSubData GetSubShipData(EShipType shipType, EShipSub level)
	{
		ShipSubData shipSubData = null;
		switch (shipType)
		{
		case EShipType.Normal:
			if (subNormalDic.ContainsKey(level))
			{
				shipSubData = subNormalDic[level];
			}
			break;
		case EShipType.Cannon:
			if (subSplashDic.ContainsKey(level))
			{
				shipSubData = subSplashDic[level];
			}
			break;
		case EShipType.Rapid:
			if (subRapidDic.ContainsKey(level))
			{
				shipSubData = subRapidDic[level];
			}
			break;
		case EShipType.Repair:
			if (subRepairDic.ContainsKey(level))
			{
				shipSubData = subRepairDic[level];
			}
			break;
		case EShipType.Fishing:
			if (subFishingDic.ContainsKey(level))
			{
				shipSubData = subFishingDic[level];
			}
			break;
		case EShipType.SuicideBomb:
			if (subSucideBombDic.ContainsKey(level))
			{
				shipSubData = subSucideBombDic[level];
			}
			break;
		case EShipType.Guide:
			if (subGuideDic.ContainsKey(level))
			{
				shipSubData = subGuideDic[level];
			}
			break;
		case EShipType.Harpoon:
			if (subHarpoonDic.ContainsKey(level))
			{
				shipSubData = subHarpoonDic[level];
			}
			break;
		case EShipType.Multi:
			if (subMultiDic.ContainsKey(level))
			{
				shipSubData = subMultiDic[level];
			}
			break;
		case EShipType.Gas:
			if (subGasDic.ContainsKey(level))
			{
				shipSubData = subGasDic[level];
			}
			break;
		case EShipType.Bomber:
			if (patrolBomberDic.ContainsKey(level))
			{
				shipSubData = patrolBomberDic[level];
			}
			break;
		}
		if (shipSubData == null && shipType != EShipType.Bomber)
		{
			shipSubData = subNormalDic[EShipSub.LEVEL_01];
		}
		return shipSubData;
	}

	public ShipBaseData GetShipBaseData(EShipType shipType)
	{
		if (shipBaseDataDic.ContainsKey(shipType))
		{
			return shipBaseDataDic[shipType];
		}
		return null;
	}

	private void SheetsInit()
	{
		mainShipData = global::UnityEngine.Resources.Load<MainShipData>("Data/MainShipData");
		cannonShipData = global::UnityEngine.Resources.Load<SubCannonShipData>("Data/SubCannonShipData");
		normalShipData = global::UnityEngine.Resources.Load<SubNormalShipData>("Data/SubNormalShipData");
		rapidShipData = global::UnityEngine.Resources.Load<SubRapidShipData>("Data/SubRapidShipData");
		sucideShipData = global::UnityEngine.Resources.Load<SubSucideShipData>("Data/SubSucideShipData");
		fishShipData = global::UnityEngine.Resources.Load<SubFishShipData>("Data/SubFishShipData");
		harpoonShipData = global::UnityEngine.Resources.Load<SubHarpoonShipData>("Data/SubHarpoonShipData");
		gasShipData = global::UnityEngine.Resources.Load<SubGasShipData>("Data/SubGasShipData");
		repairShipData = global::UnityEngine.Resources.Load<SubRepairShipData>("Data/SubRepairShipData");
		multiShipData = global::UnityEngine.Resources.Load<SubMultiShipData>("Data/SubMultiShipData");
		guideShipData = global::UnityEngine.Resources.Load<SubGuideShipData>("Data/SubGuideShipData");
		subShipUpgradePrice = global::UnityEngine.Resources.Load<SubShipUpgradePrice>("Data/SubShipUpgradePrice");
		enemyReward = global::UnityEngine.Resources.Load<EnemyReward>("Data/EnemyReward");
	}

	public SheetData GetShipSheetData(EShipType shipType, int level)
	{
		SheetData result = default(SheetData);
		level = global::UnityEngine.Mathf.Clamp(level - 1, 0, 999);
		switch (shipType)
		{
		case EShipType.Main:
		case EShipType.Elite:
		case EShipType.Bomber:
		case EShipType.Ghost:
			result.attack = mainShipData.dataArray[level].Attack;
			result.health = mainShipData.dataArray[level].Health;
			result.level = mainShipData.dataArray[level].Level;
			result.fireCoolTime = mainShipData.dataArray[level].Firecooltime;
			result.enemyHealth = mainShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = mainShipData.dataArray[level].Attack;
			break;
		case EShipType.Normal:
			result.attack = normalShipData.dataArray[level].Attack;
			result.health = normalShipData.dataArray[level].Health;
			result.level = normalShipData.dataArray[level].Level;
			result.fireCoolTime = normalShipData.dataArray[level].Firecooltime;
			result.enemyHealth = normalShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = normalShipData.dataArray[level].Enemyattack;
			break;
		case EShipType.Cannon:
			result.attack = cannonShipData.dataArray[level].Attack;
			result.health = cannonShipData.dataArray[level].Health;
			result.level = cannonShipData.dataArray[level].Level;
			result.fireCoolTime = cannonShipData.dataArray[level].Firecooltime;
			result.enemyHealth = cannonShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = cannonShipData.dataArray[level].Enemyattack;
			break;
		case EShipType.Rapid:
			result.attack = rapidShipData.dataArray[level].Attack;
			result.health = rapidShipData.dataArray[level].Health;
			result.level = rapidShipData.dataArray[level].Level;
			result.fireCoolTime = rapidShipData.dataArray[level].Firecooltime;
			result.enemyHealth = rapidShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = rapidShipData.dataArray[level].Enemyattack;
			break;
		case EShipType.SuicideBomb:
			result.attack = sucideShipData.dataArray[level].Attack;
			result.health = sucideShipData.dataArray[level].Health;
			result.level = sucideShipData.dataArray[level].Level;
			result.enemyHealth = sucideShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = sucideShipData.dataArray[level].Enemyattack;
			break;
		case EShipType.Fishing:
			result.attack = fishShipData.dataArray[level].Attack;
			result.health = fishShipData.dataArray[level].Health;
			result.level = fishShipData.dataArray[level].Level;
			break;
		case EShipType.Gas:
			result.attack = gasShipData.dataArray[level].Attack;
			result.health = gasShipData.dataArray[level].Health;
			result.level = gasShipData.dataArray[level].Level;
			result.fireCoolTime = gasShipData.dataArray[level].Firecooltime;
			result.enemyHealth = gasShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = gasShipData.dataArray[level].Enemyattack;
			break;
		case EShipType.Repair:
			result.attack = repairShipData.dataArray[level].Attack;
			result.health = repairShipData.dataArray[level].Health;
			result.level = repairShipData.dataArray[level].Level;
			break;
		case EShipType.Multi:
			result.attack = multiShipData.dataArray[level].Attack;
			result.health = multiShipData.dataArray[level].Health;
			result.level = multiShipData.dataArray[level].Level;
			result.fireCoolTime = multiShipData.dataArray[level].Firecooltime;
			result.enemyHealth = multiShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = multiShipData.dataArray[level].Enemyattack;
			break;
		case EShipType.Harpoon:
			result.attack = harpoonShipData.dataArray[level].Attack;
			result.health = harpoonShipData.dataArray[level].Health;
			result.level = harpoonShipData.dataArray[level].Level;
			break;
		case EShipType.Guide:
			result.attack = guideShipData.dataArray[level].Attack;
			result.health = guideShipData.dataArray[level].Health;
			result.level = guideShipData.dataArray[level].Level;
			result.fireCoolTime = guideShipData.dataArray[level].Firecooltime;
			result.enemyHealth = guideShipData.dataArray[level].Enemyhealth;
			result.enemyAttack = guideShipData.dataArray[level].Enemyattack;
			break;
		}
		return result;
	}
}
