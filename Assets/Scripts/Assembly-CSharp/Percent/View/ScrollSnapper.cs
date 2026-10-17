namespace Percent.View
{
	public class ScrollSnapper : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IBeginDragHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IEndDragHandler
	{
		private global::UnityEngine.RectTransform contentTrans;

		private int numOfContent;

		private global::Percent.View.ViewLifeCycle viewLifeCycle;

		private global::UnityEngine.Vector2[] posOnContent;

		private int targetIndex;

		private bool isSnapEnable;

		private readonly int SNAP_SPEED = 15;

		private readonly int LAST_SNAP = 20;

		private void Awake()
		{
			contentTrans = GetComponent<global::UnityEngine.UI.ScrollRect>().content;
			viewLifeCycle = global::UnityEngine.GameObject.Find("ViewLifeCycle").GetComponent<global::Percent.View.ViewLifeCycle>();
			global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
			obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
			global::Percent.View.CenterOnChild component = global::UnityEngine.GameObject.Find("TypeSlide").GetComponent<global::Percent.View.CenterOnChild>();
			component.onChangeCenterContent = (global::Percent.View.CenterOnChild.OnChangeCenterContent)global::System.Delegate.Combine(component.onChangeCenterContent, new global::Percent.View.CenterOnChild.OnChangeCenterContent(onChangeCenterContent));
		}

		private void Update()
		{
			if (isSnapEnable)
			{
				contentTrans.anchoredPosition = global::UnityEngine.Vector2.Lerp(contentTrans.anchoredPosition, posOnContent[targetIndex], global::UnityEngine.Time.unscaledDeltaTime * (float)SNAP_SPEED);
				if (global::UnityEngine.Vector2.Distance(contentTrans.anchoredPosition, posOnContent[targetIndex]) < (float)LAST_SNAP)
				{
					contentTrans.anchoredPosition = posOnContent[targetIndex];
					isSnapEnable = false;
				}
			}
		}

		internal void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			switch (state)
			{
			case global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS:
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
				{
					global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
					obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
				}
				break;
			case global::Percent.View.ViewLifeCycle.Status.SHOW:
				setContentPosOnContainer();
				break;
			}
		}

		private void setContentPosOnContainer()
		{
			numOfContent = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			posOnContent = new global::UnityEngine.Vector2[numOfContent];
			int num = (global::Percent.View.SlideView.TERM * numOfContent + 50) / 2;
			int num2 = (global::Percent.View.SlideView.TERM * numOfContent + 50) / numOfContent;
			for (int i = 0; i < numOfContent; i++)
			{
				posOnContent[i] = new global::UnityEngine.Vector2(num - num2 * i, 0f);
			}
			targetIndex = 0;
		}

		internal void onChangeCenterContent(int indexOfCenter, int indexOfLastCenter)
		{
			targetIndex = numOfContent - indexOfCenter;
		}

		public void OnBeginDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			isSnapEnable = false;
		}

		public void OnEndDrag(global::UnityEngine.EventSystems.PointerEventData eventData)
		{
			isSnapEnable = true;
		}
	}
}
