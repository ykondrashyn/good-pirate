namespace Percent
{
	public class Util
	{
		internal static string getPlatform()
		{
			return "android";
		}

		internal static string getRegion()
		{
			string region = PreciseLocale.GetRegion();
			string pattern = "^[A-Za-z]{2}$";
			if (!string.IsNullOrEmpty(region) && global::System.Text.RegularExpressions.Regex.IsMatch(region, pattern))
			{
				return region.ToUpper();
			}
			return "ZZ";
		}

		internal static global::Percent.View.ScreenOrientation getScreenOrientation()
		{
			global::Percent.View.ScreenOrientation result = global::Percent.View.ScreenOrientation.VERTICAL;
			switch (global::UnityEngine.Screen.orientation)
			{
			case global::UnityEngine.ScreenOrientation.LandscapeLeft:
				result = global::Percent.View.ScreenOrientation.HORIZONTAL;
				break;
			case global::UnityEngine.ScreenOrientation.LandscapeRight:
				result = global::Percent.View.ScreenOrientation.HORIZONTAL;
				break;
			case global::UnityEngine.ScreenOrientation.Portrait:
				result = global::Percent.View.ScreenOrientation.VERTICAL;
				break;
			case global::UnityEngine.ScreenOrientation.PortraitUpsideDown:
				result = global::Percent.View.ScreenOrientation.VERTICAL;
				break;
			}
			return result;
		}

		internal static void goMarket(string marketUrl)
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("http", "open-cross-promotion-market");
		}
	}
}
