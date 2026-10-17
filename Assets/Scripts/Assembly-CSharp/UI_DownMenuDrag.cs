public class UI_DownMenuDrag : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerUpHandler, global::UnityEngine.EventSystems.IDragHandler
{
	[global::UnityEngine.HideInInspector]
	public bool isPause;

	public bool isOpen;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform pivotTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform downMenuTrans;

	[global::UnityEngine.SerializeField]
	private UI_MainUpgradeMenu menu;

	private int pointerId = -99;

	private global::UnityEngine.Vector2 lastPos;

	private global::UnityEngine.Camera myUiCamera;

	private bool isNotIntoDrag = true;

	private void Awake()
	{
		myUiCamera = GetComponentInParent<global::UnityEngine.Camera>();
	}

	public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (!isPause && pointerId == -99)
		{
			pointerId = eventData.pointerId;
			if (isOpen)
			{
				menu.SetPanelActive(active: true);
			}
			myImage.color = global::UnityEngine.Color.gray;
		}
	}

	public void OnDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (pointerId == eventData.pointerId)
		{
			global::UnityEngine.Vector2 localPos = GetLocalPos(eventData.position);
			global::UnityEngine.Vector3 anchoredPosition3D = downMenuTrans.anchoredPosition3D;
			anchoredPosition3D.y = global::UnityEngine.Mathf.Clamp(downMenuTrans.anchoredPosition.y + eventData.delta.y, 0f, downMenuTrans.sizeDelta.y);
			isNotIntoDrag = false;
			lastPos = localPos;
			downMenuTrans.anchoredPosition3D = anchoredPosition3D;
		}
	}

	public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (pointerId != eventData.pointerId)
		{
			return;
		}
		global::UnityEngine.Vector2 localPos = GetLocalPos(eventData.position);
		if (isOpen)
		{
			if (localPos.y > downMenuTrans.sizeDelta.y * 0.45f)
			{
				menu.Open();
			}
			else
			{
				menu.JustCloseMove();
			}
		}
		else if (localPos.y > downMenuTrans.sizeDelta.y * 0.6f)
		{
			menu.JustOpenMove();
		}
		else
		{
			menu.Close();
		}
		if (isNotIntoDrag)
		{
			if (isOpen)
			{
				menu.Open();
			}
			else
			{
				menu.Close();
			}
		}
		myImage.color = global::UnityEngine.Color.white;
		pointerId = -99;
		lastPos = global::UnityEngine.Vector2.zero;
		isNotIntoDrag = true;
	}

	private global::UnityEngine.Vector2 GetLocalPos(global::UnityEngine.Vector2 screenPos)
	{
		if (global::UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(pivotTrans, screenPos, myUiCamera, out var localPoint))
		{
			return localPoint;
		}
		return global::UnityEngine.Vector2.zero;
	}
}
