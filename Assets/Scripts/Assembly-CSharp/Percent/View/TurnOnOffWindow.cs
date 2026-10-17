namespace Percent.View
{
	public class TurnOnOffWindow : global::UnityEngine.MonoBehaviour
	{
		public global::UnityEngine.UI.Text title;

		public global::UnityEngine.UI.Text description;

		public global::UnityEngine.UI.Text instruction;

		public global::Percent.Tween.RotateTween loading;

		public global::Percent.UUIDLoader uuidLoader;

		public global::Percent.View.PrivacyPopup privatyPopup;

		public global::Percent.Tween.PercentTween butWaitScaleTweener;

		private int index;

		private global::Percent.Tween.PercentTween scaleTweener;

		private readonly string[] titleTexts = new string[2] { "TURN ON", "TURN OFF" };

		private readonly string[] descriptionTexts = new string[2] { "You can agree or disagree on our privacy policy. You can change the status anytime if you need to change it.", "You can agree or disagree on our privacy policy. You can change the status anytime if you need to change it." };

		private readonly string[] instructionTexts = new string[2] { "Google Setting > Ads > Disable opt out of interest based ads", "Google Setting > Ads > Enable opt out of interest based ads" };

		private global::UnityEngine.AndroidJavaObject activityContext;

		private global::UnityEngine.AndroidJavaObject className;

		private global::UnityEngine.AndroidJavaObject pluginClass;

		private void Awake()
		{
			scaleTweener = GetComponent<global::Percent.Tween.ScaleTween>();
		}

		private void showWindow(bool isOn)
		{
			index = ((!isOn) ? 1 : 0);
			title.text = titleTexts[index];
			description.text = descriptionTexts[index];
			instruction.text = instructionTexts[index];
			loading.enableRotation(isEnable: true);
			InvokeRepeating("checkAdvertisingId", 0f, 0.5f);
		}

		public void showTurnOnWindow()
		{
			showWindow(isOn: true);
			if (!hideOnMatch())
			{
				scaleTweener.play();
			}
		}

		public void showTurnOffWindow()
		{
			showWindow(isOn: false);
			if (!hideOnMatch())
			{
				scaleTweener.play();
			}
		}

		private bool hideOnMatch()
		{
			if (global::Percent.UUIDLoader.isTrackingEnabled)
			{
				if (index == 0)
				{
					global::UnityEngine.Debug.Log("is on, good to go");
					hideWindow();
					return true;
				}
			}
			else if (index == 1)
			{
				global::UnityEngine.Debug.Log("is off, good to go");
				hideWindow();
				return true;
			}
			return false;
		}

		private void checkAdvertisingId()
		{
			hideOnMatch();
			try
			{
				using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activityContext = androidJavaClass.GetStatic<global::UnityEngine.AndroidJavaObject>("currentActivity");
				}
				if (pluginClass == null)
				{
					pluginClass = new global::UnityEngine.AndroidJavaClass("com.percent.crosspromotion.GetAdvertisingId");
					return;
				}
				className = pluginClass.CallStatic<global::UnityEngine.AndroidJavaObject>("getInstance", global::System.Array.Empty<object>());
				activityContext.Call("runOnUiThread", (global::UnityEngine.AndroidJavaRunnable)delegate
				{
					className.Call("initialize", activityContext);
					if (pluginClass.GetStatic<bool>("hasFinished"))
					{
						global::Percent.UUIDLoader.advertisingId = pluginClass.GetStatic<string>("strAdid");
						global::Percent.UUIDLoader.isTrackingEnabled = pluginClass.GetStatic<bool>("isTrackingEnabled");
						global::UnityEngine.Debug.Log(global::Percent.UUIDLoader.isTrackingEnabled);
						hideOnMatch();
					}
				});
			}
			catch (global::System.Exception ex)
			{
				global::UnityEngine.Debug.Log("error: " + ex.Message);
			}
		}

		private void hideWindow(bool hasFinished = true)
		{
			CancelInvoke();
			loading.enableRotation(isEnable: false);
			scaleTweener.startDelay = 0f;
			scaleTweener.playReverse();
			if (hasFinished)
			{
				global::UnityEngine.PlayerPrefs.SetInt(global::Percent.CrossPromotion.PREF_AGREEMENT, 1);
				privatyPopup.invokeHide();
			}
		}

		public void backToPreviousWindow()
		{
			hideWindow(hasFinished: false);
			if (index == 0)
			{
				privatyPopup.showFrameWindow();
			}
			else
			{
				butWaitScaleTweener.play();
			}
		}
	}
}
