public class EmojiCanvas : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform backGroundTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image emojiImage;

	private void OnEnable()
	{
		backGroundTrans.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.ShortcutExtensions.DOScale(backGroundTrans, 1f, 0.25f);
		emojiImage.transform.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOScale(emojiImage.transform, 1f, 0.4f), 0.32f), global::DG.Tweening.Ease.OutBack);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOScale(backGroundTrans, 0f, 0.3f), 2.8f), delegate
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	public void SetEmoji(global::UnityEngine.Sprite emoji)
	{
		emojiImage.sprite = emoji;
	}
}
