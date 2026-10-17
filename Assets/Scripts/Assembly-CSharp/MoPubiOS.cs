public class MoPubiOS : MoPubBase
{
	public static class PartnerApi
	{
		public static global::System.Uri CurrentConsentPrivacyPolicyUrl => MoPubBase.UrlFromString(_moPubCurrentConsentPrivacyPolicyUrl(MoPubBase.ConsentLanguageCode));

		public static global::System.Uri CurrentVendorListUrl => MoPubBase.UrlFromString(_moPubCurrentConsentVendorListUrl(MoPubBase.ConsentLanguageCode));

		public static string CurrentConsentIabVendorListFormat => _moPubCurrentConsentIabVendorListFormat();

		public static string CurrentConsentPrivacyPolicyVersion => _moPubCurrentConsentPrivacyPolicyVersion();

		public static string CurrentConsentVendorListVersion => _moPubCurrentConsentVendorListVersion();

		public static string PreviouslyConsentedIabVendorListFormat => _moPubPreviouslyConsentedIabVendorListFormat();

		public static string PreviouslyConsentedPrivacyPolicyVersion => _moPubPreviouslyConsentedPrivacyPolicyVersion();

		public static string PreviouslyConsentedVendorListVersion => _moPubPreviouslyConsentedVendorListVersion();

		public static void GrantConsent()
		{
			_moPubGrantConsent();
		}

		public static void RevokeConsent()
		{
			_moPubRevokeConsent();
		}
	}

	private static readonly global::System.Collections.Generic.Dictionary<string, MoPubBinding> PluginsDict;

	public static bool IsSdkInitialized => _moPubIsSdkInitialized();

	public static bool AllowLegitimateInterest
	{
		get
		{
			return _moPubAllowLegitimateInterest();
		}
		set
		{
			_moPubSetAllowLegitimateInterest(value);
		}
	}

	public static MoPubBase.LogLevel SdkLogLevel
	{
		get
		{
			return (MoPubBase.LogLevel)_moPubGetLogLevel();
		}
		set
		{
			MoPubBase.logLevel = value;
			_moPubSetLogLevel((int)value);
		}
	}

	public static bool CanCollectPersonalInfo => _moPubCanCollectPersonalInfo();

	public static MoPubBase.Consent.Status CurrentConsentStatus => (MoPubBase.Consent.Status)_moPubCurrentConsentStatus();

	public static bool ShouldShowConsentDialog => _moPubShouldShowConsentDialog();

	public static bool IsConsentDialogReady => _moPubIsConsentDialogReady();

	[global::System.Obsolete("Use the property name IsConsentDialogReady instead.")]
	public static bool IsConsentDialogLoaded => IsConsentDialogReady;

	public static bool? IsGdprApplicable
	{
		get
		{
			int num = _moPubIsGDPRApplicable();
			if (num != 0)
			{
				if (num <= 0)
				{
					return false;
				}
				return true;
			}
			return null;
		}
	}

	static MoPubiOS()
	{
		PluginsDict = new global::System.Collections.Generic.Dictionary<string, MoPubBinding>();
		MoPubBase.InitManager();
	}

	public static void InitializeSdk(string anyAdUnitId)
	{
		MoPubLog.Log("InitializeSdk", "SDK initialization started");
		MoPubBase.ValidateAdUnitForSdkInit(anyAdUnitId);
		InitializeSdk(new MoPubBase.SdkConfiguration
		{
			AdUnitId = anyAdUnitId
		});
	}

	public static void InitializeSdk(MoPubBase.SdkConfiguration sdkConfiguration)
	{
		MoPubBase.logLevel = sdkConfiguration.LogLevel;
		MoPubBase.ValidateAdUnitForSdkInit(sdkConfiguration.AdUnitId);
		_moPubInitializeSdk(sdkConfiguration.AdUnitId, sdkConfiguration.AdditionalNetworksString, sdkConfiguration.MediationSettingsJson, sdkConfiguration.AllowLegitimateInterest, (int)sdkConfiguration.LogLevel, sdkConfiguration.NetworkConfigurationsJson, sdkConfiguration.MoPubRequestOptionsJson);
	}

	public static void LoadBannerPluginsForAdUnits(string[] adUnitIds)
	{
		LoadPluginsForAdUnits(adUnitIds);
	}

	public static void LoadInterstitialPluginsForAdUnits(string[] adUnitIds)
	{
		LoadPluginsForAdUnits(adUnitIds);
	}

	public static void LoadRewardedVideoPluginsForAdUnits(string[] adUnitIds)
	{
		LoadPluginsForAdUnits(adUnitIds);
	}

	public static void EnableLocationSupport(bool shouldUseLocation)
	{
		_moPubEnableLocationSupport(shouldUseLocation: true);
	}

	public static void ReportApplicationOpen(string iTunesAppId = null)
	{
		_moPubReportApplicationOpen(iTunesAppId);
	}

	protected static string GetSdkName()
	{
		return "iOS SDK v" + _moPubGetSDKVersion();
	}

	private static void LoadPluginsForAdUnits(string[] adUnitIds)
	{
		foreach (string text in adUnitIds)
		{
			PluginsDict[text] = new MoPubBinding(text);
		}
		global::UnityEngine.Debug.Log(adUnitIds.Length + " AdUnits loaded for plugins:\n" + string.Join(", ", adUnitIds));
	}

	public static void ForceWKWebView(bool shouldForce)
	{
		_moPubForceWKWebView(shouldForce);
	}

	public static void CreateBanner(string adUnitId, MoPubBase.AdPosition position, MoPubBase.BannerType bannerType = MoPubBase.BannerType.Size320x50)
	{
		MoPubLog.Log("CreateBanner", "Attempting to load ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.CreateBanner(bannerType, position);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void ShowBanner(string adUnitId, bool shouldShow)
	{
		MoPubLog.Log("ShowBanner", "Attempting to show ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.ShowBanner(shouldShow);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void RefreshBanner(string adUnitId, string keywords, string userDataKeywords = "")
	{
		MoPubLog.Log("RefreshBanner", "Attempting to show ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.RefreshBanner(keywords, userDataKeywords);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public void SetAutorefresh(string adUnitId, bool enabled)
	{
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.SetAutorefresh(enabled);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public void ForceRefresh(string adUnitId)
	{
		MoPubLog.Log("ForceRefresh", "Attempting to show ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.ForceRefresh();
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void DestroyBanner(string adUnitId)
	{
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.DestroyBanner();
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void RequestInterstitialAd(string adUnitId, string keywords = "", string userDataKeywords = "")
	{
		MoPubLog.Log("RequestInterstitialAd", "Attempting to load ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.RequestInterstitialAd(keywords, userDataKeywords);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void ShowInterstitialAd(string adUnitId)
	{
		MoPubLog.Log("ShowInterstitialAd", "Attempting to show ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.ShowInterstitialAd();
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public bool IsInterstialReady(string adUnitId)
	{
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			return value.IsInterstitialReady;
		}
		MoPubBase.ReportAdUnitNotFound(adUnitId);
		return false;
	}

	public void DestroyInterstitialAd(string adUnitId)
	{
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.DestroyInterstitialAd();
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void RequestRewardedVideo(string adUnitId, global::System.Collections.Generic.List<MoPubBase.LocalMediationSetting> mediationSettings = null, string keywords = null, string userDataKeywords = null, double latitude = 99999.0, double longitude = 99999.0, string customerId = null)
	{
		MoPubLog.Log("RequestRewardedVideo", "Attempting to load ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.RequestRewardedVideo(mediationSettings, keywords, userDataKeywords, latitude, longitude, customerId);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void ShowRewardedVideo(string adUnitId, string customData = null)
	{
		MoPubLog.Log("ShowRewardedVideo", "Attempting to show ad");
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.ShowRewardedVideo(customData);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static bool HasRewardedVideo(string adUnitId)
	{
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			return value.HasRewardedVideo();
		}
		MoPubBase.ReportAdUnitNotFound(adUnitId);
		return false;
	}

	public static global::System.Collections.Generic.List<MoPubBase.Reward> GetAvailableRewards(string adUnitId)
	{
		if (!PluginsDict.TryGetValue(adUnitId, out var value))
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
			return null;
		}
		global::System.Collections.Generic.List<MoPubBase.Reward> availableRewards = value.GetAvailableRewards();
		global::UnityEngine.Debug.Log($"GetAvailableRewards found {availableRewards.Count} rewards for ad unit {adUnitId}");
		return availableRewards;
	}

	public static void SelectReward(string adUnitId, MoPubBase.Reward selectedReward)
	{
		if (PluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.SelectedReward = selectedReward;
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void LoadConsentDialog()
	{
		MoPubLog.Log("LoadConsentDialog", "Attempting to load consent dialog");
		_moPubLoadConsentDialog();
	}

	public static void ShowConsentDialog()
	{
		MoPubLog.Log("ShowConsentDialog", "Consent dialog attempting to show");
		_moPubShowConsentDialog();
	}

	public static void ForceGdprApplicable()
	{
		_moPubForceGDPRApplicable();
	}

	private static void _moPubInitializeSdk(string adUnitId, string additionalNetworksJson, string mediationSettingsJson, bool allowLegitimateInterest, int logLevel, string adapterConfigJson, string moPubRequestOptionsJson)
	{
		NoOp("initialize-ios-sdk");
	}

	private static bool _moPubIsSdkInitialized()
	{
		return false;
	}

	private static string _moPubGetSDKVersion()
	{
		return "offline";
	}

	private static void _moPubEnableLocationSupport(bool shouldUseLocation)
	{
		NoOp("enable-ios-location");
	}

	private static void _moPubSetAllowLegitimateInterest(bool allowLegitimateInterest)
	{
		NoOp("set-ios-legitimate-interest");
	}

	private static bool _moPubAllowLegitimateInterest()
	{
		return false;
	}

	private static int _moPubGetLogLevel()
	{
		return (int)MoPubBase.LogLevel.MPLogLevelNone;
	}

	private static void _moPubSetLogLevel(int logLevel)
	{
		NoOp("set-ios-log-level");
	}

	private static void _moPubForceWKWebView(bool shouldForce)
	{
		NoOp("force-ios-web-view");
	}

	private static void _moPubReportApplicationOpen(string iTunesAppId)
	{
		NoOp("report-ios-application-open");
	}

	private static bool _moPubCanCollectPersonalInfo()
	{
		return false;
	}

	private static int _moPubCurrentConsentStatus()
	{
		return (int)MoPubBase.Consent.Status.Unknown;
	}

	private static int _moPubIsGDPRApplicable()
	{
		return 0;
	}

	private static int _moPubForceGDPRApplicable()
	{
		NoOp("force-ios-gdpr-applicable");
		return 0;
	}

	private static bool _moPubShouldShowConsentDialog()
	{
		return false;
	}

	private static bool _moPubIsConsentDialogReady()
	{
		return false;
	}

	private static void _moPubLoadConsentDialog()
	{
		NoOp("load-ios-consent-dialog");
	}

	private static void _moPubShowConsentDialog()
	{
		NoOp("show-ios-consent-dialog");
	}

	private static string _moPubCurrentConsentPrivacyPolicyUrl(string isoLanguageCode = null)
	{
		return string.Empty;
	}

	private static string _moPubCurrentConsentVendorListUrl(string isoLanguageCode = null)
	{
		return string.Empty;
	}

	private static void _moPubGrantConsent()
	{
		NoOp("grant-ios-consent");
	}

	private static void _moPubRevokeConsent()
	{
		NoOp("revoke-ios-consent");
	}

	private static string _moPubCurrentConsentIabVendorListFormat()
	{
		return string.Empty;
	}

	private static string _moPubCurrentConsentPrivacyPolicyVersion()
	{
		return string.Empty;
	}

	private static string _moPubCurrentConsentVendorListVersion()
	{
		return string.Empty;
	}

	private static string _moPubPreviouslyConsentedIabVendorListFormat()
	{
		return string.Empty;
	}

	private static string _moPubPreviouslyConsentedPrivacyPolicyVersion()
	{
		return string.Empty;
	}

	private static string _moPubPreviouslyConsentedVendorListVersion()
	{
		return string.Empty;
	}

	private static void NoOp(string operation)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "mopub-ios-" + operation);
	}
}
