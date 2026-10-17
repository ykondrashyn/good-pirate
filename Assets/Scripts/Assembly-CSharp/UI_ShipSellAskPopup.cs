public class UI_ShipSellAskPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text typeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image shipIcon;

	private MyShipInventory selectedShipData;

	public void SetData(MyShipInventory shipData)
	{
		selectedShipData = shipData;
		nameText.text = shipData.shipType.ToString();
	}

	public void Sell()
	{
		GetComponent<PopupAction>().Close();
		UI_Manager.Instance.ui_mainGame.ui_shipInfoPopup.GetComponent<PopupAction>().Close();
		ShipBase shipBaseData = MyInventoryDataManager.Instance.GetShipBaseData(selectedShipData);
		if (shipBaseData != null)
		{
			ShipSubController component = shipBaseData.GetComponent<ShipSubController>();
			global::UnityEngine.GameObject obj = component.gameObject;
			if (GameManager.Instance.playerShipController.subShipList.Contains(component))
			{
				GameManager.Instance.playerShipController.subShipList.Remove(component);
			}
			global::UnityEngine.Object.Destroy(obj);
			if (UI_Manager.Instance.ui_formation.invenShipSlotManager.gameObject.activeSelf)
			{
				UI_Manager.Instance.ui_formation.invenShipSlotManager.ReGenerate();
				UI_Manager.Instance.ui_formation.Reset();
			}
			global::UnityEngine.Debug.Log(UI_Manager.Instance.ui_formation.invenShipSlotManager.gameObject.activeSelf);
			global::UnityEngine.Debug.Log(UI_Manager.Instance.shipPopupSlotManager.gameObject.activeSelf);
			if (UI_Manager.Instance.shipPopupSlotManager.gameObject.activeSelf)
			{
				UI_Manager.Instance.shipPopupSlotManager.ReGenerate();
			}
		}
	}
}
