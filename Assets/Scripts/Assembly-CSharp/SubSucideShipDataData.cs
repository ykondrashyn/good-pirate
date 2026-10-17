[global::System.Serializable]
public class SubSucideShipDataData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private int attack;

	[global::UnityEngine.SerializeField]
	private int health;

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
