namespace Percent.Badge
{
	public class BadgePoolCacher
	{
		private static string PREF_POOL_LENGTH = "percentBadgePoolLength";

		private static string PREF_BADGE_GAME_ID = "percentBadgeGameId";

		private static string PREF_BADGE_ICON_URL = "percentBadgeIconUrl";

		private static string PREF_BADGE_STORE_URL = "percentBadgeStoreUrl";

		internal static void save()
		{
			int num = global::Percent.PromotionData.badgePool.Length;
			global::UnityEngine.PlayerPrefs.SetInt(PREF_POOL_LENGTH, num);
			for (int i = 0; i < num; i++)
			{
				global::UnityEngine.PlayerPrefs.SetInt(PREF_BADGE_GAME_ID, global::Percent.PromotionData.badgePool[i].gameId);
				global::UnityEngine.PlayerPrefs.SetString(PREF_BADGE_ICON_URL, global::Percent.PromotionData.badgePool[i].iconUrl);
				global::UnityEngine.PlayerPrefs.SetString(PREF_BADGE_STORE_URL, global::Percent.PromotionData.badgePool[i].storeUrl);
			}
		}

		internal static global::Percent.Badge.BadgeData[] load()
		{
			int num = global::UnityEngine.PlayerPrefs.GetInt(PREF_POOL_LENGTH);
			global::Percent.Badge.BadgeData[] array = new global::Percent.Badge.BadgeData[num];
			for (int i = 0; i < num; i++)
			{
				array[i].gameId = global::UnityEngine.PlayerPrefs.GetInt(PREF_BADGE_GAME_ID);
				array[i].iconUrl = global::UnityEngine.PlayerPrefs.GetString(PREF_BADGE_ICON_URL);
				array[i].storeUrl = global::UnityEngine.PlayerPrefs.GetString(PREF_BADGE_STORE_URL);
			}
			return array;
		}
	}
}
