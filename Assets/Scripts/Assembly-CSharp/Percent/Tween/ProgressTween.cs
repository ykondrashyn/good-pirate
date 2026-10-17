namespace Percent.Tween
{
	public class ProgressTween : global::Percent.Tween.PercentTween
	{
		public global::UnityEngine.UI.Image image;

		public global::UnityEngine.Events.UnityEvent endTweenEvent;

		private void Awake()
		{
			image.fillAmount = 0f;
			originalStartDelay = startDelay;
		}

		internal override void resetToBegining()
		{
			image.fillAmount = 0f;
			startDelay = originalStartDelay;
		}

		internal override void toEndFrame()
		{
			image.fillAmount = 1f;
		}

		protected override void onTweenStart()
		{
		}

		protected override void onTween()
		{
			image.fillAmount = getProgress();
		}

		protected override void onTweenEnd()
		{
			image.fillAmount = 1f;
			if (endTweenEvent != null)
			{
				endTweenEvent.Invoke();
			}
		}

		internal override void playReverse()
		{
			resetToBegining();
		}
	}
}
