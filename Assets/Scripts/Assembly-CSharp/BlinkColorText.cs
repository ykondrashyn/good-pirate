public class BlinkColorText : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Text myText;

	public float blinkIntervalTime = 0.7f;

	public float blinkStopTime = 0.2f;

	public bool isUnScaledTime = true;

	private float blinkTotalTime;

	public global::UnityEngine.Color originalColor = global::UnityEngine.Color.white;

	public global::UnityEngine.Color targetColor = global::UnityEngine.Color.white;

	private global::UnityEngine.UI.Outline outline;

	private bool visibleOn;

	private void Awake()
	{
		myText = GetComponent<global::UnityEngine.UI.Text>();
		if (myText == null)
		{
			myText = GetComponentInChildren<global::UnityEngine.UI.Text>();
		}
		outline = GetComponent<global::UnityEngine.UI.Outline>();
	}

	private void OnEnable()
	{
		StartCoroutine("BlinkCoroutine");
	}

	private global::System.Collections.IEnumerator BlinkCoroutine()
	{
		_ = global::UnityEngine.Color.white;
		if (myText != null)
		{
			_ = myText.color;
		}
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
			if (myText != null)
			{
				myText.color = color;
			}
			if ((bool)outline)
			{
				global::UnityEngine.Color effectColor = outline.effectColor;
				effectColor.a = color.a;
				outline.effectColor = effectColor;
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

	public void Reset()
	{
		if (myText != null)
		{
			myText.color = originalColor;
		}
		if ((bool)outline)
		{
			global::UnityEngine.Color effectColor = outline.effectColor;
			effectColor.a = myText.color.a;
			outline.effectColor = effectColor;
		}
	}

	public void Stop()
	{
		StopAllCoroutines();
		Reset();
	}
}
