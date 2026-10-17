public class PatrolStatusComparer : global::System.Collections.Generic.IEqualityComparer<PatrolStatus>
{
	public bool Equals(PatrolStatus a, PatrolStatus b)
	{
		return a.patrolKey == b.patrolKey;
	}

	public int GetHashCode(PatrolStatus a)
	{
		return a.patrolKey.GetHashCode();
	}
}
