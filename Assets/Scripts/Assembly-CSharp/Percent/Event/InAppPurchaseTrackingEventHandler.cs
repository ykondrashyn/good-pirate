namespace Percent.Event
{
	public class InAppPurchaseTrackingEventHandler : global::Percent.Http.HttpsClient, global::Percent.Event.ITrackingEventListener
	{
		private string code;

		public void onTriggerTrackingEvent<T>(T arg)
		{
			code = (string)global::System.Convert.ChangeType(arg, global::System.Type.GetType("System.String"));
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
				global::Percent.Logger.error("Can NOT send in-app purchase event. UUID is NOT exist.");
				return string.Empty;
			}
			string gET_REQ_LOG = global::Percent.Http.PercentHttpConfig.GET_REQ_LOG;
			gET_REQ_LOG = global::Percent.Http.HttpsClient.resolve(gET_REQ_LOG);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, global::Percent.Config.GAME_ID);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, global::Percent.UUIDLoader.uuid);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PAYMENT);
			global::Percent.Http.HttpsClient.addResource(ref gET_REQ_LOG, code);
			return gET_REQ_LOG;
		}
	}
}
