[global::System.Serializable]
public class SubSucideShipData : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public SubSucideShipDataData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new SubSucideShipDataData[0];
		}
	}
}
