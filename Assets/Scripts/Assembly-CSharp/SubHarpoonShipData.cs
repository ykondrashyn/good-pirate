[global::System.Serializable]
public class SubHarpoonShipData : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public SubHarpoonShipDataData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new SubHarpoonShipDataData[0];
		}
	}
}
