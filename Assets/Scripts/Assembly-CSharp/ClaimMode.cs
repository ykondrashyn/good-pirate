[global::System.Serializable]
public class ClaimMode : global::UnityEngine.ScriptableObject
{
	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string SheetName = "";

	[global::UnityEngine.HideInInspector]
	[global::UnityEngine.SerializeField]
	public string WorksheetName = "";

	public ClaimModeData[] dataArray;

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new ClaimModeData[0];
		}
	}
}
