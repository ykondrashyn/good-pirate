public class UI_Billboard : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private BillboardAxis axis;

	public bool isAlways;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		if (global::UnityEngine.Camera.main != null)
		{
			switch (axis)
			{
			case BillboardAxis.FORWARD:
				transform.forward = global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.RIGHT:
				transform.right = global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.BACK:
				transform.forward = -global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.LEFT:
				transform.right = -global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.UP:
				transform.up = global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.DOWN:
				transform.up = -global::UnityEngine.Camera.main.transform.forward;
				break;
			}
		}
	}

	[global::UnityEngine.ContextMenu("once")]
	private void DoOnce()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		switch (axis)
		{
		case BillboardAxis.FORWARD:
			transform.forward = global::UnityEngine.Camera.main.transform.forward;
			break;
		case BillboardAxis.RIGHT:
			transform.right = global::UnityEngine.Camera.main.transform.forward;
			break;
		case BillboardAxis.BACK:
			transform.forward = -global::UnityEngine.Camera.main.transform.forward;
			break;
		case BillboardAxis.LEFT:
			transform.right = -global::UnityEngine.Camera.main.transform.forward;
			break;
		case BillboardAxis.UP:
			transform.up = global::UnityEngine.Camera.main.transform.forward;
			break;
		case BillboardAxis.DOWN:
			transform.up = -global::UnityEngine.Camera.main.transform.forward;
			break;
		}
	}

	private void LateUpdate()
	{
		if (global::UnityEngine.Camera.main != null && isAlways)
		{
			switch (axis)
			{
			case BillboardAxis.FORWARD:
				transform.forward = global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.RIGHT:
				transform.right = global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.BACK:
				transform.forward = -global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.LEFT:
				transform.right = -global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.UP:
				transform.up = global::UnityEngine.Camera.main.transform.forward;
				break;
			case BillboardAxis.DOWN:
				transform.up = -global::UnityEngine.Camera.main.transform.forward;
				break;
			}
		}
	}
}
