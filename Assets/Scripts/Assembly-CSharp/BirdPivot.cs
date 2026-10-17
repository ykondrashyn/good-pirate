public class BirdPivot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform myParent;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private global::UnityEngine.Transform cameraTrans;

	private void Awake()
	{
		if (global::UnityEngine.Camera.main != null)
		{
			cameraTrans = global::UnityEngine.Camera.main.transform;
		}
		else
		{
			StartCoroutine(WaitCamera());
		}
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private global::System.Collections.IEnumerator WaitCamera()
	{
		while (!(global::UnityEngine.Camera.main != null))
		{
			yield return null;
		}
		cameraTrans = global::UnityEngine.Camera.main.transform;
	}

	private void LateUpdate()
	{
		if (!(cameraTrans == null))
		{
			global::UnityEngine.Vector3 forward = cameraTrans.forward;
			forward.y = 0f;
			forward.Normalize();
			transform.position = myParent.position + forward * 15f;
		}
	}
}
