public class UI_StageShipPos : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = base.transform as global::UnityEngine.RectTransform;
	}
}
