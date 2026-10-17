public class UI_ShipSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text hpText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text attackText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text cannonText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text speedText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image sailorIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image sailorRankImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject sailorBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject sailorTakeBtnObj;

	public MyShipInventory myData { get; private set; }

	public void SetData(MyShipInventory data)
	{
		myData = data;
		Refresh();
	}

	public void Refresh()
	{
		int shipLevel = PlayerInfo.Instance.GetShipLevel(myData.shipType);
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(myData.shipType);
		ShipBase shipBaseData2 = MyInventoryDataManager.Instance.GetShipBaseData(myData);
		nameText.text = shipBaseData.shipName;
		levelText.text = $"Lv. {shipLevel}";
		attackText.text = shipBaseData2.Damage.ToString();
		if (shipBaseData2 != null)
		{
			hpText.text = shipBaseData2.myHealth.HP_MAX.ToString();
		}
		if (myData.equipSailerId[0] != -1)
		{
			sailorTakeBtnObj.SetActive(value: false);
			sailorBtnObj.SetActive(value: true);
		}
		else
		{
			sailorTakeBtnObj.SetActive(value: true);
			sailorBtnObj.SetActive(value: false);
		}
	}

	public void FindSailor()
	{
		UI_Manager.Instance.ui_mainGame.ui_shipSelectSailorPopup.gameObject.SetActive(value: true);
		UI_Manager.Instance.ui_mainGame.ui_shipSelectSailorPopup.SelectShip(myData);
	}
}
