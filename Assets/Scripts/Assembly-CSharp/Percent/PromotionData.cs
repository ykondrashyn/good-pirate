namespace Percent
{
	public class PromotionData
	{
		internal static global::Percent.Badge.BadgeData[] badgePool = new global::Percent.Badge.BadgeData[0];

		internal static global::Percent.CrossPromotionData crossPromotionData;

		internal static global::Boomlagoon.JSON.JSONObject extraData;

		internal static void setData(global::Boomlagoon.JSON.JSONObject json)
		{
			setBadgePool(json);
			setCrossPromotionData(json);
			setInterstitialInterval(json);
			setExtraData(json);
		}

		private static void setBadgePool(global::Boomlagoon.JSON.JSONObject json)
		{
			if (!json.ContainsKey(global::Percent.Http.PercentHttpConfig.RES_PARAM_BADGE_POOL))
			{
				return;
			}
			global::Boomlagoon.JSON.JSONArray array = json.GetArray(global::Percent.Http.PercentHttpConfig.RES_PARAM_BADGE_POOL);
			if (!array.Length.Equals(0))
			{
				badgePool = new global::Percent.Badge.BadgeData[array.Length];
				for (int i = 0; i < badgePool.Length; i++)
				{
					badgePool[i].gameId = (int)array[i].Obj.GetNumber(global::Percent.Http.PercentHttpConfig.RES_PARAM_GAME_ID);
					badgePool[i].iconUrl = array[i].Obj.GetString(global::Percent.Http.PercentHttpConfig.RES_PARAM_ICON_URL);
					badgePool[i].storeUrl = array[i].Obj.GetString(global::Percent.Http.PercentHttpConfig.RES_PARAM_STORE_URL);
				}
			}
		}

		private static void setCrossPromotionData(global::Boomlagoon.JSON.JSONObject json)
		{
			initCrossPromotionData();
			if (json.ContainsKey(global::Percent.Http.PercentHttpConfig.RES_PARAM_CROSSPROMOTION_DATA))
			{
				global::Boomlagoon.JSON.JSONObject jSONObject = json.GetObject(global::Percent.Http.PercentHttpConfig.RES_PARAM_CROSSPROMOTION_DATA);
				crossPromotionData.gameId = (int)jSONObject.GetNumber(global::Percent.Http.PercentHttpConfig.RES_PARAM_GAME_ID);
				crossPromotionData.type = getType(jSONObject.GetString(global::Percent.Http.PercentHttpConfig.RES_PARAM_CROSSPROMOTION_TYPE));
				global::Boomlagoon.JSON.JSONArray array = jSONObject.GetArray(global::Percent.Http.PercentHttpConfig.RES_PARAM_RESOURCE_URL);
				crossPromotionData.resourceUrl = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					crossPromotionData.resourceUrl[i] = array[i].Str;
				}
				crossPromotionData.storeUrl = jSONObject.GetString(global::Percent.Http.PercentHttpConfig.RES_PARAM_STORE_URL);
				crossPromotionData.rId = (int)jSONObject.GetNumber(global::Percent.Http.PercentHttpConfig.RES_PARAM_RID);
			}
		}

		private static void initCrossPromotionData()
		{
			crossPromotionData = default(global::Percent.CrossPromotionData);
			crossPromotionData.gameId = global::Percent.Constants.VALUE_INT_NULL;
		}

		private static global::Percent.PromotionType getType(string typeStr)
		{
			global::Percent.PromotionType result = global::Percent.PromotionType.IMAGE;
			if (!(typeStr == "image"))
			{
				if (typeStr == "slide")
				{
					result = global::Percent.PromotionType.SLIDE;
				}
			}
			else
			{
				result = global::Percent.PromotionType.IMAGE;
			}
			return result;
		}

		private static void setInterstitialInterval(global::Boomlagoon.JSON.JSONObject json)
		{
			if (json.ContainsKey(global::Percent.Http.PercentHttpConfig.RES_PARAM_INTERSTITIAL_INTERVAL))
			{
				double number = json.GetNumber(global::Percent.Http.PercentHttpConfig.RES_PARAM_INTERSTITIAL_INTERVAL);
				global::UnityEngine.GameObject.Find("Interstitial").GetComponent<global::Percent.Interstitial>().Interval = (int)number;
			}
		}

		private static void setExtraData(global::Boomlagoon.JSON.JSONObject json)
		{
			if (json.ContainsKey(global::Percent.Http.PercentHttpConfig.RES_PARAM_EXTRA_DATA))
			{
				extraData = json.GetObject(global::Percent.Http.PercentHttpConfig.RES_PARAM_EXTRA_DATA);
			}
		}

		internal static bool isPromotionDataNull()
		{
			if (!crossPromotionData.gameId.Equals(global::Percent.Constants.VALUE_INT_NULL))
			{
				return false;
			}
			return true;
		}

		internal static bool isBadgePoolNull()
		{
			if (!badgePool.Length.Equals(0))
			{
				return false;
			}
			return true;
		}
	}
}
