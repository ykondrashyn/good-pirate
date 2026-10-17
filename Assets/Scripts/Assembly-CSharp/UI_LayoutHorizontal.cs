[global::UnityEngine.ExecuteInEditMode]
public class UI_LayoutHorizontal : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] myContents;

	private global::UnityEngine.RectTransform rectTransform;

	public float topPadding;

	public float spacing;

	private void Awake()
	{
		rectTransform = base.transform as global::UnityEngine.RectTransform;
	}

	private void OnEnable()
	{
		Refresh();
	}

	[global::UnityEngine.ContextMenu("새로고침")]
	public void Refresh()
	{
		int childCount = base.transform.childCount;
		myContents = new global::UnityEngine.RectTransform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			myContents[i] = base.transform.GetChild(i) as global::UnityEngine.RectTransform;
		}
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(WaitEndFrame());
		}
	}

	private global::System.Collections.IEnumerator WaitEndFrame()
	{
		yield return new global::UnityEngine.WaitForEndOfFrame();
		float num = 0f - topPadding;
		for (int i = 0; i < myContents.Length; i++)
		{
			global::UnityEngine.Vector2 pivot = myContents[i].pivot;
			pivot.y = 1f;
			myContents[i].pivot = pivot;
			myContents[i].anchorMax = new global::UnityEngine.Vector2(0.5f, 1f);
			myContents[i].anchorMin = new global::UnityEngine.Vector2(0.5f, 1f);
			myContents[i].anchoredPosition = new global::UnityEngine.Vector2(0f, num);
			num -= myContents[i].sizeDelta.y + spacing;
		}
		if (rectTransform == null)
		{
			rectTransform = base.transform as global::UnityEngine.RectTransform;
		}
		global::UnityEngine.Vector2 sizeDelta = rectTransform.sizeDelta;
		sizeDelta.y = 0f - num;
		rectTransform.sizeDelta = sizeDelta;
	}
}
