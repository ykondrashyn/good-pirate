public class UI_CrewSelectSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject equippedSlot;

	private global::System.Collections.Generic.List<UI_CrewSlot> crewSlotList = new global::System.Collections.Generic.List<UI_CrewSlot>();

	private void Awake()
	{
	}

	private void OnDisable()
	{
		for (int i = 0; i < crewSlotList.Count; i++)
		{
			global::UnityEngine.Object.Destroy(crewSlotList[i].gameObject);
		}
		crewSlotList.Clear();
	}

	private void OnEnable()
	{
		global::System.Collections.Generic.List<MySailorInventory> mySailorDataList = MyInventoryDataManager.Instance.myData.mySailorDataList;
		for (int i = 0; i < mySailorDataList.Count; i++)
		{
			if (mySailorDataList[i].status == ESailorStatus.OnCall)
			{
				UI_CrewSlot component = global::UnityEngine.Object.Instantiate(slotSrc, base.transform).GetComponent<UI_CrewSlot>();
				crewSlotList.Add(component);
			}
		}
	}

	public void MakeEquippedSlot(MyShipInventory ship)
	{
	}

	public void Refresh()
	{
	}
}
