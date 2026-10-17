public class UI_KrakenInk : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Image[] myInks;

	private void Awake()
	{
		myInks = GetComponentsInChildren<global::UnityEngine.UI.Image>();
		global::UnityEngine.Debug.Log(myInks.Length);
	}

	private void OnEnable()
	{
		global::UnityEngine.UI.Image[] array = myInks;
		foreach (global::UnityEngine.UI.Image image in array)
		{
			global::UnityEngine.UI.Image nowInk = image;
			nowInk.transform.localScale = global::UnityEngine.Vector3.one * 0.3f;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(nowInk.transform, 1f, 1.5f), global::DG.Tweening.Ease.OutExpo), delegate
			{
				global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOColor(nowInk, new global::UnityEngine.Color(1f, 1f, 1f, 0f), 1f), 3f);
			});
			nowInk.color = new global::UnityEngine.Color(1f, 1f, 1f, 0f);
			global::DG.Tweening.DOTweenModuleUI.DOColor(nowInk, new global::UnityEngine.Color(1f, 1f, 1f, 0.741f), 0.3f);
		}
		Invoke("SelfInActive", 5.8f);
	}

	private void SelfInActive()
	{
		base.gameObject.SetActive(value: false);
	}
}
