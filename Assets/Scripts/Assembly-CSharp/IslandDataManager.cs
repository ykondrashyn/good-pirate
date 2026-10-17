public class IslandDataManager : global::UnityEngine.MonoBehaviour
{
	public static IslandDataManager Instance;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<IslandData> islandDataList = new global::System.Collections.Generic.List<IslandData>();

	private global::System.Collections.Generic.Dictionary<EIsland, IslandData> islandDataDic = new global::System.Collections.Generic.Dictionary<EIsland, IslandData>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<RockData> rockDataList = new global::System.Collections.Generic.List<RockData>();

	private global::System.Collections.Generic.Dictionary<ERock, RockData> rockDataDic = new global::System.Collections.Generic.Dictionary<ERock, RockData>();

	private void Awake()
	{
		Instance = this;
		AddDictionaryList();
	}

	public void AddDictionaryList()
	{
		if (islandDataDic != null)
		{
			islandDataDic.Clear();
		}
		if (islandDataDic.Count <= 0)
		{
			foreach (IslandData islandData in islandDataList)
			{
				islandDataDic.Add(islandData.id, islandData);
			}
		}
		if (rockDataDic != null)
		{
			rockDataDic.Clear();
		}
		if (rockDataDic.Count > 0)
		{
			return;
		}
		foreach (RockData rockData in rockDataList)
		{
			rockDataDic.Add(rockData.id, rockData);
		}
	}

	public IslandData GetIslandData(EIsland id)
	{
		if (islandDataDic.ContainsKey(id))
		{
			return islandDataDic[id];
		}
		return null;
	}

	public RockData GetRockData(ERock id)
	{
		if (rockDataDic.ContainsKey(id))
		{
			return rockDataDic[id];
		}
		return null;
	}
}
