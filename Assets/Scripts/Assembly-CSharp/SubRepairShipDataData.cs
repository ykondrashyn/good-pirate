[global::System.Serializable]
public class SubRepairShipDataData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private int attack;

	[global::UnityEngine.SerializeField]
	private int health;

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
}
