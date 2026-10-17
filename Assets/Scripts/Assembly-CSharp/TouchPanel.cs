using global::System.Collections.Generic;
using global::System.Globalization;

public class TouchPanel : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IDragHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IPointerUpHandler
{
	private const int NoTouch = -99;

	private const int BrowserPointerIdBase = -1000000;

	private int touchID = -99;

	private int browserSourcePointerId = NoTouch;

	private global::UnityEngine.Canvas rootCanvas;

	public TouchPad touchPad;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Animator controlUiAnimationController;

	private bool isZooming;

	private float lastDistance;

	private readonly global::System.Collections.Generic.List<global::UnityEngine.EventSystems.RaycastResult> uiRaycastResults = new global::System.Collections.Generic.List<global::UnityEngine.EventSystems.RaycastResult>();

	private void Awake()
	{
		rootCanvas = GetComponentInParent<global::UnityEngine.Canvas>();
	}

	private void OnEnable()
	{
		touchPad.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		CancelHeldPointer();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			CancelHeldPointer();
		}
	}

	public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		return;
#else
		BeginPointer(eventData, NoTouch);
#endif
	}

	public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		return;
#else
		if (touchID == eventData.pointerId)
		{
			touchPad.OnPointerUp(eventData);
			Reset();
		}
#endif
	}

	public void OnDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		return;
#else
		if (!isZooming && touchID == eventData.pointerId)
		{
			touchPad.OnDrag(eventData);
		}
