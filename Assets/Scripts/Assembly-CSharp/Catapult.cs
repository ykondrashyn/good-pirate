public class Catapult : WeaponBase
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform leverTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform leverBowlTrans;

	private global::UnityEngine.Transform poolObjParentTrans;

	private Barrel barrel;

	private float coolTimeTotal;

	private float coolTime = 7f;

	private bool isCanReady = true;

	private void Start()
	{
		isCanReady = true;
		coolTime = ShipDataManager.Instance.GetShipSheetData(myShip.baseData.id, myShip.myLevel).fireCoolTime;
		ShipBase shipBase = myShip;
		shipBase.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase.beforeDestroyAction, (global::System.Action)delegate
		{
			if (barrel != null && poolObjParentTrans != null && barrel.transform.parent != poolObjParentTrans)
			{
				barrel.transform.SetParent(poolObjParentTrans);
				barrel.gameObject.SetActive(value: false);
			}
		});
	}

	protected override void CombatMode()
	{
		StartCoroutine("LookAtTarget");
		isCanReady = true;
	}

	protected override void FollowMode()
	{
		StopCoroutine("LookAtTarget");
	}

	private void Spawn()
	{
		global::UnityEngine.GameObject obj = null;
		if (!(barrel != null) && ObjectPoolManager.TryGetNextObject("GasBarrel", leverBowlTrans.transform.position, global::UnityEngine.Quaternion.identity, out obj))
		{
			global::UnityEngine.Transform transform = obj.transform;
			poolObjParentTrans = transform.parent;
			barrel = obj.GetComponent<Barrel>();
			barrel.SetProejctile(myType);
			barrel.damage = myShip.Damage;
			transform.SetParent(leverBowlTrans);
			transform.localRotation = global::UnityEngine.Quaternion.identity;
			transform.localScale = global::UnityEngine.Vector3.one;
		}
	}

	public override void Fire()
	{
		if (barrel == null)
		{
			Spawn();
		}
		isCanReady = false;
		StartCoroutine(ThrowBarrel());
		_ = targetEnemy.transform.position;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(leverTrans, new global::UnityEngine.Vector3(80f, 0f, 0f), 2f), global::DG.Tweening.Ease.OutElastic), delegate
		{
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOLocalRotate(leverTrans, global::UnityEngine.Vector3.zero, 2.3f), 0.6f), delegate
			{
				barrel = null;
				Spawn();
				isCanReady = true;
			});
		});
	}

	private global::System.Collections.IEnumerator ThrowBarrel()
	{
		yield return new global::UnityEngine.WaitUntil(() => leverTrans.localRotation.eulerAngles.x > 75f);
		if ((bool)barrel)
		{
			barrel.transform.SetParent(poolObjParentTrans);
		}
		if ((bool)barrel)
		{
			barrel.Fire(leverBowlTrans.position, targetEnemy.transform.position);
		}
	}

	public override bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		return true;
	}

	private global::System.Collections.IEnumerator LookAtTarget()
	{
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
			if (!isCanReady)
			{
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 to = targetEnemy.transform.position - transform.position;
			to.y = 0f;
			to.Normalize();
			if (coolTimeTotal >= coolTime)
			{
				if (global::UnityEngine.Vector3.Angle(myShip.transform.forward, to) < 10f)
				{
					coolTimeTotal -= coolTime;
					Fire();
				}
			}
			else
			{
				coolTimeTotal += global::UnityEngine.Time.deltaTime;
			}
			yield return null;
		}
	}
}
