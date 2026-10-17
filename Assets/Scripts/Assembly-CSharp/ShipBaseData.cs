[global::System.Serializable]
public class ShipBaseData
{
	public EShipType id;

	public string shipName;

	public ECombatStyle combatStyle;

	[global::UnityEngine.Header("사용 인구")]
	public int useCount = 1;

	[global::UnityEngine.Header("생산 시간(초 단위)")]
	public int generateTime = 3;

	[global::UnityEngine.Header("언락 필요한 섬 수")]
	public int unlockIslandCount = 2;
}
