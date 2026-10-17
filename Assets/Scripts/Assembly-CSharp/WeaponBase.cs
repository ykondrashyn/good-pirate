public class WeaponBase : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	protected global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	protected global::UnityEngine.Transform fireTrans;

	[global::UnityEngine.SerializeField]
	protected Health targetEnemy;

	public ShipBase myShip;

	protected ShipState state;

	public bool isPause;

	protected ELayerType myType;

	public Health Target => targetEnemy;

	protected virtual void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		modelTrans = transform.Find("Model");
	}

	public virtual void SetLayer(ELayerType type)
	{
		switch (type)
		{
		case ELayerType.Enemy:
			base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("Enemy");
			break;
		case ELayerType.Player:
			base.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("Player");
			break;
		}
		myType = type;
	}

	public virtual void Fire()
	{
	}

	public void SetTarget(Health target)
	{
		targetEnemy = target;
		AfterSetTarget();
	}

	public void SetState(ShipState state)
	{
		this.state = state;
		switch (this.state)
		{
		case ShipState.Combat:
			CombatMode();
			break;
		case ShipState.Follow:
			FollowMode();
			break;
		}
	}

	protected virtual void CombatMode()
	{
	}

	protected virtual void FollowMode()
	{
	}

	protected virtual void AfterSetTarget()
	{
	}

	public virtual bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		return false;
	}
}
