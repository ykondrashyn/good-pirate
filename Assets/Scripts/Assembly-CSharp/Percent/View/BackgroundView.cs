namespace Percent.View
{
	public class BackgroundView : global::Percent.View.View
	{
		public global::Percent.PromotionType type;

		private global::Percent.Tween.PercentTween tweener;

		protected override void Awake()
		{
			base.Awake();
			tweener = GetComponent<global::Percent.Tween.ColorTween>();
		}

		internal override void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			switch (state)
			{
			case global::Percent.View.ViewLifeCycle.Status.INIT:
				render.raycastTarget = false;
				break;
			case global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS:
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(type))
				{
					global::Percent.View.ViewLifeCycle obj2 = viewLifeCycle;
					obj2.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj2.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
					base.gameObject.SetActive(value: false);
				}
				break;
			case global::Percent.View.ViewLifeCycle.Status.LOAD_FAIL:
			{
				global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
				obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
				base.gameObject.SetActive(value: false);
				break;
			}
			case global::Percent.View.ViewLifeCycle.Status.SHOW:
				render.raycastTarget = true;
				tweener.play();
				break;
			case global::Percent.View.ViewLifeCycle.Status.HIDE:
				render.raycastTarget = false;
				tweener.playReverse();
				break;
			}
		}
	}
}
