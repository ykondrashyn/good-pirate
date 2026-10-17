public class ICloudManager : global::UnityEngine.MonoBehaviour
{
	public static ICloudManager Instance;

	public global::System.Action loadAction;

	public global::System.Action saveAction;

	private const string FREE_DIAMOND = "KEY_FREEDIA";

	private void Awake()
	{
		Instance = this;
	}

	public void ICloudLoad()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "icloud-load");
		if (UI_Manager.Instance != null)
		{
			UI_Manager.Instance.BlackScreenOff(0.4f);
			UI_Manager.Instance.CloudFailPopup();
		}
	}

	public void ICloudSave()
	{
		if (saveAction != null)
		{
			saveAction();
		}
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "icloud-save");
		if (UI_Manager.Instance != null)
		{
			UI_Manager.Instance.BlackScreenOff(0.4f);
			UI_Manager.Instance.CloudFailPopup();
		}
	}
}
