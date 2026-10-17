public class UI_WorldMapSameScale : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	public global::UnityEngine.Transform targetScalePivot;

	private global::UnityEngine.RectTransform rectTransform;

	private float startScale;

	private float prevScale;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		rectTransform = GetComponent<global::UnityEngine.RectTransform>();
	}

	private void Start()
	{
		startScale = transform.localScale.x;
	}

	private void LateUpdate()
	{
		if (targetScalePivot != null)
		{
			float x = targetScalePivot.localScale.x;
			if (prevScale != x)
			{
				float num = 1f / x;
				transform.localScale = global::UnityEngine.Vector3.one * startScale * num;
			}
			prevScale = x;
		}
	}

	public void SetPosition(global::UnityEngine.Vector3 pos)
	{
		if (rectTransform != null)
		{
			rectTransform.anchoredPosition = new global::UnityEngine.Vector2(pos.x, pos.z);
			return;
		}
		rectTransform = GetComponent<global::UnityEngine.RectTransform>();
		rectTransform.anchoredPosition = new global::UnityEngine.Vector2(pos.x, pos.z);
	}

	public void SetRotation(float angle)
	{
		if (rectTransform != null)
		{
			rectTransform.localRotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f - angle);
		}
	}
}
