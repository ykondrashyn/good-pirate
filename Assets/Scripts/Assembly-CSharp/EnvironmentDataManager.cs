public class EnvironmentDataManager : global::UnityEngine.MonoBehaviour
{
	public static EnvironmentDataManager Instance;

	public global::System.Collections.Generic.List<EnvironmentData> environmentList = new global::System.Collections.Generic.List<EnvironmentData>();

	private global::System.Collections.Generic.Dictionary<EEnvironment, EnvironmentData> environmentDic = new global::System.Collections.Generic.Dictionary<EEnvironment, EnvironmentData>(new EEnvironmentComparer());

	private void Awake()
	{
		Instance = this;
		AddDictionaryList();
	}

	public void AddDictionaryList()
	{
		environmentDic.Clear();
		foreach (EnvironmentData environment in environmentList)
		{
			if (!environmentDic.ContainsKey(environment.id))
			{
				environmentDic.Add(environment.id, environment);
			}
		}
	}

	public EnvironmentData GetEnvironmentData(EEnvironment id)
	{
		if (!environmentDic.ContainsKey(id))
		{
			return null;
		}
		return environmentDic[id];
	}
}
