public class UI_IphoneXLayout : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private EPosition position;

	private global::UnityEngine.RectTransform rectTrasform;

	private void Awake()
	{
		rectTrasform = GetComponent<global::UnityEngine.RectTransform>();
	}
}
