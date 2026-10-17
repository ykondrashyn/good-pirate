public class BombSpawner : WeaponBase
{
	private float coolTimeTotal;

	private float coolTime = 2f;

	private bool isFireReady = true;

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		if (myShip == null)
		{
			myShip = GetComponentInParent<ShipBase>();
		}
		if (!(myShip == null))
		{
			coolTime = 15f;
			isFireReady = true;
			coolTimeTotal = coolTime;
		}
	}

	private void OnDestroy()
	{
	}

	protected override void CombatMode()
	{
		StopCoroutine("CombatRutine");
		StartCoroutine("CombatRutine");
	}

	protected override void FollowMode()
	{
		StopCoroutine("CombatRutine");
	}

	public override bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		return true;
	}

	private void LateUpdate()
	{
		if (!isFireReady)
		{
			coolTimeTotal += global::UnityEngine.Time.deltaTime;
			global::UnityEngine.Mathf.Clamp01(coolTimeTotal / coolTime);
			if (coolTimeTotal >= coolTime)
			{
				isFireReady = true;
			}
		}
	}

	public override void Fire()
	{
		if (!myShip.IsDie)
		{
			int myLevel = myShip.myLevel;
			myLevel = global::UnityEngine.Mathf.Clamp(myLevel - 1, 0, 999);
			ShipBase shipBase = EnemySpawnManager.Instance.Spawn(EShipType.SuicideBomb, myLevel + 1);
			if (ClaimManager.Instance.IsPlaying)
			{
				ClaimManager.Instance.AddEnemyHealth(shipBase.myHealth);
			}
			shipBase.transform.position = transform.position;
			shipBase.transform.rotation = transform.rotation;
			StartCoroutine(FireCoroutine());
		}
	}

	private global::System.Collections.IEnumerator FireCoroutine()
	{
		yield return new global::UnityEngine.WaitForSeconds(0.1f);
	}

	private global::System.Collections.IEnumerator CombatRutine()
	{
		while (true)
		{
			if (targetEnemy == null)
			{
				yield return null;
				continue;
			}
			if (targetEnemy.myShip != null && targetEnemy.myShip.IsDie)
			{
				yield return null;
				continue;
			}
			if (isFireReady)
			{
				Fire();
				isFireReady = false;
				coolTimeTotal = 0f;
			}
			yield return null;
		}
	}
}
