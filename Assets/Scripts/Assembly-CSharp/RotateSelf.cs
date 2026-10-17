public class RotateSelf : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	public bool isWorldRotate;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Vector3 rotate;

	private float eulerX;

	private float eulerY;

	private float eulerZ;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!isWorldRotate)
		{
			transform.Rotate(rotate * global::UnityEngine.Time.deltaTime, global::UnityEngine.Space.Self);
		}
		else
		{
			transform.Rotate(rotate * global::UnityEngine.Time.deltaTime, global::UnityEngine.Space.World);
		}
	}
}
