public class MyInventoryDataManager : global::UnityEngine.MonoBehaviour
{
	public static MyInventoryDataManager Instance;

	public MyInventoryData myData;

	public global::System.Collections.Generic.Dictionary<int, MyShipInventory> myShipDataDic = new global::System.Collections.Generic.Dictionary<int, MyShipInventory>();

	public global::System.Collections.Generic.Dictionary<ESailor, MySailorInventory> mySailorDataDic = new global::System.Collections.Generic.Dictionary<ESailor, MySailorInventory>(new ESailorComparer());

	private global::System.Collections.Generic.Dictionary<MyShipInventory, ShipBase> myShips = new global::System.Collections.Generic.Dictionary<MyShipInventory, ShipBase>();

	public global::System.Action AddCrewAction;

	public global::System.Action UpgradeCrewAction;

	private const string MYINVENTORY = "INVENTORY";

	private const int MAX_ID = 1000000;

	public float AbilityAttack { get; private set; }

	public float AbilityCoolTime { get; private set; }

	public float AbilityAcceleration { get; private set; }

	public float AbilityRotation { get; private set; }

	public global::System.Collections.Generic.List<ShipBase> SpawnShipList => global::System.Linq.Enumerable.ToList(myShips.Values);

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			LoadData();
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[global::UnityEngine.ContextMenu("test")]
	private void Test01()
	{
		AddSailor(ESailor.skill_avoidEnvironment, 333);
	}

	private void Test02()
	{
	}

	private void Start()
	{
		GoogleCloudManager instance = GoogleCloudManager.Instance;
		instance.loadAction = (global::System.Action)global::System.Delegate.Combine(instance.loadAction, new global::System.Action(GoogleCloudLoad));
		GoogleCloudManager instance2 = GoogleCloudManager.Instance;
		instance2.saveAction = (global::System.Action)global::System.Delegate.Combine(instance2.saveAction, new global::System.Action(GoogleCloudSave));
	}

	private void GoogleCloudSave()
	{
		if (myData == null)
		{
			global::UnityEngine.Debug.Log("myData is Null");
			return;
		}
		string value = global::UnityEngine.JsonUtility.ToJson(myData);
		GoogleCloudManager.Instance.SetString("INVENTORY", value);
	}

	private void GoogleCloudLoad()
	{
		string text = GoogleCloudManager.Instance.GetString("INVENTORY");
		if (!string.IsNullOrEmpty(text))
		{
			mySailorDataDic.Clear();
			myShipDataDic.Clear();
			MyInventoryData myInventoryData = global::UnityEngine.JsonUtility.FromJson<MyInventoryData>(text);
			myData = myInventoryData;
			for (int i = 0; i < myData.myShipDataList.Count; i++)
			{
				if (!myShipDataDic.ContainsKey(myData.myShipDataList[i].id))
				{
					myShipDataDic.Add(myData.myShipDataList[i].id, myData.myShipDataList[i]);
				}
			}
			for (int j = 0; j < myData.mySailorDataList.Count; j++)
			{
				myData.mySailorDataList[j].baseData = SailorDataManager.Instance.GetSailorBaseData(myData.mySailorDataList[j].sailorType);
				if (!mySailorDataDic.ContainsKey(myData.mySailorDataList[j].sailorType))
				{
					mySailorDataDic.Add(myData.mySailorDataList[j].sailorType, myData.mySailorDataList[j]);
				}
			}
			for (int k = 0; k < 39; k++)
			{
				if (!mySailorDataDic.ContainsKey((ESailor)k))
				{
					MySailorInventory mySailorInventory = new MySailorInventory();
					mySailorInventory.baseData = SailorDataManager.Instance.GetSailorBaseData((ESailor)k);
					mySailorInventory.isHave = false;
					mySailorInventory.haveCount = 0;
					mySailorInventory.level = 0;
					mySailorDataDic.Add((ESailor)k, mySailorInventory);
					if (!myData.mySailorDataList.Contains(mySailorInventory))
					{
						myData.mySailorDataList.Add(mySailorInventory);
					}
				}
			}
			if (myData.myFormationList.Count <= 0)
			{
				for (int l = 0; l < 3; l++)
				{
					myData.myFormationList.Add(new MyFormationData());
				}
			}
			SaveData();
		}
		else
		{
			SaveData();
		}
		AbilityRefresh();
	}

