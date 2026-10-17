namespace Percent
{
	public class CrossPromotion : global::UnityEngine.MonoBehaviour
	{
		public enum Status
		{
			INIT = 0,
			LOAD_SUCCESS = 1,
			LOAD_FAIL = 2,
			SHOW = 3,
			HIDE = 4,
			DESTROY = 5
		}

		public delegate void OnShowDelegate();

		public delegate void OnCloseDelegate();

		public delegate void OnLoadDataDelegate(bool isSuccess);

		public delegate void OnDestroyDelegate();

		internal delegate void OnStateChange(global::Percent.CrossPromotion.Status state);

		public static global::Percent.CrossPromotion instance;

		public global::Percent.Company company;

		public global::Percent.View.PrivacyPopup privacyPopup;

		public static bool isEURegion = false;

		public static bool hasAgreed = true;

		public global::Percent.CrossPromotion.OnShowDelegate onShow;

		public global::Percent.CrossPromotion.OnCloseDelegate onClose;

		public global::Percent.CrossPromotion.OnLoadDataDelegate onLoadData;

		public global::Percent.CrossPromotion.OnDestroyDelegate onDestroy;

		private global::Percent.CrossPromotion.Status state;

		internal global::Percent.CrossPromotion.OnStateChange onStateChange;

		private global::Percent.TextureLifeCycle textureLifeCycle;

		private global::Percent.SceneChecker sceneChecker;

		private global::Percent.Interstitial interstitial;

		public static readonly string PREF_AGREEMENT = "GDPRAgreement";

		public static global::Boomlagoon.JSON.JSONObject ExtraData => global::Percent.PromotionData.extraData;

		private global::Percent.CrossPromotion.Status State
		{
			set
			{
				state = value;
				if (onStateChange != null)
				{
					onStateChange(state);
				}
			}
		}

		public static void showAds(bool placeHolder = true)
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "show-cross-promotion");
		}

		private void show()
		{
			sceneChecker.saveCalledShowScene();
			if (state.Equals(global::Percent.CrossPromotion.Status.LOAD_SUCCESS))
			{
				State = global::Percent.CrossPromotion.Status.SHOW;
				triggerShowEvent();
				if (onShow != null)
				{
					onShow();
				}
			}
		}

		private void triggerShowEvent()
		{
			global::Percent.PromotionType type = global::Percent.PromotionData.crossPromotionData.type;
			int gameId = global::Percent.PromotionData.crossPromotionData.gameId;
			base.gameObject.AddComponent<global::Percent.Event.ExposeTracker>().trigger(type, gameId);
		}

		public static void addShowAdsToQueue()
		{
			if (instance == null)
			{
				global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "cross-promotion-queue");
			}
			else
			{
				instance.setPrivacyCompletion();
			}
		}

		private void setPrivacyCompletion()
		{
			onLoadData = (global::Percent.CrossPromotion.OnLoadDataDelegate)global::System.Delegate.Combine(onLoadData, new global::Percent.CrossPromotion.OnLoadDataDelegate(showAds));
		}

		public void hide()
		{
			if (state.Equals(global::Percent.CrossPromotion.Status.SHOW) && onClose != null)
			{
				onClose();
			}
			State = global::Percent.CrossPromotion.Status.HIDE;
		}

		public static bool isInterstitialShowable()
		{
			return false;
		}

		public static void reportShowInterstitial()
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("analytics", "cross-promotion-interstitial");
		}

		public static bool isNOTEURegion()
		{
			string region = global::Percent.Util.getRegion();
			string[] eU_CODES = global::Percent.View.PrivacyPopup.EU_CODES;
			for (int i = 0; i < eU_CODES.Length; i++)
			{
				if (eU_CODES[i].Equals(region))
				{
					return false;
				}
			}
			return true;
		}

		public static void showPrivacyWindow(bool isFirstScreen = false)
		{
			if (instance == null)
			{
				global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "cross-promotion-privacy");
				return;
			}
			if (!instance.privacyPopup.gameObject.activeSelf)
			{
				instance.privacyPopup.gameObject.SetActive(value: true);
			}
			if (isFirstScreen && global::UnityEngine.PlayerPrefs.HasKey(PREF_AGREEMENT))
			{
				instance.privacyPopup.hideSelf();
			}
			else
			{
				instance.privacyPopup.showWindow();
			}
		}

		public static global::Percent.Company getCompany()
		{
			if (instance != null)
			{
				return instance.company;
			}
			return global::Percent.Company.Percent;
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			State = global::Percent.CrossPromotion.Status.LOAD_FAIL;
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("ads", "cross-promotion-initialize");
			if (onLoadData != null)
			{
				onLoadData(isSuccess: false);
			}
		}

		private void onInitialize()
		{
			State = global::Percent.CrossPromotion.Status.INIT;
			global::UnityEngine.Object.DontDestroyOnLoad(this);
			sceneChecker = base.gameObject.AddComponent<global::Percent.SceneChecker>();
			textureLifeCycle = global::UnityEngine.GameObject.Find("TextureLifeCycle").GetComponent<global::Percent.TextureLifeCycle>();
			interstitial = global::UnityEngine.GameObject.Find("Interstitial").GetComponent<global::Percent.Interstitial>();
		}

		internal void onPromotionDataLoad(bool isSuccess)
		{
			if (isSuccess)
			{
				textureLifeCycle.load();
				return;
			}
			State = global::Percent.CrossPromotion.Status.LOAD_FAIL;
			if (onLoadData != null)
			{
				onLoadData(isSuccess: false);
			}
			global::Percent.Logger.error("Could NOT Load Promotion Data.");
			hide();
		}

		internal void onTextureLoad(bool isSuccess)
		{
			if (isSuccess)
			{
				State = global::Percent.CrossPromotion.Status.LOAD_SUCCESS;
				if (onLoadData != null)
				{
					onLoadData(isSuccess: true);
				}
				if (sceneChecker.isShowableScene())
				{
					show();
				}
			}
			else
			{
				State = global::Percent.CrossPromotion.Status.LOAD_FAIL;
				if (onLoadData != null)
				{
					onLoadData(isSuccess: false);
				}
				hide();
			}
		}

		private void OnDestroy()
		{
			State = global::Percent.CrossPromotion.Status.DESTROY;
			if (onDestroy != null)
			{
				onDestroy();
			}
		}
	}
}
