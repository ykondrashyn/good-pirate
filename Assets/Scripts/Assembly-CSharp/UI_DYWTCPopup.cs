public class UI_DYWTCPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private UI_ShipSelectCrewSlot crewSlot;

	private UI_ShipSelectCrewSlot selectedCrewSlot;

	private MySailorInventory selectedCrewData;

	public void SetCrewData(UI_ShipSelectCrewSlot slotData)
	{
		selectedCrewSlot = slotData;
		selectedCrewData = selectedCrewSlot.myData;
		crewSlot.SetData(selectedCrewData);
	}

	public void Change()
	{
		if (UI_Manager.Instance.ui_mainGame.ui_shipInfoPopup.AddSelectList(selectedCrewData))
		{
			selectedCrewSlot.SendMessage("CheckOn");
		}
		GetComponent<PopupAction>().Close();
	}
}
