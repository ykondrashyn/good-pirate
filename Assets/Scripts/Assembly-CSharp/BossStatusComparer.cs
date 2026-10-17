public class BossStatusComparer : global::System.Collections.Generic.IEqualityComparer<BossStatus>
{
	public bool Equals(BossStatus a, BossStatus b)
	{
		return a.bossKey == b.bossKey;
	}

	public int GetHashCode(BossStatus a)
	{
		return a.bossKey.GetHashCode();
	}
}
