public class MoPubAndroidRewardedVideo
{
	private readonly global::UnityEngine.AndroidJavaObject _plugin;

	private readonly global::System.Collections.Generic.Dictionary<MoPubBase.Reward, global::UnityEngine.AndroidJavaObject> _rewardsDict = new global::System.Collections.Generic.Dictionary<MoPubBase.Reward, global::UnityEngine.AndroidJavaObject>();

	public MoPubAndroidRewardedVideo(string adUnitId)
	{
		_plugin = new global::UnityEngine.AndroidJavaObject("com.mopub.unity.MoPubRewardedVideoUnityPlugin", adUnitId);
	}

	public void RequestRewardedVideo(global::System.Collections.Generic.List<MoPubBase.LocalMediationSetting> mediationSettings = null, string keywords = null, string userDataKeywords = null, double latitude = 99999.0, double longitude = 99999.0, string customerId = null)
	{
		string text = MoPubBase.LocalMediationSetting.ToJson(mediationSettings);
		_plugin.Call("requestRewardedVideo", text, keywords, userDataKeywords, latitude, longitude, customerId);
	}

	public void ShowRewardedVideo(string customData)
	{
		_plugin.Call("showRewardedVideo", customData);
	}

	public bool HasRewardedVideo()
	{
		return _plugin.Call<bool>("hasRewardedVideo", global::System.Array.Empty<object>());
	}

	public global::System.Collections.Generic.List<MoPubBase.Reward> GetAvailableRewards()
	{
		_rewardsDict.Clear();
		using (global::UnityEngine.AndroidJavaObject androidJavaObject = _plugin.Call<global::UnityEngine.AndroidJavaObject>("getAvailableRewards", global::System.Array.Empty<object>()))
		{
			global::UnityEngine.AndroidJavaObject[] array = global::UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<global::UnityEngine.AndroidJavaObject[]>(androidJavaObject.GetRawObject());
			if (array.Length <= 1)
			{
				return new global::System.Collections.Generic.List<MoPubBase.Reward>(_rewardsDict.Keys);
			}
			global::UnityEngine.AndroidJavaObject[] array2 = array;
			foreach (global::UnityEngine.AndroidJavaObject androidJavaObject2 in array2)
			{
				_rewardsDict.Add(new MoPubBase.Reward
				{
					Label = androidJavaObject2.Call<string>("getLabel", global::System.Array.Empty<object>()),
					Amount = androidJavaObject2.Call<int>("getAmount", global::System.Array.Empty<object>())
				}, androidJavaObject2);
			}
		}
		return new global::System.Collections.Generic.List<MoPubBase.Reward>(_rewardsDict.Keys);
	}

	public void SelectReward(MoPubBase.Reward selectedReward)
	{
		if (_rewardsDict.TryGetValue(selectedReward, out var value))
		{
			_plugin.Call("selectReward", value);
		}
		else
		{
			global::UnityEngine.Debug.LogWarning($"Selected reward {selectedReward} is not available.");
		}
	}
}
