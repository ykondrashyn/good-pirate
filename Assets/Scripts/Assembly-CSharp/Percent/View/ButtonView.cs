namespace Percent.View
{
	public class ButtonView : global::Percent.View.ResponsiveView
	{
		public global::Percent.PromotionType type;

		private global::Percent.Tween.PercentTween showScaleTweener;

		private global::Percent.Tween.PercentTween showColorTweener;

		protected override void Awake()
		{
			base.Awake();
			render = base.transform.parent.GetComponent<global::UnityEngine.UI.RawImage>();
			showScaleTweener = GetComponent<global::Percent.Tween.ScaleTween>();
			showColorTweener = GetComponent<global::Percent.Tween.ColorTween>();
		}

		internal override void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			switch (state)
			{
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
				showScaleTweener.play();
				showColorTweener.play();
				render.raycastTarget = true;
				break;
			case global::Percent.View.ViewLifeCycle.Status.HIDE:
				showScaleTweener.startDelay = 0f;
				showScaleTweener.onEnd = onAnimationEnd;
				showScaleTweener.playReverse();
				showColorTweener.startDelay = 0f;
				showColorTweener.playReverse();
				render.raycastTarget = false;
				break;
			}
		}

		public void onAnimationEnd()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
