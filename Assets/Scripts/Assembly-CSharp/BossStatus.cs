[global::System.Serializable]
public struct BossStatus
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Vector3 position;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Quaternion rotation;

	public int bossLevel;

	public EBoss bossType;

	[global::UnityEngine.Header("key (수정하지 말것)")]
	public string bossKey;

	public BossReward reward;
}
