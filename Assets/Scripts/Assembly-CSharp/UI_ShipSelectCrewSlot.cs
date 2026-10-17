public class UI_ShipSelectCrewSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.Header("status")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image crewBGImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image crewImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewGradeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewStatusText;

	[global::UnityEngine.Header("skill")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform skillGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject skillSrc;

	[global::UnityEngine.Header("select")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup checkBoxCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup checkCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button sellButton;

	[global::UnityEngine.HideInInspector]
	public bool isSellListChecked;

	public MySailorInventory myData;

	public global::UnityEngine.CanvasGroup canvasGroup;

	public bool IsEquipped => true;

	public void SetData(MySailorInventory data)
	{
		myData = data;
		Refresh();
	}

	private void Awake()
	{
		checkBoxCanvas.gameObject.SetActive(value: false);
		if ((bool)sellButton)
		{
			sellButton.enabled = false;
		}
	}

	public void Refresh()
	{
		_ = myData;
	}

	public void SetCheckBoxMode(bool active)
	{
		if (active)
		{
			sellButton.enabled = true;
			checkBoxCanvas.gameObject.SetActive(value: true);
			checkBoxCanvas.alpha = 0f;
			global::DG.Tweening.DOTweenModuleUI.DOFade(checkBoxCanvas, 1f, 0.2f);
			checkCanvas.gameObject.SetActive(value: false);
		}
		else
		{
			sellButton.enabled = false;
			checkBoxCanvas.gameObject.SetActive(value: false);
			isSellListChecked = false;
		}
	}

	public void Check()
	{
	}

	private void CheckOn()
	{
		checkCanvas.gameObject.SetActive(value: true);
		checkCanvas.alpha = 0f;
		global::DG.Tweening.DOTweenModuleUI.DOFade(checkCanvas, 1f, 0.2f);
		isSellListChecked = true;
		canvasGroup.alpha = 1f;
	}

	private void CheckOff()
	{
	}
}
