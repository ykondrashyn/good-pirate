[global::System.Serializable]
public class MyFormationData
{
	public int attackPoint;

	public int healthPoint;

	public int speedPoint;

	public int UseTotalPoint => attackPoint + healthPoint + speedPoint;
}
