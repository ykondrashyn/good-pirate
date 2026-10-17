public class HealSpeech : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text diaCountText;

	private global::UnityEngine.CanvasGroup myCanvasGruop;

	private int diamondPrice = 5;

	private int max = 20;

	private int min = 1;

	private global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();

	private void Awake()
	{
		myCanvasGruop = GetComponent<global::UnityEngine.CanvasGroup>();
	}

	private void OnEnable()
	{
		myCanvasGruop.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGruop, 1f, 0.4f), delegate
		{
			TutorialManager.Instance.StartTutorial("heal");
		});
	}

	public void Inactive()
	{
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGruop, 0f, 0.3f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void Heal()
	{
		if ((long)PlayerInfo.Instance.myData.diamond > diamondPrice)
		{
			AudioManager.Instance.PlaySFX("use_diamond");
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -diamondPrice);
			ShowAdsSuccess();
			GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "체력 회복", "체력 회복", 1L);
		}
		else
		{
			global::UnityEngine.Debug.Log("다이아 부족 스토어 오픈");
		}
	}

	public void ShowAdsHeal()
	{
		if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
		{
			ShowAdsSuccess();
			return;
		}
		MoPubAdManager.Instance.ShowRewardedVideo(ShowAdsSuccess);
		GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "체력 회복", 1L);
	}

	private void ShowAdsSuccess()
	{
		GameManager.Instance.playerShipController.FullHealth();
		base.gameObject.SetActive(value: false);
	}

	public void SetLeftTime(int totalSec)
	{
		sb.Length = 0;
		if (totalSec >= 60)
		{
			sb.Append(totalSec / 60);
			sb.Append("min Left...");
		}
		else
		{
			sb.Append(totalSec);
			sb.Append("sec Left...");
		}
		float num = global::UnityEngine.Mathf.InverseLerp(270f, 0f, totalSec);
		diamondPrice = (int)global::UnityEngine.Mathf.Lerp(min, max, 1f - num);
		diaCountText.text = diamondPrice.ToString();
		leftTimeText.text = sb.ToString();
	}
}
