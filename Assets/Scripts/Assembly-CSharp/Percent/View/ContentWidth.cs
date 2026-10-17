namespace Percent.View
{
	public class ContentWidth : global::UnityEngine.MonoBehaviour
	{
		private global::UnityEngine.RectTransform thisTrans;

		private global::Percent.View.ViewLifeCycle viewLifeCycle;

		private void Awake()
		{
			thisTrans = GetComponent<global::UnityEngine.RectTransform>();
			viewLifeCycle = global::UnityEngine.GameObject.Find("ViewLifeCycle").GetComponent<global::Percent.View.ViewLifeCycle>();
			global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
			obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
		}

		private void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			if (state == global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS)
			{
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
				{
					global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
					obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
				}
				else if (global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
				{
					setWidthBySlideViewNum();
				}
			}
		}

		private void setWidthBySlideViewNum()
		{
			int num = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			int num2 = global::Percent.View.SlideView.TERM * num + 50;
			thisTrans.sizeDelta = new global::UnityEngine.Vector2(num2, 300f);
		}
	}
}
