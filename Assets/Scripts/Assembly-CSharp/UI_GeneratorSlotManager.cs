public class UI_GeneratorSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	private global::System.Collections.Generic.List<UI_GeneratorSlot> slotList = new global::System.Collections.Generic.List<UI_GeneratorSlot>();

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		for (int i = 1; i < 14; i++)
		{
			UI_GeneratorSlot component = global::UnityEngine.Object.Instantiate(slotSrc, transform).GetComponent<UI_GeneratorSlot>();
			component.SetData((EShipType)i);
			slotList.Add(component);
		}
	}

	private void OnEnable()
	{
		Refresh();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onMoneyModify = (PlayerInfo.eventCallBack)global::System.Delegate.Combine(instance.onMoneyModify, new PlayerInfo.eventCallBack(Refresh));
		MyInventoryDataManager instance2 = MyInventoryDataManager.Instance;
		instance2.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Combine(instance2.UpgradeCrewAction, new global::System.Action(Refresh));
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onMoneyModify = (PlayerInfo.eventCallBack)global::System.Delegate.Remove(instance.onMoneyModify, new PlayerInfo.eventCallBack(Refresh));
		MyInventoryDataManager instance2 = MyInventoryDataManager.Instance;
		instance2.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Remove(instance2.UpgradeCrewAction, new global::System.Action(Refresh));
	}

	private void Refresh()
	{
		foreach (UI_GeneratorSlot slot in slotList)
		{
			slot.Refresh();
		}
	}
}
