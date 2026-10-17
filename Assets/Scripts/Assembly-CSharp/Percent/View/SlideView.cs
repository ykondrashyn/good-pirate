namespace Percent.View
{
	public class SlideView : global::Percent.View.View
	{
		internal int index;

		internal static readonly int TERM = 1200;

		private static int instanceNum;

		private global::Percent.Tween.PercentTween showScaleTweener;

		private global::Percent.Tween.PercentTween showColorTweener;

		private readonly global::UnityEngine.Vector2 VIEW_SIZE = new global::UnityEngine.Vector2(1000f, 1500f);

		protected override void Awake()
		{
			base.Awake();
			index = ++instanceNum;
		}

		protected override void Start()
		{
			base.Start();
			initialize();
			renderTexture();
		}

		private void initialize()
		{
			initScrollView();
			initAnimation();
			initButton();
		}

		private void initScrollView()
		{
			global::Percent.View.CenterOnChild component = global::UnityEngine.GameObject.Find("TypeSlide").GetComponent<global::Percent.View.CenterOnChild>();
			component.onChangeCenterContent = (global::Percent.View.CenterOnChild.OnChangeCenterContent)global::System.Delegate.Combine(component.onChangeCenterContent, new global::Percent.View.CenterOnChild.OnChangeCenterContent(onChangeCenterContent));
			thisTrans.SetParent(global::UnityEngine.GameObject.Find("Content").transform);
			thisTrans.sizeDelta = VIEW_SIZE;
			thisTrans.localPosition = getPosOfIndex();
		}

		private global::UnityEngine.Vector2 getPosOfIndex()
		{
			int num = 0;
			return new global::UnityEngine.Vector2((global::Percent.PromotionData.crossPromotionData.resourceUrl.Length - 1) * TERM / 2 - (index - 1) * TERM, num);
		}

		internal void onChangeCenterContent(int indexOfCenter, int indexOfLastCenter)
		{
			int obj = -1;
			indexOfLastCenter.Equals(obj);
		}

		private void initAnimation()
		{
			showScaleTweener = GetComponent<global::Percent.Tween.ScaleTween>();
			showColorTweener = GetComponents<global::Percent.Tween.ColorTween>()[0];
		}

		private void initButton()
		{
			global::Percent.CrossPromotionUIEventHandler component = global::UnityEngine.GameObject.Find("CrossPromotion").GetComponent<global::Percent.CrossPromotionUIEventHandler>();
			GetComponent<global::UnityEngine.UI.Button>().onClick.AddListener(component.onSlideTypeClick);
		}

		private void renderTexture()
		{
			global::Percent.TextureLifeCycle component = global::UnityEngine.GameObject.Find("TextureLifeCycle").GetComponent<global::Percent.TextureLifeCycle>();
			int num = global::Percent.PromotionData.crossPromotionData.resourceUrl.Length;
			string imageUrl = global::Percent.PromotionData.crossPromotionData.resourceUrl[num - index];
			GetComponent<global::Percent.Http.TextureLoader>().render(imageUrl, component.onEachTextureLoad);
		}

		internal override void onViewStateChange(global::Percent.View.ViewLifeCycle.Status state)
		{
			switch (state)
			{
			case global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS:
				if (!global::Percent.PromotionData.crossPromotionData.type.Equals(global::Percent.PromotionType.SLIDE))
				{
					global::Percent.View.ViewLifeCycle obj = viewLifeCycle;
					obj.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
					base.gameObject.SetActive(value: false);
				}
				break;
			case global::Percent.View.ViewLifeCycle.Status.LOAD_FAIL:
			{
				global::Percent.View.ViewLifeCycle obj2 = viewLifeCycle;
				obj2.onStateChange = (global::Percent.View.ViewLifeCycle.OnStateChange)global::System.Delegate.Remove(obj2.onStateChange, new global::Percent.View.ViewLifeCycle.OnStateChange(onViewStateChange));
				base.gameObject.SetActive(value: false);
				break;
			}
			case global::Percent.View.ViewLifeCycle.Status.SHOW:
			{
				int num = 1;
				GetComponentsInChildren<global::Percent.Tween.ScaleTween>()[num].onEnd = onShowAnimationEnd;
				if (showScaleTweener == null)
				{
					initAnimation();
				}
				showScaleTweener.play();
				showColorTweener.play();
				render.raycastTarget = true;
				break;
			}
			case global::Percent.View.ViewLifeCycle.Status.HIDE:
				showScaleTweener.startDelay = 0f;
				showScaleTweener.onEnd = onHideAnimationEnd;
				showScaleTweener.playReverse();
				showColorTweener.startDelay = 0f;
				render.raycastTarget = false;
				break;
			}
		}

		public void onShowAnimationEnd()
		{
			GetComponent<global::Percent.View.OverlayCenterXAlphaMax>().startAnimation();
		}

		public void onHideAnimationEnd()
		{
			int num = --instanceNum;
			if (num.Equals(0))
			{
				deactivateWholeView();
			}
		}

		private void deactivateWholeView()
		{
			global::UnityEngine.GameObject.Find("TypeSlide").GetComponent<global::Percent.View.Hider>().hide();
		}
	}
}
