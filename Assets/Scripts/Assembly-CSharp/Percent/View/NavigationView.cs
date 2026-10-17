namespace Percent.View
{
	public class NavigationView : global::Percent.View.View
	{
		internal int index;

		private global::Percent.Tween.PercentTween showScaleTweener;

		private global::Percent.Tween.PercentTween showColorTweener;

		private global::Percent.Tween.PercentTween slideTweener;

		internal readonly int TERM = 82;

		private static int instanceNum;

		protected override void Awake()
		{
			base.Awake();
			index = ++instanceNum;
		}

		protected override void Start()
		{
			base.Start();
			initialize();
		}

		private void initialize()
		{
			initScrollView();
			initAnimation();
		}

		private void initScrollView()
		{
			global::Percent.View.CenterOnChild component = global::UnityEngine.GameObject.Find("TypeSlide").GetComponent<global::Percent.View.CenterOnChild>();
			component.onChangeCenterContent = (global::Percent.View.CenterOnChild.OnChangeCenterContent)global::System.Delegate.Combine(component.onChangeCenterContent, new global::Percent.View.CenterOnChild.OnChangeCenterContent(onChangeCenterContent));
			thisTrans.SetParent(global::UnityEngine.GameObject.Find("Navigation").transform);
			thisTrans.localPosition = getPosOfIndex();
		}

		private global::UnityEngine.Vector2 getPosOfIndex()
		{
			int num = -809;
			return new global::UnityEngine.Vector2((global::Percent.PromotionData.crossPromotionData.resourceUrl.Length - 1) * TERM / 2 - (index - 1) * TERM, num);
		}

		private void initAnimation()
		{
			showScaleTweener = GetComponent<global::Percent.Tween.ScaleTween>();
			global::Percent.Tween.PercentTween[] components = GetComponents<global::Percent.Tween.ColorTween>();
			global::Percent.Tween.PercentTween[] array = components;
			showColorTweener = array[0];
			slideTweener = array[1];
			setShowTweenSequece();
		}

		private void setShowTweenSequece()
		{
			float num = 0.1f;
			int num2 = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			float startDelay = 0.7f + (float)(num2 - index) * num;
			showScaleTweener.startDelay = startDelay;
			showColorTweener.startDelay = startDelay;
		}

		internal void onChangeCenterContent(int indexOfCenter, int indexOfLastCenter)
		{
			int obj = -1;
			if (!indexOfLastCenter.Equals(obj))
			{
				if (indexOfCenter.Equals(index))
				{
					slideTweener.play();
				}
				else if (indexOfLastCenter.Equals(index))
				{
					slideTweener.playReverse();
				}
			}
		}

		internal override void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			switch (state)
			{
			case global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS:
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
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
				if (showScaleTweener == null)
				{
					initAnimation();
				}
				showScaleTweener.play();
				if (isFirstNavigation())
				{
					showColorTweener.onEnd = onShowAnimationEnd;
				}
				showColorTweener.play();
				break;
			case global::Percent.View.ViewLifeCycle.Status.HIDE:
				showScaleTweener.startDelay = 0f;
				showScaleTweener.playReverse();
				showColorTweener.startDelay = 0f;
				break;
			}
		}

		private bool isFirstNavigation()
		{
			int obj = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			if (index.Equals(obj))
			{
				return true;
			}
			return false;
		}

		public void onShowAnimationEnd()
		{
			render.color = global::UnityEngine.Color.white;
		}
	}
}
