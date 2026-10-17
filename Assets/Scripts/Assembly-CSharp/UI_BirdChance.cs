public class UI_BirdChance : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup btnCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject birdchancePopupObj;

	private bool isbtnActive;

	private float totalTime = 435f;

	private float coolTime = 615f;

	private float prevTimescale;

	private void Awake()
	{
		totalTime = 415f;
	}

	private void Start()
	{
		if ((bool)PlayerInfo.Instance.myData.isHaveEagle && (bool)PlayerInfo.Instance.myData.isHavePirrot)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void LateUpdate()
	{
		totalTime += global::UnityEngine.Time.deltaTime;
		if (isbtnActive)
		{
			totalTime = 0f;
		}
		else if (!GameManager.Instance.IsInIsland && !isbtnActive && totalTime >= coolTime)
		{
			BtnActive();
		}
	}

	private void BtnActive()
	{
		isbtnActive = true;
		btnObj.SetActive(value: true);
		btnObj.transform.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(btnObj.transform, 1f, 0.4f), global::DG.Tweening.Ease.OutBack);
		btnCanvas.alpha = 0f;
		global::DG.Tweening.DOTweenModuleUI.DOFade(btnCanvas, 1f, 0.2f);
	}

	public void OpenPopup()
	{
		prevTimescale = global::UnityEngine.Time.timeScale;
		birdchancePopupObj.SetActive(value: true);
		global::UnityEngine.Time.timeScale = 0f;
		btnObj.SetActive(value: false);
	}

	public void ClosePopup()
	{
		birdchancePopupObj.GetComponent<PopupAction>().Close();
		global::UnityEngine.Time.timeScale = prevTimescale;
		isbtnActive = false;
	}

	public void ShowAds()
	{
		if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
		{
			Success();
			return;
		}
		MoPubAdManager.Instance.ShowRewardedVideo(Success);
		GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "새 체험", 1L);
	}

	private void Success()
	{
		GameManager.Instance.playerShipController.GenerateEagle();
		GameManager.Instance.playerShipController.GeneratePirrot();
		GameManager.Instance.playerShipController.StartCoroutine("BirdChanceCheck");
		ClosePopup();
	}
}
