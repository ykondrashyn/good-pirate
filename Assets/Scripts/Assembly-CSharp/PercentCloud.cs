public class PercentCloud : global::UnityEngine.MonoBehaviour
{
	public delegate void JsonGetSuccess(global::Boomlagoon.JSON.JSONObject resultJson);

	public static PercentCloud Instance;

	private global::UnityEngine.WWWForm formData;

	public global::System.Action networkingError;

	private PercentCloud.JsonGetSuccess getDataSuccess;

	private const string URL_BACKUP = "";

	private void Awake()
	{
		Instance = this;
	}

	private global::System.Collections.IEnumerator Send(string url)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "percent-cloud-request");
		if (networkingError != null)
		{
			networkingError();
		}
		yield break;
	}

	public void RequestBackup(string deviceId, string data, PercentCloud.JsonGetSuccess callBack)
	{
		formData = new global::UnityEngine.WWWForm();
		formData.AddField("content", data);
		formData.AddField("saved_at", global::System.DateTime.Now.ToString("yyyy-MM-dd"));
		string url = $"";
		getDataSuccess = callBack;
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "percent-cloud-backup");
		if (networkingError != null)
		{
			networkingError();
		}
	}

	public void GetBackUpData(string deviceId, PercentCloud.JsonGetSuccess callBack)
	{
		formData = null;
		string url = $"";
		getDataSuccess = callBack;
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "percent-cloud-load");
		if (networkingError != null)
		{
			networkingError();
		}
	}
}
