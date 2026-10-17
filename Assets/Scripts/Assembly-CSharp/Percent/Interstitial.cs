namespace Percent
{
	public class Interstitial : global::UnityEngine.MonoBehaviour
	{
		private int interval = global::Percent.Config.VALUE_DEFAULT_INTERSTITIAL_INTERVAL_SEC;

		private float lastShowTimeSec;

		private const int NOT_SHOW = -1;

		private const int EVERYTIME = 0;

		private readonly string PREF_LAST_INTERVAL_FROM_SERVER = "percentLastInterval";

		internal int Interval
		{
			get
			{
				return interval;
			}
			set
			{
				if (value < -1)
				{
					global::Percent.Logger.error("Interstitial interval has NOT valid range.");
					return;
				}
				interval = value;
				global::UnityEngine.PlayerPrefs.SetInt(PREF_LAST_INTERVAL_FROM_SERVER, interval);
			}
		}

		private void Awake()
		{
			lastShowTimeSec = global::UnityEngine.Time.realtimeSinceStartup;
			tryToLoadLastInterval();
		}

		private void tryToLoadLastInterval()
		{
			if (global::UnityEngine.PlayerPrefs.HasKey(PREF_LAST_INTERVAL_FROM_SERVER))
			{
				interval = global::UnityEngine.PlayerPrefs.GetInt(PREF_LAST_INTERVAL_FROM_SERVER);
			}
			else
			{
				interval = global::Percent.Config.VALUE_DEFAULT_INTERSTITIAL_INTERVAL_SEC;
			}
		}

		internal bool isShowable()
		{
			bool flag = true;
			switch (interval)
			{
			case -1:
				return false;
			case 0:
				return true;
			default:
				return isTimeToShow();
			}
		}

		private bool isTimeToShow()
		{
			if (global::UnityEngine.Time.realtimeSinceStartup - lastShowTimeSec >= (float)interval)
			{
				return true;
			}
			return false;
		}

		internal void stampLastShowTime()
		{
			lastShowTimeSec = global::UnityEngine.Time.realtimeSinceStartup;
		}
	}
}
