public class UI_GateEnemyStatus : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject notClearObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject clearObj;

	private global::UnityEngine.CanvasGroup myCanvas;

	private Gate selectedGate;

	private void Awake()
	{
		myCanvas = GetComponent<global::UnityEngine.CanvasGroup>();
	}

	public void SetGate(Gate gate)
	{
		selectedGate = gate;
		islandNameText.text = $"World {gate.gateStatus.worldNum} Gate";
		TextRefresh();
	}

	private void OnEnable()
	{
		if (UI_Manager.Instance.ui_mainGame.ui_chestPanel.IsOpen)
		{
			UI_Manager.Instance.ui_mainGame.ui_chestPanel.Close();
		}
		myCanvas.alpha = 0f;
		global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvas, 1f, 0.5f);
	}

	private void OnDisable()
	{
		selectedGate = null;
	}

	public void TextRefresh()
	{
		if (!(selectedGate == null))
		{
			if (selectedGate.AliveEnemyCount == 0)
			{
				notClearObj.SetActive(value: false);
				clearObj.SetActive(value: true);
				Invoke("AutoInActive", 8f);
			}
			else
			{
				notClearObj.SetActive(value: true);
				clearObj.SetActive(value: false);
				leftCountText.text = $"{selectedGate.AliveEnemyCount}/{selectedGate.TotalEnemyCount}";
			}
		}
	}

	private void AutoInActive()
	{
		base.gameObject.SetActive(value: false);
	}
}
