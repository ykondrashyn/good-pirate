public class CFX2_AutoRotate : global::UnityEngine.MonoBehaviour
{
	public global::UnityEngine.Vector3 speed = new global::UnityEngine.Vector3(0f, 40f, 0f);

	private void Update()
	{
		base.transform.Rotate(speed * global::UnityEngine.Time.deltaTime);
	}
}
