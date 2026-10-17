public class UI_EmojiToggle : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private UI_EmojiSlotManager slotManager;

	public void Toggle()
	{
		if (slotManager.gameObject.activeSelf)
		{
			slotManager.Close();
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
				base.transform.localScale = global::UnityEngine.Vector3.zero;
				global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, 1f, 0.2f);
			}
		}
		else
		{
			slotManager.gameObject.SetActive(value: true);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, 0f, 0.2f), delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}
}
