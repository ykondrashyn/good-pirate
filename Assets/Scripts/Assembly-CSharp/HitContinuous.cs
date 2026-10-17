public class HitContinuous : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Collider collider;

	private global::System.Collections.Generic.Dictionary<Health, float> healthDic = new global::System.Collections.Generic.Dictionary<Health, float>();

	public global::System.Action<Health> hitAction;

	public int damage;

	public float interval = 1f;

	private void Awake()
	{
		collider = GetComponent<global::UnityEngine.Collider>();
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		Health health = other.GetComponent<Health>();
		if (health == null)
		{
			health = other.GetComponentInParent<Health>();
			if (health == null)
			{
				health = other.GetComponentInChildren<Health>();
				if (health == null)
				{
					return;
				}
			}
		}
		if (!healthDic.ContainsKey(health))
		{
			if (hitAction != null)
			{
				hitAction(health);
			}
			health.Hit(damage);
			healthDic.Add(health, 0f);
		}
	}

	private void Update()
	{
		foreach (Health item in global::System.Linq.Enumerable.ToList(healthDic.Keys))
		{
			if (item != null)
			{
				healthDic[item] += global::UnityEngine.Time.deltaTime;
				if (healthDic[item] >= interval)
				{
					healthDic[item] -= interval;
					item.Hit(damage);
				}
			}
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		Health health = other.GetComponent<Health>();
		if (health == null)
		{
			health = other.GetComponentInParent<Health>();
			if (health == null)
			{
				health = other.GetComponentInChildren<Health>();
				if (health == null)
				{
					return;
				}
			}
		}
		if (health != null && healthDic.ContainsKey(health))
		{
			healthDic.Remove(health);
		}
	}

	private void OnDisable()
	{
		healthDic.Clear();
	}
}
