namespace Percent.View
{
	public class ImageView : global::Percent.View.ResponsiveView
	{
		private global::Percent.Tween.PercentTween showScaleTweener;

		private global::Percent.Tween.PercentTween showColorTweener;

		private global::Percent.Http.TextureLoader textureLoader;

		private global::Percent.TextureLifeCycle textureLifeCycle;

		protected override void Awake()
		{
			base.Awake();
			showScaleTweener = GetComponent<global::Percent.Tween.ScaleTween>();
			showColorTweener = GetComponent<global::Percent.Tween.ColorTween>();
			textureLoader = GetComponent<global::Percent.Http.TextureLoader>();
			textureLifeCycle = global::UnityEngine.GameObject.Find("TextureLifeCycle").GetComponent<global::Percent.TextureLifeCycle>();
			global::Percent.TextureLifeCycle obj = textureLifeCycle;
			obj.onStateChange = (global::Percent.TextureLifeCycle.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.TextureLifeCycle.OnStateChange(onTextureStateChange));
		}

		internal override void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			switch (state)
			{
			case global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS:
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.IMAGE))
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

		internal void onTextureStateChange(global::Percent.TextureLifeCycle.Status state)
		{
			if (state == global::Percent.TextureLifeCycle.Status.LOAD)
			{
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.IMAGE))
				{
					global::Percent.TextureLifeCycle obj = textureLifeCycle;
					obj.onStateChange = (global::Percent.TextureLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.TextureLifeCycle.OnStateChange(onTextureStateChange));
				}
				else if (global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.IMAGE))
				{
					string[] resourceUrl = global::Percent.PromotionData.crossPromotionData.resourceUrl;
					textureLoader.render(resourceUrl[0], textureLifeCycle.onEachTextureLoad);
				}
			}
		}

		public void onAnimationEnd()
		{
			deactivateWholeView();
		}

		private void deactivateWholeView()
		{
			global::UnityEngine.GameObject.Find("TypeImage").GetComponent<global::Percent.View.Hider>().hide();
		}
	}
}
