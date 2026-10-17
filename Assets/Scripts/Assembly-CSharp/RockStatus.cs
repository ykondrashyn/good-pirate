[global::System.Serializable]
public struct RockStatus
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Vector3 rockPosition;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Quaternion rockRotaion;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Vector3 rockLocalScale;

	public ERock rockType;

	public bool isNotDrawWorldMap;
}
