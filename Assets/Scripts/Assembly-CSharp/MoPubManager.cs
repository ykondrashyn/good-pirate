public class MoPubManager : global::UnityEngine.MonoBehaviour
{
	public static MoPubManager Instance { get; private set; }

	public static event global::System.Action<string> OnSdkInitializedEvent;

	public static event global::System.Action<string, float> OnAdLoadedEvent;

	public static event global::System.Action<string, string> OnAdFailedEvent;

	public static event global::System.Action<string> OnAdClickedEvent;

	public static event global::System.Action<string> OnAdExpandedEvent;

	public static event global::System.Action<string> OnAdCollapsedEvent;

	public static event global::System.Action<string> OnInterstitialLoadedEvent;

	public static event global::System.Action<string, string> OnInterstitialFailedEvent;

	public static event global::System.Action<string> OnInterstitialDismissedEvent;

	public static event global::System.Action<string> OnInterstitialExpiredEvent;

	public static event global::System.Action<string> OnInterstitialShownEvent;

	public static event global::System.Action<string> OnInterstitialClickedEvent;

	public static event global::System.Action<string> OnRewardedVideoLoadedEvent;

	public static event global::System.Action<string, string> OnRewardedVideoFailedEvent;

	public static event global::System.Action<string> OnRewardedVideoExpiredEvent;

	public static event global::System.Action<string> OnRewardedVideoShownEvent;

	public static event global::System.Action<string> OnRewardedVideoClickedEvent;

	public static event global::System.Action<string, string> OnRewardedVideoFailedToPlayEvent;

	public static event global::System.Action<string, string, float> OnRewardedVideoReceivedRewardEvent;

	public static event global::System.Action<string> OnRewardedVideoClosedEvent;

	public static event global::System.Action<string> OnRewardedVideoLeavingApplicationEvent;

	public static event global::System.Action<MoPubBase.Consent.Status, MoPubBase.Consent.Status, bool> OnConsentStatusChangedEvent;

	public static event global::System.Action OnConsentDialogLoadedEvent;

	public static event global::System.Action<string> OnConsentDialogFailedEvent;

	public static event global::System.Action OnConsentDialogShownEvent;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			global::UnityEngine.Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private string[] DecodeArgs(string argsJson, int min)
	{
		bool flag = false;
		global::System.Collections.Generic.List<object> list = global::MoPubInternal.ThirdParty.MiniJSON.Json.Deserialize(argsJson) as global::System.Collections.Generic.List<object>;
		if (list == null)
		{
			global::UnityEngine.Debug.LogError("Invalid JSON data: " + argsJson);
			list = new global::System.Collections.Generic.List<object>();
			flag = true;
		}
		if (list.Count < min)
		{
			if (!flag)
			{
				global::UnityEngine.Debug.LogError("Missing one or more values: " + argsJson + " (expected " + min + ")");
			}
			while (list.Count < min)
			{
				list.Add("");
			}
		}
		return global::System.Linq.Enumerable.ToArray(global::System.Linq.Enumerable.Select(list, (object v) => v.ToString()));
	}

	public void EmitSdkInitializedEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 1);
		string obj = array[0];
		MoPubBase.LogLevel logLevel = MoPubBase.LogLevel.MPLogLevelNone;
		if (array.Length > 1)
		{
			try
			{
				logLevel = (MoPubBase.LogLevel)global::System.Enum.Parse(typeof(MoPubBase.LogLevel), array[1]);
			}
			catch (global::System.ArgumentException)
			{
				global::UnityEngine.Debug.LogWarning("Invalid LogLevel received: " + array[1]);
			}
		}
		else
		{
			global::UnityEngine.Debug.LogWarning("No LogLevel received");
		}
		MoPubLog.Log("EmitSdkInitializedEvent", "SDK initialized and ready to display ads.  Log Level: {0}", logLevel);
		MoPubManager.OnSdkInitializedEvent?.Invoke(obj);
	}

	public void EmitConsentStatusChangedEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 3);
		MoPubBase.Consent.Status arg = MoPubBase.Consent.FromString(array[0]);
		MoPubBase.Consent.Status status = MoPubBase.Consent.FromString(array[1]);
		bool flag = array[2].ToLower() == "true";
		MoPubLog.Log("EmitConsentStatusChangedEvent", "Consent changed to {0} from {1}: PII can{2} be collected. Reason: {3}", status, flag);
		MoPubManager.OnConsentStatusChangedEvent?.Invoke(arg, status, flag);
	}

	public void EmitConsentDialogLoadedEvent()
	{
		MoPubLog.Log("EmitConsentDialogLoadedEvent", "Consent dialog loaded");
		MoPubManager.OnConsentDialogLoadedEvent?.Invoke();
	}

	public void EmitConsentDialogFailedEvent(string argsJson)
	{
		string text = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitConsentDialogFailedEvent", "Consent dialog failed: ({0}) {1}", text);
		MoPubManager.OnConsentDialogFailedEvent?.Invoke(text);
	}

	public void EmitConsentDialogShownEvent()
	{
		MoPubLog.Log("EmitConsentDialogShownEvent", "Sucessfully showed consent dialog");
		MoPubManager.OnConsentDialogShownEvent?.Invoke();
	}

	public void EmitAdLoadedEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 2);
		string arg = array[0];
		string s = array[1];
		MoPubLog.Log("EmitAdLoadedEvent", "Ad loaded");
		MoPubLog.Log("EmitAdLoadedEvent", "Ad shown");
		MoPubManager.OnAdLoadedEvent?.Invoke(arg, float.Parse(s));
	}

	public void EmitAdFailedEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 2);
		string arg = array[0];
		string text = array[1];
		MoPubLog.Log("EmitAdFailedEvent", "Ad failed to load: ({0}) {1}", text);
		MoPubManager.OnAdFailedEvent?.Invoke(arg, text);
	}

	public void EmitAdClickedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitAdClickedEvent", "Ad tapped");
		MoPubManager.OnAdClickedEvent?.Invoke(obj);
	}

	public void EmitAdExpandedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitAdExpandedEvent", "Ad expanded");
		MoPubManager.OnAdExpandedEvent?.Invoke(obj);
	}

	public void EmitAdCollapsedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitAdCollapsedEvent", "Ad collapsed");
		MoPubManager.OnAdCollapsedEvent?.Invoke(obj);
	}

	public void EmitInterstitialLoadedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitInterstitialLoadedEvent", "Ad loaded");
		MoPubManager.OnInterstitialLoadedEvent?.Invoke(obj);
	}

	public void EmitInterstitialFailedEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 2);
		string arg = array[0];
		string text = array[1];
		MoPubLog.Log("EmitInterstitialFailedEvent", "Ad failed to load: ({0}) {1}", text);
		MoPubManager.OnInterstitialFailedEvent?.Invoke(arg, text);
	}

	public void EmitInterstitialDismissedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitInterstitialDismissedEvent", "Ad did disappear");
		MoPubManager.OnInterstitialDismissedEvent?.Invoke(obj);
	}

	public void EmitInterstitialDidExpireEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitInterstitialDidExpireEvent", "Ad expired since it was not shown within {0} minutes of it being loaded");
		MoPubManager.OnInterstitialExpiredEvent?.Invoke(obj);
	}

	public void EmitInterstitialShownEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitInterstitialShownEvent", "Ad shown");
		MoPubManager.OnInterstitialShownEvent?.Invoke(obj);
	}

	public void EmitInterstitialClickedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitInterstitialClickedEvent", "Ad tapped");
		MoPubManager.OnInterstitialClickedEvent?.Invoke(obj);
	}

	public void EmitRewardedVideoLoadedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitRewardedVideoLoadedEvent", "Ad loaded");
		MoPubManager.OnRewardedVideoLoadedEvent?.Invoke(obj);
	}

	public void EmitRewardedVideoFailedEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 2);
		string arg = array[0];
		string text = array[1];
		MoPubLog.Log("EmitRewardedVideoFailedEvent", "Ad failed to load: ({0}) {1}", text);
		MoPubManager.OnRewardedVideoFailedEvent?.Invoke(arg, text);
	}

	public void EmitRewardedVideoExpiredEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitRewardedVideoExpiredEvent", "Ad expired since it was not shown within {0} minutes of it being loaded");
		MoPubManager.OnRewardedVideoExpiredEvent?.Invoke(obj);
	}

	public void EmitRewardedVideoShownEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitRewardedVideoShownEvent", "Ad shown");
		MoPubManager.OnRewardedVideoShownEvent?.Invoke(obj);
	}

	public void EmitRewardedVideoClickedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitRewardedVideoClickedEvent", "Ad tapped");
		MoPubManager.OnRewardedVideoClickedEvent?.Invoke(obj);
	}

	public void EmitRewardedVideoFailedToPlayEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 2);
		string arg = array[0];
		string arg2 = array[1];
		MoPubManager.OnRewardedVideoFailedToPlayEvent?.Invoke(arg, arg2);
	}

	public void EmitRewardedVideoReceivedRewardEvent(string argsJson)
	{
		string[] array = DecodeArgs(argsJson, 3);
		string arg = array[0];
		string arg2 = array[1];
		string s = array[2];
		MoPubLog.Log("EmitRewardedVideoReceivedRewardEvent", "Ad should reward user with {0} {1}");
		MoPubManager.OnRewardedVideoReceivedRewardEvent?.Invoke(arg, arg2, float.Parse(s));
	}

	public void EmitRewardedVideoClosedEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubLog.Log("EmitRewardedVideoClosedEvent", "Ad did disappear");
		MoPubManager.OnRewardedVideoClosedEvent?.Invoke(obj);
	}

	public void EmitRewardedVideoLeavingApplicationEvent(string argsJson)
	{
		string obj = DecodeArgs(argsJson, 1)[0];
		MoPubManager.OnRewardedVideoLeavingApplicationEvent?.Invoke(obj);
	}
}
