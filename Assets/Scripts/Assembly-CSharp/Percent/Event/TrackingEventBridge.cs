namespace Percent.Event
{
	public class TrackingEventBridge : global::UnityEngine.MonoBehaviour
	{
		private static global::Percent.Event.ITrackingEventListener[] handlers;

		private void Awake()
		{
			handlers = new global::Percent.Event.ITrackingEventListener[2];
			handlers[0] = base.gameObject.AddComponent<global::Percent.Event.InAppPurchaseTrackingEventHandler>();
			handlers[1] = base.gameObject.AddComponent<global::Percent.Event.WatchAdsTrackingEventHandler>();
		}

		public static void triggerTrackingEvent(global::Percent.Event.PercentTracker tracker)
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("analytics", "percent-tracking-bridge");
		}
	}
}
