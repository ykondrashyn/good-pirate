public class MoPubAdManager : global::UnityEngine.MonoBehaviour
{
	private static MoPubAdManager sInstance;

	private bool shouldShowBanner;

	private global::UnityEngine.Events.UnityEvent onReward = new global::UnityEngine.Events.UnityEvent();

	private global::Percent.Event.PercentTracker tracker;

	[global::UnityEngine.SerializeField]
	private string[] _bannerAdUnitsAndroid;

	[global::UnityEngine.SerializeField]
	private string[] _interstitialAdUnitsAndroid;

	[global::UnityEngine.SerializeField]
	private string[] _rewardedVideoAdUnitsAndroid;

	[global::UnityEngine.SerializeField]
	private string[] _bannerAdUnitsIOS;

	[global::UnityEngine.SerializeField]
	private string[] _interstitialAdUnitsIOS;

	[global::UnityEngine.SerializeField]
	private string[] _rewardedVideoAdUnitsIOS;

	private string[] bannerAdUnits;

	private string[] rewardedVideoAdUnits;

	private string[] interstitialAdUnits;

	private bool hasCreatedBanner;

	public float rewardedRefreshRate = 7f;

	public float interstitialRefreshRate = 7f;

	private global::UnityEngine.Events.UnityAction rewardedCallback;

	public static MoPubAdManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new global::UnityEngine.GameObject("@MoPubAdManager").AddComponent<MoPubAdManager>();
			}
			return sInstance;
		}
	}

	private void Awake()
	{
		bannerAdUnits = _bannerAdUnitsAndroid;
		interstitialAdUnits = _interstitialAdUnitsAndroid;
		rewardedVideoAdUnits = _rewardedVideoAdUnitsAndroid;
		if (sInstance == null)
		{
			sInstance = this;
			if (tracker == null)
			{
				tracker = base.gameObject.AddComponent<global::Percent.Event.PercentTracker>();
			}
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (sInstance != this)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "initialize");
	}

	private global::System.Collections.IEnumerator retryAfterSecondsRewarded(string adUnitID, float seconds)
	{
		yield break;
	}

	private global::System.Collections.IEnumerator retryAfterSecondsInterstitial(string adUnitID, float seconds)
	{
		yield break;
	}

	public void ShowBanner()
	{
		shouldShowBanner = true;
		hasCreatedBanner = false;
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "show-banner");
	}

	public void HideBanner()
	{
		shouldShowBanner = false;
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "hide-banner");
	}

	public void ShowInterstitialAd()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "show-interstitial");
	}

	public void ShowRewardedVideo(global::UnityEngine.Events.UnityAction callback)
	{
		rewardedCallback = callback;
		onReward.RemoveAllListeners();
		if (rewardedCallback != null)
		{
			onReward.AddListener(rewardedCallback);
		}
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.LocalReward("ads", "show-rewarded-video");
		onReward.Invoke();
		onReward.RemoveAllListeners();
		rewardedCallback = null;
	}

	public bool IsNoAds()
	{
		return PlayerInfo.Instance != null && PlayerInfo.Instance.myData != null && PlayerInfo.Instance.myData.isRemoveADS;
	}
}
