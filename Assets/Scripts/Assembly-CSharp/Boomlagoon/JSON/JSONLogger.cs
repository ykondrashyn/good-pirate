namespace Boomlagoon.JSON
{
	internal static class JSONLogger
	{
		public static void Log(string str)
		{
			global::UnityEngine.Debug.Log(str);
		}

		public static void Error(string str)
		{
			global::UnityEngine.Debug.LogError(str);
		}
	}
}
