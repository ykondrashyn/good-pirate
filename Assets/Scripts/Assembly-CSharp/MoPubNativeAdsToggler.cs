[global::UnityEngine.ExecuteInEditMode]
public class MoPubNativeAdsToggler : global::UnityEngine.MonoBehaviour
{
	private void Awake()
	{
		if (global::UnityEngine.Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		base.gameObject.hideFlags = global::UnityEngine.HideFlags.HideInHierarchy | global::UnityEngine.HideFlags.HideInInspector;
		foreach (global::UnityEngine.Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}
}
