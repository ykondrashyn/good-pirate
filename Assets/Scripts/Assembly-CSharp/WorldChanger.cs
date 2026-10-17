public class WorldChanger : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.InputField cubeInputField;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.InputField worldInputField;

	public void CubeNumberCheck()
	{
		int value = int.Parse(cubeInputField.text);
		value = global::UnityEngine.Mathf.Clamp(value, 1, 6);
		cubeInputField.text = value.ToString();
	}

	public void WorldNumberCheck()
	{
		int value = int.Parse(worldInputField.text);
		value = global::UnityEngine.Mathf.Clamp(value, 1, 6);
		worldInputField.text = value.ToString();
	}

	public void Go()
	{
		UI_Manager.Instance.WorldMapSetActive(active: false);
		int num = int.Parse(cubeInputField.text);
		int num2 = int.Parse(worldInputField.text);
		PlayerInfo.Instance.myData.selectedCubeNum = num;
		PlayerInfo.Instance.myData.selectedWorldNum = num2;
		PlayerInfo.Instance.SaveData();
		UI_Manager.Instance.BlackScreenOn(1.5f, delegate
		{
			global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
		});
	}
}
