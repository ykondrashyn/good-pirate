namespace Percent.Tween
{
	public class ScaleTween : global::Percent.Tween.PercentTween
	{
		public global::UnityEngine.Vector3 fromScale;

		public global::UnityEngine.Vector3 toScale;

		private global::UnityEngine.RectTransform targetTrans;

		private global::UnityEngine.Vector3 deltaScale;

		private bool isSwaped;

		public bool playOnStart;

		public bool repositionAtStart;

		public global::UnityEngine.Vector3 startPosition = global::UnityEngine.Vector3.zero;

		private void Awake()
		{
			targetTrans = GetComponent<global::UnityEngine.RectTransform>();
			if (playOnStart)
			{
				play();
			}
			originalStartDelay = startDelay;
		}

		protected override void onTweenStart()
		{
			deltaScale = fromScale - toScale;
			if (repositionAtStart)
			{
				targetTrans.localPosition = startPosition;
				repositionAtStart = false;
			}
		}

		protected override void onTween()
		{
			global::UnityEngine.Vector3 localScale = fromScale - deltaScale * animCurve.Evaluate(getProgress());
			targetTrans.localScale = localScale;
		}

		protected override void onTweenEnd()
		{
			global::UnityEngine.Debug.Log("on tween end ");
			targetTrans.localScale = toScale;
			if (isSwaped)
			{
				swapFromAndToScale();
				isSwaped = false;
			}
			startDelay = originalStartDelay;
		}

		internal override void resetToBegining()
		{
			targetTrans.localScale = fromScale;
		}

		internal override void toEndFrame()
		{
			targetTrans.localScale = toScale;
		}

		internal override void playReverse()
		{
			if (!isSwaped)
			{
				swapFromAndToScale();
				isSwaped = true;
				play();
			}
		}

		private void swapFromAndToScale()
		{
			global::UnityEngine.Vector3 vector = fromScale;
			fromScale = toScale;
			toScale = vector;
		}

		public void playTween()
		{
			isCoroutineStart = false;
			play();
		}

		public void playReverseTween()
		{
			startDelay = 0f;
			playReverse();
		}
	}
}
