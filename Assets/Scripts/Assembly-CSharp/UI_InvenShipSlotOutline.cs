public class UI_InvenShipSlotOutline : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private global::UnityEngine.Transform target;

	private global::UnityEngine.Vector3 prevPos;

	private global::UnityEngine.UI.Image myImage;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		myImage = GetComponent<global::UnityEngine.UI.Image>();
	}

	public void SetTarget(global::UnityEngine.Transform trans)
	{
		target = trans;
		if (target != null)
		{
			prevPos = trans.position;
			transform.position = prevPos;
			transform.SetAsLastSibling();
		}
	}

	public void SetColor(global::UnityEngine.Color color)
	{
		myImage.color = color;
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			if (target.position != prevPos)
			{
				transform.position = target.position;
			}
			prevPos = target.position;
		}
	}
}
