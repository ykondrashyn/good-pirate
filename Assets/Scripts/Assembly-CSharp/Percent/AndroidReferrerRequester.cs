namespace Percent
{
	public class AndroidReferrerRequester : global::Percent.Http.HttpsClient
	{
		public global::UnityEngine.Events.UnityEvent onMessageReceive;

		private string message;

		public void onNativeMessageReceive(string message)
		{
			this.message = message;
			onMessageReceive.Invoke();
		}

		internal void request()
		{
			sendGETRequest(getUrl());
		}

		private string getUrl()
		{
			string prefix = global::Percent.Http.HttpsClient.resolve(global::Percent.Http.PercentHttpConfig.GET_REQ_LOG);
			global::Percent.Http.HttpsClient.addResource(ref prefix, global::Percent.Config.GAME_ID);
			global::Percent.Http.HttpsClient.addResource(ref prefix, global::Percent.UUIDLoader.uuid);
			global::Percent.Http.HttpsClient.addResource(ref prefix, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_REFERRER);
			global::Percent.Http.Parameter parameter = new global::Percent.Http.Parameter(prefix);
			parameter.addParamater("referrer", message);
			return parameter.getUrl();
		}
	}
}
