public class UI_ItemSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.Header("money")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text amountText;

	[global::UnityEngine.Header("crew")]
	[global::UnityEngine.SerializeField]
	private UI_CrewSlot crewSlot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text getCountText;

	[global::UnityEngine.Header("crew")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image gradeBG;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color sRankColor = myColor.GetColor(42, 0, 101);

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color aRankColor = myColor.GetColor(255, 197, 0);

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color bRankColor = myColor.GetColor(154, 154, 154);

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color cRankColor = myColor.GetColor(195, 121, 62);

	[global::UnityEngine.Header("etc")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject contentObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image whiteImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform white2Rect;

	public global::UnityEngine.CanvasGroup canvas;

	public MySailorInventory crewData { get; private set; }

	private void OnEnable()
	{
		whiteImage.color = global::UnityEngine.Color.white;
		contentObj.SetActive(value: false);
	}

	public void SetData(EMonenyType monenyType, long amount)
	{
		iconImage.sprite = UI_Manager.Instance.GetMoneyIcon(monenyType);
		amountText.text = Numbering.Convert(amount);
	}

	public void SetData(ERewardBox box, int amount, int level)
	{
		iconImage.sprite = UI_Manager.Instance.ui_mainGame.BoxSprite(box);
		amountText.text = $"Lv.{level}\nx{amount}";
	}

	public void SetData(MySailorInventory sailorData, int getCount)
	{
		if (gradeBG != null)
		{
			switch (sailorData.baseData.grade)
			{
			case EGrade.cClass:
				gradeBG.color = cRankColor;
				break;
			case EGrade.bClass:
				gradeBG.color = bRankColor;
				break;
			case EGrade.aClass:
				gradeBG.color = aRankColor;
				break;
			case EGrade.sClass:
				gradeBG.color = sRankColor;
				break;
			}
		}
		else
		{
			crewData = sailorData;
			crewSlot.SetData(crewData, null);
			getCountText.text = "x" + getCount;
		}
	}

	public void SetCountMessage(string str)
	{
		getCountText.text = str;
	}

	public void FreeBoxOpen()
	{
		white2Rect.gameObject.SetActive(value: false);
		whiteImage.gameObject.SetActive(value: false);
		contentObj.SetActive(value: true);
		base.transform.localScale = global::UnityEngine.Vector3.one * 0.4f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, 1f, 0.3f), isIndependentUpdate: true);
	}

	public void Open()
	{
		white2Rect.gameObject.SetActive(value: true);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(white2Rect, 0f, 0.3f), isIndependentUpdate: true), delegate
		{
			white2Rect.gameObject.SetActive(value: false);
			contentObj.SetActive(value: true);
			global::UnityEngine.Color color = whiteImage.color;
			color.a = 0f;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOColor(whiteImage, color, 0.7f), isIndependentUpdate: true), 0.2f), delegate
			{
				whiteImage.gameObject.SetActive(value: false);
			});
		});
	}

	public void Skip()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(white2Rect);
		global::DG.Tweening.ShortcutExtensions.DOKill(whiteImage);
		whiteImage.gameObject.SetActive(value: false);
		white2Rect.gameObject.SetActive(value: false);
		contentObj.SetActive(value: true);
	}

	public void ShowCrewInfo()
	{
		UI_Manager.Instance.ui_mainGame.ui_crewInfoPopup.gameObject.SetActive(value: true);
		UI_Manager.Instance.ui_mainGame.ui_crewInfoPopup.SetData(crewData);
	}
}
