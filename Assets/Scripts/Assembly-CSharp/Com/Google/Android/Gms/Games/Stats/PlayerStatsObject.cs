namespace Com.Google.Android.Gms.Games.Stats
{
	public class PlayerStatsObject : global::Google.Developers.JavaObjWrapper, global::Com.Google.Android.Gms.Games.Stats.PlayerStats
	{
		private const string CLASS_NAME = "com/google/android/gms/games/stats/PlayerStats";

		public static float UNSET_VALUE => global::Google.Developers.JavaObjWrapper.GetStaticFloatField("com/google/android/gms/games/stats/PlayerStats", "UNSET_VALUE");

		public static int CONTENTS_FILE_DESCRIPTOR => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/games/stats/PlayerStats", "CONTENTS_FILE_DESCRIPTOR");

		public static int PARCELABLE_WRITE_RETURN_VALUE => global::Google.Developers.JavaObjWrapper.GetStaticIntField("com/google/android/gms/games/stats/PlayerStats", "PARCELABLE_WRITE_RETURN_VALUE");

		public PlayerStatsObject(global::System.IntPtr ptr)
			: base(ptr)
		{
		}

		public float getAverageSessionLength()
		{
			return InvokeCall<float>("getAverageSessionLength", "()F", global::System.Array.Empty<object>());
		}

		public float getChurnProbability()
		{
			return InvokeCall<float>("getChurnProbability", "()F", global::System.Array.Empty<object>());
		}

		public int getDaysSinceLastPlayed()
		{
			return InvokeCall<int>("getDaysSinceLastPlayed", "()I", global::System.Array.Empty<object>());
		}

		public int getNumberOfPurchases()
		{
			return InvokeCall<int>("getNumberOfPurchases", "()I", global::System.Array.Empty<object>());
		}

		public int getNumberOfSessions()
		{
			return InvokeCall<int>("getNumberOfSessions", "()I", global::System.Array.Empty<object>());
		}

		public float getSessionPercentile()
		{
			return InvokeCall<float>("getSessionPercentile", "()F", global::System.Array.Empty<object>());
		}

		public float getSpendPercentile()
		{
			return InvokeCall<float>("getSpendPercentile", "()F", global::System.Array.Empty<object>());
		}

		public float getSpendProbability()
		{
			return InvokeCall<float>("getSpendProbability", "()F", global::System.Array.Empty<object>());
		}

		public float getHighSpenderProbability()
		{
			return InvokeCall<float>("getHighSpenderProbability", "()F", global::System.Array.Empty<object>());
		}

		public float getTotalSpendNext28Days()
		{
			return InvokeCall<float>("getTotalSpendNext28Days", "()F", global::System.Array.Empty<object>());
		}
	}
}
