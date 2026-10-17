namespace Percent.Tween
{
	public abstract class PercentTween : global::UnityEngine.MonoBehaviour
	{
		internal delegate void OnTweenStart();

		internal delegate void OnTweenEnd();

		public global::UnityEngine.AnimationCurve animCurve;

		public float duration;

		public float startDelay;

		public bool ignoreTimeScale = true;

		internal global::Percent.Tween.PercentTween.OnTweenStart onStart;

		internal global::Percent.Tween.PercentTween.OnTweenEnd onEnd;

		protected bool isCoroutineStart;

		protected float originalStartDelay;

		private float timeSinceStartTween;

		internal void play()
		{
			if (!isCoroutineStart)
			{
				StartCoroutine(tween());
			}
		}

		internal virtual void playReverse()
		{
		}

		internal void stop()
		{
			if (isCoroutineStart)
			{
				StopCoroutine(tween());
			}
		}

		internal abstract void resetToBegining();

		internal abstract void toEndFrame();

		private global::System.Collections.IEnumerator tween()
		{
			isCoroutineStart = true;
			timeSinceStartTween = 0f;
			if (ignoreTimeScale)
			{
				if (startDelay > 0f)
				{
					yield return new global::UnityEngine.WaitForSecondsRealtime(startDelay);
				}
				else if (startDelay > 0f)
				{
					yield return new global::UnityEngine.WaitForSeconds(startDelay);
				}
			}
			onTweenStart();
			if (onStart != null)
			{
				onStart();
			}
			while (getProgress() <= 1f)
			{
				onTween();
				yield return null;
			}
			onTweenEnd();
			if (onEnd != null)
			{
				onEnd();
			}
			isCoroutineStart = false;
		}

		protected abstract void onTweenStart();

		protected abstract void onTween();

		protected abstract void onTweenEnd();

		protected float getProgress()
		{
			if (ignoreTimeScale)
			{
				timeSinceStartTween += global::UnityEngine.Time.unscaledDeltaTime;
			}
			else
			{
				timeSinceStartTween += global::UnityEngine.Time.deltaTime;
			}
			return timeSinceStartTween / duration;
		}
	}
}
