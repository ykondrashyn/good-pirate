public class eShipSkinComparer : global::System.Collections.Generic.IEqualityComparer<eShipSkin>
{
	public bool Equals(eShipSkin a, eShipSkin b)
	{
		return a == b;
	}

	public int GetHashCode(eShipSkin a)
	{
		return (int)a;
	}
}
