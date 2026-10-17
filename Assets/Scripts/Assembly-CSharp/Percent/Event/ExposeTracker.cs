namespace Percent.Event
{
	public class ExposeTracker : global::Percent.Event.InHouseTracker
	{
		protected override string getUrl(global::Percent.PromotionType type, int clickedGameId)
		{
			if (!global::Percent.UUIDLoader.hasUUID())
			{
				global::Percent.Logger.error("Can NOT send expose event. UUID is NOT exist.");
				return string.Empty;
			}
			string result = global::Percent.Http.HttpsClient.resolve(global::Percent.Http.PercentHttpConfig.GET_REQ_LOG);
			global::Percent.Http.HttpsClient.addResource(ref result, global::Percent.Config.GAME_ID);
			global::Percent.Http.HttpsClient.addResource(ref result, global::Percent.UUIDLoader.uuid);
			global::Percent.Http.HttpsClient.addResource(ref result, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_EXPOSE);
			global::Percent.Http.HttpsClient.addResource(ref result, getResourceOf(type));
			global::Percent.Http.HttpsClient.addResource(ref result, clickedGameId);
			return result;
		}

		protected override string addParamater(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return string.Empty;
			}
			global::Percent.Http.Parameter parameter = new global::Percent.Http.Parameter(url);
			parameter.addParamater(global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_ID, global::Percent.PromotionData.crossPromotionData.rId);
			return parameter.getUrl();
		}
	}
}
