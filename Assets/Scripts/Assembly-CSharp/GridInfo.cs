public class GridInfo
{
	public global::UnityEngine.Vector3 center;

	public global::UnityEngine.Vector3 top;

	public global::UnityEngine.Vector3 left;

	public global::UnityEngine.Vector3 right;

	public global::UnityEngine.Vector3 bottom;

	public global::UnityEngine.Vector3 GetRandomPos()
	{
		return new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(left.x, right.x), 0f, global::UnityEngine.Random.Range(top.z, bottom.z));
	}
}
