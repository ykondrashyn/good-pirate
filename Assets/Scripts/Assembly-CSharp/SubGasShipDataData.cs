[global::System.Serializable]
public class SubGasShipDataData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private int attack;

	[global::UnityEngine.SerializeField]
	private int health;

	[global::UnityEngine.SerializeField]
	private float firecooltime;

	[global::UnityEngine.SerializeField]
	private int cannonnum;

	[global::UnityEngine.SerializeField]
	private int enemyattack;

	[global::UnityEngine.SerializeField]
	private int enemyhealth;

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

	public int Attack
	{
		get
		{
			return attack;
		}
		set
		{
			attack = value;
		}
	}

	public int Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
		}
	}

	public float Firecooltime
	{
		get
		{
			return firecooltime;
		}
		set
		{
			firecooltime = value;
		}
	}

	public int Cannonnum
	{
		get
		{
			return cannonnum;
		}
		set
		{
			cannonnum = value;
		}
	}

	public int Enemyattack
	{
		get
		{
			return enemyattack;
		}
		set
		{
			enemyattack = value;
		}
	}

	public int Enemyhealth
	{
		get
		{
			return enemyhealth;
		}
		set
		{
			enemyhealth = value;
		}
	}
}
