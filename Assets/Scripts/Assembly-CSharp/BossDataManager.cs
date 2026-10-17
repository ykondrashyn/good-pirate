public class BossDataManager : global::UnityEngine.MonoBehaviour
{
	public static BossDataManager Instance;

	public global::System.Collections.Generic.List<BossData> bossDataList = new global::System.Collections.Generic.List<BossData>();

	private global::System.Collections.Generic.Dictionary<EBoss, BossData> bossDataDic = new global::System.Collections.Generic.Dictionary<EBoss, BossData>(new EBossComparer());

	private void Awake()
	{
		Instance = this;
		foreach (BossData bossData in bossDataList)
		{
			if (!bossDataDic.ContainsKey(bossData.id))
			{
				bossDataDic.Add(bossData.id, bossData);
			}
		}
	}

	public BossData GetBossData(EBoss id)
	{
		if (!bossDataDic.ContainsKey(id))
		{
			return null;
		}
		return bossDataDic[id];
	}
}
