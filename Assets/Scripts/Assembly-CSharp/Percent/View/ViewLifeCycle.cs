namespace Percent.View
{
	public class ViewLifeCycle : global::UnityEngine.MonoBehaviour
	{
		public enum Status
		{
			INIT = 0,
			LOAD_SUCCESS = 1,
			LOAD_FAIL = 2,
			SHOW = 3,
			HIDE = 4
		}

		internal delegate void OnStateChange(global::Percent.View.ViewLifeCycle.Status state);

		public global::Percent.CrossPromotion crossPromotion;

		private global::Percent.View.ViewLifeCycle.Status state;

		internal global::Percent.View.ViewLifeCycle.OnStateChange onStateChange;

		private global::Percent.View.ViewLifeCycle.Status State
		{
			set
			{
				state = value;
				if (onStateChange != null)
				{
					onStateChange(state);
				}
			}
		}

		private void Awake()
		{
			global::Percent.CrossPromotion obj = crossPromotion;
			obj.onStateChange = (global::Percent.CrossPromotion.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.CrossPromotion.OnStateChange(onCrossPromotionStateChange));
		}

		private void Start()
		{
			initialize();
		}

		private void initialize()
		{
			State = global::Percent.View.ViewLifeCycle.Status.INIT;
		}

		private void onCrossPromotionStateChange(global::Percent.CrossPromotion.Status state)
		{
			switch (state)
			{
			case global::Percent.CrossPromotion.Status.LOAD_SUCCESS:
				State = global::Percent.View.ViewLifeCycle.Status.LOAD_SUCCESS;
				break;
			case global::Percent.CrossPromotion.Status.LOAD_FAIL:
				State = global::Percent.View.ViewLifeCycle.Status.LOAD_FAIL;
				break;
			case global::Percent.CrossPromotion.Status.SHOW:
				State = global::Percent.View.ViewLifeCycle.Status.SHOW;
				break;
			case global::Percent.CrossPromotion.Status.HIDE:
				State = global::Percent.View.ViewLifeCycle.Status.HIDE;
				break;
			}
		}
	}
}
