public class MoPubBinding
{
	public MoPubBase.Reward SelectedReward;

	private readonly string _adUnitId;

	public bool IsInterstitialReady => false;

	public MoPubBinding(string adUnitId)
	{
		_adUnitId = adUnitId;
		SelectedReward = new MoPubBase.Reward
		{
			Label = string.Empty
		};
	}

	public void CreateBanner(MoPubBase.BannerType bannerType, MoPubBase.AdPosition position)
	{
		NoOp("create-banner");
	}

	public void DestroyBanner()
	{
		NoOp("destroy-banner");
	}

	public void ShowBanner(bool shouldShow)
	{
		NoOp("show-banner");
	}

	public void RefreshBanner(string keywords = "", string userDataKeywords = "")
	{
		NoOp("refresh-banner");
	}

	public void SetAutorefresh(bool enabled)
	{
		NoOp("set-banner-autorefresh");
	}

	public void ForceRefresh()
	{
		NoOp("force-banner-refresh");
	}

	public void RequestInterstitialAd(string keywords = "", string userDataKeywords = "")
	{
		NoOp("request-interstitial");
	}

	public void ShowInterstitialAd()
	{
		NoOp("show-interstitial");
	}

	public void DestroyInterstitialAd()
	{
		NoOp("destroy-interstitial");
	}

	public void RequestRewardedVideo(global::System.Collections.Generic.List<MoPubBase.LocalMediationSetting> mediationSettings = null, string keywords = null, string userDataKeywords = null, double latitude = 99999.0, double longitude = 99999.0, string customerId = null)
	{
		NoOp("request-rewarded-video");
	}

	public bool HasRewardedVideo()
	{
		return false;
	}

	public global::System.Collections.Generic.List<MoPubBase.Reward> GetAvailableRewards()
	{
		return new global::System.Collections.Generic.List<MoPubBase.Reward>();
	}

	public void ShowRewardedVideo(string customData)
	{
		NoOp("show-rewarded-video");
	}

	private void NoOp(string operation)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("ads", "mopub-binding-" + operation + "-" + _adUnitId);
	}
}
