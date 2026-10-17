public class GuideProjectile : Projectile
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] particles;

	private global::UnityEngine.Transform targetTrans;

	private float speed = 5f;

	private global::UnityEngine.Transform parent;

	public void SetTarget(global::UnityEngine.Transform trans)
	{
		targetTrans = trans;
	}

	public override void Fire()
	{
		speed = 20f;
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(FlyToTarget());
			StartCoroutine(JustGo());
			CheckFloor();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (parent == null)
		{
			parent = transform.parent;
		}
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	private void OnDisable()
	{
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	protected override void BeforeInactive()
	{
		transform.parent = parent;
	}

	private global::System.Collections.IEnumerator JustGo()
	{
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			transform.position += transform.forward * speed * global::UnityEngine.Time.fixedDeltaTime;
			if (transform.position.y > 50f)
			{
				base.gameObject.SetActive(value: false);
			}
			else if (transform.position.y < -10f)
			{
				base.gameObject.SetActive(value: false);
			}
			yield return wait;
		}
	}

	protected override void Particle()
	{
		global::UnityEngine.GameObject obj = null;
		if (ObjectPoolManager.TryGetNextObject("GuideExplosion", transform.position, global::UnityEngine.Quaternion.identity, out obj))
		{
			obj.transform.localScale = global::UnityEngine.Vector3.one * 1.6f;
		}
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	private global::System.Collections.IEnumerator FlyToTarget()
	{
		float totalTime = 0f;
		float duration = 1.6f;
		global::UnityEngine.Vector3 startForward = transform.forward;
		yield return new global::UnityEngine.WaitUntil(() => transform.position.y > 17f);
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float time = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			time = animCurve.Evaluate(time);
			if ((bool)targetTrans)
			{
				global::UnityEngine.Vector3 normalized = (targetTrans.position - transform.position).normalized;
				transform.forward = global::UnityEngine.Vector3.Slerp(startForward, normalized, time);
			}
			if (!(time >= 1f))
			{
				yield return null;
				continue;
			}
			break;
		}
	}
}
