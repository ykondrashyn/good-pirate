[global::System.Serializable]
public class EnemyReward : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public EnemyRewardData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new EnemyRewardData[0];
		}
	}
}
