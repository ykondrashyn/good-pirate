[global::System.Serializable]
public struct IslandSpawnData
{
	[global::UnityEngine.Header("메인 배가 있을경우 보조배들은 메인배를 따라다님")]
	public EShipType shipType;

	public int count;

	public IslandSpawnData(EShipType shipType, int count)
	{
		this.shipType = shipType;
		this.count = count;
	}
}
