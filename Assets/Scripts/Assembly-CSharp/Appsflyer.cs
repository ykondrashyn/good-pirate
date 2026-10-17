public class Appsflyer : global::UnityEngine.MonoBehaviour
{
	public static Appsflyer Instance;

	private global::System.Collections.Generic.Dictionary<string, string> purchaseEvent = new global::System.Collections.Generic.Dictionary<string, string>();

	private global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();

	private string appName = "GoodPirate_";

	private void Awake()
	{
		Instance = this;
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("attribution", "initialize");
	}

	private void JsEval(ref string json)
	{
		for (int i = 0; i < json.Length; i++)
		{
			if (string.Equals("\\", json[i].ToString()))
			{
				json = json.Remove(i, 1);
			}
		}
	}

	public void ValidateReceipt(string publicKey, global::UnityEngine.Purchasing.PurchaseEventArgs args, string price, string category)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("attribution", "validate-receipt");
	}

	public void SendEvent(string eventName)
	{
		sb.Length = 0;
		sb.Append(appName);
		sb.Append(eventName);
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("attribution", sb.ToString());
	}
}
