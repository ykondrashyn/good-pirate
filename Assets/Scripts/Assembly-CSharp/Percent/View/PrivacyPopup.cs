namespace Percent.View
{
	public class PrivacyPopup : global::UnityEngine.MonoBehaviour
	{
		public global::Percent.Tween.ColorTween showBackgroundColorTweener;

		public global::Percent.Tween.PercentTween showFrameScaleTweener;

		public global::Percent.Tween.PercentTween showWaitFrameScaleTweener;

		public global::UnityEngine.GameObject waitPopupGO;

		public global::Percent.View.TurnOnOffWindow turnOnOffWindow;

		public global::Percent.UUIDLoader uuidLoader;

		public global::UnityEngine.UI.Text descriptionText;

		public global::Percent.Http.TextLoader textLoader;

		public global::UnityEngine.RectTransform scrollMe;

		public global::UnityEngine.GameObject thirdParties;

		private global::Percent.TextTool textTool;

		private global::UnityEngine.UI.Scrollbar scrollBar;

		public int privacyPolicyVersion;

		private readonly string PRIVACY_POLICY_VERSION = "PrivacyPolicyVersion";

		private readonly string PRIVACY_POLICY = "privacyPolicy";

		public static readonly string[] EU_CODES = new string[32]
		{
			"AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR",
			"DE", "GR", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL",
			"PL", "PT", "RO", "SK", "SI", "ES", "SE", "AE", "BGN", "HRK",
			"EUR", "RON"
		};

		private global::UnityEngine.AndroidJavaObject activityContext;

		private global::UnityEngine.AndroidJavaObject className;

		private global::UnityEngine.AndroidJavaObject pluginClass;

		private void Start()
		{
			global::UnityEngine.Application.RequestAdvertisingIdentifierAsync(delegate(string advertisingId, bool trackingEnabled, string error)
			{
				global::Percent.UUIDLoader.advertisingId = advertisingId;
				global::Percent.UUIDLoader.isTrackingEnabled = trackingEnabled;
			});
			if (global::UnityEngine.PlayerPrefs.HasKey(PRIVACY_POLICY_VERSION))
			{
				privacyPolicyVersion = global::UnityEngine.PlayerPrefs.GetInt(PRIVACY_POLICY_VERSION);
			}
			textTool = new global::Percent.TextTool();
			replaceDescription(textLoader.readDeafultDescription());
			hideSelfWithoutThirdparties();
		}

		public void showWindow()
		{
			textLoader.request(onReceieveText);
		}

		private void playShowWindow()
		{
			CancelInvoke();
			scrollBar = showFrameScaleTweener.transform.Find("InnerTextBox").Find("Scrollbar").GetComponent<global::UnityEngine.UI.Scrollbar>();
			showFrameScaleTweener.onEnd = delegate
			{
				scrollBar.GetComponent<ScrollToTop>().repositionScrollbar();
			};
			showBackgroundColorTweener.play();
			showFrameScaleTweener.play();
		}

		private void onReceieveText(bool isSuccess)
		{
			global::UnityEngine.Debug.Log("DDDDDDDDDDDDD " + isSuccess);
			if (isSuccess)
			{
				if (!global::Percent.CrossPromotion.isNOTEURegion())
				{
					playShowWindow();
				}
			}
			else
			{
				hideSelf();
			}
		}

		public void hideWindowWithAgreement()
		{
			InvokeRepeating("getAndroidAdvertisingId", 0f, 0.1f);
		}

		private void getAndroidAdvertisingId()
		{
			try
			{
				using (global::UnityEngine.AndroidJavaClass androidJavaClass = new global::UnityEngine.AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					activityContext = androidJavaClass.GetStatic<global::UnityEngine.AndroidJavaObject>("currentActivity");
				}
				if (pluginClass != null)
				{
					className = pluginClass.CallStatic<global::UnityEngine.AndroidJavaObject>("getInstance", global::System.Array.Empty<object>());
					activityContext.Call("runOnUiThread", (global::UnityEngine.AndroidJavaRunnable)delegate
					{
						className.Call("initialize", activityContext);
						if (pluginClass.GetStatic<bool>("hasFinished"))
						{
							global::Percent.UUIDLoader.advertisingId = pluginClass.GetStatic<string>("strAdid");
							global::Percent.UUIDLoader.isTrackingEnabled = pluginClass.GetStatic<bool>("isTrackingEnabled");
							global::UnityEngine.Debug.Log("android uuid enabled? " + global::Percent.UUIDLoader.isTrackingEnabled);
							checkTrackability();
							CancelInvoke();
						}
					});
				}
				else
				{
					pluginClass = new global::UnityEngine.AndroidJavaClass("com.percent.crosspromotion.GetAdvertisingId");
				}
			}
			catch (global::System.Exception ex)
			{
				global::UnityEngine.Debug.Log("error: " + ex.Message);
			}
		}

		private void checkTrackability()
		{
			if (global::Percent.UUIDLoader.isTrackingEnabled)
			{
				global::Percent.CrossPromotion.hasAgreed = true;
				invokeHide();
			}
			else
			{
				turnOnOffWindow.showTurnOnWindow();
				showFrameScaleTweener.startDelay = 0f;
				showFrameScaleTweener.playReverse();
			}
		}

		public void invokeHide()
		{
			showFrameScaleTweener.startDelay = 0f;
			showFrameScaleTweener.playReverse();
			global::Percent.Tween.PercentTween percentTween = showFrameScaleTweener;
			percentTween.onEnd = (global::Percent.Tween.PercentTween.OnTweenEnd)global::System.Delegate.Combine(percentTween.onEnd, new global::Percent.Tween.PercentTween.OnTweenEnd(hideSelf));
			showBackgroundColorTweener.startDelay = 0f;
			showBackgroundColorTweener.playReverse();
		}

		public void hideSelf()
		{
			global::UnityEngine.PlayerPrefs.SetInt(global::Percent.CrossPromotion.PREF_AGREEMENT, 1);
			hideSelfWithoutThirdparties();
			if (!thirdParties.activeSelf)
			{
				thirdParties.SetActive(value: true);
			}
			global::Percent.SessionLifeCycle.initializeCrossPromotionSession();
			StartCoroutine(InactiveSelfRutine());
		}

		private global::System.Collections.IEnumerator InactiveSelfRutine()
		{
			yield return new global::UnityEngine.WaitForSecondsRealtime(0.5f);
			inactivateSelf();
		}

		private void inactivateSelf()
		{
			base.gameObject.SetActive(value: false);
		}

		private void hideSelfWithoutThirdparties()
		{
			turnOnOffWindow.GetComponent<global::UnityEngine.RectTransform>().localScale = global::UnityEngine.Vector3.zero;
			global::Percent.Tween.PercentTween percentTween = showFrameScaleTweener;
			percentTween.onEnd = (global::Percent.Tween.PercentTween.OnTweenEnd)global::System.Delegate.Remove(percentTween.onEnd, new global::Percent.Tween.PercentTween.OnTweenEnd(hideSelf));
		}

		public void showWaitWindow()
		{
			waitPopupGO.SetActive(value: true);
			showWaitFrameScaleTweener.play();
			showFrameScaleTweener.playReverse();
			global::Percent.CrossPromotion.hasAgreed = false;
		}

		public void showFrameWindow()
		{
			showFrameScaleTweener.play();
		}

		public void parseData(global::Boomlagoon.JSON.JSONObject json)
		{
			if (global::Percent.CrossPromotion.isNOTEURegion())
			{
				hideSelf();
			}
			else
			{
				extractDescription(json);
			}
		}

		private void extractDescription(global::Boomlagoon.JSON.JSONObject json)
		{
			if (!json.ContainsKey("version"))
			{
				return;
			}
			int num = (int)json.GetNumber("version");
			if (num == privacyPolicyVersion)
			{
				string text = textTool.loadTextFromCache(PRIVACY_POLICY);
				if (!text.Equals(string.Empty))
				{
					replaceDescription(text);
				}
				return;
			}
			privacyPolicyVersion = num;
			if (json.ContainsKey(PRIVACY_POLICY))
			{
				string text2 = json.GetString(PRIVACY_POLICY);
				if (!text2.Equals(string.Empty))
				{
					replaceDescription(text2);
					textTool.saveText(PRIVACY_POLICY, text2);
					global::UnityEngine.PlayerPrefs.SetInt(PRIVACY_POLICY_VERSION, num);
				}
				else
				{
					global::UnityEngine.PlayerPrefs.SetInt(PRIVACY_POLICY_VERSION, 0);
				}
			}
		}

		private void replaceDescription(string newDescription)
		{
			descriptionText.text = global::System.Text.RegularExpressions.Regex.Unescape(newDescription);
			scrollMe.sizeDelta = new global::UnityEngine.Vector2(scrollMe.sizeDelta.x, descriptionText.GetComponent<global::UnityEngine.RectTransform>().sizeDelta.y + 500f);
		}
	}
}
