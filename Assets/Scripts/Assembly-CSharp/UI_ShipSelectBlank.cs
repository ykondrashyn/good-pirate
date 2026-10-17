public class UI_ShipSelectBlank : global::UnityEngine.MonoBehaviour
{
	public void OpenSelectMode()
	{
		UI_Manager.Instance.ui_mainGame.ui_shipInfoPopup.SetCrewSelectMode(active: true);
	}
}
