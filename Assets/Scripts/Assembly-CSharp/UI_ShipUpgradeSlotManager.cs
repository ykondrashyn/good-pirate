public class UI_ShipUpgradeSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	private global::System.Collections.Generic.List<UI_ShipUpgradeSlot> slotList = new global::System.Collections.Generic.List<UI_ShipUpgradeSlot>();

	private global::System.Collections.Generic.List<EShipType> spawnShipType = new global::System.Collections.Generic.List<EShipType>();

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		spawnShipType.Add(EShipType.Normal);
		spawnShipType.Add(EShipType.Rapid);
		spawnShipType.Add(EShipType.Cannon);
		spawnShipType.Add(EShipType.Gas);
		spawnShipType.Add(EShipType.Guide);
		spawnShipType.Add(EShipType.Multi);
	}

	private void OnEnable()
	{
		ReGenerate();
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onMoneyModify = (PlayerInfo.eventCallBack)global::System.Delegate.Combine(instance.onMoneyModify, new PlayerInfo.eventCallBack(Refresh));
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onMoneyModify = (PlayerInfo.eventCallBack)global::System.Delegate.Remove(instance.onMoneyModify, new PlayerInfo.eventCallBack(Refresh));
	}

	private void Generate()
	{
		for (int i = 0; i < spawnShipType.Count; i++)
		{
			UI_ShipUpgradeSlot component = global::UnityEngine.Object.Instantiate(slotSrc, transform).GetComponent<UI_ShipUpgradeSlot>();
			component.SetData(spawnShipType[i]);
			slotList.Add(component);
		}
	}

	private void AllDelete()
	{
		for (int i = 0; i < slotList.Count; i++)
		{
			global::UnityEngine.Object.Destroy(slotList[i].gameObject);
		}
		slotList.Clear();
	}

	public void ReGenerate()
	{
		AllDelete();
		Generate();
	}

	public void Refresh()
	{
		foreach (UI_ShipUpgradeSlot slot in slotList)
		{
			slot.DataRefresh();
		}
	}
}
