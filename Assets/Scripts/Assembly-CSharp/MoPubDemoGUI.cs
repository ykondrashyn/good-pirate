public class MoPubDemoGUI : global::UnityEngine.MonoBehaviour
{
	private readonly global::System.Collections.Generic.Dictionary<string, bool> _adUnitToLoadedMapping = new global::System.Collections.Generic.Dictionary<string, bool>();

	private readonly global::System.Collections.Generic.Dictionary<string, bool> _adUnitToShownMapping = new global::System.Collections.Generic.Dictionary<string, bool>();

	private readonly global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<MoPubBase.Reward>> _adUnitToRewardsMapping = new global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<MoPubBase.Reward>>();

	private bool _consentDialogLoaded;

	private readonly string[] _bannerAdUnits = new string[1] { "b195f8dd8ded45fe847ad89ed1d016da" };

	private readonly string[] _interstitialAdUnits = new string[1] { "24534e1901884e398f1253216226017e" };

	private readonly string[] _rewardedVideoAdUnits = new string[1] { "920b6145fb1546cf8b5cf2ac34638bb7" };

	private readonly string[] _rewardedRichMediaAdUnits = new string[1] { "a96ae2ef41d44822af45c6328c4e1eb1" };

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GUISkin _skin;

	private global::UnityEngine.GUIStyle _smallerFont;

	private int _sectionMarginSize;

	private global::UnityEngine.GUIStyle _centeredStyle;

	private static string _customDataDefaultText = "Optional custom data";

	private string _rvCustomData = _customDataDefaultText;

	private string _rrmCustomData = _customDataDefaultText;

	private bool _canCollectPersonalInfo;

	private MoPubBase.Consent.Status _currentConsentStatus;

	private bool _shouldShowConsentDialog;

	private bool? _isGdprApplicable = false;

	private bool _isGdprForced;

	private string _status = string.Empty;

	public bool ConsentDialogLoaded
	{
		private get
		{
			return _consentDialogLoaded;
		}
		set
		{
			_consentDialogLoaded = value;
			if (_consentDialogLoaded)
			{
				UpdateStatusLabel("Consent dialog loaded");
			}
		}
	}

	private static bool IsAdUnitArrayNullOrEmpty(global::System.Collections.Generic.ICollection<string> adUnitArray)
	{
		if (adUnitArray != null)
		{
			return adUnitArray.Count == 0;
		}
		return true;
	}

	private void AddAdUnitsToStateMaps(global::System.Collections.Generic.IEnumerable<string> adUnits)
	{
		foreach (string adUnit in adUnits)
		{
			_adUnitToLoadedMapping[adUnit] = false;
			_adUnitToShownMapping[adUnit] = false;
		}
	}

	public void SdkInitialized()
	{
		UpdateConsentValues();
	}

	public void UpdateStatusLabel(string message)
	{
		_status = message;
	}

	public void ClearStatusLabel()
	{
		UpdateStatusLabel(string.Empty);
	}

	public void ConsentStatusChanged(MoPubBase.Consent.Status newStatus, bool canCollectPersonalInfo)
	{
		_canCollectPersonalInfo = canCollectPersonalInfo;
		_currentConsentStatus = newStatus;
		_shouldShowConsentDialog = MoPubAndroid.ShouldShowConsentDialog;
		UpdateStatusLabel("Consent status changed");
	}

	public void LoadAvailableRewards(string adUnitId, global::System.Collections.Generic.List<MoPubBase.Reward> availableRewards)
	{
		_adUnitToRewardsMapping.Remove(adUnitId);
		if (availableRewards != null)
		{
			_adUnitToRewardsMapping[adUnitId] = availableRewards;
		}
	}

	public void BannerLoaded(string adUnitId, float height)
	{
		AdLoaded(adUnitId);
		_adUnitToShownMapping[adUnitId] = true;
	}

	public void AdLoaded(string adUnit)
	{
		_adUnitToLoadedMapping[adUnit] = true;
		UpdateStatusLabel("Loaded " + adUnit);
	}

	public void AdDismissed(string adUnit)
	{
		_adUnitToLoadedMapping[adUnit] = false;
		ClearStatusLabel();
	}

	private void Awake()
	{
		if (global::UnityEngine.Screen.width < 960 && global::UnityEngine.Screen.height < 960)
		{
			_skin.button.fixedHeight = 50f;
		}
		_smallerFont = new global::UnityEngine.GUIStyle(_skin.label)
		{
			fontSize = _skin.button.fontSize
		};
		_centeredStyle = new global::UnityEngine.GUIStyle(_skin.label)
		{
			alignment = global::UnityEngine.TextAnchor.UpperCenter
		};
		_sectionMarginSize = _skin.label.fontSize;
		AddAdUnitsToStateMaps(_bannerAdUnits);
		AddAdUnitsToStateMaps(_interstitialAdUnits);
		AddAdUnitsToStateMaps(_rewardedVideoAdUnits);
		AddAdUnitsToStateMaps(_rewardedRichMediaAdUnits);
		ConsentDialogLoaded = false;
	}

	private void Start()
	{
		string adUnitId = _bannerAdUnits[0];
		MoPubAndroid.InitializeSdk(new MoPubBase.SdkConfiguration
		{
			AdUnitId = adUnitId,
			LogLevel = MoPubBase.LogLevel.MPLogLevelDebug,
			MediatedNetworks = new MoPubBase.MediatedNetwork[0]
		});
		MoPubAndroid.LoadBannerPluginsForAdUnits(_bannerAdUnits);
		MoPubAndroid.LoadInterstitialPluginsForAdUnits(_interstitialAdUnits);
		MoPubAndroid.LoadRewardedVideoPluginsForAdUnits(_rewardedVideoAdUnits);
		MoPubAndroid.LoadRewardedVideoPluginsForAdUnits(_rewardedRichMediaAdUnits);
		global::UnityEngine.GameObject gameObject = global::UnityEngine.GameObject.Find("MoPubNativeAds");
		if (gameObject != null)
		{
			gameObject.SetActive(value: false);
		}
	}

	private void OnGUI()
	{
		global::UnityEngine.GUI.skin = _skin;
		global::UnityEngine.Rect safeArea = global::UnityEngine.Screen.safeArea;
		safeArea.x += 20f;
		safeArea.width -= 40f;
		global::UnityEngine.GUILayout.BeginArea(safeArea);
		CreateTitleSection();
		CreateBannersSection();
		CreateInterstitialsSection();
		CreateRewardedVideosSection();
		CreateRewardedRichMediaSection();
		CreateUserConsentSection();
		CreateActionsSection();
		CreateStatusSection();
		global::UnityEngine.GUILayout.EndArea();
	}

	private void CreateTitleSection()
	{
		int fontSize = _centeredStyle.fontSize;
		_centeredStyle.fontSize = 48;
		global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(0f, 10f, global::UnityEngine.Screen.width, 60f), MoPubBase.PluginName, _centeredStyle);
		_centeredStyle.fontSize = fontSize;
		global::UnityEngine.GUI.Label(new global::UnityEngine.Rect(0f, 70f, global::UnityEngine.Screen.width, 60f), "with " + MoPub.SdkName, _centeredStyle);
	}

	private void CreateBannersSection()
	{
		global::UnityEngine.GUILayout.Space(102f);
		global::UnityEngine.GUILayout.Label("Banners");
		if (!IsAdUnitArrayNullOrEmpty(_bannerAdUnits))
		{
			string[] bannerAdUnits = _bannerAdUnits;
			foreach (string text in bannerAdUnits)
			{
				global::UnityEngine.GUILayout.BeginHorizontal();
				global::UnityEngine.GUI.enabled = !_adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button(CreateRequestButtonLabel(text)))
				{
					global::UnityEngine.Debug.Log("requesting banner with AdUnit: " + text);
					UpdateStatusLabel("Requesting " + text);
					MoPubAndroid.CreateBanner(text, MoPubBase.AdPosition.BottomCenter);
				}
				global::UnityEngine.GUI.enabled = _adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button("Destroy"))
				{
					ClearStatusLabel();
					MoPubAndroid.DestroyBanner(text);
					_adUnitToLoadedMapping[text] = false;
					_adUnitToShownMapping[text] = false;
				}
				global::UnityEngine.GUI.enabled = _adUnitToLoadedMapping[text] && !_adUnitToShownMapping[text];
				if (global::UnityEngine.GUILayout.Button("Show"))
				{
					ClearStatusLabel();
					MoPubAndroid.ShowBanner(text, shouldShow: true);
					_adUnitToShownMapping[text] = true;
				}
				global::UnityEngine.GUI.enabled = _adUnitToLoadedMapping[text] && _adUnitToShownMapping[text];
				if (global::UnityEngine.GUILayout.Button("Hide"))
				{
					ClearStatusLabel();
					MoPubAndroid.ShowBanner(text, shouldShow: false);
					_adUnitToShownMapping[text] = false;
				}
				global::UnityEngine.GUI.enabled = true;
				global::UnityEngine.GUILayout.EndHorizontal();
			}
		}
		else
		{
			global::UnityEngine.GUILayout.Label("No banner AdUnits available", _smallerFont, (global::UnityEngine.GUILayoutOption[])null);
		}
	}

	private void CreateInterstitialsSection()
	{
		global::UnityEngine.GUILayout.Space(_sectionMarginSize);
		global::UnityEngine.GUILayout.Label("Interstitials");
		if (!IsAdUnitArrayNullOrEmpty(_interstitialAdUnits))
		{
			string[] interstitialAdUnits = _interstitialAdUnits;
			foreach (string text in interstitialAdUnits)
			{
				global::UnityEngine.GUILayout.BeginHorizontal();
				global::UnityEngine.GUI.enabled = !_adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button(CreateRequestButtonLabel(text)))
				{
					global::UnityEngine.Debug.Log("requesting interstitial with AdUnit: " + text);
					UpdateStatusLabel("Requesting " + text);
					MoPubAndroid.RequestInterstitialAd(text);
				}
				global::UnityEngine.GUI.enabled = _adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button("Show"))
				{
					ClearStatusLabel();
					MoPubAndroid.ShowInterstitialAd(text);
				}
				global::UnityEngine.GUI.enabled = true;
				global::UnityEngine.GUILayout.EndHorizontal();
			}
		}
		else
		{
			global::UnityEngine.GUILayout.Label("No interstitial AdUnits available", _smallerFont, (global::UnityEngine.GUILayoutOption[])null);
		}
	}

	private void CreateRewardedVideosSection()
	{
		global::UnityEngine.GUILayout.Space(_sectionMarginSize);
		global::UnityEngine.GUILayout.Label("Rewarded Videos");
		if (!IsAdUnitArrayNullOrEmpty(_rewardedVideoAdUnits))
		{
			CreateCustomDataField("rvCustomDataField", ref _rvCustomData);
			string[] rewardedVideoAdUnits = _rewardedVideoAdUnits;
			foreach (string text in rewardedVideoAdUnits)
			{
				global::UnityEngine.GUILayout.BeginHorizontal();
				global::UnityEngine.GUI.enabled = !_adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button(CreateRequestButtonLabel(text)))
				{
					global::UnityEngine.Debug.Log("requesting rewarded video with AdUnit: " + text);
					UpdateStatusLabel("Requesting " + text);
					MoPubAndroid.RequestRewardedVideo(text, null, "rewarded, video, mopub", null, 37.7833, 122.4167, "customer101");
				}
				global::UnityEngine.GUI.enabled = _adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button("Show"))
				{
					ClearStatusLabel();
					MoPubAndroid.ShowRewardedVideo(text, GetCustomData(_rvCustomData));
				}
				global::UnityEngine.GUI.enabled = true;
				global::UnityEngine.GUILayout.EndHorizontal();
				if (!MoPubAndroid.HasRewardedVideo(text) || !_adUnitToRewardsMapping.ContainsKey(text) || _adUnitToRewardsMapping[text].Count <= 1)
				{
					continue;
				}
				global::UnityEngine.GUILayout.BeginVertical();
				global::UnityEngine.GUILayout.Space(_sectionMarginSize);
				global::UnityEngine.GUILayout.Label("Select a reward:");
				foreach (MoPubBase.Reward item in _adUnitToRewardsMapping[text])
				{
					if (global::UnityEngine.GUILayout.Button(item.ToString()))
					{
						MoPubAndroid.SelectReward(text, item);
					}
				}
				global::UnityEngine.GUILayout.Space(_sectionMarginSize);
				global::UnityEngine.GUILayout.EndVertical();
			}
		}
		else
		{
			global::UnityEngine.GUILayout.Label("No rewarded video AdUnits available", _smallerFont, (global::UnityEngine.GUILayoutOption[])null);
		}
	}

	private void CreateRewardedRichMediaSection()
	{
		global::UnityEngine.GUILayout.Space(_sectionMarginSize);
		global::UnityEngine.GUILayout.Label("Rewarded Rich Media");
		if (!IsAdUnitArrayNullOrEmpty(_rewardedRichMediaAdUnits))
		{
			CreateCustomDataField("rrmCustomDataField", ref _rrmCustomData);
			string[] rewardedRichMediaAdUnits = _rewardedRichMediaAdUnits;
			foreach (string text in rewardedRichMediaAdUnits)
			{
				global::UnityEngine.GUILayout.BeginHorizontal();
				global::UnityEngine.GUI.enabled = !_adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button(CreateRequestButtonLabel(text)))
				{
					global::UnityEngine.Debug.Log("requesting rewarded rich media with AdUnit: " + text);
					UpdateStatusLabel("Requesting " + text);
					MoPubAndroid.RequestRewardedVideo(text, null, "rewarded, video, mopub", null, 37.7833, 122.4167, "customer101");
				}
				global::UnityEngine.GUI.enabled = _adUnitToLoadedMapping[text];
				if (global::UnityEngine.GUILayout.Button("Show"))
				{
					ClearStatusLabel();
					MoPubAndroid.ShowRewardedVideo(text, GetCustomData(_rrmCustomData));
				}
				global::UnityEngine.GUI.enabled = true;
				global::UnityEngine.GUILayout.EndHorizontal();
				if (!MoPubAndroid.HasRewardedVideo(text) || !_adUnitToRewardsMapping.ContainsKey(text) || _adUnitToRewardsMapping[text].Count <= 1)
				{
					continue;
				}
				global::UnityEngine.GUILayout.BeginVertical();
				global::UnityEngine.GUILayout.Space(_sectionMarginSize);
				global::UnityEngine.GUILayout.Label("Select a reward:");
				foreach (MoPubBase.Reward item in _adUnitToRewardsMapping[text])
				{
					if (global::UnityEngine.GUILayout.Button(item.ToString()))
					{
						MoPubAndroid.SelectReward(text, item);
					}
				}
				global::UnityEngine.GUILayout.Space(_sectionMarginSize);
				global::UnityEngine.GUILayout.EndVertical();
			}
		}
		else
		{
			global::UnityEngine.GUILayout.Label("No rewarded rich media AdUnits available", _smallerFont, (global::UnityEngine.GUILayoutOption[])null);
		}
	}

	private void CreateUserConsentSection()
	{
		global::UnityEngine.GUILayout.Space(_sectionMarginSize);
		global::UnityEngine.GUILayout.Label("User Consent");
		global::UnityEngine.GUILayout.Label("Can collect personally identifiable information: " + _canCollectPersonalInfo, _smallerFont);
		global::UnityEngine.GUILayout.Label("Current consent status: " + _currentConsentStatus, _smallerFont);
		global::UnityEngine.GUILayout.Label("Should show consent dialog: " + _shouldShowConsentDialog, _smallerFont);
		global::UnityEngine.GUILayout.Label("Is GDPR applicable: " + (_isGdprApplicable.HasValue ? _isGdprApplicable.ToString() : "Unknown"), _smallerFont);
		global::UnityEngine.GUILayout.BeginHorizontal();
		global::UnityEngine.GUI.enabled = !ConsentDialogLoaded;
		if (global::UnityEngine.GUILayout.Button("Load Consent Dialog"))
		{
			UpdateStatusLabel("Loading consent dialog");
			MoPubAndroid.LoadConsentDialog();
		}
		global::UnityEngine.GUI.enabled = ConsentDialogLoaded;
		if (global::UnityEngine.GUILayout.Button("Show Consent Dialog"))
		{
			ClearStatusLabel();
			MoPubAndroid.ShowConsentDialog();
		}
		global::UnityEngine.GUI.enabled = !_isGdprForced;
		if (global::UnityEngine.GUILayout.Button("Force GDPR"))
		{
			ClearStatusLabel();
			MoPubAndroid.ForceGdprApplicable();
			UpdateConsentValues();
			_isGdprForced = true;
		}
		global::UnityEngine.GUI.enabled = true;
		if (global::UnityEngine.GUILayout.Button("Grant Consent"))
		{
			MoPubAndroid.PartnerApi.GrantConsent();
		}
		if (global::UnityEngine.GUILayout.Button("Revoke Consent"))
		{
			MoPubAndroid.PartnerApi.RevokeConsent();
		}
		global::UnityEngine.GUI.enabled = true;
		global::UnityEngine.GUILayout.EndHorizontal();
	}

	private void CreateActionsSection()
	{
		global::UnityEngine.GUILayout.Space(_sectionMarginSize);
		global::UnityEngine.GUILayout.Label("Actions");
		if (global::UnityEngine.GUILayout.Button("Report App Open"))
		{
			ClearStatusLabel();
			MoPubAndroid.ReportApplicationOpen();
		}
		if (global::UnityEngine.GUILayout.Button("Enable Location Support"))
		{
			ClearStatusLabel();
			MoPubAndroid.EnableLocationSupport(shouldUseLocation: true);
		}
	}

	private void UpdateConsentValues()
	{
		_canCollectPersonalInfo = MoPubAndroid.CanCollectPersonalInfo;
		_currentConsentStatus = MoPubAndroid.CurrentConsentStatus;
		_shouldShowConsentDialog = MoPubAndroid.ShouldShowConsentDialog;
		_isGdprApplicable = MoPubAndroid.IsGdprApplicable;
	}

	private static void CreateCustomDataField(string fieldName, ref string customDataValue)
	{
		global::UnityEngine.GUI.SetNextControlName(fieldName);
		customDataValue = global::UnityEngine.GUILayout.TextField(customDataValue, global::UnityEngine.GUILayout.MinWidth(200f));
		if (global::UnityEngine.Event.current.type == global::UnityEngine.EventType.Repaint)
		{
			if (global::UnityEngine.GUI.GetNameOfFocusedControl() == fieldName && customDataValue == _customDataDefaultText)
			{
				customDataValue = string.Empty;
			}
			else if (global::UnityEngine.GUI.GetNameOfFocusedControl() != fieldName && string.IsNullOrEmpty(customDataValue))
			{
				customDataValue = _customDataDefaultText;
			}
		}
	}

	private void CreateStatusSection()
	{
		global::UnityEngine.GUILayout.Space(40f);
		global::UnityEngine.GUILayout.Label(_status, _smallerFont);
	}

	private static string GetCustomData(string customDataFieldValue)
	{
		if (!(customDataFieldValue != _customDataDefaultText))
		{
			return null;
		}
		return customDataFieldValue;
	}

	private static string CreateRequestButtonLabel(string adUnit)
	{
		if (adUnit.Length <= 10)
		{
			return adUnit;
		}
		return "Request " + adUnit.Substring(0, 10) + "...";
	}
}
