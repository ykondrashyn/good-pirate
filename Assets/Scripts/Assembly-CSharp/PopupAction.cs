[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.CanvasGroup))]
public class PopupAction : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform panelTrans;

	[global::UnityEngine.SerializeField]
	private bool isUnscaledTime;

	[global::UnityEngine.SerializeField]
	private float Duration = 0.2f;

	public global::UnityEngine.Events.UnityEvent onEnableEvent;

	public global::UnityEngine.Events.UnityEvent onDisableEvent;

	private global::UnityEngine.CanvasGroup myCanvasGroup;

	private void Awake()
	{
		myCanvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
	}

	private void OnEnable()
	{
		AudioManager.PlayButtonClick();
		if ((bool)panelTrans)
		{
			panelTrans.localScale = global::UnityEngine.Vector3.one * 0.4f;
			global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 1f, Duration), isUnscaledTime);
		}
		myCanvasGroup.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 1f, Duration), isUnscaledTime);
		onEnableEvent.Invoke();
	}

	public void Close()
	{
		AudioManager.PlayButtonClick();
		onDisableEvent.Invoke();
		if ((bool)panelTrans)
		{
			global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 0.4f, Duration), isUnscaledTime);
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 0f, Duration), isUnscaledTime), delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}
}
