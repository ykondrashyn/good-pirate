[global::System.Serializable]
public struct PatrolStatus
{
	[global::UnityEngine.HideInInspector]
	public EShipType enemyType;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Vector3 startPosition;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Quaternion startRotation;

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<global::UnityEngine.Vector3> patrolCourseList;

	[global::UnityEngine.Header("key (수정하지 말것)")]
	public string patrolKey;

	[global::UnityEngine.Header("최소 레벨")]
	public int minLevel;

	[global::UnityEngine.Header("잡았을경우 재생성 시간(초단위)")]
	public int coolTime;

	[global::UnityEngine.Header("주변에 소환될 보조함들")]
	public global::System.Collections.Generic.List<IslandSpawnData> spawnList;
}
