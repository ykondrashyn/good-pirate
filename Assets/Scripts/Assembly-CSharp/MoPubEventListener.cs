public class MoPubEventListener : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private MoPubDemoGUI _demoGUI;

	private void Awake()
	{
		if (_demoGUI == null)
		{
			_demoGUI = GetComponent<MoPubDemoGUI>();
		}
		if (!(_demoGUI != null))
		{
			global::UnityEngine.Debug.LogError("Missing reference to MoPubDemoGUI.  Please fix in the editor.");
			global::UnityEngine.Object.Destroy(this);
		}
	}

	private void OnEnable()
	{
		MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;
		MoPubManager.OnConsentStatusChangedEvent += OnConsentStatusChangedEvent;
		MoPubManager.OnConsentDialogLoadedEvent += OnConsentDialogLoadedEvent;
		MoPubManager.OnConsentDialogFailedEvent += OnConsentDialogFailedEvent;
		MoPubManager.OnConsentDialogShownEvent += OnConsentDialogShownEvent;
		MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
		MoPubManager.OnAdFailedEvent += OnAdFailedEvent;
		MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
		MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
		MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
		MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
		MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
		MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
		MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
	}

	private void OnDisable()
	{
		MoPubManager.OnSdkInitializedEvent -= OnSdkInitializedEvent;
		MoPubManager.OnConsentStatusChangedEvent -= OnConsentStatusChangedEvent;
		MoPubManager.OnConsentDialogLoadedEvent -= OnConsentDialogLoadedEvent;
		MoPubManager.OnConsentDialogFailedEvent -= OnConsentDialogFailedEvent;
		MoPubManager.OnConsentDialogShownEvent -= OnConsentDialogShownEvent;
		MoPubManager.OnAdLoadedEvent -= OnAdLoadedEvent;
		MoPubManager.OnAdFailedEvent -= OnAdFailedEvent;
		MoPubManager.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
		MoPubManager.OnInterstitialFailedEvent -= OnInterstitialFailedEvent;
		MoPubManager.OnInterstitialDismissedEvent -= OnInterstitialDismissedEvent;
		MoPubManager.OnRewardedVideoLoadedEvent -= OnRewardedVideoLoadedEvent;
		MoPubManager.OnRewardedVideoFailedEvent -= OnRewardedVideoFailedEvent;
		MoPubManager.OnRewardedVideoFailedToPlayEvent -= OnRewardedVideoFailedToPlayEvent;
		MoPubManager.OnRewardedVideoClosedEvent -= OnRewardedVideoClosedEvent;
	}

	private void AdFailed(string adUnitId, string action, string error)
	{
		string text = "Failed to " + action + " ad unit " + adUnitId;
		if (!string.IsNullOrEmpty(error))
		{
			text = text + ": " + error;
		}
		_demoGUI.UpdateStatusLabel("Error: " + text);
	}

	private void OnSdkInitializedEvent(string adUnitId)
	{
		_demoGUI.SdkInitialized();
	}

	private void OnConsentStatusChangedEvent(MoPubBase.Consent.Status oldStatus, MoPubBase.Consent.Status newStatus, bool canCollectPersonalInfo)
	{
		_demoGUI.ConsentStatusChanged(newStatus, canCollectPersonalInfo);
	}

	private void OnConsentDialogLoadedEvent()
	{
		_demoGUI.ConsentDialogLoaded = true;
	}

	private void OnConsentDialogFailedEvent(string err)
	{
		_demoGUI.UpdateStatusLabel(err);
	}

	private void OnConsentDialogShownEvent()
	{
		_demoGUI.ConsentDialogLoaded = false;
	}

	private void OnAdLoadedEvent(string adUnitId, float height)
	{
		_demoGUI.BannerLoaded(adUnitId, height);
	}

	private void OnAdFailedEvent(string adUnitId, string error)
	{
		AdFailed(adUnitId, "load banner", error);
	}

	private void OnInterstitialLoadedEvent(string adUnitId)
	{
		_demoGUI.AdLoaded(adUnitId);
	}

	private void OnInterstitialFailedEvent(string adUnitId, string error)
	{
		AdFailed(adUnitId, "load interstitial", error);
	}

	private void OnInterstitialDismissedEvent(string adUnitId)
	{
		_demoGUI.AdDismissed(adUnitId);
	}

	private void OnRewardedVideoLoadedEvent(string adUnitId)
	{
		global::System.Collections.Generic.List<MoPubBase.Reward> availableRewards = MoPubAndroid.GetAvailableRewards(adUnitId);
		_demoGUI.AdLoaded(adUnitId);
		_demoGUI.LoadAvailableRewards(adUnitId, availableRewards);
	}

	private void OnRewardedVideoFailedEvent(string adUnitId, string error)
	{
		AdFailed(adUnitId, "load rewarded video", error);
	}

	private void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
	{
		AdFailed(adUnitId, "play rewarded video", error);
	}

	private void OnRewardedVideoClosedEvent(string adUnitId)
	{
		_demoGUI.AdDismissed(adUnitId);
	}
}
