public class UI_EmojiSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject emojiSlotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform contentTrans;

	private global::System.Collections.Generic.List<UI_EmojiSlot> emojiList = new global::System.Collections.Generic.List<UI_EmojiSlot>();

	private global::UnityEngine.CanvasGroup canvasGroup;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		canvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
		for (int num = 9; num >= 0; num--)
		{
			UI_EmojiSlot component = global::UnityEngine.Object.Instantiate(emojiSlotSrc, contentTrans).GetComponent<UI_EmojiSlot>();
			component.SetEmoji((eEmoji)num, EmojiManager.Instance.GetEmojiSprite((eEmoji)num));
			emojiList.Add(component);
		}
	}

	private void OnEnable()
	{
		canvasGroup.alpha = 0f;
		global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
		global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.2f);
		transform.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.ShortcutExtensions.DOKill(transform);
		global::DG.Tweening.ShortcutExtensions.DOScale(transform, 1f, 0.2f);
	}

	public void Close()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
		global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.2f);
		global::DG.Tweening.ShortcutExtensions.DOKill(transform);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(transform, 0.6f, 0.2f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void ActiveCooltime()
	{
		foreach (UI_EmojiSlot emoji in emojiList)
		{
			emoji.ActiveCoolTime();
		}
	}
}
