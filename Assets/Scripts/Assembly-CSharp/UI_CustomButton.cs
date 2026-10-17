public class UI_CustomButton : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerUpHandler
{
	private int pointerID = -99;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic targetGraphic;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color normalColor = global::UnityEngine.Color.white;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color pressedColor = new global::UnityEngine.Color(0.3f, 0.3f, 0.3f);

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Events.UnityEvent onClick;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Events.UnityEvent onPushingRepeat;

	[global::UnityEngine.SerializeField]
	private float preDelay = 0.4f;

	[global::UnityEngine.SerializeField]
	private float repeatInterval = 0.15f;

	private float totalTime;

	private bool isRepeat;

	private bool isExit;

	private void Reset()
	{
		pointerID = -99;
		totalTime = 0f;
		isRepeat = false;
		if (targetGraphic != null)
		{
			targetGraphic.color = normalColor;
		}
	}

	public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (pointerID == -99)
		{
			if (targetGraphic != null)
			{
				targetGraphic.color = pressedColor;
			}
			pointerID = eventData.pointerId;
		}
	}

	public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (pointerID == eventData.pointerId)
		{
			if (!eventData.dragging && !isRepeat)
			{
				onClick.Invoke();
			}
			Reset();
		}
	}

	private void LateUpdate()
	{
		if (pointerID == -99)
		{
			return;
		}
		totalTime += global::UnityEngine.Time.deltaTime;
		if (!(totalTime < preDelay) && totalTime > preDelay + repeatInterval)
		{
			if (!isRepeat)
			{
				isRepeat = true;
			}
			totalTime -= repeatInterval;
			onPushingRepeat.Invoke();
		}
	}
}
