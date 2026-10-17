[global::System.Serializable]
public class CrewUpgradePrice : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public CrewUpgradePriceData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new CrewUpgradePriceData[0];
		}
	}
}
