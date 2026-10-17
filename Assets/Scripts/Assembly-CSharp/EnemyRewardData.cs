[global::System.Serializable]
public class EnemyRewardData
{
	[global::UnityEngine.SerializeField]
	private int level;

	[global::UnityEngine.SerializeField]
	private long normalgold;

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

	public long Normalgold
	{
		get
		{
			return normalgold;
		}
		set
		{
			normalgold = value;
		}
	}
}
