public class Hit : global::UnityEngine.MonoBehaviour
{
	public bool isOnlyOnceAttack;

	private global::UnityEngine.Collider collider;

	private global::System.Collections.Generic.HashSet<Health> healthOverlapCheck = new global::System.Collections.Generic.HashSet<Health>();

	public global::System.Action<Health> hitAction;

	public global::System.Action hittingAction;

	public int damage;

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
		if (!healthOverlapCheck.Contains(health))
		{
			if (hittingAction != null)
			{
				hittingAction();
			}
			if (hitAction != null)
			{
				hitAction(health);
			}
			health.Hit(damage);
			healthOverlapCheck.Add(health);
			if (isOnlyOnceAttack)
			{
				collider.enabled = false;
			}
		}
	}

	private void OnDisable()
	{
		healthOverlapCheck.Clear();
	}
}
