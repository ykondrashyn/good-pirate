public class EShipSubComparer : global::System.Collections.Generic.IEqualityComparer<EShipSub>
{
	public bool Equals(EShipSub a, EShipSub b)
	{
		return a == b;
	}

	public int GetHashCode(EShipSub a)
	{
		return (int)a;
	}
}
