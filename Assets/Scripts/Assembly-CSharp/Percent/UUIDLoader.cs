namespace Percent
{
	public class UUIDLoader : global::Percent.Http.HttpsClient
	{
		internal static string uuid = "";

		private static readonly string PREF_UUID = "percentUUID";

		private readonly string VALUE_DEFAULT_ADVERTISING_ID = "00000000-0000-0000-0000-000000000000";

		public static bool isTrackingEnabled = false;

		public static string advertisingId;

		internal void setUUID()
		{
			if (hasUUID())
			{
				uuid = global::UnityEngine.PlayerPrefs.GetString(PREF_UUID);
			}
		}

		public static bool hasUUID()
		{
			if (!global::UnityEngine.PlayerPrefs.HasKey(PREF_UUID))
			{
				return false;
			}
			return true;
		}

		public static string tryGetUUID()
		{
			if (hasUUID())
			{
				return uuid;
			}
			return string.Empty;
		}

		public void request(global::UnityEngine.Events.UnityAction<bool> onReceiveUUID)
		{
			onGETResponse = onReceiveUUID;
			sendGETRequestAsync();
		}

		public void sendGETRequestAsync()
		{
			string text = global::Percent.Http.HttpsClient.resolve(global::Percent.Http.PercentHttpConfig.GET_REQ_LOG);
			global::Percent.Http.HttpsClient.addResource(ref text, global::Percent.Config.GAME_ID);
			global::Percent.Http.HttpsClient.addResource(ref text, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_INSTALL);
			global::Percent.Http.HttpsClient.addResource(ref text, global::Percent.Util.getPlatform());
			global::Percent.Http.HttpsClient.addResource(ref text, global::Percent.Util.getRegion());
			if (isTrackingEnabled)
			{
				global::Percent.Http.HttpsClient.addResource(ref text, advertisingId);
				onGetUrl(text);
			}
			else
			{
				global::Percent.Http.HttpsClient.addResource(ref text, VALUE_DEFAULT_ADVERTISING_ID);
				onGetUrl(text);
			}
		}

		private void onGetUrl(string url)
		{
			sendGETRequest(url, isResponseJson: true);
		}

		internal override void onGETResponseSuccess(global::Boomlagoon.JSON.JSONObject json)
		{
			uuid = json.GetString("uuid");
			saveUUID();
			base.onGETResponseSuccess();
		}

		private void saveUUID()
		{
			global::UnityEngine.PlayerPrefs.SetString(PREF_UUID, uuid);
		}
	}
}
