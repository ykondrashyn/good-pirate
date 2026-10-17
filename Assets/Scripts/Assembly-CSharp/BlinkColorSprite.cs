public class BlinkColorSprite : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.SpriteRenderer mySprite;

	public float blinkIntervalTime = 0.2f;

	public float blinkStopTime = 0.05f;

	private float blinkTotalTime;

	public global::UnityEngine.Color originalColor = global::UnityEngine.Color.white;

	public global::UnityEngine.Color targetColor = global::UnityEngine.Color.red;

	private bool visibleOn;

	public bool isUnScaledTime = true;

	private void Awake()
	{
		mySprite = GetComponent<global::UnityEngine.SpriteRenderer>();
		if (mySprite == null)
		{
			mySprite = GetComponentInChildren<global::UnityEngine.SpriteRenderer>();
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
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(blinkStopTime);
		if (mySprite != null)
		{
			_ = mySprite.color;
		}
		while (true)
		{
			blinkTotalTime += global::UnityEngine.Time.deltaTime;
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
				yield return wait;
			}
			yield return null;
		}
	}
}
