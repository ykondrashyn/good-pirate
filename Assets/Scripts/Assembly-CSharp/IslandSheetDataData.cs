[global::System.Serializable]
public class IslandSheetDataData
{
	[global::UnityEngine.SerializeField]
	private int id;

	[global::UnityEngine.SerializeField]
	private long gold;

	[global::UnityEngine.SerializeField]
	private int enemylevelmin;

	[global::UnityEngine.SerializeField]
	private int enemylevelmax;

	[global::UnityEngine.SerializeField]
	private int diamond;

	[global::UnityEngine.SerializeField]
	private int skillcrewid;

	[global::UnityEngine.SerializeField]
	private int crewgrade;

	[global::UnityEngine.SerializeField]
	private int crewcount;

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
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

	public int Enemylevelmin
	{
		get
		{
			return enemylevelmin;
		}
		set
		{
			enemylevelmin = value;
		}
	}

	public int Enemylevelmax
	{
		get
		{
			return enemylevelmax;
		}
		set
		{
			enemylevelmax = value;
		}
	}

	public int Diamond
	{
		get
		{
			return diamond;
		}
		set
		{
			diamond = value;
		}
	}

	public int Skillcrewid
	{
		get
		{
			return skillcrewid;
		}
		set
		{
			skillcrewid = value;
		}
	}

	public int Crewgrade
	{
		get
		{
			return crewgrade;
		}
		set
		{
			crewgrade = value;
		}
	}

	public int Crewcount
	{
		get
		{
			return crewcount;
		}
		set
		{
			crewcount = value;
		}
	}
}
