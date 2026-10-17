public class IapManager : global::UnityEngine.MonoBehaviour, global::UnityEngine.Purchasing.IStoreListener
{
	private string receiptUrl = "";

	private string receiptDevUrl = "";

	private string gameID = "";

	private string uuid = "0000";

	private bool checkReceipt;

	private global::Percent.Event.PercentTracker percentTracker;

	[global::UnityEngine.Header("etc")]
	public string inapp_removeads;

	public string inapp_parrot;

	public string inapp_eagle;

	public string inapp_blackbird;

	public string inapp_skin_raft;

	[global::UnityEngine.Header("package")]
	public string inapp_pack_starter;

	public string inapp_pack_today;

	public string inapp_pack_birds;

	public string inapp_pack_inter;

	public string inapp_pack_expert;

	[global::UnityEngine.Header("Crystal")]
	public string inapp_crystal_00;

	public string inapp_crystal_01;

	public string inapp_crystal_02;

	public string inapp_crystal_03;

	public string inapp_crystal_04;

	public string inapp_crystal_05;

	private static global::UnityEngine.Purchasing.IStoreController m_StoreController;

	private static global::UnityEngine.Purchasing.IExtensionProvider m_StoreExtensionProvider;

	private static IapManager sInstance;

	private const string PUBLICKEY = "";

	public global::System.Action loadingBlockOn;

	public global::System.Action loadingBlockOff;

	private bool isBuy;

	public static IapManager Instance
	{
		get
		{
			return sInstance;
		}
		private set
		{
		}
	}

	private void Awake()
	{
		if (sInstance == null)
		{
			ApplyOfflineProductDefaults();
			percentTracker = GetComponent<global::Percent.Event.PercentTracker>();
			sInstance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void ApplyOfflineProductDefaults()
	{
		inapp_removeads = DefaultProductId(inapp_removeads, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.RemoveAds);
		inapp_parrot = DefaultProductId(inapp_parrot, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Parrot);
		inapp_eagle = DefaultProductId(inapp_eagle, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Eagle);
		inapp_blackbird = DefaultProductId(inapp_blackbird, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.BlackBird);
		inapp_skin_raft = DefaultProductId(inapp_skin_raft, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.RaftSkin);
		inapp_pack_starter = DefaultProductId(inapp_pack_starter, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.StarterPack);
		inapp_pack_today = DefaultProductId(inapp_pack_today, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.TodayPack);
		inapp_pack_birds = DefaultProductId(inapp_pack_birds, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.BirdsPack);
		inapp_pack_inter = DefaultProductId(inapp_pack_inter, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.IntermediatePack);
		inapp_pack_expert = DefaultProductId(inapp_pack_expert, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.ExpertPack);
		inapp_crystal_00 = DefaultProductId(inapp_crystal_00, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Crystal00);
		inapp_crystal_01 = DefaultProductId(inapp_crystal_01, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Crystal01);
		inapp_crystal_02 = DefaultProductId(inapp_crystal_02, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Crystal02);
		inapp_crystal_03 = DefaultProductId(inapp_crystal_03, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Crystal03);
		inapp_crystal_04 = DefaultProductId(inapp_crystal_04, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Crystal04);
		inapp_crystal_05 = DefaultProductId(inapp_crystal_05, global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.Crystal05);
	}

	private static string DefaultProductId(string serializedId, string fallback)
	{
		return string.IsNullOrEmpty(serializedId) ? fallback : serializedId;
	}

	private void Start()
	{
		if (m_StoreController == null)
		{
			InitializePurchasing();
		}
	}

	public void InitializePurchasing()
	{
		m_StoreController = null;
		m_StoreExtensionProvider = null;
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("billing", "initialize");
	}

	private bool IsInitialized()
	{
		return false;
	}

	public void BuyProductID(string productId)
	{
		checkReceipt = false;
		isBuy = false;
		if (loadingBlockOn != null)
		{
			loadingBlockOn();
		}
		try
		{
			TryBuyOfflineProduct(productId);
		}
		finally
		{
			if (loadingBlockOff != null)
			{
				loadingBlockOff();
			}
		}
	}

	public bool TryBuyOfflineProduct(string productId)
	{
		if (!global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.TryGet(productId, out var definition))
		{
			global::UnityEngine.Debug.LogWarning("Unknown offline product: " + productId);
			return false;
		}
		if (!CanGrantOfflineProduct(productId))
		{
			global::UnityEngine.Debug.Log("Offline product already owned or unavailable today: " + productId);
			return false;
		}

		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.LocalReward("billing", "offline-buy-" + definition.Id);
		return handlePurchaseSuccess(definition.Id);
	}

	public void MakeAndroidRestore()
	{
		RestoreCheck();
	}

	public void RestorePurchases()
	{
		checkReceipt = false;
		if (loadingBlockOn != null)
		{
			loadingBlockOn();
		}
		try
		{
			RestoreOfflineEntitlements();
		}
		finally
		{
			if (loadingBlockOff != null)
			{
				loadingBlockOff();
			}
		}
	}

	public void RestoreCheck()
	{
		RestorePurchases();
	}

	public int RestoreOfflineEntitlements()
	{
		var restored = 0;
		foreach (var product in global::GoodPirate113.Reconstruction.Offline.GoodPirateOfflineProductCatalog.All)
		{
			if (product.IsRestorable && TryBuyOfflineProduct(product.Id))
			{
				restored++;
			}
		}
		return restored;
	}

	private global::System.Collections.IEnumerator RestoreCheckDirection()
	{
		yield return new global::UnityEngine.WaitForSeconds(1f);
		if (loadingBlockOff != null)
		{
			loadingBlockOff();
		}
	}

	public void OnInitialized(global::UnityEngine.Purchasing.IStoreController controller, global::UnityEngine.Purchasing.IExtensionProvider extensions)
	{
		m_StoreController = null;
		m_StoreExtensionProvider = null;
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("billing", "initialized-callback");
	}

	public void OnInitializeFailed(global::UnityEngine.Purchasing.InitializationFailureReason error)
	{
		if (loadingBlockOff != null)
		{
			loadingBlockOff();
		}
		isBuy = false;
		global::UnityEngine.Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public global::UnityEngine.Purchasing.PurchaseProcessingResult ProcessPurchase(global::UnityEngine.Purchasing.PurchaseEventArgs args)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.LocalReward("billing", "process-purchase");
		handlePurchaseSuccess(args);
		if (loadingBlockOff != null)
		{
			loadingBlockOff();
		}
		return global::UnityEngine.Purchasing.PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(global::UnityEngine.Purchasing.Product product, global::UnityEngine.Purchasing.PurchaseFailureReason failureReason)
	{
		global::UnityEngine.Debug.Log("OnPurchaseFailed");
		if (loadingBlockOff != null)
		{
			loadingBlockOff();
		}
		isBuy = false;
	}

	private void OnApplicationQuit()
	{
		m_StoreController = null;
		m_StoreExtensionProvider = null;
	}

	private global::System.Collections.IEnumerator sendPostReceipt(global::UnityEngine.Purchasing.PurchaseEventArgs args)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.LocalReward("billing", "receipt-validation-bypass");
		handlePurchaseSuccess(args);
		yield break;
	}

	private void JsEval(ref string json)
	{
		for (int i = 0; i < json.Length; i++)
		{
			if (string.Equals("\\", json[i].ToString()))
			{
				json = json.Remove(i, 1);
			}
		}
	}

	private string buildAndroidPayload(global::UnityEngine.Purchasing.PurchaseEventArgs args)
	{
		global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
		stringBuilder.Append("{\"packageName\":\"");
		stringBuilder.Append(global::UnityEngine.Application.identifier);
		stringBuilder.Append("\",\"productId\":\"");
		stringBuilder.Append(args.purchasedProduct.definition.id);
		stringBuilder.Append("\",\"token\":\"");
		string json = global::Boomlagoon.JSON.JSONObject.Parse(args.purchasedProduct.receipt).GetString("Payload");
		JsEval(ref json);
		string json2 = global::Boomlagoon.JSON.JSONObject.Parse(json).GetString("json");
		JsEval(ref json2);
		string value = global::Boomlagoon.JSON.JSONObject.Parse(json2).GetString("purchaseToken");
		stringBuilder.Append(value);
		stringBuilder.Append("\"}");
		return stringBuilder.ToString();
	}

	private void handlePurchaseSuccess(global::UnityEngine.Purchasing.PurchaseEventArgs args)
	{
		if (args == null || args.purchasedProduct == null || args.purchasedProduct.definition == null)
		{
			global::UnityEngine.Debug.LogWarning("Offline purchase callback did not include a product.");
			return;
		}
		TryBuyOfflineProduct(args.purchasedProduct.definition.id);
	}

	private bool CanGrantOfflineProduct(string productId)
	{
		if (PlayerInfo.Instance == null || PlayerInfo.Instance.myData == null)
		{
			return false;
		}
		if (string.Equals(productId, inapp_removeads, global::System.StringComparison.Ordinal))
		{
			return !(bool)PlayerInfo.Instance.myData.isRemoveADS;
		}
		if (string.Equals(productId, inapp_parrot, global::System.StringComparison.Ordinal))
		{
			return !(bool)PlayerInfo.Instance.myData.isHavePirrot;
		}
		if (string.Equals(productId, inapp_eagle, global::System.StringComparison.Ordinal))
		{
			return !(bool)PlayerInfo.Instance.myData.isHaveEagle;
		}
		if (string.Equals(productId, inapp_blackbird, global::System.StringComparison.Ordinal))
		{
			return !PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot;
		}
		if (string.Equals(productId, inapp_skin_raft, global::System.StringComparison.Ordinal))
		{
			return !PlayerInfo.Instance.myData.isBuyRaft
				|| !PlayerInfo.Instance.GetIsHaveShipSkin(eShipSkin.로빈슨의땟목);
		}
		if (string.Equals(productId, inapp_pack_starter, global::System.StringComparison.Ordinal))
		{
			return !PlayerInfo.Instance.myData.isBuyStarter;
		}
		if (string.Equals(productId, inapp_pack_today, global::System.StringComparison.Ordinal))
		{
			return PlayerInfo.Instance.myData.isTodayPackCanBuy;
		}
		if (string.Equals(productId, inapp_pack_birds, global::System.StringComparison.Ordinal))
		{
			return !(bool)PlayerInfo.Instance.myData.isHavePirrot
				|| !(bool)PlayerInfo.Instance.myData.isHaveEagle
				|| !PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot;
		}
		if (string.Equals(productId, inapp_pack_inter, global::System.StringComparison.Ordinal))
		{
			return !PlayerInfo.Instance.myData.isBuyIntermediate;
		}
		if (string.Equals(productId, inapp_pack_expert, global::System.StringComparison.Ordinal))
		{
			return !PlayerInfo.Instance.myData.isBuyExpert;
		}

		return string.Equals(productId, inapp_crystal_00, global::System.StringComparison.Ordinal)
			|| string.Equals(productId, inapp_crystal_01, global::System.StringComparison.Ordinal)
			|| string.Equals(productId, inapp_crystal_02, global::System.StringComparison.Ordinal)
			|| string.Equals(productId, inapp_crystal_03, global::System.StringComparison.Ordinal)
			|| string.Equals(productId, inapp_crystal_04, global::System.StringComparison.Ordinal)
			|| string.Equals(productId, inapp_crystal_05, global::System.StringComparison.Ordinal);
	}

	private bool handlePurchaseSuccess(string productId)
	{
		var args = default(global::UnityEngine.Purchasing.PurchaseEventArgs);
		isBuy = false;
		bool flag = true;
		if (string.Equals(productId, inapp_removeads, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isRemoveADS = true;
			PlayerInfo.Instance.SaveData();
			if (UI_Manager.Instance != null)
			{
				UI_Manager.Instance.inappObj.Refresh();
			}
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_parrot, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isHavePirrot = true;
			PlayerInfo.Instance.SaveData();
			if (GameManager.Instance != null && GameManager.Instance.playerShipController != null)
			{
				GameManager.Instance.playerShipController.GeneratePirrot();
			}
			if (UI_Manager.Instance != null)
			{
				UI_Manager.Instance.inappObj.Refresh();
			}
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_eagle, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isHaveEagle = true;
			PlayerInfo.Instance.SaveData();
			if (GameManager.Instance != null && GameManager.Instance.playerShipController != null)
			{
				GameManager.Instance.playerShipController.GenerateEagle();
			}
			if (UI_Manager.Instance != null)
			{
				UI_Manager.Instance.inappObj.Refresh();
			}
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_blackbird, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot = true;
			PlayerInfo.Instance.SaveData();
			if (UI_Manager.Instance != null)
			{
				UI_Manager.Instance.ui_chestPanel.CheckHaveThridBox();
				UI_Manager.Instance.inappObj.Refresh();
			}
			if (GameManager.Instance != null && GameManager.Instance.playerShipController != null)
			{
				GameManager.Instance.playerShipController.GenerateChestBird();
			}
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_pack_starter, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isBuyStarter = true;
			PlayerInfo.Instance.SaveData();
			MyInventoryDataManager.Instance.AddSailor(ESailor.boatswain_s_inappp, 1);
			for (int i = 0; i < 5; i++)
			{
				PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, new RewardBoxStatus(ERewardBox.GOLD, global::UnityEngine.Mathf.Min(1000, (int)PlayerInfo.Instance.myData.mainShipLevel + 25)));
			}
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 100L);
			RefreshStore();
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_pack_today, global::System.StringComparison.Ordinal))
		{
			if (UI_Manager.Instance != null && UI_Manager.Instance.inappObj != null)
			{
				UI_Manager.Instance.inappObj.OpenSelectedLegend();
			}
			PlayerInfo.Instance.myData.isTodayPackCanBuy = false;
			for (int j = 0; j < 5; j++)
			{
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.sClass);
			}
			RefreshStoreCards();
			RefreshStore();
			PlayerInfo.Instance.SaveData();
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_pack_inter, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isBuyIntermediate = true;
			PlayerInfo.Instance.SaveData();
			for (int k = 0; k < 5; k++)
			{
				PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, new RewardBoxStatus(ERewardBox.BLACK, global::UnityEngine.Mathf.Min(1000, (int)PlayerInfo.Instance.myData.mainShipLevel + 25)));
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.sClass);
			}
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 300L);
			RefreshStoreCards();
			RefreshStore();
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "9.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_pack_expert, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.myData.isBuyExpert = true;
			PlayerInfo.Instance.SaveData();
			for (int l = 0; l < 5; l++)
			{
				PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, new RewardBoxStatus(ERewardBox.GOLD, global::UnityEngine.Mathf.Min(1000, (int)PlayerInfo.Instance.myData.mainShipLevel + 25)));
				PlayerInfo.Instance.myData.storeCardList.Insert(0, EGrade.sClass);
			}
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 1000L);
			RefreshStoreCards();
			RefreshStore();
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "17.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_pack_birds, global::System.StringComparison.Ordinal))
		{
			bool hadEagle = PlayerInfo.Instance.myData.isHaveEagle;
			bool hadParrot = PlayerInfo.Instance.myData.isHavePirrot;
			bool hadChestBird = PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot;
			PlayerInfo.Instance.myData.isHaveEagle = true;
			PlayerInfo.Instance.myData.isHavePirrot = true;
			PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot = true;
			if (UI_Manager.Instance != null)
			{
				UI_Manager.Instance.ui_chestPanel.CheckHaveThridBox();
				UI_Manager.Instance.inappObj.Refresh();
			}
			PlayerInfo.Instance.SaveData();
			if (GameManager.Instance != null && GameManager.Instance.playerShipController != null)
			{
				if (!hadEagle)
				{
					GameManager.Instance.playerShipController.GenerateEagle();
				}
				if (!hadParrot)
				{
					GameManager.Instance.playerShipController.GeneratePirrot();
				}
				if (!hadChestBird)
				{
					GameManager.Instance.playerShipController.GenerateChestBird();
				}
			}
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else if (string.Equals(productId, inapp_crystal_00, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 100L);
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "1.99", "Consume");
			}
		}
		else if (string.Equals(productId, inapp_crystal_01, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 300L);
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "Consume");
			}
		}
		else if (string.Equals(productId, inapp_crystal_02, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 1000L);
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "14.99", "Consume");
			}
		}
		else if (string.Equals(productId, inapp_crystal_03, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 2100L);
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "29.99", "Consume");
			}
		}
		else if (string.Equals(productId, inapp_crystal_04, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 3900L);
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "49.99", "Consume");
			}
		}
		else if (string.Equals(productId, inapp_crystal_05, global::System.StringComparison.Ordinal))
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 7400L);
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "89.99", "Consume");
			}
		}
		else if (string.Equals(productId, inapp_skin_raft, global::System.StringComparison.Ordinal))
		{
			bool firstUnlock = !PlayerInfo.Instance.myData.isBuyRaft;
			if (!PlayerInfo.Instance.GetIsHaveShipSkin(eShipSkin.로빈슨의땟목))
			{
				PlayerInfo.Instance.BuyShipSkin(eShipSkin.로빈슨의땟목);
			}
			PlayerInfo.Instance.myData.isBuyRaft = true;
			if (firstUnlock)
			{
				PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 100L);
			}
			else
			{
				PlayerInfo.Instance.SaveData();
			}
			RefreshStore();
			if (isBuy)
			{
				Appsflyer.Instance.ValidateReceipt("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoh3K8yG4xqHHDpYJTluv5BeiMUo9csuDjHm8+o3wESbem7I0R1vFkhsOPIkj1GxOhCjpMlwabmCgTPFYfsZYrsBZ1jt5TJtx67EnvtGObAe842BYFbezfYnx3fRBed4pf83NUynH83O9S5I+J1P+wFjt31utbmcoNxLnZJD0JaFyaSKouYasLXXDwENlhj4YBn02O2Q/+LDPMN/jJZuVVjWe49FMvrxONQ3FhZl8VMOmRMLc2ZIvhmdsC5jW9YCOK0sW5hIlLIpneEESk8xTWXWFyjP9A9tCMJhSqUg9ux9gIfadukCQaV4mauffRKz+O2to/s1CS1nMTHf+RbsJpQIDAQAB", args, "4.99", "NonConsume");
			}
		}
		else
		{
			flag = false;
			global::UnityEngine.Debug.Log("PURCHASE FAILED");
		}
		if (flag)
		{
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.PlaySFX("ui_inapp_buy");
			}
			if (isBuy)
			{
				if (percentTracker != null)
				{
					percentTracker.triggerInAppPurchase(productId);
				}
				Appsflyer.Instance.SendEvent("buy_inappID_" + productId);
				Appsflyer.Instance.SendEvent("BUY_SUM_PurchaseCount");
				GoogleAnalyticsV3.instance.LogEvent("Inapp", "Buy", productId, 1L);
			}
			global::UnityEngine.Debug.Log(productId + " offline unlock success");
		}
		isBuy = false;
		return flag;
	}

	private static void RefreshStore()
	{
		if (UI_Manager.Instance != null && UI_Manager.Instance.inappObj != null)
		{
			UI_Manager.Instance.inappObj.Refresh();
		}
	}

	private static void RefreshStoreCards()
	{
		if (UI_Manager.Instance != null && UI_Manager.Instance.ui_storeGetCard != null)
		{
			UI_Manager.Instance.ui_storeGetCard.Refresh();
		}
	}
}
