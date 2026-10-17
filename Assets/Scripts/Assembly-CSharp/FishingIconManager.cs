public class FishingIconManager : global::UnityEngine.MonoBehaviour
{
	public static FishingIconManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform pivot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject fishIconSrc;

	private global::UnityEngine.CanvasGroup myCanvasGroup;

	private void Awake()
	{
		Instance = this;
		myCanvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
	}

	public FishingIcon Generate()
	{
		return global::UnityEngine.Object.Instantiate(fishIconSrc, pivot).GetComponent<FishingIcon>();
	}
}
