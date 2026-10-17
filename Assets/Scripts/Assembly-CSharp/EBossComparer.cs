public class EBossComparer : global::System.Collections.Generic.IEqualityComparer<EBoss>
{
	public bool Equals(EBoss a, EBoss b)
	{
		return a == b;
	}

	public int GetHashCode(EBoss a)
	{
		return (int)a;
	}
}
