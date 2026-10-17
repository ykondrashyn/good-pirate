namespace Percent.Tween
{
	public class MoveXTween : global::Percent.Tween.PercentTween
	{
		public float targetLocalX;

		private global::UnityEngine.RectTransform thisTrans;

		private float distanceX;

		private float startX;

		private void Awake()
		{
			thisTrans = GetComponent<global::UnityEngine.RectTransform>();
			originalStartDelay = startDelay;
		}

		protected override void onTweenStart()
		{
			startX = thisTrans.localPosition.x;
			distanceX = startX - targetLocalX;
		}

		protected override void onTween()
		{
			float x = startX - distanceX * animCurve.Evaluate(getProgress());
			thisTrans.localPosition = new global::UnityEngine.Vector3(x, thisTrans.localPosition.y, thisTrans.localPosition.z);
		}

		protected override void onTweenEnd()
		{
			thisTrans.localPosition = new global::UnityEngine.Vector3(targetLocalX, thisTrans.localPosition.y, thisTrans.localPosition.z);
		}

		internal override void resetToBegining()
		{
			thisTrans.localPosition = new global::UnityEngine.Vector3(startX, thisTrans.localPosition.y, thisTrans.localPosition.z);
		}

		internal override void toEndFrame()
		{
			thisTrans.localPosition = new global::UnityEngine.Vector3(targetLocalX, thisTrans.localPosition.y, thisTrans.localPosition.z);
		}

		internal override void playReverse()
		{
			resetToBegining();
			startDelay = originalStartDelay;
		}
	}
}
