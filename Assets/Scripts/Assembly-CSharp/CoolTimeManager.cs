public static class CoolTimeManager
{
	private const string COOLTIMEKEY = "COOLTIME_{0}";

	public static global::System.TimeSpan GetLeftCoolTime(string key)
	{
		long num = -1L;
		string text = global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetString($"COOLTIME_{key}", "");
		global::System.DateTime dateTime = global::System.DateTime.Now;
		if (text != "")
		{
			num = global::System.Convert.ToInt64(text);
		}
		if (num != -1)
		{
			dateTime = global::System.DateTime.FromBinary(num);
		}
		return dateTime.Subtract(global::System.DateTime.Now);
	}

	public static string GetTimeToString(global::System.TimeSpan timeSpan)
	{
		if (timeSpan.TotalSeconds > 0.0)
		{
			return string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));
		}
		return "";
	}

	public static void AddCooltimeSecondFromNow(string key, int coolTime)
	{
		global::System.DateTime dateTime = global::System.DateTime.Now.AddSeconds(coolTime);
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetString($"COOLTIME_{key}", dateTime.ToBinary().ToString());
	}

	public static void DeleteCoolTime(string key)
	{
		string key2 = $"COOLTIME_{key}";
		if (global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.HasKey(key2))
		{
			global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.DeleteKey(key2);
		}
	}

	public static bool HaveKey(string key)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.HasKey($"COOLTIME_{key}");
	}
}
