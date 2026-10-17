public class EEnvironmentComparer : global::System.Collections.Generic.IEqualityComparer<EEnvironment>
{
	public bool Equals(EEnvironment a, EEnvironment b)
	{
		return a == b;
	}

	public int GetHashCode(EEnvironment a)
	{
		return (int)a;
	}
}
