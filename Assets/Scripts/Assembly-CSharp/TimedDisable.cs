[global::UnityEngine.AddComponentMenu("EZ Object Pools/Pooled Objects/Timed Disable")]
public class TimedDisable : global::EZObjectPools.PooledObject
{
	private float timer;

	public float DisableTime;

	private void OnEnable()
	{
		timer = 0f;
	}

	private void Update()
	{
		timer += global::UnityEngine.Time.deltaTime;
		if (timer > DisableTime)
		{
			base.transform.parent = ParentPool.transform;
			base.gameObject.SetActive(value: false);
		}
	}
}
