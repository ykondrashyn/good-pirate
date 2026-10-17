public class Guide : WeaponBase
{
	[global::UnityEngine.SerializeField]
	private float coolTime = 5f;

	private global::UnityEngine.Transform poolObjParentTrans;

	private GuideProjectile spawnedProjectile;

	[global::UnityEngine.SerializeField]
	private string missilePoolName = "GuideProjectile_0";

	private void SpawnProjectile()
	{
		if (spawnedProjectile != null)
		{
			return;
		}
		global::UnityEngine.GameObject obj = null;
		if (ObjectPoolManager.TryGetNextObject(missilePoolName, global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.identity, out obj))
		{
			if (poolObjParentTrans == null)
			{
				poolObjParentTrans = obj.transform.parent;
			}
			obj.transform.SetParent(fireTrans);
			spawnedProjectile = obj.GetComponent<GuideProjectile>();
			spawnedProjectile.transform.localPosition = global::UnityEngine.Vector3.zero;
			spawnedProjectile.transform.localScale = global::UnityEngine.Vector3.one;
			spawnedProjectile.transform.localRotation = global::UnityEngine.Quaternion.identity;
			spawnedProjectile.rigidbody.isKinematic = true;
			spawnedProjectile.SetProejctile(myType);
		}
	}

	private void Start()
	{
		SpawnProjectile();
		if (myShip.InvenShipData != null)
		{
			int shipLevel = PlayerInfo.Instance.GetShipLevel(myShip.baseData.id);
			coolTime = ShipDataManager.Instance.GetShipSheetData(myShip.baseData.id, shipLevel).fireCoolTime;
		}
		else
		{
			PlayerInfo.Instance.GetShipLevel(myShip.baseData.id);
			coolTime = ShipDataManager.Instance.GetShipSheetData(myShip.baseData.id, myShip.myLevel).fireCoolTime;
		}
	}

	private void OnDisable()
	{
		if (spawnedProjectile != null && (bool)poolObjParentTrans && spawnedProjectile.transform.parent != poolObjParentTrans)
		{
			spawnedProjectile.transform.SetParent(poolObjParentTrans);
		}
	}

	public override void Fire()
	{
		if (!myShip.IsDie)
		{
			if (spawnedProjectile == null)
			{
				SpawnProjectile();
				return;
			}
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(fireTrans, new global::UnityEngine.Vector3(-70f, 0f, 0f), 0.8f), global::DG.Tweening.Ease.Linear);
			StartCoroutine(FireDelay());
		}
	}

	private global::System.Collections.IEnumerator FireDelay()
	{
		yield return new global::UnityEngine.WaitForSeconds(1.2f);
		yield return new global::UnityEngine.WaitUntil(() => base.Target != null);
		if (spawnedProjectile != null)
		{
			spawnedProjectile.SetTarget(base.Target.transform);
			spawnedProjectile.transform.SetParent(poolObjParentTrans);
			spawnedProjectile.damage = myShip.Damage;
			spawnedProjectile.Fire();
		}
		spawnedProjectile = null;
		yield return new global::UnityEngine.WaitForSeconds(0.8f);
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(fireTrans, new global::UnityEngine.Vector3(-15f, 0f, 0f), 0.8f), global::DG.Tweening.Ease.Linear);
		yield return new global::UnityEngine.WaitForSeconds(1f);
		SpawnProjectile();
	}

	public override bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		return true;
	}

	protected override void CombatMode()
	{
		StartCoroutine("CoolTime");
	}

	protected override void FollowMode()
	{
		StopCoroutine("CoolTime");
	}

	private global::System.Collections.IEnumerator CoolTime()
	{
		float coolTimeTotal = 2f;
		while (true)
		{
			if (targetEnemy == null)
			{
				yield return null;
				continue;
			}
			if (isPause)
			{
				yield return null;
				continue;
			}
			coolTimeTotal += global::UnityEngine.Time.deltaTime;
			if (coolTimeTotal >= coolTime)
			{
				coolTimeTotal -= coolTime;
				Fire();
			}
			yield return null;
		}
	}
}
