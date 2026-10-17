namespace Percent.Event
{
	public class WatchAdsTrackingEventHandler : global::Percent.Http.HttpsClient, global::Percent.Event.ITrackingEventListener
	{
		private string adType;

		public void onTriggerTrackingEvent<T>(T arg)
		{
			adType = (string)global::System.Convert.ChangeType(arg, global::System.Type.GetType("System.String"));
			string value = getUrl();
			if (!string.IsNullOrEmpty(value))
			{
				sendGETRequest(value);
			}
		}

		internal string getUrl()
		{
			if (!global::Percent.UUIDLoader.hasUUID())
			{
				global::Percent.Logger.error("Can NOT send watchAds event. UUID is NOT exist.");
				return string.Empty;
			}
			string gET_REQ_LOG = global::Percent.Http.PercentHttpConfig.GET_REQ_LOG;
			gET_REQ_LOG = global::Percent.Http.HttpsClient.resolve(gET_REQ_LOG);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, global::Percent.Config.GAME_ID);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, global::Percent.UUIDLoader.uuid);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_AD);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, adType);
			return gET_REQ_LOG;
		}
	}
}
