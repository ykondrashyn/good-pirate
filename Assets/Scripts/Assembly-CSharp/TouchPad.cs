public class TouchPad : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IDragHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IPointerUpHandler
{
	public delegate void PointerAction(float strength, global::UnityEngine.Vector3 normalVec, global::UnityEngine.Vector3 ControlVec);

	public static TouchPad Instance;

	private int touchID = -99;

	private global::UnityEngine.Canvas rootCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image outCircle;

	private float resultDist;

	private float radius;

	private float realRadius;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform controlUiTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform controlUiMoveHandler;

	public TouchPad.PointerAction successAction;

	public TouchPad.PointerAction failAction;

	public TouchPad.PointerAction dragAction;

	public TouchPad.PointerAction pointerUpAction;

	public TouchPad.PointerAction pointerDownAction;

	public TouchPad.PointerAction touchingUpdateAction;

	public TouchPad.PointerAction touchingFixedUpdateAction;

	public global::System.Action<global::UnityEngine.Vector2> doubleTouch;

	private bool isDoubleTouch;

	private global::UnityEngine.EventSystems.PointerEventData lastEventData;

	public global::UnityEngine.Vector3 ControlVector { get; private set; }

	public global::UnityEngine.Vector3 ControlNormalizeVector { get; private set; }

	public float ControlStrength { get; private set; }

	private void Awake()
	{
		Instance = this;
		rootCanvas = GetComponentInParent<global::UnityEngine.Canvas>();
		ControlVector = global::UnityEngine.Vector3.zero;
		ControlNormalizeVector = global::UnityEngine.Vector3.zero;
		ControlStrength = 0f;
		radius = 350f;
	}

	private void Start()
	{
		realRadius = (rootCanvas.transform as global::UnityEngine.RectTransform).sizeDelta.x / 1242f * radius;
	}

	public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (touchID != -99)
		{
			return;
		}
		lastEventData = eventData;
		touchID = eventData.pointerId;
		SetInCircle(eventData.position);
		ControlVector = global::UnityEngine.Vector3.zero;
		ControlNormalizeVector = global::UnityEngine.Vector3.zero;
		ControlStrength = 0f;
		controlUiTrans.gameObject.SetActive(value: true);
		if (!TutorialManager.Instance.IsHadTutorial("move_base") && TutorialManager.Instance.IsPlaying("move_base"))
		{
			if (IsInvoking("CheckTutorial"))
			{
				CancelInvoke("CheckTutorial");
			}
			Invoke("CheckTutorial", 1f);
		}
		if (pointerDownAction != null)
		{
			pointerDownAction(ControlStrength, ControlNormalizeVector, ControlVector);
		}
	}

	private void DobleTouchTimeOver()
	{
		isDoubleTouch = false;
	}

	private void CheckTutorial()
	{
		if (TutorialManager.Instance.IsPlaying("move_base") && !IsInvoking("MoveBaseTutorialComplete"))
		{
			Invoke("MoveBaseTutorialComplete", 4f);
		}
	}

	private void MoveBaseTutorialComplete()
	{
		TutorialManager.Instance.Off();
		GameManager.Instance.Invoke("GenerateTutorialRootBox", 1.5f);
	}

	public void OnDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (touchID == eventData.pointerId)
		{
			SetInCircle(eventData.position);
			controlUiTrans.right = -ControlNormalizeVector;
			if (dragAction != null)
			{
				dragAction(ControlStrength, ControlNormalizeVector, ControlVector);
			}
		}
	}

	public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (touchID != eventData.pointerId)
		{
			return;
		}
		if (pointerUpAction != null)
		{
			pointerUpAction(ControlStrength, ControlNormalizeVector, ControlVector);
		}
		if (!isDoubleTouch)
		{
			isDoubleTouch = true;
			if (IsInvoking("DobleTouchTimeOver"))
			{
				CancelInvoke("DobleTouchTimeOver");
			}
			Invoke("DobleTouchTimeOver", 0.23f);
		}
		else
		{
			if (doubleTouch != null)
			{
				doubleTouch(eventData.position);
			}
			if (!TutorialManager.Instance.IsHadTutorial("doubletap") && TutorialManager.Instance.IsPlaying("doubletap"))
			{
				TutorialManager.Instance.Off();
			}
			CancelInvoke("DobleTouchTimeOver");
			isDoubleTouch = false;
		}
		if (!TutorialManager.Instance.IsHadTutorial("move_base") && TutorialManager.Instance.IsPlaying("move_base") && IsInvoking("CheckTutorial"))
		{
			CancelInvoke("CheckTutorial");
		}
		Reset();
	}

	public void ForceTouchUp()
	{
		if (lastEventData != null)
		{
			OnPointerUp(lastEventData);
		}
	}

	public void Reset()
	{
		touchID = -99;
		lastEventData = null;
		outCircle.transform.localPosition = global::UnityEngine.Vector3.zero;
		ControlVector = global::UnityEngine.Vector2.zero;
		ControlNormalizeVector = global::UnityEngine.Vector2.zero;
		ControlStrength = 0f;
		controlUiMoveHandler.localPosition = new global::UnityEngine.Vector3(-100f, 0f, 0f);
	}

	private void FixedUpdate()
	{
		if (touchID != -99 && touchingFixedUpdateAction != null)
		{
			touchingFixedUpdateAction(ControlStrength, ControlNormalizeVector, ControlVector);
		}
	}

	private void Update()
	{
		if (touchID != -99 && touchingUpdateAction != null)
		{
			touchingUpdateAction(ControlStrength, ControlNormalizeVector, ControlVector);
		}
	}

	private void SetInCircle(global::UnityEngine.Vector2 screenPos)
	{
		global::UnityEngine.Vector2 localPoint = global::UnityEngine.Vector2.zero;
		if (global::UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform as global::UnityEngine.RectTransform, screenPos, (rootCanvas.renderMode == global::UnityEngine.RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera, out localPoint))
		{
			resultDist = localPoint.magnitude;
			if (resultDist > realRadius)
			{
				localPoint.Normalize();
				ControlVector = localPoint;
				ControlNormalizeVector = localPoint;
				ControlStrength = 1f;
				resultDist = realRadius;
				localPoint *= realRadius;
			}
			else if (resultDist == 0f)
			{
				ControlVector = global::UnityEngine.Vector3.zero;
				ControlNormalizeVector = global::UnityEngine.Vector3.zero;
				ControlStrength = 0f;
			}
			else
			{
				ControlVector = localPoint / realRadius;
				ControlNormalizeVector = localPoint.normalized;
				ControlStrength = resultDist / realRadius;
			}
			outCircle.transform.localPosition = localPoint;
			controlUiMoveHandler.transform.localPosition = ((resultDist > 100f) ? resultDist : 100f) * new global::UnityEngine.Vector3(-1f, 0f, 0f);
		}
	}
}
