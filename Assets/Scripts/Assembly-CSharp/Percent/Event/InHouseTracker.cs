namespace Percent.Event
{
	public class InHouseTracker : global::Percent.Http.HttpsClient
	{
		internal virtual void trigger(global::Percent.PromotionType type, int gameId)
		{
			string value = (type.Equals(global::Percent.PromotionType.BADGE) ? getUrl(type, gameId) : addParamater(getUrl(type, gameId)));
			if (!string.IsNullOrEmpty(value))
			{
				sendGETRequest(value);
			}
		}

		protected virtual string getUrl(global::Percent.PromotionType type, int gameId)
		{
			return string.Empty;
		}

		protected virtual string addParamater(string url)
		{
			return url;
		}

		protected string getResourceOf(global::Percent.PromotionType type)
		{
			string result = string.Empty;
			switch (type)
			{
			case global::Percent.PromotionType.IMAGE:
				result = global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PROMOTION_TYPE_IMAGE;
				break;
			case global::Percent.PromotionType.SLIDE:
				result = global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PROMOTION_TYPE_SLIDE;
				break;
			case global::Percent.PromotionType.VIDEO:
				result = global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PROMOTION_TYPE_VIDEO;
				break;
			case global::Percent.PromotionType.PLAYABLE:
				result = global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PROMOTION_TYPE_PLAYABLE;
				break;
			case global::Percent.PromotionType.BADGE:
				result = global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PROMOTION_TYPE_BADGE;
				break;
			}
			return result;
		}
	}
}
