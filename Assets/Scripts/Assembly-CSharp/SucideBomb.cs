public class SucideBomb : WeaponBase
{
	private bool isCollision;

	public override bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		return true;
	}

	public override void SetLayer(ELayerType type)
	{
		switch (type)
		{
		case ELayerType.Enemy:
			base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("EnemyProjectile");
			break;
		case ELayerType.Player:
			base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("PlayerProjectile");
			break;
		}
		myType = type;
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		if (!isCollision)
		{
			ShipBase shipBase = other.gameObject.GetComponent<ShipBase>();
			if (shipBase == null)
			{
				shipBase = other.GetComponentInParent<ShipBase>();
			}
			isCollision = true;
			CameraShakeManager.Instance.CameraShakeRotate(0.5f);
			global::UnityEngine.GameObject obj = null;
			if (ObjectPoolManager.TryGetNextObject("SucideBoom", transform.position + global::UnityEngine.Vector3.up * 1f, global::UnityEngine.Quaternion.identity, out obj))
			{
				obj.transform.localScale = global::UnityEngine.Vector3.one * 2f;
			}
			shipBase.myHealth.Hit(myShip.Damage);
			myShip.ShipDestroy(dropItem: false);
		}
	}
}
