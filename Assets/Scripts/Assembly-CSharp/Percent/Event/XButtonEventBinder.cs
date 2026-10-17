namespace Percent.Event
{
	public class XButtonEventBinder : global::Percent.Event.ButtonEventBinder
	{
		protected override global::UnityEngine.Events.UnityAction onFindMethod()
		{
			return global::UnityEngine.GameObject.Find("CrossPromotion").GetComponent<global::Percent.CrossPromotionUIEventHandler>().onHide;
		}
	}
}
