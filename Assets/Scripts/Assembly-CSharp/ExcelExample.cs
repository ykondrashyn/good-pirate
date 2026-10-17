[global::System.Serializable]
public class ExcelExample : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public ExcelExampleData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new ExcelExampleData[0];
		}
	}
}
