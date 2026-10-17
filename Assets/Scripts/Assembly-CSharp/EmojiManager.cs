public class EmojiManager : global::UnityEngine.MonoBehaviour
{
	public static EmojiManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject emojiSrc;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<EmojiData> emojiDatas = new global::System.Collections.Generic.List<EmojiData>();

	private global::System.Collections.Generic.Dictionary<eEmoji, EmojiData> emojiDic = new global::System.Collections.Generic.Dictionary<eEmoji, EmojiData>(new eEmojiComparer());

	private void Awake()
	{
		Instance = this;
		foreach (EmojiData emojiData in emojiDatas)
		{
			if (!emojiDic.ContainsKey(emojiData.emojiType))
			{
				emojiDic.Add(emojiData.emojiType, emojiData);
			}
		}
	}

	public global::UnityEngine.Sprite GetEmojiSprite(eEmoji emojiType)
	{
		if (emojiDic.ContainsKey(emojiType))
		{
			return emojiDic[emojiType].emojiSprite;
		}
		return null;
	}

	public void MakeEmoji(eEmoji emojiType, global::UnityEngine.Transform trans)
	{
		global::UnityEngine.Object.Instantiate(emojiSrc, trans.position, global::UnityEngine.Quaternion.identity, trans).GetComponent<EmojiCanvas>().SetEmoji(GetEmojiSprite(emojiType));
	}
}
