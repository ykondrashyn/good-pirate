public class MoPubBase
{
	public enum AdPosition
	{
		TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		Centered = 3,
		BottomLeft = 4,
		BottomCenter = 5,
		BottomRight = 6
	}

	public static class Consent
	{
		public enum Status
		{
			Unknown = 0,
			Denied = 1,
			DoNotTrack = 2,
			PotentialWhitelist = 3,
			Consented = 4
		}

		private static class Strings
		{
			public const string ExplicitYes = "explicit_yes";

			public const string ExplicitNo = "explicit_no";

			public const string Unknown = "unknown";

			public const string PotentialWhitelist = "potential_whitelist";

			public const string Dnt = "dnt";
		}

		public static MoPubBase.Consent.Status FromString(string status)
		{
			switch (status)
			{
			case "explicit_yes":
				return MoPubBase.Consent.Status.Consented;
			case "explicit_no":
				return MoPubBase.Consent.Status.Denied;
			case "dnt":
				return MoPubBase.Consent.Status.DoNotTrack;
			case "potential_whitelist":
				return MoPubBase.Consent.Status.PotentialWhitelist;
			case "unknown":
				return MoPubBase.Consent.Status.Unknown;
			default:
				try
				{
					return (MoPubBase.Consent.Status)global::System.Enum.Parse(typeof(MoPubBase.Consent.Status), status);
				}
				catch
				{
					global::UnityEngine.Debug.LogError("Unknown consent status string: " + status);
					return MoPubBase.Consent.Status.Unknown;
				}
			}
		}
	}

	public enum BannerType
	{
		Size320x50 = 0,
		Size300x250 = 1,
		Size728x90 = 2,
		Size160x600 = 3
	}

	public enum LogLevel
	{
		MPLogLevelDebug = 20,
		MPLogLevelInfo = 30,
		MPLogLevelNone = 70
	}

	public struct SdkConfiguration
	{
		public string AdUnitId;

		public MoPubBase.MediatedNetwork[] MediatedNetworks;

		public bool AllowLegitimateInterest;

		private MoPubBase.LogLevel _logLevel;

		public MoPubBase.LogLevel LogLevel
		{
			get
			{
				if (_logLevel == (MoPubBase.LogLevel)0)
				{
					return MoPubBase.LogLevel.MPLogLevelNone;
				}
				return _logLevel;
			}
			set
			{
				_logLevel = value;
			}
		}

		public string AdditionalNetworksString
		{
			get
			{
				global::System.Collections.Generic.IEnumerable<MoPubBase.MediatedNetwork> mediatedNetworks = MediatedNetworks;
				global::System.Collections.Generic.IEnumerable<string> source = global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Where(mediatedNetworks ?? global::System.Linq.Enumerable.Empty<MoPubBase.MediatedNetwork>(), (MoPubBase.MediatedNetwork n) => n != null && !(n is MoPubBase.SupportedNetwork)), (MoPubBase.MediatedNetwork n) => !string.IsNullOrEmpty(n.AdapterConfigurationClassName)), (MoPubBase.MediatedNetwork n) => n.AdapterConfigurationClassName);
				return string.Join(",", global::System.Linq.Enumerable.ToArray(source));
			}
		}

		public string NetworkConfigurationsJson
		{
			get
			{
				global::System.Collections.Generic.IEnumerable<MoPubBase.MediatedNetwork> mediatedNetworks = MediatedNetworks;
				return global::MoPubInternal.ThirdParty.MiniJSON.Json.Serialize(global::System.Linq.Enumerable.ToDictionary(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Where(mediatedNetworks ?? global::System.Linq.Enumerable.Empty<MoPubBase.MediatedNetwork>(), (MoPubBase.MediatedNetwork n) => n.NetworkConfiguration != null), (MoPubBase.MediatedNetwork n) => !string.IsNullOrEmpty(n.AdapterConfigurationClassName)), (MoPubBase.MediatedNetwork n) => n.AdapterConfigurationClassName, (MoPubBase.MediatedNetwork n) => n.NetworkConfiguration));
			}
		}

		public string MediationSettingsJson
		{
			get
			{
				global::System.Collections.Generic.IEnumerable<MoPubBase.MediatedNetwork> mediatedNetworks = MediatedNetworks;
				return global::MoPubInternal.ThirdParty.MiniJSON.Json.Serialize(global::System.Linq.Enumerable.ToDictionary(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Where(mediatedNetworks ?? global::System.Linq.Enumerable.Empty<MoPubBase.MediatedNetwork>(), (MoPubBase.MediatedNetwork n) => n.MediationSettings != null), (MoPubBase.MediatedNetwork n) => !string.IsNullOrEmpty(n.MediationSettingsClassName)), (MoPubBase.MediatedNetwork n) => n.MediationSettingsClassName, (MoPubBase.MediatedNetwork n) => n.MediationSettings));
			}
		}

		public string MoPubRequestOptionsJson
		{
			get
			{
				global::System.Collections.Generic.IEnumerable<MoPubBase.MediatedNetwork> mediatedNetworks = MediatedNetworks;
				return global::MoPubInternal.ThirdParty.MiniJSON.Json.Serialize(global::System.Linq.Enumerable.ToDictionary(global::System.Linq.Enumerable.Where(global::System.Linq.Enumerable.Where(mediatedNetworks ?? global::System.Linq.Enumerable.Empty<MoPubBase.MediatedNetwork>(), (MoPubBase.MediatedNetwork n) => n.MoPubRequestOptions != null), (MoPubBase.MediatedNetwork n) => !string.IsNullOrEmpty(n.AdapterConfigurationClassName)), (MoPubBase.MediatedNetwork n) => n.AdapterConfigurationClassName, (MoPubBase.MediatedNetwork n) => n.MoPubRequestOptions));
			}
		}
	}

	public class LocalMediationSetting : global::System.Collections.Generic.Dictionary<string, object>
	{
		public class AdColony : MoPubBase.LocalMediationSetting
		{
			public AdColony()
				: base("AdColony")
			{
			}
		}

		public class AdMob : MoPubBase.LocalMediationSetting
		{
			public AdMob()
				: base("GooglePlayServices", "MPGoogle")
			{
			}
		}

		public class Chartboost : MoPubBase.LocalMediationSetting
		{
			public Chartboost()
				: base("Chartboost")
			{
			}
		}

		public class Vungle : MoPubBase.LocalMediationSetting
		{
			public Vungle()
				: base("Vungle")
			{
			}
		}

		public string MediationSettingsClassName { get; set; }

		public LocalMediationSetting()
		{
		}

		public LocalMediationSetting(string adVendor)
		{
			MediationSettingsClassName = adVendor;
		}

		public LocalMediationSetting(string android, string ios)
			: this(android)
		{
		}

		public static string ToJson(global::System.Collections.Generic.IEnumerable<MoPubBase.LocalMediationSetting> localMediationSettings)
		{
			return global::MoPubInternal.ThirdParty.MiniJSON.Json.Serialize(global::System.Linq.Enumerable.ToDictionary(global::System.Linq.Enumerable.Where(localMediationSettings ?? global::System.Linq.Enumerable.Empty<MoPubBase.LocalMediationSetting>(), (MoPubBase.LocalMediationSetting n) => n != null && !string.IsNullOrEmpty(n.MediationSettingsClassName)), (MoPubBase.LocalMediationSetting n) => n.MediationSettingsClassName, (MoPubBase.LocalMediationSetting n) => n));
		}
	}

	public class MediatedNetwork
	{
		public string AdapterConfigurationClassName { get; set; }

		public string MediationSettingsClassName { get; set; }

		public global::System.Collections.Generic.Dictionary<string, object> NetworkConfiguration { get; set; }

		public global::System.Collections.Generic.Dictionary<string, object> MediationSettings { get; set; }

		public global::System.Collections.Generic.Dictionary<string, object> MoPubRequestOptions { get; set; }
	}

	public class SupportedNetwork : MoPubBase.MediatedNetwork
	{
		public class AdColony : MoPubBase.SupportedNetwork
		{
			public AdColony()
				: base("AdColony")
			{
			}
		}

		public class AdMob : MoPubBase.SupportedNetwork
		{
			public AdMob()
				: base("GooglePlayServices", "MPGoogle")
			{
			}
		}

		public class AppLovin : MoPubBase.SupportedNetwork
		{
			public AppLovin()
				: base("AppLovin")
			{
			}
		}

		public class Chartboost : MoPubBase.SupportedNetwork
		{
			public Chartboost()
				: base("Chartboost")
			{
			}
		}

		public class Facebook : MoPubBase.SupportedNetwork
		{
			public Facebook()
				: base("Facebook")
			{
			}
		}

		public class IronSource : MoPubBase.SupportedNetwork
		{
			public IronSource()
				: base("IronSource")
			{
			}
		}

		public class OnebyAOL : MoPubBase.SupportedNetwork
		{
			public OnebyAOL()
				: base("Millennial", "MPMillennial")
			{
			}
		}

		public class Tapjoy : MoPubBase.SupportedNetwork
		{
			public Tapjoy()
				: base("Tapjoy")
			{
			}
		}

		public class Unity : MoPubBase.SupportedNetwork
		{
			public Unity()
				: base("Unity", "UnityAds")
			{
			}
		}

		public class Vungle : MoPubBase.SupportedNetwork
		{
			public Vungle()
				: base("Vungle")
			{
			}
		}

		protected SupportedNetwork(string adVendor)
		{
			base.AdapterConfigurationClassName = "com.mopub.mobileads." + adVendor + "AdapterConfiguration";
			base.MediationSettingsClassName = adVendor;
		}

		protected SupportedNetwork(string android, string ios)
			: this(android)
		{
		}
	}

	public struct Reward
	{
		public string Label;

		public int Amount;

		public override string ToString()
		{
			return $"\"{Amount} {Label}\"";
		}

		public bool IsValid()
		{
			if (!string.IsNullOrEmpty(Label))
			{
				return Amount > 0;
			}
			return false;
		}
	}

	public const double LatLongSentinel = 99999.0;

	public static readonly string moPubSDKVersion = "5.5.0";

	private static string _pluginName;

	private static bool _allowLegitimateInterest;

	public static string ConsentLanguageCode { get; set; }

	public static MoPubBase.LogLevel logLevel { get; protected set; }

	public static string PluginName => _pluginName ?? (_pluginName = "MoPub Unity Plugin v" + moPubSDKVersion);

	public static int CompareVersions(string a, string b)
	{
		int[] array = VersionStringToInts(a);
		int[] array2 = VersionStringToInts(b);
		for (int i = 0; i < global::UnityEngine.Mathf.Max(array.Length, array2.Length); i++)
		{
			if (VersionPiece(array, i) < VersionPiece(array2, i))
			{
				return -1;
			}
			if (VersionPiece(array, i) > VersionPiece(array2, i))
			{
				return 1;
			}
		}
		return 0;
	}

	protected static void ValidateAdUnitForSdkInit(string adUnitId)
	{
		if (string.IsNullOrEmpty(adUnitId))
		{
			global::UnityEngine.Debug.LogError("A valid ad unit ID is needed to initialize the MoPub SDK.");
		}
	}

	protected static void ReportAdUnitNotFound(string adUnitId)
	{
		global::UnityEngine.Debug.LogWarning($"AdUnit {adUnitId} not found: no plugin was initialized");
	}

	protected static global::System.Uri UrlFromString(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return null;
		}
		try
		{
			return new global::System.Uri(url);
		}
		catch
		{
			global::UnityEngine.Debug.LogError("Invalid URL: " + url);
			return null;
		}
	}

	private static int VersionPiece(global::System.Collections.Generic.IList<int> versionInts, int pieceIndex)
	{
		if (pieceIndex >= versionInts.Count)
		{
			return 0;
		}
		return versionInts[pieceIndex];
	}

	private static int[] VersionStringToInts(string version)
	{
		return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(version.Split('.'), (string v) => int.TryParse(v, out var piece) ? piece : 0));
	}

	protected static void InitManager()
	{
		global::System.Type typeFromHandle = typeof(MoPubManager);
		MoPubManager component = new global::UnityEngine.GameObject("MoPubManager", typeFromHandle).GetComponent<MoPubManager>();
		if (MoPubManager.Instance != component)
		{
			global::UnityEngine.Debug.LogWarning("It looks like you have the " + typeFromHandle.Name + " on a GameObject in your scene. Please remove the script from your scene.");
		}
	}

	protected MoPubBase()
	{
	}
}