#endif
	}

	public void OnBrowserPointerDown(string payload)
	{
		if (!TryParseBrowserPointer(payload, out var pointerId, out var position, out var delta))
		{
			global::UnityEngine.Debug.LogWarning("GP113_INPUT pointer-down malformed");
			return;
		}
		global::UnityEngine.GameObject blockingUi;
		if (TryGetBlockingUi(position, out blockingUi))
		{
			global::UnityEngine.Debug.Log("GP113_INPUT pointer-down blocked-by " + GetHierarchyPath(blockingUi));
			return;
		}
		global::UnityEngine.Debug.Log("GP113_INPUT pointer-down accepted");
		BeginPointer(CreateBrowserEvent(pointerId, position, delta), pointerId);
	}

	public void OnBrowserPointerMove(string payload)
	{
		if (!TryParseBrowserPointer(payload, out var pointerId, out var position, out var delta) || pointerId != browserSourcePointerId)
		{
			return;
		}
		if (!isZooming && touchID != NoTouch)
		{
			touchPad.OnDrag(CreateBrowserEvent(pointerId, position, delta));
		}
	}

	public void OnBrowserPointerUp(string payload)
	{
		if (!TryParseBrowserPointer(payload, out var pointerId, out var position, out var delta) || pointerId != browserSourcePointerId)
		{
			return;
		}
		if (touchID != NoTouch)
		{
			touchPad.OnPointerUp(CreateBrowserEvent(pointerId, position, delta));
		}
		Reset();
	}

	public void OnBrowserPointerCancel(string payload)
	{
		if (!TryParseBrowserPointer(payload, out var pointerId, out _, out _) || pointerId != browserSourcePointerId)
		{
			return;
		}
		CancelHeldPointer();
	}

	public void OnBrowserInputLostFocus(string reason)
	{
		CancelHeldPointer();
	}

	public void Reset()
	{
		if (touchID != NoTouch)
		{
			touchID = NoTouch;
			browserSourcePointerId = NoTouch;
			StartCoroutine("WaitAnimationControlUI");
			touchPad.Reset();
		}
	}

	private void BeginPointer(global::UnityEngine.EventSystems.PointerEventData eventData, int browserPointerId)
	{
		if (eventData == null || touchPad == null || touchID != NoTouch)
		{
			return;
		}
		touchID = eventData.pointerId;
		browserSourcePointerId = browserPointerId;
		touchPad.gameObject.SetActive(value: true);
		global::UnityEngine.Vector2 localPoint = global::UnityEngine.Vector2.zero;
		if (global::UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform as global::UnityEngine.RectTransform, eventData.position, (rootCanvas.renderMode == global::UnityEngine.RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera, out localPoint))
		{
			touchPad.transform.localPosition = localPoint;
		}
		touchPad.OnPointerDown(eventData);
		StopCoroutine("WaitAnimationControlUI");
	}

	private void CancelHeldPointer()
	{
		if (touchID == NoTouch)
		{
			browserSourcePointerId = NoTouch;
			return;
		}
		touchPad.ForceTouchUp();
		Reset();
	}

	private bool TryGetBlockingUi(global::UnityEngine.Vector2 position, out global::UnityEngine.GameObject blockingUi)
	{
		blockingUi = null;
		global::UnityEngine.EventSystems.EventSystem eventSystem = global::UnityEngine.EventSystems.EventSystem.current;
		if (eventSystem == null)
		{
			return false;
		}
		global::UnityEngine.EventSystems.PointerEventData pointer = new global::UnityEngine.EventSystems.PointerEventData(eventSystem)
		{
			position = position
		};
		uiRaycastResults.Clear();
		eventSystem.RaycastAll(pointer, uiRaycastResults);
		for (int index = 0; index < uiRaycastResults.Count; index++)
		{
			if (!(uiRaycastResults[index].module is global::UnityEngine.UI.GraphicRaycaster))
			{
				continue;
			}
			global::UnityEngine.GameObject hit = uiRaycastResults[index].gameObject;
			if (hit == null || !hit.activeInHierarchy)
			{
				continue;
			}
			if (!hit.transform.IsChildOf(transform) && hit.transform != transform)
			{
				blockingUi = hit;
				return true;
			}
		}
		return false;
	}

	private static string GetHierarchyPath(global::UnityEngine.GameObject gameObject)
	{
		if (gameObject == null)
		{
			return "<null>";
		}
		string path = gameObject.name;
		global::UnityEngine.Transform parent = gameObject.transform.parent;
		while (parent != null)
		{
			path = parent.name + "/" + path;
			parent = parent.parent;
		}
		return path;
	}

	private static global::UnityEngine.EventSystems.PointerEventData CreateBrowserEvent(int pointerId, global::UnityEngine.Vector2 position, global::UnityEngine.Vector2 delta)
	{
		return new global::UnityEngine.EventSystems.PointerEventData(global::UnityEngine.EventSystems.EventSystem.current)
		{
			pointerId = MapBrowserPointerId(pointerId),
			position = position,
			delta = delta,
			button = global::UnityEngine.EventSystems.PointerEventData.InputButton.Left
		};
	}

	private static int MapBrowserPointerId(int pointerId)
	{
		int offset = (int)(global::System.Math.Abs((long)pointerId) % 1000000L);
		return BrowserPointerIdBase - offset;
	}

	private static bool TryParseBrowserPointer(string payload, out int pointerId, out global::UnityEngine.Vector2 position, out global::UnityEngine.Vector2 delta)
	{
		pointerId = NoTouch;
		position = global::UnityEngine.Vector2.zero;
		delta = global::UnityEngine.Vector2.zero;
		if (global::System.String.IsNullOrEmpty(payload))
		{
			return false;
		}
		string[] values = payload.Split(',');
		if (values.Length < 5 || !int.TryParse(values[0], global::System.Globalization.NumberStyles.Integer, global::System.Globalization.CultureInfo.InvariantCulture, out pointerId) || !float.TryParse(values[1], global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out var x) || !float.TryParse(values[2], global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out var y) || !float.TryParse(values[3], global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out var deltaX) || !float.TryParse(values[4], global::System.Globalization.NumberStyles.Float, global::System.Globalization.CultureInfo.InvariantCulture, out var deltaY))
		{
			return false;
		}
		position = new global::UnityEngine.Vector2(x, y);
		delta = new global::UnityEngine.Vector2(deltaX, deltaY);
		return true;
	}

	private global::System.Collections.IEnumerator WaitAnimationControlUI()
	{
		controlUiAnimationController.Play("Inactive");
		yield return null;
		while (true)
		{
			if (controlUiAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				touchPad.gameObject.SetActive(value: false);
			}
			yield return null;
		}
	}

	private void Update()
	{
		if (GameManager.Instance == null || GameManager.Instance.IsInIsland)
		{
			return;
		}
		if (global::UnityEngine.Input.touchCount >= 2)
		{
			if (global::UnityEngine.Input.touches[1].phase == global::UnityEngine.TouchPhase.Began)
			{
				lastDistance = (global::UnityEngine.Input.touches[0].position - global::UnityEngine.Input.touches[1].position).magnitude;
			}
			else if (global::UnityEngine.Input.touches[0].phase == global::UnityEngine.TouchPhase.Moved || global::UnityEngine.Input.touches[1].phase == global::UnityEngine.TouchPhase.Moved)
			{
				isZooming = true;
				Reset();
				float magnitude = (global::UnityEngine.Input.touches[0].position - global::UnityEngine.Input.touches[1].position).magnitude;
				float num = lastDistance - magnitude;
				float orthographicSize = global::UnityEngine.Camera.main.orthographicSize;
				orthographicSize += num * 0.1f;
				orthographicSize = global::UnityEngine.Mathf.Clamp(orthographicSize, 25f, 80f);
				global::UnityEngine.Camera.main.orthographicSize = orthographicSize;
				lastDistance = magnitude;
			}
		}
		else
		{
			isZooming = false;
		}
		if (!(global::UnityEngine.Camera.main == null))
		{
			if (global::UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				float orthographicSize2 = global::UnityEngine.Camera.main.orthographicSize;
				orthographicSize2 -= 1f;
				orthographicSize2 = global::UnityEngine.Mathf.Clamp(orthographicSize2, 25f, 80f);
				global::UnityEngine.Camera.main.orthographicSize = orthographicSize2;
			}
			else if (global::UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				float orthographicSize3 = global::UnityEngine.Camera.main.orthographicSize;
				orthographicSize3 += 1f;
				orthographicSize3 = global::UnityEngine.Mathf.Clamp(orthographicSize3, 25f, 80f);
				global::UnityEngine.Camera.main.orthographicSize = orthographicSize3;
			}
		}
	}
}
