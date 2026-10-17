public class UI_LanguageChange : global::UnityEngine.MonoBehaviour
{
	private bool isActive;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup scrollCanvas;

	[global::UnityEngine.SerializeField]
	private UI_LayoutHorizontal verticalUI;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform contentPivot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject listSrc;

	[global::UnityEngine.SerializeField]
	private UI_LanguageSlot nowSelectedLanguage;

	private void Awake()
	{
		LanguageManager.Fonts[] fonts = LanguageManager.Instance.fonts;
		for (int i = 0; i < fonts.Length; i++)
		{
			LanguageManager.Fonts fonts2 = fonts[i];
			global::UnityEngine.Object.Instantiate(listSrc, verticalUI.transform).GetComponent<UI_LanguageSlot>().SetSystemLanguage(fonts2.lang);
		}
		scrollCanvas.alpha = 0f;
		verticalUI.Refresh();
		nowSelectedLanguage.SetSystemLanguage(LanguageManager.Instance.language);
		scrollCanvas.gameObject.SetActive(value: false);
	}

	public void Toggle()
	{
		isActive = !isActive;
		if (isActive)
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(scrollCanvas);
			scrollCanvas.gameObject.SetActive(value: true);
			global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(scrollCanvas, 1f, 0.25f), isIndependentUpdate: true);
		}
		else
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(scrollCanvas);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(scrollCanvas, 0f, 0.25f), isIndependentUpdate: true), delegate
			{
				scrollCanvas.gameObject.SetActive(value: false);
			});
		}
	}
}
