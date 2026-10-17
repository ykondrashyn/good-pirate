[global::System.Serializable]
public class SubCannonShipData : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public SubCannonShipDataData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new SubCannonShipDataData[0];
		}
	}
}
