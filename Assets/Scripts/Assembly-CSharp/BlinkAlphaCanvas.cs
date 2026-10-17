public class BlinkAlphaCanvas : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.CanvasGroup canvasGroup;

	public float blinkIntervalTime = 0.7f;

	public float blinkStopTime = 0.2f;

	public bool isUnScaledTime = true;

	private float blinkTotalTime;

	[global::UnityEngine.Range(0f, 1f)]
	public float originalAlpha = 1f;

	[global::UnityEngine.Range(0f, 1f)]
	public float targetAlpha;

	private bool visibleOn;

	private void Awake()
	{
		canvasGroup = GetComponent<global::UnityEngine.CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = GetComponentInChildren<global::UnityEngine.CanvasGroup>();
		}
	}

	private void OnEnable()
	{
		StartCoroutine("BlinkCoroutine");
	}

	private global::System.Collections.IEnumerator BlinkCoroutine()
	{
		_ = canvasGroup.alpha;
		object wait = ((!isUnScaledTime) ? ((object)new global::UnityEngine.WaitForSeconds(blinkStopTime)) : ((object)new global::UnityEngine.WaitForSecondsRealtime(blinkStopTime)));
		while (true)
		{
			if (isUnScaledTime)
			{
				blinkTotalTime += global::UnityEngine.Time.unscaledDeltaTime;
			}
			else
			{
				blinkTotalTime += global::UnityEngine.Time.deltaTime;
			}
			float t = global::UnityEngine.Mathf.Clamp01(blinkTotalTime / blinkIntervalTime);
			float b;
			float a;
			if (!visibleOn)
			{
				b = originalAlpha;
				a = targetAlpha;
			}
			else
			{
				b = targetAlpha;
				a = originalAlpha;
			}
			float alpha = global::UnityEngine.Mathf.Lerp(a, b, t);
			if (canvasGroup != null)
			{
				canvasGroup.alpha = alpha;
			}
			if (blinkTotalTime >= blinkIntervalTime)
			{
				blinkTotalTime = 0f;
				visibleOn = !visibleOn;
				yield return wait;
			}
			yield return null;
		}
	}

	public void Reset()
	{
		if (canvasGroup != null)
		{
			canvasGroup.alpha = originalAlpha;
		}
	}
}
