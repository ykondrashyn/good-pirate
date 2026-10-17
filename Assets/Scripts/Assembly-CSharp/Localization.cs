[global::System.Serializable]
public class Localization : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public LocalizationData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new LocalizationData[0];
		}
	}
}
