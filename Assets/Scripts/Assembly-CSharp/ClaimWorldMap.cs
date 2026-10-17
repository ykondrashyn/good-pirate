public class ClaimWorldMap : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform panelRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] closeBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup missionCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup minimapImageCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text missionContentText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	private global::UnityEngine.Vector2 size = global::UnityEngine.Vector2.zero;

	private void Awake()
	{
		size = panelRect.sizeDelta;
	}

	public void SettingShowMission()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		panelRect.sizeDelta = size;
		minimapImageCanvas.alpha = 0f;
		missionCanvas.gameObject.SetActive(value: true);
		missionCanvas.alpha = 1f;
		missionContentText.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, -71f);
		leftTimeText.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(12f, 30f);
		global::UnityEngine.UI.Button[] array = closeBtn;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	public void SettingMinimapMode()
	{
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(missionCanvas, 0f, 0.2f), delegate
		{
			missionCanvas.gameObject.SetActive(value: false);
		});
		global::DG.Tweening.DOTweenModuleUI.DOFade(minimapImageCanvas, 1f, 1f);
		global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(missionContentText.rectTransform, 526f, 1f);
		global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(leftTimeText.rectTransform, -526f, 1f);
		global::UnityEngine.UI.Button[] array = closeBtn;
		for (int num = 0; num < array.Length; num++)
		{
			array[num].enabled = false;
		}
	}

	public void SetMissionTime(string str)
	{
		leftTimeText.text = str;
	}

	public void SetMissionContent(string str)
	{
		missionContentText.text = str;
	}

	public void Toggle()
	{
		if (!base.gameObject.activeSelf)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void Open()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (size == global::UnityEngine.Vector2.zero)
		{
			size = panelRect.sizeDelta;
		}
		global::UnityEngine.Vector2 sizeDelta = size;
		sizeDelta.y = 0f;
		panelRect.sizeDelta = sizeDelta;
		global::DG.Tweening.ShortcutExtensions.DOKill(panelRect);
		global::DG.Tweening.DOTweenModuleUI.DOSizeDelta(panelRect, size, 0.4f);
		global::UnityEngine.UI.Button[] array = closeBtn;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
	}

	public void Close()
	{
		global::UnityEngine.Vector2 sizeDelta = panelRect.sizeDelta;
		sizeDelta.y = 0f;
		global::DG.Tweening.ShortcutExtensions.DOKill(panelRect);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOSizeDelta(panelRect, sizeDelta, 0.4f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
		global::UnityEngine.UI.Button[] array = closeBtn;
		for (int num = 0; num < array.Length; num++)
		{
			array[num].enabled = true;
		}
	}
}
