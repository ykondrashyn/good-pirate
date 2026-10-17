[global::System.Serializable]
public class WorldData
{
	public global::System.Collections.Generic.List<IslandStatus> islandStatuslist = new global::System.Collections.Generic.List<IslandStatus>();

	public global::System.Collections.Generic.List<RockStatus> rockStatuslist = new global::System.Collections.Generic.List<RockStatus>();

	public int cubeNum;

	public int worldNum;

	public int worldSizeX;

	public int worldSizeZ;

	public global::UnityEngine.Color cameraColor;

	public global::UnityEngine.Color lightColor;

	public bool haveGate;

	public bool removeWall;

	public global::System.Collections.Generic.List<GateStatus> gateList = new global::System.Collections.Generic.List<GateStatus>();

	public global::System.Collections.Generic.List<PatrolStatus> patrolStatusList = new global::System.Collections.Generic.List<PatrolStatus>();

	public global::System.Collections.Generic.List<BossStatus> bossStatusList = new global::System.Collections.Generic.List<BossStatus>();

	public global::System.Collections.Generic.List<EnvironmentStatus> environmentList = new global::System.Collections.Generic.List<EnvironmentStatus>();
}
