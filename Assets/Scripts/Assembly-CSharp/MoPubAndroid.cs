public class MoPubAndroid : MoPubBase
{
	public enum LocationAwareness
	{
		TRUNCATED = 0,
		DISABLED = 1,
		NORMAL = 2
	}

	public static class PartnerApi
	{
		public static global::System.Uri CurrentConsentPrivacyPolicyUrl => MoPubBase.UrlFromString(PluginClass.CallStatic<string>("getCurrentPrivacyPolicyLink", new object[1] { MoPubBase.ConsentLanguageCode }));

		public static global::System.Uri CurrentVendorListUrl => MoPubBase.UrlFromString(PluginClass.CallStatic<string>("getCurrentVendorListLink", new object[1] { MoPubBase.ConsentLanguageCode }));

		public static string CurrentConsentIabVendorListFormat => PluginClass.CallStatic<string>("getCurrentVendorListIabFormat", global::System.Array.Empty<object>());

		public static string CurrentConsentPrivacyPolicyVersion => PluginClass.CallStatic<string>("getCurrentPrivacyPolicyVersion", global::System.Array.Empty<object>());

		public static string CurrentConsentVendorListVersion => PluginClass.CallStatic<string>("getCurrentVendorListVersion", global::System.Array.Empty<object>());

		public static string PreviouslyConsentedIabVendorListFormat => PluginClass.CallStatic<string>("getConsentedVendorListIabFormat", global::System.Array.Empty<object>());

		public static string PreviouslyConsentedPrivacyPolicyVersion => PluginClass.CallStatic<string>("getConsentedPrivacyPolicyVersion", global::System.Array.Empty<object>());

		public static string PreviouslyConsentedVendorListVersion => PluginClass.CallStatic<string>("getConsentedVendorListVersion", global::System.Array.Empty<object>());

		public static void GrantConsent()
		{
			PluginClass.CallStatic("grantConsent");
		}

		public static void RevokeConsent()
		{
			PluginClass.CallStatic("revokeConsent");
		}
	}

	private static readonly global::UnityEngine.AndroidJavaClass PluginClass;

	private static readonly global::System.Collections.Generic.Dictionary<string, MoPubAndroidBanner> BannerPluginsDict;

	private static readonly global::System.Collections.Generic.Dictionary<string, MoPubAndroidInterstitial> InterstitialPluginsDict;

	private static readonly global::System.Collections.Generic.Dictionary<string, MoPubAndroidRewardedVideo> RewardedVideoPluginsDict;

	public static bool IsSdkInitialized => PluginClass.CallStatic<bool>("isSdkInitialized", global::System.Array.Empty<object>());

	public static bool AllowLegitimateInterest
	{
		get
		{
			return PluginClass.CallStatic<bool>("shouldAllowLegitimateInterest", global::System.Array.Empty<object>());
		}
		set
		{
			PluginClass.CallStatic("setAllowLegitimateInterest", value);
		}
	}

	public static MoPubBase.LogLevel SdkLogLevel
	{
		get
		{
			return (MoPubBase.LogLevel)PluginClass.CallStatic<int>("getLogLevel", global::System.Array.Empty<object>());
		}
		set
		{
			PluginClass.CallStatic<int>("setLogLevel", new object[1] { (int)value });
			MoPubBase.logLevel = value;
		}
	}

	public static bool CanCollectPersonalInfo => PluginClass.CallStatic<bool>("canCollectPersonalInfo", global::System.Array.Empty<object>());

	public static MoPubBase.Consent.Status CurrentConsentStatus => MoPubBase.Consent.FromString(PluginClass.CallStatic<string>("getPersonalInfoConsentState", global::System.Array.Empty<object>()));

	public static bool ShouldShowConsentDialog => PluginClass.CallStatic<bool>("shouldShowConsentDialog", global::System.Array.Empty<object>());

	public static bool IsConsentDialogReady => PluginClass.CallStatic<bool>("isConsentDialogReady", global::System.Array.Empty<object>());

	[global::System.Obsolete("Use the property name IsConsentDialogReady instead.")]
	public static bool IsConsentDialogLoaded => IsConsentDialogReady;

	public static bool? IsGdprApplicable
	{
		get
		{
			int num = PluginClass.CallStatic<int>("gdprApplies", global::System.Array.Empty<object>());
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

	static MoPubAndroid()
	{
		PluginClass = new global::UnityEngine.AndroidJavaClass("com.mopub.unity.MoPubUnityPlugin");
		BannerPluginsDict = new global::System.Collections.Generic.Dictionary<string, MoPubAndroidBanner>();
		InterstitialPluginsDict = new global::System.Collections.Generic.Dictionary<string, MoPubAndroidInterstitial>();
		RewardedVideoPluginsDict = new global::System.Collections.Generic.Dictionary<string, MoPubAndroidRewardedVideo>();
		MoPubBase.InitManager();
	}

	public static void InitializeSdk(string anyAdUnitId)
	{
		MoPubBase.ValidateAdUnitForSdkInit(anyAdUnitId);
		InitializeSdk(new MoPubBase.SdkConfiguration
		{
			AdUnitId = anyAdUnitId
		});
	}

	public static void InitializeSdk(MoPubBase.SdkConfiguration sdkConfiguration)
	{
		MoPubBase.logLevel = sdkConfiguration.LogLevel;
		MoPubLog.Log("InitializeSdk", "SDK initialization started");
		MoPubBase.ValidateAdUnitForSdkInit(sdkConfiguration.AdUnitId);
		PluginClass.CallStatic("initializeSdk", sdkConfiguration.AdUnitId, sdkConfiguration.AdditionalNetworksString, sdkConfiguration.MediationSettingsJson, sdkConfiguration.AllowLegitimateInterest, (int)sdkConfiguration.LogLevel, sdkConfiguration.NetworkConfigurationsJson, sdkConfiguration.MoPubRequestOptionsJson);
	}

	public static void LoadBannerPluginsForAdUnits(string[] bannerAdUnitIds)
	{
		foreach (string text in bannerAdUnitIds)
		{
			BannerPluginsDict[text] = new MoPubAndroidBanner(text);
		}
		global::UnityEngine.Debug.Log(bannerAdUnitIds.Length + " banner AdUnits loaded for plugins:\n" + string.Join(", ", bannerAdUnitIds));
	}

	public static void LoadInterstitialPluginsForAdUnits(string[] interstitialAdUnitIds)
	{
		foreach (string text in interstitialAdUnitIds)
		{
			InterstitialPluginsDict[text] = new MoPubAndroidInterstitial(text);
		}
		global::UnityEngine.Debug.Log(interstitialAdUnitIds.Length + " interstitial AdUnits loaded for plugins:\n" + string.Join(", ", interstitialAdUnitIds));
	}

	public static void LoadRewardedVideoPluginsForAdUnits(string[] rewardedVideoAdUnitIds)
	{
		foreach (string text in rewardedVideoAdUnitIds)
		{
			RewardedVideoPluginsDict[text] = new MoPubAndroidRewardedVideo(text);
		}
		global::UnityEngine.Debug.Log(rewardedVideoAdUnitIds.Length + " rewarded video AdUnits loaded for plugins:\n" + string.Join(", ", rewardedVideoAdUnitIds));
	}

	public static void EnableLocationSupport(bool shouldUseLocation)
	{
		PluginClass.CallStatic("setLocationAwareness", MoPubAndroid.LocationAwareness.NORMAL.ToString());
	}

	public static void ReportApplicationOpen(string iTunesAppId = null)
	{
		PluginClass.CallStatic("reportApplicationOpen");
	}

	protected static string GetSdkName()
	{
		return "Android SDK v" + PluginClass.CallStatic<string>("getSDKVersion", global::System.Array.Empty<object>());
	}

	public static void AddFacebookTestDeviceId(string hashedDeviceId)
	{
		PluginClass.CallStatic("addFacebookTestDeviceId", hashedDeviceId);
	}

	public static void CreateBanner(string adUnitId, MoPubBase.AdPosition position)
	{
		MoPubLog.Log("CreateBanner", "Attempting to load ad");
		if (BannerPluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.CreateBanner(position);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void ShowBanner(string adUnitId, bool shouldShow)
	{
		MoPubLog.Log("ShowBanner", "Attempting to show ad");
		if (BannerPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (BannerPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (BannerPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (BannerPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (BannerPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (InterstitialPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (InterstitialPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (InterstitialPluginsDict.TryGetValue(adUnitId, out var value))
		{
			return value.IsInterstitialReady;
		}
		MoPubBase.ReportAdUnitNotFound(adUnitId);
		return false;
	}

	public void DestroyInterstitialAd(string adUnitId)
	{
		if (InterstitialPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (RewardedVideoPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (RewardedVideoPluginsDict.TryGetValue(adUnitId, out var value))
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
		if (RewardedVideoPluginsDict.TryGetValue(adUnitId, out var value))
		{
			return value.HasRewardedVideo();
		}
		MoPubBase.ReportAdUnitNotFound(adUnitId);
		return false;
	}

	public static global::System.Collections.Generic.List<MoPubBase.Reward> GetAvailableRewards(string adUnitId)
	{
		if (RewardedVideoPluginsDict.TryGetValue(adUnitId, out var value))
		{
			return value.GetAvailableRewards();
		}
		MoPubBase.ReportAdUnitNotFound(adUnitId);
		return null;
	}

	public static void SelectReward(string adUnitId, MoPubBase.Reward selectedReward)
	{
		if (RewardedVideoPluginsDict.TryGetValue(adUnitId, out var value))
		{
			value.SelectReward(selectedReward);
		}
		else
		{
			MoPubBase.ReportAdUnitNotFound(adUnitId);
		}
	}

	public static void LoadConsentDialog()
	{
		MoPubLog.Log("LoadConsentDialog", "Attempting to load consent dialog");
		PluginClass.CallStatic("loadConsentDialog");
	}

	public static void ShowConsentDialog()
	{
		MoPubLog.Log("ShowConsentDialog", "Consent dialog attempting to show");
		PluginClass.CallStatic("showConsentDialog");
	}

	public static void ForceGdprApplicable()
	{
		PluginClass.CallStatic("forceGdprApplies");
	}
}
