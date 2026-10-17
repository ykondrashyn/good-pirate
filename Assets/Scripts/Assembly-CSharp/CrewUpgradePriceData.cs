[global::System.Serializable]
public class CrewUpgradePriceData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private int rankc;

	[global::UnityEngine.SerializeField]
	private int rankb;

	[global::UnityEngine.SerializeField]
	private int ranka;

	[global::UnityEngine.SerializeField]
	private int ranks;

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

	public int Rankc
	{
		get
		{
			return rankc;
		}
		set
		{
			rankc = value;
		}
	}

	public int Rankb
	{
		get
		{
			return rankb;
		}
		set
		{
			rankb = value;
		}
	}

	public int Ranka
	{
		get
		{
			return ranka;
		}
		set
		{
			ranka = value;
		}
	}

	public int Ranks
	{
		get
		{
			return ranks;
		}
		set
		{
			ranks = value;
		}
	}
}
