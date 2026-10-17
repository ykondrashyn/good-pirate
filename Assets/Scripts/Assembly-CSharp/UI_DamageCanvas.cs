public class UI_DamageCanvas : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Text myText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasGroup;

	private void Awake()
	{
		myText = GetComponentInChildren<global::UnityEngine.UI.Text>();
		canvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = base.gameObject.AddComponent<global::UnityEngine.CanvasGroup>();
		}
	}

	private void Start()
	{
		if (UI_Manager.Instance != null)
		{
			UI_MainGame ui_mainGame = UI_Manager.Instance.ui_mainGame;
			ui_mainGame.uiToggleDelegate = (UI_MainGame.showUIToggleDelegate)global::System.Delegate.Combine(ui_mainGame.uiToggleDelegate, new UI_MainGame.showUIToggleDelegate(SetActive));
		}
		canvasGroup.gameObject.SetActive(UI_Manager.Instance.ui_mainGame.IsShowGuideUIToggle);
	}

	private void OnDestroy()
	{
		if (UI_Manager.Instance != null)
		{
			UI_MainGame ui_mainGame = UI_Manager.Instance.ui_mainGame;
			ui_mainGame.uiToggleDelegate = (UI_MainGame.showUIToggleDelegate)global::System.Delegate.Remove(ui_mainGame.uiToggleDelegate, new UI_MainGame.showUIToggleDelegate(SetActive));
		}
	}

	public void SetDamage(int damage)
	{
		myText.text = damage.ToString();
	}

	public void SetActive(bool active)
	{
		if (active)
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
			global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.6f);
			return;
		}
		global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.6f), delegate
		{
			canvasGroup.gameObject.SetActive(value: false);
		});
	}
}
