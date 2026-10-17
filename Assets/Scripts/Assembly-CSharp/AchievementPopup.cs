public class AchievementPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform contentTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	private global::System.Collections.Generic.List<AchievementSlot> slotList = new global::System.Collections.Generic.List<AchievementSlot>();

	private float prevTimeScale = 1f;

	private void Awake()
	{
		for (int i = 0; i < 7; i++)
		{
			AchievementSlot component = global::UnityEngine.Object.Instantiate(slotSrc, contentTrans).GetComponent<AchievementSlot>();
			component.SetAchievement((EAchievement)i);
			slotList.Add(component);
		}
	}

	private void OnEnable()
	{
		prevTimeScale = global::UnityEngine.Time.timeScale;
		global::UnityEngine.Time.timeScale = 0f;
		Refresh();
	}

	private void OnDisable()
	{
		global::UnityEngine.Time.timeScale = prevTimeScale;
	}

	public void Refresh()
	{
		foreach (AchievementSlot slot in slotList)
		{
			slot.Refresh();
		}
	}
}
