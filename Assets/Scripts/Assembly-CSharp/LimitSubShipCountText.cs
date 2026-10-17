public class LimitSubShipCountText : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	private int prevSubCount = -1;

	private int nowCount;

	private int prevMaxCount;

	private void Awake()
	{
		if (myText == null)
		{
			myText = GetComponent<global::UnityEngine.UI.Text>();
		}
	}

	private void OnEnable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Combine(instance.UpgradeCrewAction, new global::System.Action(Refresh));
	}

	private void OnDisable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Remove(instance.UpgradeCrewAction, new global::System.Action(Refresh));
	}

	private void Start()
	{
		if (!(GameManager.Instance == null))
		{
			myText.text = $"{PlayerInfo.Instance.UseShipCount}/{PlayerInfo.Instance.MaxSubShipCount}";
		}
	}

	public void Refresh()
	{
		myText.text = $"{nowCount}/{PlayerInfo.Instance.MaxSubShipCount}";
	}

	private void LateUpdate()
	{
		nowCount = PlayerInfo.Instance.UseShipCount;
		if (prevSubCount != nowCount)
		{
			Refresh();
		}
		prevSubCount = nowCount;
	}
}
