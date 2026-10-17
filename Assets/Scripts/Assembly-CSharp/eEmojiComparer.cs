public class eEmojiComparer : global::System.Collections.Generic.IEqualityComparer<eEmoji>
{
	public bool Equals(eEmoji a, eEmoji b)
	{
		return a == b;
	}

	public int GetHashCode(eEmoji a)
	{
		return (int)a;
	}
}
