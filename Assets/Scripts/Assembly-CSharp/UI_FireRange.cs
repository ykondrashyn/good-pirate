public class UI_FireRange : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] allCircles;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] allCircleRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] cooltimeRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform leftCircleRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasGroup;

	private float range;

	private global::UnityEngine.Transform targetTrans;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void Start()
	{
		if (UI_Manager.Instance != null)
		{
			UI_MainGame ui_mainGame = UI_Manager.Instance.ui_mainGame;
			ui_mainGame.uiToggleDelegate = (UI_MainGame.showUIToggleDelegate)global::System.Delegate.Combine(ui_mainGame.uiToggleDelegate, new UI_MainGame.showUIToggleDelegate(SetActive));
		}
		if (!UI_Manager.Instance.ui_mainGame.IsShowGuideUIToggle)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (UI_Manager.Instance != null)
		{
			UI_MainGame ui_mainGame = UI_Manager.Instance.ui_mainGame;
			ui_mainGame.uiToggleDelegate = (UI_MainGame.showUIToggleDelegate)global::System.Delegate.Remove(ui_mainGame.uiToggleDelegate, new UI_MainGame.showUIToggleDelegate(SetActive));
		}
	}

	public void SetTargetTransform(global::UnityEngine.Transform trans)
	{
		targetTrans = trans;
	}

	private void LateUpdate()
	{
		if (!(targetTrans == null))
		{
			global::UnityEngine.Vector3 position = targetTrans.position;
			position.y = 0.15f;
			transform.position = position;
			global::UnityEngine.Vector3 forward = targetTrans.forward;
			forward.y = 0f;
			transform.forward = forward;
		}
	}

	public void SetRange(float range)
	{
		this.range = range * 2f;
		global::UnityEngine.RectTransform[] array = allCircleRect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sizeDelta = new global::UnityEngine.Vector2(this.range, this.range);
		}
	}

	public void SetAngle(float angle)
	{
		float fillAmount = angle / 360f;
		global::UnityEngine.UI.Image[] array = allCircles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].fillAmount = fillAmount;
		}
		float z = angle * 0.5f;
		leftCircleRect.localRotation = global::UnityEngine.Quaternion.Euler(90f, 0f, z);
	}

	public void SetCoolTime(float ratio)
	{
		float num = range * ratio;
		global::UnityEngine.RectTransform[] array = cooltimeRect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sizeDelta = new global::UnityEngine.Vector2(num, num);
		}
	}

	public void SetActive(bool active)
	{
		if (active)
		{
			canvasGroup.gameObject.SetActive(value: true);
			global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
			global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.6f);
		}
		else
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.6f), delegate
			{
				canvasGroup.gameObject.SetActive(value: false);
			});
		}
	}
}
