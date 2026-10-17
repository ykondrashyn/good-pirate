public class EnvironmentObject : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private float moveSpeed = 3f;

	public global::System.Action InactiveAction;

	private float MAX_DISTANCE = 65025f;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void Update()
	{
		transform.Translate(transform.forward * global::UnityEngine.Time.deltaTime * moveSpeed, global::UnityEngine.Space.World);
	}

	private void LateUpdate()
	{
		if (!(GameManager.Instance.playerShip == null) && (GameManager.Instance.playerShip.transform.position - transform.position).sqrMagnitude > MAX_DISTANCE)
		{
			if (InactiveAction != null)
			{
				InactiveAction();
			}
			base.gameObject.SetActive(value: false);
		}
	}
}
