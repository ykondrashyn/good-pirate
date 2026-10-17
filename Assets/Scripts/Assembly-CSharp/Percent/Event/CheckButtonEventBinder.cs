namespace Percent.Event
{
	public class CheckButtonEventBinder : global::Percent.Event.ButtonEventBinder
	{
		public global::Percent.PromotionType type;

		protected override global::UnityEngine.Events.UnityAction onFindMethod()
		{
			global::Percent.CrossPromotionUIEventHandler component = global::UnityEngine.GameObject.Find("CrossPromotion").GetComponent<global::Percent.CrossPromotionUIEventHandler>();
			global::UnityEngine.Events.UnityAction result = null;
			switch (type)
			{
			case global::Percent.PromotionType.IMAGE:
				result = component.onImageTypeClick;
				break;
			case global::Percent.PromotionType.SLIDE:
				result = component.onSlideTypeClick;
				break;
			}
			return result;
		}
	}
}
