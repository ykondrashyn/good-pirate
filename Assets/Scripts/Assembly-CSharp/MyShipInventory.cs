[global::System.Serializable]
public class MyShipInventory
{
	public int id;

	public int[] equipSailerId = new int[3];

	public EShipType shipType;

	public int[] lineIndex = new int[3];

	public int[] lineTempIndex = new int[3];
}
