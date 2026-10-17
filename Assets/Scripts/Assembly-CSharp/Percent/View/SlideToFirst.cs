namespace Percent.View
{
	public class SlideToFirst : global::UnityEngine.MonoBehaviour
	{
		public global::Percent.CrossPromotion crossPromotion;

		private global::UnityEngine.UI.ScrollRect scrollRect;

		private void Awake()
		{
			global::Percent.CrossPromotion obj = crossPromotion;
			obj.onStateChange = (global::Percent.CrossPromotion.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.CrossPromotion.OnStateChange(onCrossPromotionStateChange));
			scrollRect = GetComponent<global::UnityEngine.UI.ScrollRect>();
		}

		private void onCrossPromotionStateChange(global::Percent.CrossPromotion.Status state)
		{
			if (state == global::Percent.CrossPromotion.Status.SHOW)
			{
				move();
			}
		}

		internal void move()
		{
			scrollRect.normalizedPosition = new global::UnityEngine.Vector2(0f, 1f);
		}
	}
}
