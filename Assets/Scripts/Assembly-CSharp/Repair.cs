public class Repair : WeaponBase
{
	private float coolTimeTotal;

	private float coolTime;

	private void OnEnable()
	{
		StartCoroutine(FindTarget());
	}

	private void Start()
	{
	}

	public override void SetLayer(ELayerType type)
	{
		switch (type)
		{
		case ELayerType.Enemy:
			base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("Enemy");
			myShip.targetFinder.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("PlayerProjectile");
			break;
		case ELayerType.Player:
			base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("Player");
			myShip.targetFinder.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("EnemyProjectile");
			break;
		}
		myType = type;
		myShip.targetFinder.Clear();
	}

	private global::System.Collections.IEnumerator FindTarget()
	{
		yield return null;
		while (true)
		{
			if (GameManager.Instance.IsInIsland || myShip.IsDie)
			{
				yield return null;
				continue;
			}
			coolTimeTotal += global::UnityEngine.Time.deltaTime;
			if (coolTimeTotal >= coolTime)
			{
				Health target = null;
				float ratioMin = 1f;
				while (true)
				{
					if (myShip.targetFinder.HealthList == null)
					{
						global::UnityEngine.Debug.Log("1");
						yield return null;
						continue;
					}
					foreach (Health health in myShip.targetFinder.HealthList)
					{
						if (health != null && !(health == myShip) && health.HpRatio < 1f && health.HpRatio < ratioMin)
						{
							ratioMin = health.HpRatio;
							target = health;
						}
					}
					if (target != null)
					{
						break;
					}
					yield return new global::UnityEngine.WaitForSeconds(0.3f);
				}
				global::UnityEngine.GameObject obj = null;
				if (ObjectPoolManager.TryGetNextObject("HealObject", transform.position, global::UnityEngine.Quaternion.identity, out obj))
				{
					HealObject component = obj.GetComponent<HealObject>();
					component.healAmount = myShip.Damage;
					component.Fire(target);
				}
				coolTimeTotal -= coolTime;
			}
			yield return null;
		}
	}
}