	private void ICloudSave()
	{
		if (myData == null)
		{
			global::UnityEngine.Debug.Log("myData is Null");
			return;
		}
		string value = global::UnityEngine.JsonUtility.ToJson(myData);
		global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetString("INVENTORY", value);
	}

	private void ICloudLoad()
	{
		global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey("INVENTORY");
		if (iSN_NSKeyValueObject != null)
		{
			string stringValue = iSN_NSKeyValueObject.StringValue;
			if (!string.IsNullOrEmpty(stringValue))
			{
				mySailorDataDic.Clear();
				myShipDataDic.Clear();
				MyInventoryData myInventoryData = global::UnityEngine.JsonUtility.FromJson<MyInventoryData>(stringValue);
				myData = myInventoryData;
				for (int i = 0; i < myData.myShipDataList.Count; i++)
				{
					if (!myShipDataDic.ContainsKey(myData.myShipDataList[i].id))
					{
						myShipDataDic.Add(myData.myShipDataList[i].id, myData.myShipDataList[i]);
					}
				}
				for (int j = 0; j < myData.mySailorDataList.Count; j++)
				{
					myData.mySailorDataList[j].baseData = SailorDataManager.Instance.GetSailorBaseData(myData.mySailorDataList[j].sailorType);
					if (!mySailorDataDic.ContainsKey(myData.mySailorDataList[j].sailorType))
					{
						mySailorDataDic.Add(myData.mySailorDataList[j].sailorType, myData.mySailorDataList[j]);
					}
				}
				for (int k = 0; k < 39; k++)
				{
					if (!mySailorDataDic.ContainsKey((ESailor)k))
					{
						MySailorInventory mySailorInventory = new MySailorInventory();
						mySailorInventory.baseData = SailorDataManager.Instance.GetSailorBaseData((ESailor)k);
						mySailorInventory.isHave = false;
						mySailorInventory.haveCount = 0;
						mySailorInventory.level = 0;
						mySailorDataDic.Add((ESailor)k, mySailorInventory);
						if (!myData.mySailorDataList.Contains(mySailorInventory))
						{
							myData.mySailorDataList.Add(mySailorInventory);
						}
					}
				}
				if (myData.myFormationList.Count <= 0)
				{
					for (int l = 0; l < 3; l++)
					{
						myData.myFormationList.Add(new MyFormationData());
					}
				}
				SaveData();
			}
		}
		else
		{
			SaveData();
		}
		AbilityRefresh();
	}

	public void SaveData()
	{
		if (myData == null)
		{
			global::UnityEngine.Debug.Log("myData is Null");
			return;
		}
		string value = global::UnityEngine.JsonUtility.ToJson(myData);
		if (!global::GoodPirate113.Reconstruction.Save.GoodPirate113LegacyStateBridge.TrySaveInventoryData(value))
		{
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetString("INVENTORY", value);
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.Save();
		}
	}

	public void LoadData()
	{
		string text;
		if (!global::GoodPirate113.Reconstruction.Save.GoodPirate113LegacyStateBridge.TryLoadInventoryData(out text))
		{
			text = global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetString("INVENTORY");
		}
		if (!string.IsNullOrEmpty(text))
		{
			MyInventoryData myInventoryData = global::UnityEngine.JsonUtility.FromJson<MyInventoryData>(text);
			myData = myInventoryData;
			for (int i = 0; i < myData.myShipDataList.Count; i++)
			{
				if (!myShipDataDic.ContainsKey(myData.myShipDataList[i].id))
				{
					myShipDataDic.Add(myData.myShipDataList[i].id, myData.myShipDataList[i]);
				}
			}
			for (int j = 0; j < myData.mySailorDataList.Count; j++)
			{
				myData.mySailorDataList[j].baseData = SailorDataManager.Instance.GetSailorBaseData(myData.mySailorDataList[j].sailorType);
				if (!mySailorDataDic.ContainsKey(myData.mySailorDataList[j].sailorType))
				{
					mySailorDataDic.Add(myData.mySailorDataList[j].sailorType, myData.mySailorDataList[j]);
				}
			}
			for (int k = 0; k < 39; k++)
			{
				if (!mySailorDataDic.ContainsKey((ESailor)k))
				{
					MySailorInventory mySailorInventory = new MySailorInventory();
					mySailorInventory.baseData = SailorDataManager.Instance.GetSailorBaseData((ESailor)k);
					mySailorInventory.isHave = false;
					mySailorInventory.haveCount = 0;
					mySailorInventory.level = 0;
					mySailorDataDic.Add((ESailor)k, mySailorInventory);
					if (!myData.mySailorDataList.Contains(mySailorInventory))
					{
						myData.mySailorDataList.Add(mySailorInventory);
					}
				}
			}
			if (myData.myFormationList.Count <= 0)
			{
				for (int l = 0; l < 3; l++)
				{
					myData.myFormationList.Add(new MyFormationData());
				}
			}
		}
		else
		{
			myData = new MyInventoryData();
			myData.myShipDataList = new global::System.Collections.Generic.List<MyShipInventory>();
			myData.mySailorDataList = new global::System.Collections.Generic.List<MySailorInventory>();
			myData.myFormationList = new global::System.Collections.Generic.List<MyFormationData>();
			for (int m = 0; m < 39; m++)
			{
				MySailorInventory mySailorInventory2 = new MySailorInventory();
				mySailorInventory2.baseData = SailorDataManager.Instance.GetSailorBaseData((ESailor)m);
				mySailorInventory2.isHave = false;
				mySailorInventory2.haveCount = 0;
				mySailorInventory2.level = 0;
				mySailorInventory2.sailorType = (ESailor)m;
				mySailorDataDic.Add((ESailor)m, mySailorInventory2);
				myData.mySailorDataList.Add(mySailorInventory2);
			}
			for (int n = 0; n < 3; n++)
			{
				myData.myFormationList.Add(new MyFormationData());
			}
			AddNewShip(EShipType.Normal, autoEquip: true);
			SaveData();
		}
		AbilityRefresh();
	}

