[global::System.Serializable]
public class SubShipUpgradePriceData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private long gold;

	[global::UnityEngine.SerializeField]
	private long wood;

	[global::UnityEngine.SerializeField]
	private long iron;

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
		}
	}

	public long Gold
	{
		get
		{
			return gold;
		}
		set
		{
			gold = value;
		}
	}

	public long Wood
	{
		get
		{
			return wood;
		}
		set
		{
			wood = value;
		}
	}

	public long Iron
	{
		get
		{
			return iron;
		}
		set
		{
			iron = value;
		}
	}
}
