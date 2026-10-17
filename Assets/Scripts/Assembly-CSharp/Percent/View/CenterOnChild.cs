namespace Percent.View
{
	public class CenterOnChild : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IDragHandler, global::UnityEngine.EventSystems.IEventSystemHandler
	{
		public delegate void OnChangeCenterContent(int indexOfCenter, int indexOfLastCenter);

		public global::Percent.View.CenterOnChild.OnChangeCenterContent onChangeCenterContent;

		private global::UnityEngine.UI.ScrollRect scrollRect;

		private global::UnityEngine.RectTransform contentContainer;

		private global::UnityEngine.RectTransform[] contents = new global::UnityEngine.RectTransform[0];

		private global::Percent.View.ViewLifeCycle viewLifeCycle;

		private global::UnityEngine.Vector2 scrollDir;

		private int lastIndex = -1;

		private readonly float CENTER_FIX = 540f;

		private void Awake()
		{
			scrollRect = GetComponent<global::UnityEngine.UI.ScrollRect>();
			scrollRect.onValueChanged.AddListener(onValueChanged);
			contentContainer = scrollRect.content;
			viewLifeCycle = global::UnityEngine.GameObject.Find("ViewLifeCycle").GetComponent<global::Percent.View.ViewLifeCycle>();
			global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
			obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
		}

		public void OnDrag(global::UnityEngine.EventSystems.PointerEventData data)
		{
			scrollDir = data.delta.normalized;
		}

		internal void onValueChanged(global::UnityEngine.Vector2 dir)
		{
			int num = 0;
			float num2 = float.MaxValue;
			float num3 = contentContainer.localPosition.x + scrollDir.x * CENTER_FIX;
			for (int i = 1; i < contents.Length; i++)
			{
				if (global::UnityEngine.Mathf.Abs(contents[i].localPosition.x + num3) < num2)
				{
					num = i;
					num2 = contents[i].localPosition.x + num3;
				}
			}
			if (!lastIndex.Equals(num))
			{
				if (onChangeCenterContent != null)
				{
					onChangeCenterContent(num, lastIndex);
				}
				lastIndex = num;
			}
		}

		internal void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			if (state != global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS)
			{
				return;
			}
			if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
			{
				global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
				obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
			}
			else if (global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
			{
				global::Percent.View.SlideView[] componentsInChildren = global::UnityEngine.GameObject.Find("Content").GetComponentsInChildren<global::Percent.View.SlideView>();
				int num = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
				contents = new global::UnityEngine.RectTransform[num + 1];
				for (int i = 1; i <= componentsInChildren.Length; i++)
				{
					contents[i] = componentsInChildren[i - 1].GetComponent<global::UnityEngine.RectTransform>();
				}
			}
		}
	}
}
