namespace Percent.Http
{
	public class TextLoader : global::Percent.Http.HttpsClient
	{
		private global::UnityEngine.Events.UnityAction<bool> onReceieveText;

		public global::Percent.View.PrivacyPopup privacyPopup;

		public global::UnityEngine.TextAsset percentDescription;

		public global::UnityEngine.TextAsset zzooDescription;

		public string readDeafultDescription()
		{
			switch (global::Percent.CrossPromotion.getCompany())
			{
			case global::Percent.Company.Percent:
				return percentDescription.ToString();
			case global::Percent.Company.ZZOO:
				return zzooDescription.ToString();
			default:
				return percentDescription.ToString();
			}
		}

		public void request(global::UnityEngine.Events.UnityAction<bool> onReceieveText)
		{
			this.onReceieveText = onReceieveText;
			sendGETRequest(getUrl(), isResponseJson: true);
		}

		private string getUrl()
		{
			string prefix = global::Percent.Http.PercentHttpConfig.HOST + global::Percent.Http.PercentHttpConfig.GET_REQ_V3;
			global::Percent.Http.HttpsClient.addResource(ref prefix, global::Percent.Http.PercentHttpConfig.REQ_RESOURCE_PRIVACY_POLICY);
			global::Percent.Http.HttpsClient.addResource(ref prefix, global::Percent.Http.PercentHttpConfig.SLASH);
			global::Percent.Http.Parameter parameter = new global::Percent.Http.Parameter(prefix);
			parameter.addParamater(global::Percent.Http.PercentHttpConfig.RES_PARAM_GAME_ID, global::Percent.Config.GAME_ID);
			parameter.addParamater(global::Percent.Http.PercentHttpConfig.RES_PARAM_COUNTRY_CODE, global::Percent.Util.getRegion());
			parameter.addParamater(global::Percent.Http.PercentHttpConfig.RES_PARAM_PRIVACY_POLICY_VERSION, privacyPopup.privacyPolicyVersion);
			return parameter.getUrl();
		}

		internal override void onGETResponseSuccess(global::Boomlagoon.JSON.JSONObject json)
		{
			privacyPopup.parseData(json);
			if (onReceieveText != null)
			{
				onReceieveText(arg0: true);
			}
		}

		internal override void onGETResponseFail()
		{
			if (onReceieveText != null)
			{
				onReceieveText(arg0: false);
			}
		}
	}
}
