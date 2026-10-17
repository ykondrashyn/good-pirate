public class IslandRewardGrid : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cardSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject crewCardSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject moneySrc;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> objList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform[] slots;

	private IslandSheetData sheetData;

	[global::UnityEngine.HideInInspector]
	public int number;

	private void Awake()
	{
		sheetData = global::UnityEngine.Resources.Load<IslandSheetData>("Data/IslandSheetData");
	}

	private void OnEnable()
	{
		foreach (global::UnityEngine.GameObject obj in objList)
		{
			global::UnityEngine.Object.Destroy(obj);
		}
		objList.Clear();
	}

	public void SetRewardData(IslandStatus status, bool isAddRealData = false)
	{
		number = 0;
		IslandSheetDataData islandSheetDataData = sheetData.dataArray[status.islandId];
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(moneySrc, slots[0]);
		UI_ItemSlot component = gameObject.GetComponent<UI_ItemSlot>();
		if (isAddRealData)
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.gold, islandSheetDataData.Gold);
		}
		component.SetData(EMonenyType.gold, islandSheetDataData.Gold);
		component.FreeBoxOpen();
		objList.Add(gameObject);
		if (islandSheetDataData.Diamond == 0)
		{
			if (islandSheetDataData.Skillcrewid != 0)
			{
				ESailor skillcrewid = (ESailor)islandSheetDataData.Skillcrewid;
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skillcrewid);
				if (mySailorData.level >= mySailorData.baseData.maxCount)
				{
					gameObject = global::UnityEngine.Object.Instantiate(moneySrc, slots[1]);
					objList.Add(gameObject);
					UI_ItemSlot component2 = gameObject.GetComponent<UI_ItemSlot>();
					component2.SetData(EMonenyType.diamond, 5L);
					if (isAddRealData)
					{
						PlayerInfo.Instance.AddMoney(EMonenyType.diamond, islandSheetDataData.Diamond);
					}
					component2.FreeBoxOpen();
				}
				else
				{
					gameObject = global::UnityEngine.Object.Instantiate(crewCardSrc, slots[1]);
					UI_ItemSlot component3 = gameObject.GetComponent<UI_ItemSlot>();
					objList.Add(gameObject);
					if (isAddRealData)
					{
						MyInventoryDataManager.Instance.AddSailor(skillcrewid, 1);
					}
					component3.SetData(mySailorData, 1);
					component3.FreeBoxOpen();
				}
			}
			else
			{
				EGrade crewgrade = (EGrade)islandSheetDataData.Crewgrade;
				SailorDataManager.Instance.GetSailorAbility(out var abilityList);
				if (SailorDataManager.Instance.GetSailor(crewgrade, abilityList, out var getSailor))
				{
					if (getSailor != ESailor.END)
					{
						MySailorInventory mySailorData2 = MyInventoryDataManager.Instance.GetMySailorData(getSailor);
						if (isAddRealData)
						{
							MyInventoryDataManager.Instance.AddSailor(getSailor, islandSheetDataData.Crewcount);
						}
						gameObject = global::UnityEngine.Object.Instantiate(cardSrc, slots[1]);
						UI_ItemSlot component4 = gameObject.GetComponent<UI_ItemSlot>();
						objList.Add(gameObject);
						component4.SetData(mySailorData2, islandSheetDataData.Crewcount);
						component4.FreeBoxOpen();
					}
					else
					{
						gameObject = global::UnityEngine.Object.Instantiate(moneySrc, slots[1]);
						objList.Add(gameObject);
						UI_ItemSlot component5 = gameObject.GetComponent<UI_ItemSlot>();
						component5.SetData(EMonenyType.diamond, 5L);
						if (isAddRealData)
						{
							PlayerInfo.Instance.AddMoney(EMonenyType.diamond, islandSheetDataData.Diamond);
						}
						component5.FreeBoxOpen();
					}
				}
			}
		}
		else
		{
			gameObject = global::UnityEngine.Object.Instantiate(moneySrc, slots[1]);
			objList.Add(gameObject);
			UI_ItemSlot component6 = gameObject.GetComponent<UI_ItemSlot>();
			component6.SetData(EMonenyType.diamond, islandSheetDataData.Diamond);
			if (isAddRealData)
			{
				PlayerInfo.Instance.AddMoney(EMonenyType.diamond, islandSheetDataData.Diamond);
			}
			component6.FreeBoxOpen();
		}
		if (slots.Length > 2)
		{
			slots[2].gameObject.SetActive(value: false);
			slots[3].gameObject.SetActive(value: false);
		}
	}

	public void AddItemSlot(EMonenyType moneyType, int amount)
	{
		if (number < slots.Length)
		{
			if (!slots[number].gameObject.activeSelf)
			{
				slots[number].gameObject.SetActive(value: true);
			}
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(moneySrc, slots[number]);
			objList.Add(gameObject);
			UI_ItemSlot component = gameObject.GetComponent<UI_ItemSlot>();
			PlayerInfo.Instance.waitActionDiamond = true;
			PlayerInfo.Instance.waitActionDiamond = true;
			component.SetData(moneyType, amount);
			PlayerInfo.Instance.AddMoney(moneyType, amount);
			component.FreeBoxOpen();
			number++;
			if (number <= slots.Length - 1)
			{
				slots[number].gameObject.SetActive(value: false);
			}
		}
	}

	public void AddItemSlot(ERewardBox boxType, int level, int amount)
	{
		if (number < slots.Length)
		{
			if (!slots[number].gameObject.activeSelf)
			{
				slots[number].gameObject.SetActive(value: true);
			}
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(moneySrc, slots[number]);
			objList.Add(gameObject);
			UI_ItemSlot component = gameObject.GetComponent<UI_ItemSlot>();
			component.SetData(boxType, amount, level);
			for (int i = 0; i < amount; i++)
			{
				RewardBoxStatus rewardBoxStatus = new RewardBoxStatus();
				rewardBoxStatus.boxType = boxType;
				rewardBoxStatus.level = level;
				PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, rewardBoxStatus);
			}
			component.FreeBoxOpen();
			number++;
			if (number <= slots.Length - 1)
			{
				slots[number].gameObject.SetActive(value: false);
			}
			PlayerInfo.Instance.SaveData();
		}
	}

	public void AddItemSlot(ESailor sailorType, int count)
	{
		if (number < slots.Length)
		{
			if (!slots[number].gameObject.activeSelf)
			{
				slots[number].gameObject.SetActive(value: true);
			}
			_ = SailorDataManager.Instance.GetSailorBaseData(sailorType).grade;
			if (SailorDataManager.Instance.IsCanGiveSailor(sailorType))
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(cardSrc, slots[number]);
				UI_ItemSlot component = gameObject.GetComponent<UI_ItemSlot>();
				objList.Add(gameObject);
				MyInventoryDataManager.Instance.AddSailor(sailorType, count);
				global::UnityEngine.Debug.Log(sailorType);
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(sailorType);
				component.SetData(mySailorData, count);
				component.FreeBoxOpen();
			}
			else
			{
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(moneySrc, slots[number]);
				objList.Add(gameObject2);
				UI_ItemSlot component2 = gameObject2.GetComponent<UI_ItemSlot>();
				PlayerInfo.Instance.waitActionDiamond = true;
				PlayerInfo.Instance.waitActionDiamond = true;
				component2.SetData(EMonenyType.diamond, 10L);
				PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 10L);
				component2.FreeBoxOpen();
			}
			number++;
			if (number <= slots.Length - 1)
			{
				slots[number].gameObject.SetActive(value: false);
			}
		}
	}
}
