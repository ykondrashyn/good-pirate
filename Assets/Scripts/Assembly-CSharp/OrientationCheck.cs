public class OrientationCheck : global::UnityEngine.MonoBehaviour
{
	public static OrientationCheck Instance;

	private global::UnityEngine.ScreenOrientation curr;

	public global::UnityEngine.Events.UnityEvent landscapeEvent = new global::UnityEngine.Events.UnityEvent();

	public global::UnityEngine.Events.UnityEvent portraitEvent = new global::UnityEngine.Events.UnityEvent();

	public bool IsPortrait => global::UnityEngine.Screen.orientation == global::UnityEngine.ScreenOrientation.Portrait;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		curr = global::UnityEngine.Screen.orientation;
	}

	private void LateUpdate()
	{
		if ((global::UnityEngine.Screen.orientation == global::UnityEngine.ScreenOrientation.LandscapeLeft || global::UnityEngine.Screen.orientation == global::UnityEngine.ScreenOrientation.LandscapeRight || global::UnityEngine.Screen.orientation == global::UnityEngine.ScreenOrientation.LandscapeLeft) && curr != global::UnityEngine.ScreenOrientation.LandscapeLeft && curr != global::UnityEngine.ScreenOrientation.LandscapeRight && curr != global::UnityEngine.ScreenOrientation.LandscapeLeft)
		{
			global::UnityEngine.Camera.main.orthographicSize = 30f;
			curr = global::UnityEngine.ScreenOrientation.LandscapeLeft;
			landscapeEvent.Invoke();
		}
		else if (global::UnityEngine.Screen.orientation == global::UnityEngine.ScreenOrientation.Portrait && curr != global::UnityEngine.ScreenOrientation.Portrait)
		{
			curr = global::UnityEngine.Screen.orientation;
			global::UnityEngine.Camera.main.orthographicSize = 45f;
			portraitEvent.Invoke();
		}
	}

	public void AddLandscapeEvent(global::UnityEngine.Events.UnityAction ac)
	{
		landscapeEvent.AddListener(ac);
	}

	public void AddPortraitEvent(global::UnityEngine.Events.UnityAction ac)
	{
		portraitEvent.AddListener(ac);
	}
}
