public class UI_MiniMapController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject mapPopupObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject mapCamera;

	public void MapPopupOpen()
	{
		mapCamera.SetActive(value: true);
		mapPopupObj.SetActive(value: true);
		global::UnityEngine.Time.timeScale = 0f;
	}

	public void MapPopupClose()
	{
		mapCamera.SetActive(value: false);
		global::UnityEngine.Time.timeScale = 1f;
		UI_Manager.Instance.FormationMode(active: true);
	}
}
