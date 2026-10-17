namespace Percent
{
	public class SessionLifeCycle : global::UnityEngine.MonoBehaviour
	{
		public enum Status
		{
			INIT = 0,
			UUID_SUCCESS = 1,
			UUID_FAIL = 2,
			ACCESS_SUCCESS = 3,
			ACCESS_FAIL = 4
		}

		internal delegate void OnStateChange(global::Percent.SessionLifeCycle.Status state);

		public static global::Percent.SessionLifeCycle instance;

		public global::Percent.CrossPromotion crossPromotion;

		public global::Percent.UUIDLoader uuidLoader;

		public global::Percent.AccessDataLoader accessDataLoader;

		public global::Percent.AndroidReferrerRequester referrerRequester;

		private global::Percent.SessionLifeCycle.Status state;

		internal global::Percent.SessionLifeCycle.OnStateChange onStateChange;

		private bool isGotUUID;

		private bool isGotReferrer;

		private global::Percent.SessionLifeCycle.Status State
		{
			set
			{
				state = value;
				if (onStateChange != null)
				{
					onStateChange(state);
				}
			}
		}

		private void Start()
		{
			instance = this;
		}

		private void initalizeSessionLifeCycle()
		{
			if (global::Percent.Config.didCrossPromotionIDSet())
			{
				State = global::Percent.SessionLifeCycle.Status.INIT;
				if (!global::Percent.UUIDLoader.hasUUID())
				{
					uuidLoader.request(onReceiveUUID);
					return;
				}
				uuidLoader.setUUID();
				onSendAccess();
			}
		}

		public static void initializeCrossPromotionSession()
		{
			global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("ads", "cross-promotion-session");
		}

		private void onReceiveUUID(bool isSuccess)
		{
			if (isSuccess)
			{
				State = global::Percent.SessionLifeCycle.Status.UUID_SUCCESS;
				isGotUUID = true;
				onSendAccess();
				trySendReferrer();
			}
			else
			{
				State = global::Percent.SessionLifeCycle.Status.UUID_FAIL;
			}
		}

		private void trySendReferrer()
		{
			if (isGotUUID && isGotReferrer)
			{
				onSendReferrer();
			}
		}

		public void onGetReferrer()
		{
			isGotReferrer = true;
			trySendReferrer();
		}

		private void onSendReferrer()
		{
			referrerRequester.request();
		}

		private void onSendAccess()
		{
			accessDataLoader.request(onReceiveAccess);
		}

		private void onReceiveAccess(bool isSuccess)
		{
			if (isSuccess)
			{
				State = global::Percent.SessionLifeCycle.Status.ACCESS_SUCCESS;
				crossPromotion.onPromotionDataLoad(isSuccess: true);
			}
			else
			{
				State = global::Percent.SessionLifeCycle.Status.ACCESS_FAIL;
				crossPromotion.onPromotionDataLoad(isSuccess: false);
			}
		}
	}
}
