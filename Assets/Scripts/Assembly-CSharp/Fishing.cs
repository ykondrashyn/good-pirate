public class Fishing : global::UnityEngine.MonoBehaviour
{
	private float coolTime;

	private float coolTimeTotal;

	private FishingIcon myIcon;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private ShipBaseData baseData;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		myIcon = FishingIconManager.Instance.Generate();
		baseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Fishing);
		myIcon.SetPosition(transform.position);
	}

	private void OnDestroy()
	{
		if (myIcon != null)
		{
			global::UnityEngine.Object.Destroy(myIcon.gameObject);
		}
	}

	private void Update()
	{
		myIcon.SetPosition(transform.position);
		if (GameManager.Instance.IsInIsland)
		{
			if (myIcon.isActive)
			{
				myIcon.SetActive(active: false);
			}
			return;
		}
		if (!myIcon.gameObject.activeSelf)
		{
			myIcon.SetActive(active: true);
		}
		myIcon.SetPosition(transform.position);
		coolTimeTotal += global::UnityEngine.Time.deltaTime;
		if (coolTimeTotal >= coolTime)
		{
			coolTimeTotal -= coolTime;
			long num = ShipDataManager.Instance.GetShipSheetData(EShipType.Fishing, PlayerInfo.Instance.GetShipLevel(EShipType.Fishing)).attack;
			if ((bool)PlayerInfo.Instance.myData.isGetDouble)
			{
				num *= 2;
			}
			UI_Manager.Instance.AddReward(transform.position + new global::UnityEngine.Vector3(0f, -5f, 0f), new Reward(EMonenyType.fish, num));
		}
	}
}
