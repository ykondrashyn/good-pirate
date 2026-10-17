public class UI_ShipSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	private global::System.Collections.Generic.List<UI_ShipSlot> shipSlotList = new global::System.Collections.Generic.List<UI_ShipSlot>();

	private void Awake()
	{
	}

	private void OnDisable()
	{
		for (int i = 0; i < shipSlotList.Count; i++)
		{
			global::UnityEngine.Object.Destroy(shipSlotList[i].gameObject);
		}
		shipSlotList.Clear();
	}

	public void Refresh()
	{
		foreach (UI_ShipSlot shipSlot in shipSlotList)
		{
			shipSlot.Refresh();
		}
	}

	private void OnEnable()
	{
		global::System.Collections.Generic.List<MyShipInventory> myShipDataList = MyInventoryDataManager.Instance.myData.myShipDataList;
		for (int i = 0; i < myShipDataList.Count; i++)
		{
			UI_ShipSlot component = global::UnityEngine.Object.Instantiate(slotSrc, base.transform).GetComponent<UI_ShipSlot>();
			component.SetData(myShipDataList[i]);
			shipSlotList.Add(component);
		}
	}
}
