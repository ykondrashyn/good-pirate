namespace Percent.View
{
	public class Hider : global::UnityEngine.MonoBehaviour
	{
		public global::Percent.PromotionType type;

		public global::Percent.CrossPromotion crossPromotion;

		private void Awake()
		{
			global::Percent.SessionLifeCycle component = global::UnityEngine.GameObject.Find("SessionLifeCycle").GetComponent<global::Percent.SessionLifeCycle>();
			component.onStateChange = (global::Percent.SessionLifeCycle.OnStateChange)global::System.Delegate.Combine(component.onStateChange, new global::Percent.SessionLifeCycle.OnStateChange(onSessionStateChange));
			global::Percent.CrossPromotion obj = crossPromotion;
			obj.onStateChange = (global::Percent.CrossPromotion.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.CrossPromotion.OnStateChange(onCrossPromotionStateChange));
		}

		private void onSessionStateChange(global::Percent.SessionLifeCycle.Status state)
		{
			switch (state)
			{
			case global::Percent.SessionLifeCycle.Status.ACCESS_SUCCESS:
				if (!type.Equals(global::Percent.PromotionData.crossPromotionData.type))
				{
					hide();
				}
				break;
			case global::Percent.SessionLifeCycle.Status.UUID_FAIL:
				hide();
				break;
			case global::Percent.SessionLifeCycle.Status.ACCESS_FAIL:
				hide();
				break;
			}
		}

		private void onCrossPromotionStateChange(global::Percent.CrossPromotion.Status state)
		{
			if (state == global::Percent.CrossPromotion.Status.LOAD_FAIL)
			{
				hide();
			}
		}

		public void hide()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
