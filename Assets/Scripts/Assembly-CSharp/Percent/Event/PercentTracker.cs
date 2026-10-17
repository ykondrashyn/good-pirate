namespace Percent.Event
{
	public class PercentTracker : global::UnityEngine.MonoBehaviour
	{
		public global::Percent.Event.TrackerEventType type;

		public string inappCode;

		public void trigger()
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("analytics", "percent-event-" + type);
		}

		public void triggerInAppPurchase(string inappCode)
		{
			this.inappCode = inappCode;
			type = global::Percent.Event.TrackerEventType.IN_APP_PURCHASE;
			trigger();
		}

		public void triggerSeeAdsInterstitial()
		{
			type = global::Percent.Event.TrackerEventType.SEE_ADS_INTERSTITIAL;
			trigger();
		}

		public void triggerSeeAdsReward()
		{
			type = global::Percent.Event.TrackerEventType.SEE_ADS_REWARD;
			trigger();
		}
	}
}
