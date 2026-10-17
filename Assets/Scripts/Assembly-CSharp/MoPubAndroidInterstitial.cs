public class MoPubAndroidInterstitial
{
	private readonly global::UnityEngine.AndroidJavaObject _interstitialPlugin;

	public bool IsInterstitialReady => _interstitialPlugin.Call<bool>("isReady", global::System.Array.Empty<object>());

	public MoPubAndroidInterstitial(string adUnitId)
	{
		_interstitialPlugin = new global::UnityEngine.AndroidJavaObject("com.mopub.unity.MoPubInterstitialUnityPlugin", adUnitId);
	}

	public void RequestInterstitialAd(string keywords = "", string userDataKeywords = "")
	{
		_interstitialPlugin.Call("request", keywords, userDataKeywords);
	}

	public void ShowInterstitialAd()
	{
		_interstitialPlugin.Call("show");
	}

	public void DestroyInterstitialAd()
	{
		_interstitialPlugin.Call("destroy");
	}
}
