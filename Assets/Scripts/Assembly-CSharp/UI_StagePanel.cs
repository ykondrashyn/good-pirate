public class UI_StagePanel : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Image[] stageImages;

	private int stageCount;

	public global::UnityEngine.UI.Button[] stageButtons { get; private set; }

	public UI_StageShipPos[] shipsPosition { get; private set; }

	private void Awake()
	{
		stageImages = GetComponentsInChildren<global::UnityEngine.UI.Image>();
		stageButtons = GetComponentsInChildren<global::UnityEngine.UI.Button>();
		shipsPosition = GetComponentsInChildren<UI_StageShipPos>();
		stageCount = stageImages.Length;
	}

	public void ClearStage(int num)
	{
		for (int i = 0; i < stageImages.Length; i++)
		{
			if (i < num)
			{
				stageImages[i].color = myColor.GetColor(0, 188, 64);
			}
			else
			{
				stageImages[i].color = myColor.GetColor(72, 103, 140);
			}
		}
	}
}
