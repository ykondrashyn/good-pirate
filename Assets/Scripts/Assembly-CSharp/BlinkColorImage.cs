public class BlinkColorImage : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Image mySprite;

	public float blinkIntervalTime = 0.7f;

	public float blinkStopTime = 0.2f;

	private float blinkTotalTime;

	public float startBlinkRatio;

	public global::UnityEngine.Color originalColor = global::UnityEngine.Color.white;

	public global::UnityEngine.Color targetColor = global::UnityEngine.Color.white;

	private bool visibleOn;

	public bool isUnScaledTime = true;

	private void Awake()
	{
		mySprite = GetComponent<global::UnityEngine.UI.Image>();
		if (mySprite == null)
		{
			mySprite = GetComponentInChildren<global::UnityEngine.UI.Image>();
		}
	}

	private void OnEnable()
	{
		StartCoroutine("BlinkCoroutine");
	}

	private void OnDisable()
	{
		StopCoroutine("BlinkCoroutine");
	}

	private global::System.Collections.IEnumerator BlinkCoroutine()
	{
		_ = global::UnityEngine.Color.white;
		if (mySprite != null)
		{
			_ = mySprite.color;
		}
		blinkTotalTime = startBlinkRatio * blinkIntervalTime;
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
			global::UnityEngine.Color b;
			global::UnityEngine.Color a;
			if (!visibleOn)
			{
				b = originalColor;
				a = targetColor;
			}
			else
			{
				b = targetColor;
				a = originalColor;
			}
			global::UnityEngine.Color color = global::UnityEngine.Color.Lerp(a, b, t);
			if (mySprite != null)
			{
				mySprite.color = color;
			}
			if (blinkTotalTime >= blinkIntervalTime)
			{
				blinkTotalTime = 0f;
				visibleOn = !visibleOn;
				if (isUnScaledTime)
				{
					yield return new global::UnityEngine.WaitForSecondsRealtime(blinkStopTime);
				}
				else
				{
					yield return new global::UnityEngine.WaitForSeconds(blinkStopTime);
				}
			}
			yield return null;
		}
	}
}
