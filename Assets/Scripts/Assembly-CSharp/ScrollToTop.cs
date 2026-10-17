public class ScrollToTop : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Scrollbar scrollbar;

	private void Awake()
	{
		scrollbar = GetComponent<global::UnityEngine.UI.Scrollbar>();
	}

	public void repositionScrollbar()
	{
		scrollbar.value = 1f;
		scrollbar.size = 0.08560523f;
	}
}
