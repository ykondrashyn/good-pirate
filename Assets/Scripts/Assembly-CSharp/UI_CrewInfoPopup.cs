public class UI_CrewInfoPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private UI_InvenCrewSlot invenCrew;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] skillBgImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] skillNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] skillContent;

	private MySailorInventory myData;

	public void SetData(MySailorInventory data)
	{
		myData = data;
		invenCrew.SetData(myData);
	}
}
