[global::System.Serializable]
public class ClaimModeData
{
	[global::UnityEngine.SerializeField]
	private int id;

	[global::UnityEngine.SerializeField]
	private int enemylevelmax;

	[global::UnityEngine.SerializeField]
	private int claimtype;

	[global::UnityEngine.SerializeField]
	private int allycount;

	[global::UnityEngine.SerializeField]
	private int bosstype;

	[global::UnityEngine.SerializeField]
	private int bosssub;

	[global::UnityEngine.SerializeField]
	private int enemyspawncount;

	[global::UnityEngine.SerializeField]
	private int enemyspawngroupcount;

	[global::UnityEngine.SerializeField]
	private float enemyspawnintervaltime;

	[global::UnityEngine.SerializeField]
	private float limittime;

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

	public int Claimtype
	{
		get
		{
			return claimtype;
		}
		set
		{
			claimtype = value;
		}
	}

	public int Allycount
	{
		get
		{
			return allycount;
		}
		set
		{
			allycount = value;
		}
	}

	public int Bosstype
	{
		get
		{
			return bosstype;
		}
		set
		{
			bosstype = value;
		}
	}

	public int Bosssub
	{
		get
		{
			return bosssub;
		}
		set
		{
			bosssub = value;
		}
	}

	public int Enemyspawncount
	{
		get
		{
			return enemyspawncount;
		}
		set
		{
			enemyspawncount = value;
		}
	}

	public int Enemyspawngroupcount
	{
		get
		{
			return enemyspawngroupcount;
		}
		set
		{
			enemyspawngroupcount = value;
		}
	}

	public float Enemyspawnintervaltime
	{
		get
		{
			return enemyspawnintervaltime;
		}
		set
		{
			enemyspawnintervaltime = value;
		}
	}

	public float Limittime
	{
		get
		{
			return limittime;
		}
		set
		{
			limittime = value;
		}
	}
}
