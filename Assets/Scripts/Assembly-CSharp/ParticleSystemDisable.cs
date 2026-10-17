[global::UnityEngine.AddComponentMenu("EZ Object Pool/Pooled Objects/Particle System Disable")]
public class ParticleSystemDisable : global::EZObjectPools.PooledObject
{
	public global::UnityEngine.ParticleSystem Particles;

	private void Awake()
	{
		if (Particles == null)
		{
			Particles = GetComponentInChildren<global::UnityEngine.ParticleSystem>();
		}
	}

	private void OnEnable()
	{
		if (Particles == null)
		{
			global::UnityEngine.Debug.LogError("ParticleSystemDisable " + base.gameObject.name + " could not find any particle systems!");
		}
	}

	private void Update()
	{
		if (!Particles.IsAlive())
		{
			base.transform.parent = ParentPool.transform;
			base.gameObject.SetActive(value: false);
		}
	}
}