	public void ReloadFromReconstructionSave()
	{
		myShipDataDic.Clear();
		mySailorDataDic.Clear();
		myShips.Clear();
		LoadData();
	}

	public void AbilityRefresh()
	{
		AbilityAttack = 0f;
		AbilityCoolTime = 0f;
		AbilityAcceleration = 0f;
		AbilityRotation = 0f;
		foreach (MySailorInventory value in mySailorDataDic.Values)
		{
			if (value.isHave && value.baseData.id != ESailor.boatswain_s_inappp)
			{
				switch (value.baseData.abilityType)
				{
				case ESailorAbility.ATTACK:
					AbilityAttack += GetAbilityValue(value.baseData.abilityType, value.baseData.grade, value.level);
					break;
				case ESailorAbility.COOLTIME:
					AbilityCoolTime += GetAbilityValue(value.baseData.abilityType, value.baseData.grade, value.level);
					break;
				case ESailorAbility.ACCELERATION:
					AbilityAcceleration += GetAbilityValue(value.baseData.abilityType, value.baseData.grade, value.level);
					break;
				case ESailorAbility.ROTATION:
					AbilityRotation += GetAbilityValue(value.baseData.abilityType, value.baseData.grade, value.level);
					break;
				}
			}
		}
	}

	public float GetAbilityValue(ESailorAbility abilityType, EGrade grade, int level)
	{
		float result = 0f;
		if (level == 0)
		{
			return 0f;
		}
		switch (abilityType)
		{
		case ESailorAbility.ATTACK:
			switch (grade)
			{
			case EGrade.aClass:
				switch (level)
				{
				case 1:
					result = 2.5f;
					break;
				case 2:
					result = 3f;
					break;
				case 3:
					result = 3.5f;
					break;
				case 4:
					result = 4f;
					break;
				case 5:
					result = 5f;
					break;
				}
				break;
			case EGrade.bClass:
				switch (level)
				{
				case 1:
					result = 1.5f;
					break;
				case 2:
					result = 1.8f;
					break;
				case 3:
					result = 2.1f;
					break;
				case 4:
					result = 2.4f;
					break;
				case 5:
					result = 3f;
					break;
				}
				break;
			case EGrade.cClass:
				switch (level)
				{
				case 1:
					result = 1f;
					break;
				case 2:
					result = 1.2f;
					break;
				case 3:
					result = 1.4f;
					break;
				case 4:
					result = 1.6f;
					break;
				case 5:
					result = 2f;
					break;
				}
				break;
			case EGrade.sClass:
				switch (level)
				{
				case 1:
					result = 4f;
					break;
				case 2:
					result = 4.8f;
					break;
				case 3:
					result = 5.6f;
					break;
				case 4:
					result = 6.4f;
					break;
				case 5:
					result = 8f;
					break;
				}
				break;
			}
			break;
		case ESailorAbility.COOLTIME:
			switch (grade)
			{
			case EGrade.aClass:
				switch (level)
				{
				case 1:
					result = 2.5f;
					break;
				case 2:
					result = 3f;
					break;
				case 3:
					result = 3.5f;
					break;
				case 4:
					result = 4f;
					break;
				case 5:
					result = 5f;
					break;
				}
				break;
			case EGrade.bClass:
				switch (level)
				{
				case 1:
					result = 1.5f;
					break;
				case 2:
					result = 1.8f;
					break;
				case 3:
					result = 2.1f;
					break;
				case 4:
					result = 2.4f;
					break;
				case 5:
					result = 3f;
					break;
				}
				break;
			case EGrade.cClass:
				switch (level)
				{
				case 1:
					result = 1f;
					break;
				case 2:
					result = 1.2f;
					break;
				case 3:
					result = 1.4f;
					break;
				case 4:
					result = 1.6f;
					break;
				case 5:
					result = 2f;
					break;
				}
				break;
			case EGrade.sClass:
				switch (level)
				{
				case 1:
					result = 4f;
					break;
				case 2:
					result = 4.8f;
					break;
				case 3:
					result = 5.6f;
					break;
				case 4:
					result = 6.4f;
					break;
				case 5:
					result = 8f;
					break;
				}
				break;
			}
			break;
		case ESailorAbility.ACCELERATION:
			switch (grade)
			{
			case EGrade.aClass:
				switch (level)
				{
				case 1:
					result = 2.5f;
					break;
				case 2:
					result = 3f;
					break;
				case 3:
					result = 3.5f;
					break;
				case 4:
					result = 4f;
					break;
				case 5:
					result = 5f;
					break;
				}
				break;
			case EGrade.bClass:
				switch (level)
				{
				case 1:
					result = 1.5f;
					break;
				case 2:
					result = 1.8f;
					break;
				case 3:
					result = 2.1f;
					break;
				case 4:
					result = 2.4f;
					break;
				case 5:
					result = 3f;
					break;
				}
				break;
			case EGrade.cClass:
				switch (level)
				{
				case 1:
					result = 1f;
					break;
				case 2:
					result = 1.3f;
					break;
				case 3:
					result = 1.6f;
					break;
				case 4:
					result = 1.9f;
					break;
				case 5:
					result = 2.2f;
					break;
				}
				break;
			case EGrade.sClass:
				switch (level)
				{
				case 1:
					result = 5f;
					break;
				case 2:
					result = 6f;
					break;
				case 3:
					result = 7f;
					break;
				case 4:
					result = 8f;
					break;
				case 5:
					result = 10f;
					break;
				}
				break;
			}
			break;
		case ESailorAbility.ROTATION:
			switch (grade)
			{
			case EGrade.aClass:
				switch (level)
				{
				case 1:
					result = 3.1f;
					break;
				case 2:
					result = 3.9f;
					break;
				case 3:
					result = 4.7f;
					break;
				case 4:
					result = 5.5f;
					break;
				case 5:
					result = 6.3f;
					break;
				}
				break;
			case EGrade.bClass:
				switch (level)
				{
				case 1:
					result = 2.5f;
					break;
				case 2:
					result = 3f;
					break;
				case 3:
					result = 3.5f;
					break;
				case 4:
					result = 4f;
					break;
				case 5:
					result = 4.5f;
					break;
				}
				break;
			case EGrade.cClass:
				switch (level)
				{
				case 1:
					result = 1f;
					break;
				case 2:
					result = 1.2f;
					break;
				case 3:
					result = 1.4f;
					break;
				case 4:
					result = 1.6f;
					break;
				case 5:
					result = 2f;
					break;
				}
				break;
			case EGrade.sClass:
				switch (level)
				{
				case 1:
					result = 6f;
					break;
				case 2:
					result = 7.5f;
					break;
				case 3:
					result = 9f;
					break;
				case 4:
					result = 10.5f;
					break;
				case 5:
					result = 12f;
					break;
				}
				break;
			}
			break;
		}
		return result;
	}

