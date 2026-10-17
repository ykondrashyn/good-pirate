[global::System.Serializable]
public struct IslandStatus
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Vector3 islandPosition;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Quaternion islandRotation;

	public EIsland islandType;

	public int islandId;

	public int islandLevel;

	public int selectedDockNumber;

	public string nameText;

	public global::System.Collections.Generic.List<IslandSpawnData> spawnList;

	public global::System.Collections.Generic.List<IslandRewardData> rewardList;
}
