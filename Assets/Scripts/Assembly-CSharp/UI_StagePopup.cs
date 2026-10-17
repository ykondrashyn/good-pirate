public class UI_StagePopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] stagePanelSrcs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform panelPivot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform shipImageRectTransform;

	private UI_StagePanel selectedStagePanel;

	private int selectedWorldNum;

	private int selectedStageNum;

	public int SelectStage => selectedWorldNum * 10 + selectedStageNum;

	private void Start()
	{
		SetStage(0);
	}

	private void SetStage(int slotNum)
	{
		selectedWorldNum = slotNum;
		slotNum = global::UnityEngine.Mathf.Clamp(slotNum, 0, stagePanelSrcs.Length);
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(stagePanelSrcs[slotNum], panelPivot);
		selectedStagePanel = gameObject.GetComponent<UI_StagePanel>();
		shipImageRectTransform.SetAsLastSibling();
		ButtonInit();
		int num = global::UnityEngine.Random.Range(0, selectedStagePanel.shipsPosition.Length);
		selectedStagePanel.ClearStage(num);
		StageSelect(num - 1);
		PlayerInfo.Instance.myData.myBestStage = num;
	}

	public void ButtonInit()
	{
		global::UnityEngine.UI.Button[] stageButtons = selectedStagePanel.stageButtons;
		for (int i = 0; i < stageButtons.Length; i++)
		{
			int num = i;
			stageButtons[i].onClick.RemoveAllListeners();
			stageButtons[i].onClick.AddListener(delegate
			{
				StageSelect(num);
			});
		}
	}

	public void StageSelect(int num)
	{
		if (selectedWorldNum * 10 + num >= PlayerInfo.Instance.myData.myBestStage)
		{
			global::UnityEngine.Debug.Log("아직 도달하지 않은 스테이지 입니다.");
			return;
		}
		num = global::UnityEngine.Mathf.Clamp(num, 0, selectedStagePanel.shipsPosition.Length);
		UI_StageShipPos uI_StageShipPos = selectedStagePanel.shipsPosition[num];
		shipImageRectTransform.position = uI_StageShipPos.transform.position;
	}
}
