public class UI_EnterIslandPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandText;

	private global::UnityEngine.Events.UnityAction yesEvent;

	private Docking selectedDock;

	private float prevTimeScale;

	private void OnEnable()
	{
		prevTimeScale = global::UnityEngine.Time.timeScale;
		global::UnityEngine.Time.timeScale = 0f;
		UI_Manager.Instance.WorldMapSetActive(active: false);
	}

	public void SetDock(Docking dock, global::UnityEngine.Events.UnityAction yesCallback)
	{
		selectedDock = dock;
		yesEvent = yesCallback;
		islandText.text = string.Format("{0} {1}", LanguageManager.Instance.translate("key_island"), selectedDock.myIsland.status.islandId + 1);
	}

	public void Enter()
	{
		UI_Manager.Instance.BlackScreenOn(0.4f, delegate
		{
			GameManager.Instance.InIsland(selectedDock);
		});
		selectedDock.OverlapCheckClear();
		if (yesEvent != null)
		{
			yesEvent();
		}
		Close();
		MoPubAdManager.Instance.ShowInterstitialAd();
	}

	public void Close()
	{
		global::UnityEngine.Time.timeScale = prevTimeScale;
		GetComponent<PopupAction>().Close();
	}
}