	public MyShipInventory AddNewShip(EShipType shipType, bool autoEquip = false)
	{
		MyShipInventory myShipInventory = new MyShipInventory();
		int num = global::UnityEngine.Random.Range(0, 1000000);
		while (myShipDataDic.ContainsKey(num))
		{
			num = global::UnityEngine.Random.Range(0, 1000000);
		}
		myShipInventory.id = num;
		myShipInventory.shipType = shipType;
		global::System.Collections.Generic.List<int> list = new global::System.Collections.Generic.List<int>();
		for (int i = 0; i < 3; i++)
		{
			myShipInventory.equipSailerId[i] = -1;
			if (!autoEquip)
			{
				myShipInventory.lineIndex[i] = -1;
				continue;
			}
			for (int j = 0; j < ConstValue.Line_Max; j++)
			{
				list.Add(j);
			}
			foreach (global::System.Collections.Generic.KeyValuePair<int, MyShipInventory> item in myShipDataDic)
			{
				if (list.Contains(item.Value.lineIndex[i]))
				{
					list.Remove(item.Value.lineIndex[i]);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			myShipInventory.lineIndex[i] = list[0];
			myShipInventory.lineTempIndex[i] = list[0];
			list.Clear();
		}
		myShipDataDic.Add(myShipInventory.id, myShipInventory);
		myData.myShipDataList.Add(myShipInventory);
		SaveData();
		return myShipInventory;
	}

	public void AddSailor(ESailor sailor, int count)
	{
		SailorDataManager.Instance.HaveCheck(sailor);
		if (mySailorDataDic.ContainsKey(sailor))
		{
			MySailorInventory mySailorInventory = mySailorDataDic[sailor];
			mySailorInventory.haveCount += count;
			mySailorInventory.isNew = true;
			for (int i = 0; i < myData.mySailorDataList.Count; i++)
			{
				if (myData.mySailorDataList[i].baseData.id == sailor)
				{
					myData.mySailorDataList[i] = mySailorInventory;
				}
			}
		}
		else
		{
			MySailorInventory mySailorInventory2 = new MySailorInventory();
			mySailorInventory2.status = ESailorStatus.OnCall;
			mySailorInventory2.sailorType = sailor;
			mySailorInventory2.haveCount = count;
			mySailorInventory2.isNew = true;
			mySailorInventory2.baseData = SailorDataManager.Instance.GetSailorBaseData(sailor);
			mySailorDataDic.Add(sailor, mySailorInventory2);
			myData.mySailorDataList.Add(mySailorInventory2);
		}
		SaveData();
		if (AddCrewAction != null)
		{
			AddCrewAction();
		}
	}

	public bool UpgradeSailor(ESailor key)
	{
		if (mySailorDataDic.ContainsKey(key))
		{
			MySailorInventory mySailorInventory = mySailorDataDic[key];
			if (!mySailorInventory.isHave)
			{
				mySailorInventory.isHave = true;
			}
			mySailorInventory.haveCount -= mySailorInventory.NextNeedCount;
			mySailorInventory.level++;
			for (int i = 0; i < myData.mySailorDataList.Count; i++)
			{
				if (myData.mySailorDataList[i].baseData.id == key)
				{
					myData.mySailorDataList[i] = mySailorInventory;
				}
			}
			AbilityRefresh();
			SaveData();
			if (UpgradeCrewAction != null)
			{
				UpgradeCrewAction();
			}
			AudioManager.Instance.PlaySFX("ui_upgrade");
			return true;
		}
		global::UnityEngine.Debug.Log("Dont Have Weapon");
		return false;
	}

	public void RemoveShip(int id)
	{
		if (myShipDataDic.ContainsKey(id))
		{
			myData.myShipDataList.Remove(myShipDataDic[id]);
			myShipDataDic.Remove(id);
		}
		SaveData();
	}

	public MySailorInventory GetMySailorData(ESailor key)
	{
		if (mySailorDataDic.ContainsKey(key))
		{
			return mySailorDataDic[key];
		}
		return null;
	}

	public MyShipInventory GetMyShipData(int id)
	{
		if (myShipDataDic.ContainsKey(id))
		{
			return myShipDataDic[id];
		}
		return null;
	}

	public void AddSubShip(ShipBase ship)
	{
		if (!myShips.ContainsKey(ship.InvenShipData))
		{
			myShips.Add(ship.InvenShipData, ship);
		}
	}

	public ShipBase GetShipBaseData(MyShipInventory key)
	{
		if (myShips.ContainsKey(key))
		{
			return myShips[key];
		}
		return null;
	}

	public void SpawnShipClear()
	{
		global::System.Collections.Generic.List<ShipBase> list = global::System.Linq.Enumerable.ToList(myShips.Values);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null)
			{
				global::UnityEngine.Object.Destroy(list[i].gameObject);
			}
		}
		myShips.Clear();
	}
}
