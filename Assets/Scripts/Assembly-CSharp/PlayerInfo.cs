public class PlayerInfo : global::UnityEngine.MonoBehaviour
{
	public delegate void eventCallBack();

	private static PlayerInfo sInstance;

	public bool dataDelete;

	public MyData myData;

	private const string MYDATA = "MyData";

	public PlayerInfo.eventCallBack onMoneyModify;

	private const string CLOUD_ISLAND_KEY = "clearIslandKey_{0}";

	private const string COOLTIMEKEY = "COOLTIME_{0}";

	private global::UnityEngine.Coroutine ironCoroutine;

	private global::UnityEngine.Coroutine fishCoroutine;

	private global::UnityEngine.Coroutine goldCoroutine;

	private global::UnityEngine.Coroutine diamondCoroutine;

	private global::UnityEngine.Coroutine woodCoroutine;

	public bool waitActionGold;

	public bool waitActionDiamond;

	private const string flagKey = "Flag_{0}";

	private const string flagAdsKey = "Flag_{0}_ADS";

	private const string shipKey = "Ship_{0}";

	private const string levelKey = "_Level";

	private const string haveKey = "_IsHave";

	private global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder(64);

	private global::System.Collections.Generic.List<EShipType> spawnShipType = new global::System.Collections.Generic.List<EShipType>();

	private const string islandClear = "Cube_Island_{0}";

	private const string islandClaimClear = "Island_Claim_{0}";

	private const string worldRewardKey = "World_{0}_Reward";

	private const string worldDoorClear = "Cube{0}_Island_{1}_Door_{2}";

	private const string skinKey = "ShipSkin_{0}";

	public static PlayerInfo Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new global::UnityEngine.GameObject("@PlayerInfo").AddComponent<PlayerInfo>();
				global::UnityEngine.Object.DontDestroyOnLoad(sInstance.gameObject);
			}
			return sInstance;
		}
	}

	public long SHOWGOLD { get; set; }

	public long SHOWFISH { get; set; }

	public long SHOWWOOD { get; set; }

	public long SHOWIRON { get; set; }

	public long SHOWDIAMOND { get; set; }

	public string facebookURL { get; private set; }

	public string youtubeURL { get; private set; }

	public int MaxSubShipCount
	{
		get
		{
			int num = 3;
			int clearIslandCount = Instance.ClearIslandCount;
			if (clearIslandCount >= ConstValue.island_clear_4 && clearIslandCount < ConstValue.island_clear_5)
			{
				num = 4;
			}
			else if (clearIslandCount >= ConstValue.island_clear_5 && clearIslandCount < ConstValue.island_clear_6)
			{
				num = 5;
			}
			else if (clearIslandCount >= ConstValue.island_clear_6 && clearIslandCount < ConstValue.island_clear_7)
			{
				num = 6;
			}
			else if (clearIslandCount >= ConstValue.island_clear_7)
			{
				num = 7;
			}
			int num2 = 0;
			global::System.Collections.Generic.List<ESailor> skill_subshipUnLimit = SailorDataManager.Instance.skill_subshipUnLimit;
			if (skill_subshipUnLimit.Count > 0)
			{
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_subshipUnLimit[0]);
				if (mySailorData.isHave)
				{
					switch (mySailorData.level)
					{
					case 1:
						num2 = 1;
						break;
					case 2:
						num2 = 2;
						break;
					}
				}
			}
			return num + num2;
		}
	}

	public int PlayerShipHpMax => ShipDataManager.Instance.GetShipSheetData(EShipType.Main, Instance.myData.mainShipLevel).health;

	public int UseShipCount
	{
		get
		{
			int num = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<int, MyShipInventory> item in MyInventoryDataManager.Instance.myShipDataDic)
			{
				ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(item.Value.shipType);
				num += shipBaseData.useCount;
			}
			return num + (int)myData.reserveShipCount;
		}
	}

	public int ClearIslandCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < ConstValue.islandCount; i++)
			{
				if (GetIsClearIsland(i))
				{
					num++;
				}
			}
			return num;
		}
	}

	public int ClearIslandClaimCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < ConstValue.islandCount; i++)
			{
				if (GetIsClearIslandClaim(i))
				{
					num++;
				}
			}
			return num;
		}
	}

	private void Awake()
	{
		if (sInstance == null)
		{
			if (!GoodPirate113PortablePreferences.TryApplyCurrent(out string preferenceError))
			{
				var runtime = global::GoodPirate113.Reconstruction.Bootstrap.GoodPirateRuntimeBootstrap.Instance;
				string rollbackError;
				if (runtime != null
					&& runtime.TryRollbackImportedSave(out rollbackError)
					&& GoodPirate113PortablePreferences.TryApplyCurrent(out rollbackError))
				{
					global::UnityEngine.Debug.LogWarning(
						"Imported preferences failed validation and the previous local save was restored.");
				}
				else
				{
					global::UnityEngine.Debug.LogError(
						"Portable preferences were rejected: " + preferenceError);
				}
			}
			global::SA.CrossPlatform.Notifications.UM_NotificationCenter.Client.RemoveAllDeliveredNotifications();
			if (dataDelete)
			{
				global::UnityEngine.Debug.Log("삭제완료");
				global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.DeleteAll();
			}
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			global::UnityEngine.Screen.sleepTimeout = -1;
			LoadData();
			if (!GetIsHaveShipSkin(myData.selectedSkin))
			{
				myData.selectedSkin = eShipSkin.Normal;
			}
			SHOWGOLD = myData.gold;
			SHOWFISH = myData.fish;
			SHOWWOOD = myData.wood;
			SHOWIRON = myData.iron;
			SHOWDIAMOND = myData.diamond;
			facebookURL = "https://www.facebook.com/111percent";
			youtubeURL = "https://www.youtube.com/channel/UCSKvHQ-fuH-LLFzeCH7zt6A";
			sInstance = this;
			if (myData.lastConnectedMonth == 0)
			{
				myData.lastConnectedMonth = global::System.DateTime.Now.Month;
				myData.lastConnectedDay = global::System.DateTime.Now.Day;
				myData.isTodayPackCanBuy = true;
			}
			else
			{
				if (myData.lastConnectedDay != global::System.DateTime.Now.Day)
				{
					myData.isTodayPackCanBuy = true;
					myData.isRefreshTodayPack = false;
					myData.todayPackRefreshCount = 0;
				}
				else if (myData.lastConnectedMonth != global::System.DateTime.Now.Month)
				{
					myData.isTodayPackCanBuy = true;
					myData.isRefreshTodayPack = false;
					myData.todayPackRefreshCount = 0;
				}
				myData.lastConnectedMonth = global::System.DateTime.Now.Month;
				myData.lastConnectedDay = global::System.DateTime.Now.Day;
			}
			if (myData.isTargetFps60)
			{
				global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(60);
			}
			else
			{
				global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(30);
			}
			if (myData.achievement_lvl_attend <= 0)
			{
				myData.achievement_lvl_attend = 1;
			}
			if (myData.achievement_lvl_level <= 0)
			{
				myData.achievement_lvl_level = 1;
			}
			if (myData.achievement_lvl_getcrew <= 0)
			{
				myData.achievement_lvl_getcrew = 1;
			}
			if (myData.achievement_lvl_getisland <= 0)
			{
				myData.achievement_lvl_getisland = 1;
			}
			if (myData.achievement_lvl_killenemy <= 0)
			{
				myData.achievement_lvl_killenemy = 1;
			}
			if (myData.achievement_lvl_getflag <= 0)
			{
				myData.achievement_lvl_getflag = 1;
			}
			if (myData.achievement_lvl_killboss <= 0)
			{
				myData.achievement_lvl_killboss = 1;
			}
			CheckSubShip();
			SaveData();
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		spawnShipType.Add(EShipType.Normal);
		spawnShipType.Add(EShipType.Rapid);
		spawnShipType.Add(EShipType.Cannon);
		spawnShipType.Add(EShipType.Gas);
		spawnShipType.Add(EShipType.Guide);
		spawnShipType.Add(EShipType.Multi);
		GoogleCloudManager instance = GoogleCloudManager.Instance;
		instance.loadAction = (global::System.Action)global::System.Delegate.Combine(instance.loadAction, new global::System.Action(GoogleCloudLoad));
		GoogleCloudManager instance2 = GoogleCloudManager.Instance;
		instance2.saveAction = (global::System.Action)global::System.Delegate.Combine(instance2.saveAction, new global::System.Action(GoogleCloudSave));
	}

	private void GoogleCloudSave()
	{
		global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream();
		binaryFormatter.Serialize(memoryStream, myData);
		GoogleCloudManager.Instance.SetString("MyData", global::System.Convert.ToBase64String(memoryStream.GetBuffer()));
		for (int i = 1; i <= 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				string key = $"Cube{1}_Island_{i}_Door_{j}";
				bool isClearDoor = GetIsClearDoor(1, i, j);
				GoogleCloudManager.Instance.SetBool(key, isClearDoor);
			}
		}
		foreach (FlagInfo flag in FlagManager.Instance.flagList)
		{
			string key2 = $"Flag_{flag.id}";
			string key3 = $"Flag_{flag.id}_ADS";
			GoogleCloudManager.Instance.SetBool(key2, IsHaveFlag(flag.id));
			GoogleCloudManager.Instance.SetInt(key3, GetFlagAdsCount(flag.id));
		}
		for (int k = 1; k < 14; k++)
		{
			EShipType eShipType = (EShipType)k;
			string key4 = $"Ship_{eShipType.ToString()}" + "_IsHave";
			eShipType = (EShipType)k;
			string key5 = $"Ship_{eShipType.ToString()}" + "_Level";
			GoogleCloudManager.Instance.SetBool(key4, IsHaveSubShip((EShipType)k));
			GoogleCloudManager.Instance.SetInt(key5, GetShipLevel((EShipType)k));
		}
		for (int l = 1; l <= 6; l++)
		{
			WorldData worldData = GameManager.Instance.worldMaker.GetWorldData(1, l);
			foreach (BossStatus bossStatus in worldData.bossStatusList)
			{
				GoogleCloudManager.Instance.SetBool(bossStatus.bossKey, GetIsClearBoss(bossStatus.bossKey));
			}
			foreach (PatrolStatus patrolStatus in worldData.patrolStatusList)
			{
				string key6 = $"COOLTIME_{patrolStatus.patrolKey}";
				GoogleCloudManager.Instance.SetString(patrolStatus.patrolKey, global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetString(key6, ""));
			}
		}
		for (int m = 0; m < ConstValue.islandCount; m++)
		{
			string key7 = $"clearIslandKey_{m}";
			GoogleCloudManager.Instance.SetBool(key7, GetIsClearIsland(m));
		}
		for (int n = 0; n < 21; n++)
		{
			string key8 = $"ShipSkin_{n}";
			GoogleCloudManager.Instance.SetBool(key8, GetIsHaveShipSkin((eShipSkin)n));
		}
		for (int num = 0; num < ConstValue.islandCount; num++)
		{
			string key9 = $"Island_Claim_{num}";
			GoogleCloudManager.Instance.SetBool(key9, GetIsClearIslandClaim(num));
		}
		for (int num2 = 1; num2 <= 6; num2++)
		{
			string key10 = $"World_{num2}_Reward";
			GoogleCloudManager.Instance.SetBool(key10, GetIsGotWorldReward(num2));
		}
	}

	private void GoogleCloudLoad()
	{
		string text = GoogleCloudManager.Instance.GetString("MyData");
		if (!string.IsNullOrEmpty(text))
		{
			global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			global::System.IO.MemoryStream serializationStream = new global::System.IO.MemoryStream(global::System.Convert.FromBase64String(text));
			myData = (MyData)binaryFormatter.Deserialize(serializationStream);
			if ((int)myData.mainShipLevel < 10)
			{
				myData.mainShipLevel = 10;
			}
			SaveData();
			SHOWDIAMOND = myData.diamond;
			SHOWGOLD = myData.gold;
			SHOWFISH = myData.fish;
			SHOWWOOD = myData.wood;
			SHOWIRON = myData.iron;
		}
		else
		{
			SaveData();
		}
		for (int i = 1; i <= 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				string key = $"Cube{1}_Island_{i}_Door_{j}";
				bool isClear = GoogleCloudManager.Instance.GetBool(key);
				SetClearDoor(1, i, j, isClear);
			}
		}
		foreach (FlagInfo flag in FlagManager.Instance.flagList)
		{
			string key2 = $"Flag_{flag.id}";
			string key3 = $"Flag_{flag.id}_ADS";
			bool isTrue = GoogleCloudManager.Instance.GetBool(key2);
			int count = GoogleCloudManager.Instance.GetInt(key3);
			BuyFlag(flag.id, isTrue);
			SetFlagAdsCount(flag.id, count);
		}
		for (int k = 1; k < 14; k++)
		{
			EShipType eShipType = (EShipType)k;
			string key4 = $"Ship_{eShipType.ToString()}" + "_IsHave";
			eShipType = (EShipType)k;
			string key5 = $"Ship_{eShipType.ToString()}" + "_Level";
			bool isTrue2 = GoogleCloudManager.Instance.GetBool(key4);
			int level = GoogleCloudManager.Instance.GetInt(key5);
			SetShipLevel((EShipType)k, level);
			BuySubShip((EShipType)k, isTrue2);
		}
		for (int l = 1; l <= 6; l++)
		{
			WorldData worldData = GameManager.Instance.worldMaker.GetWorldData(1, l);
			foreach (BossStatus bossStatus in worldData.bossStatusList)
			{
				bool isClear2 = GoogleCloudManager.Instance.GetBool(bossStatus.bossKey);
				SetClearBoss(bossStatus.bossKey, isClear2);
			}
			foreach (PatrolStatus patrolStatus in worldData.patrolStatusList)
			{
				string key6 = $"COOLTIME_{patrolStatus.patrolKey}";
				string value = GoogleCloudManager.Instance.GetString(patrolStatus.patrolKey);
				global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetString(key6, value);
			}
		}
		for (int m = 0; m < 210; m++)
		{
			string key7 = $"clearIslandKey_{m}";
			bool clear = GoogleCloudManager.Instance.GetBool(key7);
			SetClearIsland(m, clear);
		}
		for (int n = 0; n < 21; n++)
		{
			string key8 = $"ShipSkin_{n}";
			bool buy = GoogleCloudManager.Instance.GetBool(key8);
			BuyShipSkin((eShipSkin)n, buy);
		}
		for (int num = 0; num < ConstValue.islandCount; num++)
		{
			string key9 = $"Island_Claim_{num}";
			SetClearIslandClaim(num, GoogleCloudManager.Instance.GetBool(key9));
		}
		for (int num2 = 1; num2 <= 6; num2++)
		{
			string key10 = $"World_{num2}_Reward";
			SetIsGotWorldReward(num2, GoogleCloudManager.Instance.GetBool(key10));
		}
	}

	private void ICloudSave()
	{
		global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream();
		binaryFormatter.Serialize(memoryStream, myData);
		global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetString("MyData", global::System.Convert.ToBase64String(memoryStream.GetBuffer()));
		for (int i = 1; i <= 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				string key = $"Cube{1}_Island_{i}_Door_{j}";
				bool isClearDoor = GetIsClearDoor(1, i, j);
				global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool(key, isClearDoor);
			}
		}
		foreach (FlagInfo flag in FlagManager.Instance.flagList)
		{
			string key2 = $"Flag_{flag.id}";
			string key3 = $"Flag_{flag.id}_ADS";
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool(key2, IsHaveFlag(flag.id));
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetInt(key3, GetFlagAdsCount(flag.id));
		}
		for (int k = 1; k < 14; k++)
		{
			EShipType eShipType = (EShipType)k;
			string key4 = $"Ship_{eShipType.ToString()}" + "_IsHave";
			eShipType = (EShipType)k;
			string key5 = $"Ship_{eShipType.ToString()}" + "_Level";
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool(key4, IsHaveSubShip((EShipType)k));
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetInt(key5, GetShipLevel((EShipType)k));
		}
		for (int l = 1; l <= 6; l++)
		{
			WorldData worldData = GameManager.Instance.worldMaker.GetWorldData(1, l);
			foreach (BossStatus bossStatus in worldData.bossStatusList)
			{
				global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool(bossStatus.bossKey, GetIsClearBoss(bossStatus.bossKey));
			}
			foreach (PatrolStatus patrolStatus in worldData.patrolStatusList)
			{
				string key6 = $"COOLTIME_{patrolStatus.patrolKey}";
				global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetString(patrolStatus.patrolKey, global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetString(key6, ""));
			}
		}
		for (int m = 0; m < 210; m++)
		{
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool($"clearIslandKey_{m}", GetIsClearIsland(m));
		}
		for (int n = 0; n < 21; n++)
		{
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool($"ShipSkin_{n}", GetIsHaveShipSkin((eShipSkin)n));
		}
		for (int num = 0; num < ConstValue.islandCount; num++)
		{
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool($"Island_Claim_{num}", GetIsClearIslandClaim(num));
		}
		for (int num2 = 1; num2 <= 6; num2++)
		{
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool($"World_{num2}_Reward", GetIsGotWorldReward(num2));
		}
	}

	private void ICloudLoad()
	{
		global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey("MyData");
		if (iSN_NSKeyValueObject != null)
		{
			string stringValue = iSN_NSKeyValueObject.StringValue;
			if (!string.IsNullOrEmpty(stringValue))
			{
				global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				global::System.IO.MemoryStream serializationStream = new global::System.IO.MemoryStream(global::System.Convert.FromBase64String(stringValue));
				myData = (MyData)binaryFormatter.Deserialize(serializationStream);
				if ((int)myData.mainShipLevel < 10)
				{
					myData.mainShipLevel = 10;
				}
				SaveData();
				SHOWDIAMOND = myData.diamond;
				SHOWGOLD = myData.gold;
				SHOWFISH = myData.fish;
				SHOWWOOD = myData.wood;
				SHOWIRON = myData.iron;
			}
			else
			{
				SaveData();
			}
		}
		for (int i = 1; i <= 6; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject2 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey($"Cube{1}_Island_{i}_Door_{j}");
				if (iSN_NSKeyValueObject2 != null)
				{
					SetClearDoor(1, i, j, iSN_NSKeyValueObject2.BoolValue);
				}
			}
		}
		foreach (FlagInfo flag in FlagManager.Instance.flagList)
		{
			string key = $"Flag_{flag.id}";
			string key2 = $"Flag_{flag.id}_ADS";
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject3 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey(key);
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject4 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey(key2);
			if (iSN_NSKeyValueObject3 != null)
			{
				BuyFlag(flag.id, iSN_NSKeyValueObject3.BoolValue);
			}
			if (iSN_NSKeyValueObject4 != null)
			{
				SetFlagAdsCount(flag.id, iSN_NSKeyValueObject4.IntValue);
			}
		}
		for (int k = 1; k < 14; k++)
		{
			EShipType eShipType = (EShipType)k;
			string key3 = $"Ship_{eShipType.ToString()}" + "_IsHave";
			eShipType = (EShipType)k;
			string key4 = $"Ship_{eShipType.ToString()}" + "_Level";
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject5 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey(key3);
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject6 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey(key4);
			if (iSN_NSKeyValueObject6 != null)
			{
				SetShipLevel((EShipType)k, iSN_NSKeyValueObject6.IntValue);
			}
			if (iSN_NSKeyValueObject5 != null)
			{
				BuySubShip((EShipType)k, iSN_NSKeyValueObject5.BoolValue);
			}
		}
		for (int l = 1; l <= 6; l++)
		{
			WorldData worldData = GameManager.Instance.worldMaker.GetWorldData(1, l);
			foreach (BossStatus bossStatus in worldData.bossStatusList)
			{
				global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject7 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey(bossStatus.bossKey);
				if (iSN_NSKeyValueObject7 != null)
				{
					SetClearBoss(bossStatus.bossKey, iSN_NSKeyValueObject7.BoolValue);
				}
			}
			foreach (PatrolStatus patrolStatus in worldData.patrolStatusList)
			{
				string key5 = $"COOLTIME_{patrolStatus.patrolKey}";
				global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject8 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey(patrolStatus.patrolKey);
				if (iSN_NSKeyValueObject8 != null)
				{
					global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetString(key5, iSN_NSKeyValueObject8.StringValue);
				}
			}
		}
		for (int m = 0; m < 210; m++)
		{
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject9 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey($"clearIslandKey_{m}");
			if (iSN_NSKeyValueObject9 != null)
			{
				SetClearIsland(m, iSN_NSKeyValueObject9.BoolValue);
			}
		}
		for (int n = 0; n < 21; n++)
		{
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject10 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey($"ShipSkin_{n}");
			if (iSN_NSKeyValueObject10 != null)
			{
				BuyShipSkin((eShipSkin)n, iSN_NSKeyValueObject10.BoolValue);
			}
		}
		for (int num = 0; num < ConstValue.islandCount; num++)
		{
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject11 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey($"Island_Claim_{num}");
			if (iSN_NSKeyValueObject11 != null)
			{
				SetClearIslandClaim(num, iSN_NSKeyValueObject11.BoolValue);
			}
		}
		for (int num2 = 1; num2 <= 6; num2++)
		{
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject12 = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey($"World_{num2}_Reward");
			if (iSN_NSKeyValueObject12 != null)
			{
				SetIsGotWorldReward(num2, iSN_NSKeyValueObject12.BoolValue);
			}
		}
	}

	private void LoadData()
	{
		string text;
		if (!global::GoodPirate113.Reconstruction.Save.GoodPirate113LegacyStateBridge.TryLoadPlayerData(out text))
		{
			text = global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetString("MyData");
		}
		if (!string.IsNullOrEmpty(text))
		{
			global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			global::System.IO.MemoryStream serializationStream = new global::System.IO.MemoryStream(global::System.Convert.FromBase64String(text));
			myData = (MyData)binaryFormatter.Deserialize(serializationStream);
			if ((int)myData.mainShipLevel == 0)
			{
				myData.mainShipLevel = 1;
			}
		}
		else
		{
			myData = new MyData();
			myData.mainShipLevel = 10;
			myData.gold = 5000L;
			myData.diamond = 50L;
			myData.destinationIslandId = -1;
			BuySubShip(EShipType.Normal);
			BuySubShip(EShipType.Rapid);
			myData.isTargetFps60 = true;
			BuyFlag(0);
			SetClearIsland(0);
			CoolTimeManager.AddCooltimeSecondFromNow("RewardBoxSlot_0", ConstValue.rewardFreeBoxTime);
			SaveData();
		}
		if (myData.storeGetRewardBoxList == null)
		{
			myData.storeGetRewardBoxList = new global::System.Collections.Generic.List<RewardBoxStatus>();
		}
		if (myData.storeCardList == null)
		{
			myData.storeCardList = new global::System.Collections.Generic.List<EGrade>();
		}
		if (myData.isBuyRaft)
		{
			BuyShipSkin(eShipSkin.로빈슨의땟목);
		}
	}

	public void SaveData()
	{
		global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream();
		binaryFormatter.Serialize(memoryStream, myData);
		string playerDataBase64 = global::System.Convert.ToBase64String(memoryStream.GetBuffer());
		if (!global::GoodPirate113.Reconstruction.Save.GoodPirate113LegacyStateBridge.TrySavePlayerData(playerDataBase64))
		{
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetString("MyData", playerDataBase64);
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.Save();
		}
	}

	public void ReloadFromReconstructionSave()
	{
		LoadData();
		if (!GetIsHaveShipSkin(myData.selectedSkin))
		{
			myData.selectedSkin = eShipSkin.Normal;
		}
		if (myData.storeGetRewardBoxList == null)
		{
			myData.storeGetRewardBoxList = new global::System.Collections.Generic.List<RewardBoxStatus>();
		}
		if (myData.storeCardList == null)
		{
			myData.storeCardList = new global::System.Collections.Generic.List<EGrade>();
		}
		SHOWGOLD = myData.gold;
		SHOWFISH = myData.fish;
		SHOWWOOD = myData.wood;
		SHOWIRON = myData.iron;
		SHOWDIAMOND = myData.diamond;
		if (myData.lastConnectedMonth == 0
			|| myData.lastConnectedMonth != global::System.DateTime.Now.Month
			|| myData.lastConnectedDay != global::System.DateTime.Now.Day)
		{
			myData.isTodayPackCanBuy = true;
			myData.isRefreshTodayPack = false;
			myData.todayPackRefreshCount = 0;
		}
		myData.lastConnectedMonth = global::System.DateTime.Now.Month;
		myData.lastConnectedDay = global::System.DateTime.Now.Day;
		global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(
			myData.isTargetFps60 ? 60 : 30);
		myData.achievement_lvl_attend = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_attend);
		myData.achievement_lvl_level = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_level);
		myData.achievement_lvl_getcrew = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_getcrew);
		myData.achievement_lvl_getisland = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_getisland);
		myData.achievement_lvl_killenemy = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_killenemy);
		myData.achievement_lvl_getflag = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_getflag);
		myData.achievement_lvl_killboss = global::UnityEngine.Mathf.Max(1, myData.achievement_lvl_killboss);
		CheckSubShip();
		SaveData();
	}

	public void SoundOn()
	{
		myData.isSoundOn = true;
		SaveData();
	}

	public void SoundOff()
	{
		myData.isSoundOn = false;
		SaveData();
	}

	public void MusicOn()
	{
		myData.isMusicOn = true;
		SaveData();
	}

	public void MusicOff()
	{
		myData.isMusicOn = false;
		SaveData();
	}

	public void AddMoney(EMonenyType moneyType, long i, bool isSave = true, float delay = 0f, float duration = 0.5f)
	{
		switch (moneyType)
		{
		case EMonenyType.diamond:
		{
			MyData obj3 = myData;
			obj3.diamond = (long)obj3.diamond + i;
			if (diamondCoroutine != null)
			{
				StopCoroutine(diamondCoroutine);
			}
			diamondCoroutine = StartCoroutine(MoneyCoroutine(moneyType, delay, duration));
			break;
		}
		case EMonenyType.fish:
		{
			MyData obj5 = myData;
			obj5.fish = (long)obj5.fish + i;
			if (fishCoroutine != null)
			{
				StopCoroutine(fishCoroutine);
			}
			fishCoroutine = StartCoroutine(MoneyCoroutine(moneyType, delay, duration));
			break;
		}
		case EMonenyType.gold:
		{
			MyData obj2 = myData;
			obj2.gold = (long)obj2.gold + i;
			if (goldCoroutine != null)
			{
				StopCoroutine(goldCoroutine);
			}
			goldCoroutine = StartCoroutine(MoneyCoroutine(moneyType, delay, duration));
			break;
		}
		case EMonenyType.iron:
		{
			MyData obj4 = myData;
			obj4.iron = (long)obj4.iron + i;
			if (ironCoroutine != null)
			{
				StopCoroutine(ironCoroutine);
			}
			ironCoroutine = StartCoroutine(MoneyCoroutine(moneyType, delay, duration));
			break;
		}
		case EMonenyType.wood:
		{
			MyData obj = myData;
			obj.wood = (long)obj.wood + i;
			if (woodCoroutine != null)
			{
				StopCoroutine(woodCoroutine);
			}
			woodCoroutine = StartCoroutine(MoneyCoroutine(moneyType, delay, duration));
			break;
		}
		}
		if (onMoneyModify != null)
		{
			onMoneyModify();
		}
		if (isSave)
		{
			SaveData();
		}
	}

	private global::System.Collections.IEnumerator MoneyCoroutine(EMonenyType moneyType, float delay, float duration)
	{
		long startMoney = 0L;
		long targetMoney = 0L;
		switch (moneyType)
		{
		case EMonenyType.diamond:
			startMoney = SHOWDIAMOND;
			targetMoney = myData.diamond;
			break;
		case EMonenyType.fish:
			startMoney = SHOWFISH;
			targetMoney = myData.fish;
			break;
		case EMonenyType.gold:
			startMoney = SHOWGOLD;
			targetMoney = myData.gold;
			break;
		case EMonenyType.iron:
			startMoney = SHOWIRON;
			targetMoney = myData.iron;
			break;
		case EMonenyType.wood:
			startMoney = SHOWWOOD;
			targetMoney = myData.wood;
			break;
		}
		switch (moneyType)
		{
		case EMonenyType.gold:
			yield return new global::UnityEngine.WaitUntil(() => !waitActionGold);
			break;
		case EMonenyType.diamond:
			yield return new global::UnityEngine.WaitUntil(() => !waitActionDiamond);
			break;
		}
		float totalTime = 0f;
		yield return new global::UnityEngine.WaitForSecondsRealtime(delay);
		while (true)
		{
			totalTime += global::UnityEngine.Time.unscaledDeltaTime;
			double num = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			long num2 = myMathf.Lerp(startMoney, targetMoney, num);
			switch (moneyType)
			{
			case EMonenyType.diamond:
				SHOWDIAMOND = num2;
				break;
			case EMonenyType.fish:
				SHOWFISH = num2;
				break;
			case EMonenyType.gold:
				SHOWGOLD = num2;
				break;
			case EMonenyType.iron:
				SHOWIRON = num2;
				break;
			case EMonenyType.wood:
				SHOWWOOD = num2;
				break;
			}
			if (!(num >= 1.0))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public bool IsHaveFlag(int flagId)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Flag_{flagId}", defaultValue: false);
	}

	public void BuyFlag(int flagId, bool isTrue = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Flag_{flagId}", isTrue);
	}

	public int GetFlagAdsCount(int flagId)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetInt($"Flag_{flagId}_ADS", 0);
	}

	public void SetFlagAdsCount(int flagId, int count)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetInt($"Flag_{flagId}_ADS", count);
	}

	public bool IsHaveSubShip(EShipType shipType)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Ship_{shipType.ToString()}" + "_IsHave", defaultValue: false);
	}

	public void BuySubShip(EShipType shipType, bool isTrue = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Ship_{shipType.ToString()}" + "_IsHave", isTrue);
	}

	public int GetShipLevel(EShipType shipType)
	{
		sb.Length = 0;
		sb.Append("Ship_");
		sb.Append(shipType.ToString());
		sb.Append("_Level");
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetInt(sb.ToString(), 1);
	}

	public void SetShipLevel(EShipType shipType, int level)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetInt($"Ship_{shipType.ToString()}" + "_Level", level);
	}

	public void CheckSubShip()
	{
		int clearIslandCount = ClearIslandCount;
		foreach (EShipType item in spawnShipType)
		{
			if (!IsHaveSubShip(item))
			{
				ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(item);
				if (clearIslandCount >= shipBaseData.unlockIslandCount)
				{
					BuySubShip(item);
					SetIsNew(item, isNew: true);
					UI_Manager.Instance.ui_spawnShipManager.Refresh();
					UI_Manager.Instance.ui_mainGame.ui_mainUpgradeMenu.NewOpen();
				}
			}
		}
	}

	public bool GetIsClearIsland(int level)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Cube_Island_{level}", defaultValue: false);
	}

	public void SetClearIsland(int level, bool clear = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Cube_Island_{level}", clear);
	}

	public bool GetIsClearIslandClaim(int level)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Island_Claim_{level}", defaultValue: false);
	}

	public void SetClearIslandClaim(int level, bool clear = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Island_Claim_{level}", clear);
	}

	public bool GetIsGotWorldReward(int worldNum)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"World_{worldNum}_Reward", defaultValue: false);
	}

	public void SetIsGotWorldReward(int worldNum, bool get = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"World_{worldNum}_Reward", get);
	}

	public bool GetIsClearDoor(int cube, int island, int door)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Cube{cube}_Island_{island}_Door_{door}", defaultValue: false);
	}

	public void SetClearDoor(int cube, int island, int door, bool isClear = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Cube{cube}_Island_{island}_Door_{door}", isClear);
	}

	public bool GetIsNew(EShipType type)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool(type.ToString() + "_isNew", defaultValue: false);
	}

	public void SetIsNew(EShipType type, bool isNew)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool(type.ToString() + "_isNew", isNew);
	}

	public bool GetIsClearBoss(string key)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool(key, defaultValue: false);
	}

	public void SetClearBoss(string key, bool isClear)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool(key, isClear);
	}

	public void BuyShipSkin(eShipSkin skin, bool buy = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"ShipSkin_{(int)skin}", buy);
	}

	public bool GetIsHaveShipSkin(eShipSkin skin)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"ShipSkin_{(int)skin}", defaultValue: false);
	}
}
