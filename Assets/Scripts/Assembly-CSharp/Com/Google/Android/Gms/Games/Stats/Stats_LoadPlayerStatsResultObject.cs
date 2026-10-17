namespace Com.Google.Android.Gms.Games.Stats
{
	public class Stats_LoadPlayerStatsResultObject : global::Google.Developers.JavaObjWrapper, global::Com.Google.Android.Gms.Games.Stats.Stats_LoadPlayerStatsResult, global::Com.Google.Android.Gms.Common.Api.Result
	{
		private const string CLASS_NAME = "com/google/android/gms/games/stats/Stats$LoadPlayerStatsResult";

		public Stats_LoadPlayerStatsResultObject(global::System.IntPtr ptr)
			: base(ptr)
		{
		}

		public global::Com.Google.Android.Gms.Games.Stats.PlayerStats getPlayerStats()
		{
			return new global::Com.Google.Android.Gms.Games.Stats.PlayerStatsObject(InvokeCall<global::System.IntPtr>("getPlayerStats", "()Lcom/google/android/gms/games/stats/PlayerStats;", global::System.Array.Empty<object>()));
		}

		public global::Com.Google.Android.Gms.Common.Api.Status getStatus()
		{
			return new global::Com.Google.Android.Gms.Common.Api.Status(InvokeCall<global::System.IntPtr>("getStatus", "()Lcom/google/android/gms/common/api/Status;", global::System.Array.Empty<object>()));
		}
	}
}
