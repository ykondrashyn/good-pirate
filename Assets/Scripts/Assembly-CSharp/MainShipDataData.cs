[global::System.Serializable]
public class MainShipDataData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private int attack;

	[global::UnityEngine.SerializeField]
	private float firecooltime;

	[global::UnityEngine.SerializeField]
	private float rotatepersec;

	[global::UnityEngine.SerializeField]
	private float acceleration;

	[global::UnityEngine.SerializeField]
	private int health;

	[global::UnityEngine.SerializeField]
	private float levelfactor;

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

	public float Rotatepersec
	{
		get
		{
			return rotatepersec;
		}
		set
		{
			rotatepersec = value;
		}
	}

	public float Acceleration
	{
		get
		{
			return acceleration;
		}
		set
		{
			acceleration = value;
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

	public float Levelfactor
	{
		get
		{
			return levelfactor;
		}
		set
		{
			levelfactor = value;
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
