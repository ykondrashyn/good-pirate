public class FishingIcon : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.RectTransform rectTransform;

	private global::UnityEngine.Camera mainCamera;

	private global::UnityEngine.Vector2 sizeDelta;

	private global::UnityEngine.CanvasGroup canvasGroup;

	[global::UnityEngine.HideInInspector]
	public bool isActive = true;

	private void Awake()
	{
		rectTransform = GetComponent<global::UnityEngine.RectTransform>();
		canvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
	}

	private void Start()
	{
		mainCamera = global::UnityEngine.Camera.main;
		sizeDelta = GetComponentInParent<global::UnityEngine.Canvas>().GetComponent<global::UnityEngine.RectTransform>().sizeDelta;
		isActive = true;
		SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		if (isActive == active)
		{
			return;
		}
		isActive = active;
		if (isActive)
		{
			base.gameObject.SetActive(value: true);
			global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
			canvasGroup.alpha = 0f;
			global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.4f);
		}
		else
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.4f), delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}

	public void SetPosition(global::UnityEngine.Vector3 pos)
	{
		if (mainCamera == null)
		{
			mainCamera = global::UnityEngine.Camera.main;
		}
		else if (mainCamera.gameObject.activeSelf)
		{
			global::UnityEngine.Vector3 vector = mainCamera.WorldToViewportPoint(pos);
			global::UnityEngine.Vector2 anchoredPosition = new global::UnityEngine.Vector2(vector.x * sizeDelta.x, vector.y * sizeDelta.y);
			rectTransform.anchoredPosition = anchoredPosition;
		}
	}
}
