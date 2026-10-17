public class UI_DeadAskPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private float askTime = 5f;

	[global::UnityEngine.Header("etc..")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image topBar;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] btns;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject haveObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject notHaveObj;

	private void OnEnable()
	{
		global::UnityEngine.UI.Button[] array = btns;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		StartCoroutine("LimitTimeRutine");
		bool flag = (int)PlayerInfo.Instance.myData.resurrectionTicket > 0;
		haveObj.SetActive(flag);
		notHaveObj.SetActive(!flag);
		global::UnityEngine.Time.timeScale = 0f;
	}

	public void JustClose()
	{
		global::UnityEngine.UI.Button[] array = btns;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		GameManager.Instance.DieSure();
		StopCoroutine("LimitTimeRutine");
		MoPubAdManager.Instance.ShowInterstitialAd();
		global::UnityEngine.Time.timeScale = 1f;
	}

	public void ShowAds()
	{
		StopCoroutine("LimitTimeRutine");
		if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
		{
			AdsSuccess();
			return;
		}
		MoPubAdManager.Instance.ShowRewardedVideo(AdsSuccess);
		GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "부활", 1L);
	}

	private void AdsSuccess()
	{
		GameManager.Instance.DieShowAds(delegate
		{
			global::UnityEngine.Time.timeScale = 1f;
			GetComponent<PopupAction>().Close();
		});
	}

	public void UseResurrection()
	{
		StopCoroutine("LimitTimeRutine");
		if ((int)PlayerInfo.Instance.myData.resurrectionTicket > 0)
		{
			--PlayerInfo.Instance.myData.resurrectionTicket;
			GameManager.Instance.DieUseResurrection();
			GetComponent<PopupAction>().Close();
		}
		else if ((long)PlayerInfo.Instance.myData.diamond >= 10)
		{
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -10L);
			GameManager.Instance.DieUseResurrection();
			GetComponent<PopupAction>().Close();
		}
		else
		{
			global::UnityEngine.Debug.Log("스토어 오픈");
		}
	}

	private global::System.Collections.IEnumerator LimitTimeRutine()
	{
		float totalTime = 0f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.unscaledDeltaTime;
			float num = global::UnityEngine.Mathf.Clamp01(totalTime / askTime);
			topBar.fillAmount = 1f - num;
			if (num >= 1f)
			{
				break;
			}
			yield return null;
		}
		GetComponent<PopupAction>().Close();
		JustClose();
	}
}
