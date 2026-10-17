public class UI_CrewPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private UI_InvenCrewSlotManager crewSlotManager;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform sellBtnTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button sellBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform backBtnTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup backBtnCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button backBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text totalSellPriceText;

	private global::System.Collections.Generic.List<MySailorInventory> sellSailorList = new global::System.Collections.Generic.List<MySailorInventory>();

	[global::UnityEngine.SerializeField]
	private UI_CrewSellAskPopup ui_crewSellAskPopup;

	private bool isSellMode;

	private long price;

	private void OnEnable()
	{
		isSellMode = false;
		backBtnCanvas.gameObject.SetActive(value: false);
		backBtn.enabled = false;
		sellBtnTrans.anchoredPosition = new global::UnityEngine.Vector2(0f, 148.7f);
		sellBtn.onClick.RemoveAllListeners();
		sellBtn.onClick.AddListener(SellMode);
		totalSellPriceText.gameObject.SetActive(value: false);
		price = 0L;
	}

	public void SellMode()
	{
		if (!isSellMode)
		{
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(sellBtnTrans, 463f, 0.28f);
			backBtnTrans.anchoredPosition = new global::UnityEngine.Vector2(0f, 148.7f);
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(backBtnTrans, -405f, 0.28f);
			backBtn.enabled = true;
			backBtnCanvas.gameObject.SetActive(value: true);
			backBtnCanvas.alpha = 0f;
			global::DG.Tweening.DOTweenModuleUI.DOFade(backBtnCanvas, 1f, 0.2f);
			sellBtn.onClick.RemoveAllListeners();
			sellBtn.onClick.AddListener(OpenSellAsk);
			isSellMode = true;
			totalSellPriceText.gameObject.SetActive(value: true);
			crewSlotManager.SellMode(active: true);
			sellSailorList.Clear();
			Refresh();
		}
		else
		{
			sellSailorList.Clear();
			sellBtn.onClick.RemoveAllListeners();
			sellBtn.onClick.AddListener(SellMode);
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(sellBtnTrans, 0f, 0.28f);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(backBtnCanvas, 0f, 0.2f), delegate
			{
				backBtnCanvas.gameObject.SetActive(value: false);
			});
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(backBtnTrans, 0f, 0.28f);
			backBtn.enabled = false;
			isSellMode = false;
			totalSellPriceText.gameObject.SetActive(value: false);
			crewSlotManager.SellMode(active: false);
		}
	}

	public void OpenSellAsk()
	{
		ui_crewSellAskPopup.gameObject.SetActive(value: true);
		ui_crewSellAskPopup.SetList(sellSailorList, price);
	}

	public void Sell()
	{
		global::UnityEngine.Debug.Log("돈줘야함");
	}

	public bool AddSellList(MySailorInventory data)
	{
		if (!sellSailorList.Contains(data))
		{
			sellSailorList.Add(data);
			Refresh();
			return true;
		}
		return false;
	}

	public bool RemoveSellList(MySailorInventory data)
	{
		if (sellSailorList.Contains(data))
		{
			sellSailorList.Remove(data);
			Refresh();
			return true;
		}
		return false;
	}

	private void Refresh()
	{
		totalSellPriceText.text = sellSailorList.Count + "명의 가격";
		price = 0L;
		foreach (MySailorInventory sellSailor in sellSailorList)
		{
			_ = sellSailor;
			price += 3546L;
		}
	}
}
