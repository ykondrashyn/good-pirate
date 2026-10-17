[global::System.Serializable]
public class RewardBoxDataData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private long gold;

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
}
