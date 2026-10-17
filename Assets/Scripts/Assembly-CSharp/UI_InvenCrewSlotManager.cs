public class UI_InvenCrewSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform contentTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject crewSlotSrc;

	private global::System.Collections.Generic.List<UI_InvenCrewSlot> crewSlotList = new global::System.Collections.Generic.List<UI_InvenCrewSlot>();

	private void OnEnable()
	{
		Refresh();
	}

	public void SellMode(bool active)
	{
		foreach (UI_InvenCrewSlot crewSlot in crewSlotList)
		{
			crewSlot.SetSellMode(active);
		}
	}

	public void Refresh()
	{
		foreach (UI_InvenCrewSlot crewSlot in crewSlotList)
		{
			global::UnityEngine.Object.Destroy(crewSlot.gameObject);
		}
		crewSlotList.Clear();
		foreach (global::System.Collections.Generic.KeyValuePair<ESailor, MySailorInventory> item in MyInventoryDataManager.Instance.mySailorDataDic)
		{
			UI_InvenCrewSlot component = global::UnityEngine.Object.Instantiate(crewSlotSrc, contentTrans).GetComponent<UI_InvenCrewSlot>();
			component.SetData(item.Value);
			crewSlotList.Add(component);
		}
	}

	public void OrderAll()
	{
		foreach (UI_InvenCrewSlot crewSlot in crewSlotList)
		{
			if (!crewSlot.gameObject.activeSelf)
			{
				crewSlot.gameObject.SetActive(value: true);
			}
		}
	}

	public void OrderEquipped()
	{
	}

	public void OrderUnequipped()
	{
	}
}
