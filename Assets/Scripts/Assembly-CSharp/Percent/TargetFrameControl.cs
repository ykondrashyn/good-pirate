namespace Percent
{
	public class TargetFrameControl : global::UnityEngine.MonoBehaviour
	{
		public global::Percent.CrossPromotion crossPromotion;

		private int gameTargetFrame;

		private void Awake()
		{
			global::Percent.CrossPromotion obj = crossPromotion;
			obj.onStateChange = (global::Percent.CrossPromotion.OnStateChange)global::System.Delegate.Combine(obj.onStateChange, new global::Percent.CrossPromotion.OnStateChange(onCrossPromotionStateChange));
		}

		private void Start()
		{
			gameTargetFrame = global::UnityEngine.Application.targetFrameRate;
			global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(global::Percent.Config.VALUE_TARGET_FRAME_RATE);
		}

		internal void onCrossPromotionStateChange(global::Percent.CrossPromotion.Status state)
		{
			if (state == global::Percent.CrossPromotion.Status.HIDE)
			{
				Invoke("returnFrameRate", 2f);
			}
		}

		private void returnFrameRate()
		{
			global::GoodPirate113.Reconstruction.Platform.GoodPirateFrameRatePolicy.ApplyRecoveredTargetFrameRate(gameTargetFrame);
		}
	}
}
