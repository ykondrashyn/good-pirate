[global::System.Serializable]
public class IslandSheetData : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public IslandSheetDataData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new IslandSheetDataData[0];
		}
	}
}
