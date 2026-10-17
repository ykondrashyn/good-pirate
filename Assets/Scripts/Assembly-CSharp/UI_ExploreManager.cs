public class UI_ExploreManager : global::UnityEngine.MonoBehaviour
{
	public static UI_ExploreManager Instance;

	public global::System.Collections.Generic.Dictionary<ESailor, ExploreTimeData> coolTimeData = new global::System.Collections.Generic.Dictionary<ESailor, ExploreTimeData>();

	private string sailorKey = "SailorExplore_{0}";

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		foreach (global::System.Collections.Generic.KeyValuePair<ESailor, ExploreTimeData> coolTimeDatum in coolTimeData)
		{
			if (coolTimeDatum.Value.LeftTimeRatio <= 0f)
			{
				coolTimeDatum.Value.unityAction();
			}
		}
	}

	public void Explore(MySailorInventory sailorData, int generateTime, global::UnityEngine.Events.UnityAction generateCallBack, out ExploreTimeData outData)
	{
		string key = string.Format(sailorKey, sailorData.sailorType.ToString());
		if (!coolTimeData.ContainsKey(sailorData.sailorType))
		{
			ExploreTimeData exploreTimeData = new ExploreTimeData();
			if (CoolTimeManager.HaveKey(key))
			{
				global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(key);
				if (leftCoolTime.TotalSeconds < 0.0)
				{
					CoolTimeManager.DeleteCoolTime(key);
				}
				else
				{
					exploreTimeData.startTicks = global::System.DateTime.Now.Subtract(leftCoolTime).Ticks;
					exploreTimeData.endTicks = global::System.DateTime.Now.Add(leftCoolTime).Ticks;
				}
			}
			outData = exploreTimeData;
			coolTimeData.Add(sailorData.sailorType, exploreTimeData);
		}
		else
		{
			outData = null;
		}
	}
}
