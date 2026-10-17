public class ParticleUnscaledTime : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.ParticleSystem particleSystems;

	private bool isUnscale = true;

	private void Awake()
	{
		particleSystems = GetComponent<global::UnityEngine.ParticleSystem>();
	}

	private void Update()
	{
		if (global::UnityEngine.Time.timeScale < 1f)
		{
			isUnscale = true;
		}
		else if (isUnscale)
		{
			isUnscale = false;
			StopCoroutine(LateUpdateCoroutine());
		}
	}

	public void Play()
	{
		StartCoroutine(LateUpdateCoroutine());
	}

	private void OnDisable()
	{
		particleSystems.Stop();
	}

	private global::System.Collections.IEnumerator LateUpdateCoroutine()
	{
		isUnscale = true;
		particleSystems.Play();
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		while (true)
		{
			particleSystems.Simulate(global::UnityEngine.Time.unscaledDeltaTime, withChildren: true, restart: false);
			yield return wait;
		}
	}
}
