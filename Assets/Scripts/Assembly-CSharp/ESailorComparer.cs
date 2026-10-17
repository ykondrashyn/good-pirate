public class ESailorComparer : global::System.Collections.Generic.IEqualityComparer<ESailor>
{
	public bool Equals(ESailor a, ESailor b)
	{
		return a == b;
	}

	public int GetHashCode(ESailor a)
	{
		return (int)a;
	}
}
