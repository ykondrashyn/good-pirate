[global::System.Serializable]
public class RewardBoxStatus
{
	public ERewardBox boxType;

	public int level;

	public RewardBoxStatus()
	{
	}

	public RewardBoxStatus(ERewardBox box, int lev)
	{
		boxType = box;
		level = lev;
	}
}
