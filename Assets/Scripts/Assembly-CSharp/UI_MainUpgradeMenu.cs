public class UI_MainUpgradeMenu : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform downMenuRectTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] tabImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] tabButtons;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform openButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform closeButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Events.UnityEvent openEvnets;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Events.UnityEvent closeEvnets;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] disableObjects;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] panelCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform hpBarRectTransform;

	private global::UnityEngine.Color tabActiveColor = myColor.GetColor(197, 214, 226);

	private global::UnityEngine.Color tabInActiveColor = myColor.GetColor(166, 190, 204);

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject newObj;

	private bool isOpened;

	private int selectedNum = -1;

	private void Start()
	{
		OrientationCheck.Instance.AddLandscapeEvent(SetLandscape);
		OrientationCheck.Instance.AddPortraitEvent(SetPortrait);
		global::UnityEngine.GameObject[] array = disableObjects;
		foreach (global::UnityEngine.GameObject gameObject in array)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	private void OnEnable()
	{
	}

	private void SetPortrait()
	{
		downMenuRectTrans.anchorMin = new global::UnityEngine.Vector2(0.5f, 0f);
		downMenuRectTrans.anchorMax = new global::UnityEngine.Vector2(0.5f, 0f);
		downMenuRectTrans.pivot = new global::UnityEngine.Vector2(0.5f, 1f);
		if (isOpened)
		{
			downMenuRectTrans.anchoredPosition = new global::UnityEngine.Vector2(0f, 1242f);
		}
		else
		{
			downMenuRectTrans.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f);
		}
		downMenuRectTrans.sizeDelta = new global::UnityEngine.Vector2(1242f, 986f);
		openButton.anchorMin = new global::UnityEngine.Vector2(0.5f, 1f);
		openButton.anchorMax = new global::UnityEngine.Vector2(0.5f, 1f);
		openButton.localRotation = global::UnityEngine.Quaternion.identity;
		openButton.anchoredPosition = global::UnityEngine.Vector2.zero;
		closeButton.anchorMin = new global::UnityEngine.Vector2(0.5f, 1f);
		closeButton.anchorMax = new global::UnityEngine.Vector2(0.5f, 1f);
		closeButton.localRotation = global::UnityEngine.Quaternion.identity;
		closeButton.anchoredPosition = global::UnityEngine.Vector2.zero;
		hpBarRectTransform.anchorMin = new global::UnityEngine.Vector2(0.5f, 1f);
		hpBarRectTransform.anchorMax = new global::UnityEngine.Vector2(0.5f, 1f);
		hpBarRectTransform.pivot = new global::UnityEngine.Vector2(0.5f, 0f);
		hpBarRectTransform.localRotation = global::UnityEngine.Quaternion.identity;
		hpBarRectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f);
	}

	private void SetLandscape()
	{
		downMenuRectTrans.anchorMin = new global::UnityEngine.Vector2(1f, 0.5f);
		downMenuRectTrans.anchorMax = new global::UnityEngine.Vector2(1f, 0.5f);
		downMenuRectTrans.pivot = new global::UnityEngine.Vector2(0f, 0.5f);
		downMenuRectTrans.sizeDelta = new global::UnityEngine.Vector2(1242f, 1242f);
		if (isOpened)
		{
			downMenuRectTrans.anchoredPosition = new global::UnityEngine.Vector2(-1242f, 0f);
		}
		else
		{
			downMenuRectTrans.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f);
		}
		openButton.anchorMin = new global::UnityEngine.Vector2(0f, 0.5f);
		openButton.anchorMax = new global::UnityEngine.Vector2(0f, 0.5f);
		openButton.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 90f);
		openButton.anchoredPosition = global::UnityEngine.Vector2.zero;
		closeButton.anchorMin = new global::UnityEngine.Vector2(0f, 0.5f);
		closeButton.anchorMax = new global::UnityEngine.Vector2(0f, 0.5f);
		closeButton.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 90f);
		closeButton.anchoredPosition = global::UnityEngine.Vector2.zero;
		hpBarRectTransform.anchorMin = new global::UnityEngine.Vector2(0f, 0.5f);
		hpBarRectTransform.anchorMax = new global::UnityEngine.Vector2(0f, 0.5f);
		hpBarRectTransform.pivot = new global::UnityEngine.Vector2(0.5f, 0.5f);
		hpBarRectTransform.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 90f);
		hpBarRectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f);
	}

	public void SetPanelActive(bool active)
	{
		global::UnityEngine.GameObject[] array = disableObjects;
		foreach (global::UnityEngine.GameObject gameObject in array)
		{
			if (gameObject.activeSelf == !active)
			{
				gameObject.SetActive(active);
			}
		}
	}

	public void JustCloseMove()
	{
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(downMenuRectTrans, 0f, 0.3f), delegate
		{
			SetPanelActive(active: false);
			if (newObj.activeSelf)
			{
				newObj.SetActive(value: false);
			}
		});
	}

	public void JustOpenMove()
	{
		global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(downMenuRectTrans, downMenuRectTrans.sizeDelta.y, 0.3f);
		if (newObj.activeSelf)
		{
			newObj.SetActive(value: false);
		}
	}

	public void Open()
	{
		if (!isOpened)
		{
			AudioManager.PlayButtonClick();
			isOpened = true;
			closeButton.gameObject.SetActive(value: true);
			openButton.gameObject.SetActive(value: false);
			SetPanelActive(active: true);
			global::DG.Tweening.ShortcutExtensions.DOKill(downMenuRectTrans);
			global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(downMenuRectTrans, downMenuRectTrans.sizeDelta.y, 0.3f), isIndependentUpdate: true);
			openEvnets.Invoke();
			if (newObj.activeSelf)
			{
				newObj.SetActive(value: false);
			}
		}
	}

	public void Close()
	{
		if (isOpened)
		{
			isOpened = false;
			AudioManager.PlayButtonClick();
			closeButton.gameObject.SetActive(value: false);
			openButton.gameObject.SetActive(value: true);
			global::DG.Tweening.ShortcutExtensions.DOKill(downMenuRectTrans);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(downMenuRectTrans, 0f, 0.3f), isIndependentUpdate: true), delegate
			{
				SetPanelActive(active: false);
			});
			closeEvnets.Invoke();
			if (newObj.activeSelf)
			{
				newObj.SetActive(value: false);
			}
		}
	}

	public void NewOpen()
	{
		newObj.SetActive(value: true);
	}
}
