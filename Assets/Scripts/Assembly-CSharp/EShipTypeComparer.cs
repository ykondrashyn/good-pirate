public class EShipTypeComparer : global::System.Collections.Generic.IEqualityComparer<EShipType>
{
	public bool Equals(EShipType a, EShipType b)
	{
		return a == b;
	}

	public int GetHashCode(EShipType a)
	{
		return (int)a;
	}
}
