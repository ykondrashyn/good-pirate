public class UI_Cheat : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.InputField world;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.InputField island;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.InputField level;

	public void Move()
	{
		if (island.text != "")
		{
			PlayerInfo.Instance.myData.selectedIslandId = int.Parse(island.text);
		}
		else
		{
			PlayerInfo.Instance.myData.selectedIslandId = 0;
		}
		if (world.text != "")
		{
			PlayerInfo.Instance.myData.selectedWorldNum = int.Parse(world.text);
		}
		else
		{
			PlayerInfo.Instance.myData.selectedWorldNum = 1;
		}
		int min = 0;
		int max = 0;
		switch (PlayerInfo.Instance.myData.selectedWorldNum)
		{
		case 1:
			min = 0;
			max = 19;
			break;
		case 2:
			min = 20;
			max = 39;
			break;
		case 3:
			min = 40;
			max = 79;
			break;
		case 4:
			min = 80;
			max = 119;
			break;
		case 5:
			min = 120;
			max = 159;
			break;
		case 6:
			min = 160;
			max = 209;
			break;
		}
		PlayerInfo.Instance.myData.selectedIslandId = global::UnityEngine.Mathf.Clamp(PlayerInfo.Instance.myData.selectedIslandId, min, max);
		global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
	}

	public void SetLevel()
	{
		PlayerInfo.Instance.myData.mainShipLevel = int.Parse(level.text);
		global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
	}
}
