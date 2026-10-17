namespace Percent
{
	public class AccessDataLoader : global::Percent.Http.HttpsClient
	{
		private global::UnityEngine.Events.UnityAction<bool> onReceiveAccess;

		internal void request(global::UnityEngine.Events.UnityAction<bool> onReceiveAccess)
		{
			this.onReceiveAccess = onReceiveAccess;
			sendGETRequest(getUrl(), isResponseJson: true);
		}

		private string getUrl()
		{
			string result = global::Percent.Http.HttpsClient.resolve(global::Percent.Http.PercentHttpConfig.GET_REQ_LOG);
			global::Percent.Http.HttpsClient.addResource(ref result, global::Percent.Config.GAME_ID);
			global::Percent.Http.HttpsClient.addResource(ref result, global::Percent.UUIDLoader.uuid);
			global::Percent.Http.HttpsClient.addResource(ref result, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_ACCESS);
			return result;
		}

		internal override void onGETResponseSuccess(global::Boomlagoon.JSON.JSONObject json)
		{
			global::Percent.PromotionData.setData(json);
			base.onGETResponseSuccess(json);
			if (!global::Percent.PromotionData.isPromotionDataNull())
			{
				if (onReceiveAccess != null)
				{
					onReceiveAccess(arg0: true);
				}
			}
			else if (onReceiveAccess != null)
			{
				onReceiveAccess(arg0: false);
			}
		}

		internal override void onGETResponseFail()
		{
			base.onGETResponseFail();
			if (onReceiveAccess != null)
			{
				onReceiveAccess(arg0: false);
			}
		}
	}
}
