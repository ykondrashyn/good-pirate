[global::System.Serializable]
public class MainUpgradePrice : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public MainUpgradePriceData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new MainUpgradePriceData[0];
		}
	}
}
