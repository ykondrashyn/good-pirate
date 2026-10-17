[global::System.Serializable]
public class SubRepairShipData : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public SubRepairShipDataData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new SubRepairShipDataData[0];
		}
	}
}
