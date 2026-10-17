public class HpBarManager : global::UnityEngine.MonoBehaviour
{
	public static HpBarManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform pivot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject barBigSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject barSmallSrc;

	private global::UnityEngine.CanvasGroup myCanvasGroup;

	private void Awake()
	{
		Instance = this;
		myCanvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
	}

	public HpBar Generate(ELayerType type, bool isBig = false)
	{
		global::UnityEngine.GameObject gameObject = null;
		gameObject = ((!isBig) ? barSmallSrc : barBigSrc);
		HpBar component = global::UnityEngine.Object.Instantiate(gameObject, pivot).GetComponent<HpBar>();
		component.SetType(type);
		return component;
	}

	public void SetActive(bool active)
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(myCanvasGroup);
		if (active)
		{
			pivot.gameObject.SetActive(value: true);
			myCanvasGroup.alpha = 0f;
			global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 1f, 0.6f);
		}
		else
		{
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 0f, 0.6f), delegate
			{
				pivot.gameObject.SetActive(value: false);
			});
		}
	}
}
