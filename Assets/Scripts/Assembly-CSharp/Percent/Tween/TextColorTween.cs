namespace Percent.Tween
{
	public class TextColorTween : global::Percent.Tween.PercentTween
	{
		public global::UnityEngine.Color fromColor;

		public global::UnityEngine.Color toColor;

		public global::UnityEngine.Events.UnityEvent endTweenEvent;

		private global::UnityEngine.UI.Text text;

		private global::UnityEngine.Color deltaColor;

		private bool isSwaped;

		private void Awake()
		{
			text = GetComponent<global::UnityEngine.UI.Text>();
			originalStartDelay = startDelay;
		}

		protected override void onTweenStart()
		{
			deltaColor = fromColor - toColor;
		}

		protected override void onTween()
		{
			global::UnityEngine.Color color = fromColor - deltaColor * animCurve.Evaluate(getProgress());
			text.color = color;
		}

		protected override void onTweenEnd()
		{
			text.color = toColor;
			if (isSwaped)
			{
				swapFromAndToColor();
				isSwaped = false;
			}
			if (endTweenEvent != null)
			{
				endTweenEvent.Invoke();
			}
			startDelay = originalStartDelay;
		}

		internal override void resetToBegining()
		{
			text.color = fromColor;
		}

		internal override void toEndFrame()
		{
			text.color = toColor;
		}

		internal override void playReverse()
		{
			if (!isSwaped)
			{
				swapFromAndToColor();
				isSwaped = true;
				play();
			}
		}

		private void swapFromAndToColor()
		{
			global::UnityEngine.Color color = fromColor;
			fromColor = toColor;
			toColor = color;
		}

		public void playTween()
		{
			play();
		}
	}
}
