public class CFX_AutoRotate : global::UnityEngine.MonoBehaviour
{
	public global::UnityEngine.Vector3 rotation;

	public global::UnityEngine.Space space = global::UnityEngine.Space.Self;

	private void Update()
	{
		base.transform.Rotate(rotation * global::UnityEngine.Time.deltaTime, space);
	}
}
