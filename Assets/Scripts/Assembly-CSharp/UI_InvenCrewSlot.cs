public class UI_InvenCrewSlot : global::UnityEngine.MonoBehaviour
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

	[global::UnityEngine.Header("SellMode")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup checkBoxCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup checkCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button sellButton;

	[global::UnityEngine.HideInInspector]
	public bool isSellListChecked;

	public MySailorInventory myData;

	public void SetData(MySailorInventory data)
	{
		myData = data;
		Refresh();
	}

	private void Awake()
	{
		if ((bool)checkBoxCanvas)
		{
			checkBoxCanvas.gameObject.SetActive(value: false);
		}
		if ((bool)sellButton)
		{
			sellButton.enabled = false;
		}
	}

	public void Refresh()
	{
		_ = myData;
	}

	public void SetSellMode(bool active)
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
		if (!isSellListChecked)
		{
			if (UI_Manager.Instance.ui_mainGame.ui_crewPopup.AddSellList(myData))
			{
				CheckOn();
			}
		}
		else if (UI_Manager.Instance.ui_mainGame.ui_crewPopup.RemoveSellList(myData))
		{
			CheckOff();
		}
	}

	private void CheckOn()
	{
		checkCanvas.gameObject.SetActive(value: true);
		checkCanvas.alpha = 0f;
		global::DG.Tweening.DOTweenModuleUI.DOFade(checkCanvas, 1f, 0.2f);
		isSellListChecked = true;
	}

	private void CheckOff()
	{
		checkCanvas.gameObject.SetActive(value: false);
		isSellListChecked = false;
	}

	public void CrewInfoPopup()
	{
		UI_Manager.Instance.ui_mainGame.ui_crewInfoPopup.gameObject.SetActive(value: true);
		UI_Manager.Instance.ui_mainGame.ui_crewInfoPopup.SetData(myData);
	}
}
