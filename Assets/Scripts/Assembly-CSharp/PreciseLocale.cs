public class PreciseLocale
{
	private interface PlatformBridge
	{
		string GetRegion();

		string GetLanguage();

		string GetLanguageID();

		string GetCurrencyCode();

		string GetCurrencySymbol();
	}

	private class EditorBridge : PreciseLocale.PlatformBridge
	{
		public string GetRegion()
		{
			return "US";
		}

		public string GetLanguage()
		{
			return "en";
		}

		public string GetLanguageID()
		{
			return "en_US";
		}

		public string GetCurrencyCode()
		{
			return "USD";
		}

		public string GetCurrencySymbol()
		{
			return "$";
		}
	}

	private class PreciseLocaleAndroid : PreciseLocale.PlatformBridge
	{
		private static global::UnityEngine.AndroidJavaClass _preciseLocale = new global::UnityEngine.AndroidJavaClass("com.kokosoft.preciselocale.PreciseLocale");

		public string GetRegion()
		{
			return _preciseLocale.CallStatic<string>("getRegion", global::System.Array.Empty<object>());
		}

		public string GetLanguage()
		{
			return _preciseLocale.CallStatic<string>("getLanguage", global::System.Array.Empty<object>());
		}

		public string GetLanguageID()
		{
			return _preciseLocale.CallStatic<string>("getLanguageID", global::System.Array.Empty<object>());
		}

		public string GetCurrencyCode()
		{
			return _preciseLocale.CallStatic<string>("getCurrencyCode", global::System.Array.Empty<object>());
		}

		public string GetCurrencySymbol()
		{
			return _preciseLocale.CallStatic<string>("getCurrencySymbol", global::System.Array.Empty<object>());
		}
	}

	private static PreciseLocale.PlatformBridge _platform;

	private static PreciseLocale.PlatformBridge platform
	{
		get
		{
			if (_platform == null)
			{
				_platform = new PreciseLocale.PreciseLocaleAndroid();
			}
			return _platform;
		}
	}

	public static string GetRegion()
	{
		return platform.GetRegion();
	}

	public static string GetLanguageID()
	{
		return platform.GetLanguageID();
	}

	public static string GetLanguage()
	{
		return platform.GetLanguage();
	}

	public static string GetCurrencyCode()
	{
		return platform.GetCurrencyCode();
	}

	public static string GetCurrencySymbol()
	{
		return platform.GetCurrencySymbol();
	}
}
