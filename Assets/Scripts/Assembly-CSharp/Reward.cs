[global::System.Serializable]
public struct Reward
{
	public EMonenyType moneyType;

	public long amount;

	public Reward(EMonenyType moneyType, long amount)
	{
		this.moneyType = moneyType;
		this.amount = amount;
	}
}
