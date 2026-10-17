public class AutoDestroy : global::UnityEngine.MonoBehaviour
{
	public float duration = 0.6f;

	public bool isUnscaleTime;

	private void OnEnable()
	{
		if (isUnscaleTime)
		{
			StartCoroutine("Timer");
		}
		else
		{
			Invoke("Die", duration);
		}
	}

	private global::System.Collections.IEnumerator Timer()
	{
		float totalTime = 0f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.unscaledDeltaTime;
			if (totalTime >= duration)
			{
				break;
			}
			yield return null;
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Die()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}
}
