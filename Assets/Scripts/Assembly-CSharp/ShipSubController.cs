[global::UnityEngine.RequireComponent(typeof(ShipBase))]
public class ShipSubController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	public ShipState state;

	public global::UnityEngine.Transform targetTrans;

	public Health targetEnemy;

	[global::UnityEngine.HideInInspector]
	public ShipBase myCaptain;

	[global::UnityEngine.SerializeField]
	private ECombatStyle combatStyle;

	public bool isFollowPause;

	private global::UnityEngine.Vector3 startPos;

	[global::UnityEngine.HideInInspector]
	public ShipBase myShip { get; private set; }

	public global::UnityEngine.Vector3 offset { get; private set; }

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void OnEnable()
	{
		StopCoroutine("Follow");
		StartCoroutine("Follow");
	}

	private void Start()
	{
		ShipBase shipBase = myShip;
		shipBase.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase.beforeDestroyAction, new global::System.Action(Die));
		myShip.myHealth.AddDieAction(Die);
	}

	public void SetBaseData(ShipBaseData data)
	{
		myShip.SetBaseData(data);
		combatStyle = data.combatStyle;
	}

	public void SetTarget(global::UnityEngine.Transform target, global::UnityEngine.Vector3 offset)
	{
		targetTrans = target;
		this.offset = offset;
		if (target != null)
		{
			startPos = targetTrans.TransformVector(offset) + targetTrans.position;
		}
		StopCoroutine("Follow");
		StartCoroutine("Follow");
	}

	public void SetTarget(ShipBase captain, global::UnityEngine.Vector3 offset)
	{
		myCaptain = captain;
		if (!(myCaptain == null))
		{
			SetTarget(myCaptain.transform, offset);
		}
	}

	public void SetState(ShipState state)
	{
		this.state = state;
		if ((bool)myShip)
		{
			myShip.SetState(state);
		}
	}

	private global::System.Collections.IEnumerator Follow()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		yield return null;
		while (true)
		{
			if (GameManager.Instance != null && GameManager.Instance.IsInIsland)
			{
				yield return wait;
				continue;
			}
			if (isFollowPause)
			{
				yield return wait;
				continue;
			}
			global::UnityEngine.Vector3 vector = ((!(targetTrans != null)) ? startPos : (targetTrans.TransformVector(offset) + targetTrans.position));
			float magnitude = (vector - transform.position).magnitude;
			if (magnitude > 150f)
			{
				transform.position = vector;
				myShip.rigidbody.velocity = global::UnityEngine.Vector3.zero;
				ShipGenerator.Instance.JustSpawnParticle(transform.position);
				if (!GameManager.Instance.IsInIsland)
				{
					AudioManager.Instance.PlaySFX("ship_spawn");
				}
			}
			switch (state)
			{
			case ShipState.Combat:
				CombatStyleMove(magnitude);
				if (magnitude < 35f)
				{
					foreach (WeaponBase weapon in myShip.weaponList)
					{
						weapon.isPause = false;
					}
					yield return wait;
					continue;
				}
				if (combatStyle == ECombatStyle.Rush)
				{
					break;
				}
				foreach (WeaponBase weapon2 in myShip.weaponList)
				{
					weapon2.isPause = true;
				}
				FollowTarget(vector, magnitude);
				break;
			case ShipState.Follow:
				myShip.isRotateMode = false;
				if (magnitude < 6f)
				{
					if (myCaptain != null)
					{
						float angleForward = myMathf.GetAngleForward(myCaptain.transform.forward);
						myShip.SetTargetAngle(angleForward);
					}
					yield return wait;
					continue;
				}
				FollowTarget(vector, magnitude);
				break;
			}
			yield return wait;
		}
	}

	private void CombatStyleMove(float distToFollowPos)
	{
		if (targetEnemy == null)
		{
			foreach (WeaponBase weapon in myShip.weaponList)
			{
				if (weapon.Target != null)
				{
					targetEnemy = weapon.Target;
					break;
				}
			}
			return;
		}
		global::UnityEngine.Vector3 vector = targetEnemy.transform.position - transform.position;
		vector.y = 0f;
		vector.Normalize();
		switch (combatStyle)
		{
		case ECombatStyle.LootAt:
			myShip.isRotateMode = true;
			myShip.SetTargetAngle(myMathf.GetAngleForward(vector));
			break;
		case ECombatStyle.Rush:
			myShip.isRotateMode = false;
			myShip.SetTargetAngle(myMathf.GetAngleForward(vector));
			myShip.Acceleration(myShip.acceleration);
			break;
		case ECombatStyle.Vertical:
		{
			global::UnityEngine.Vector3 vector2 = global::UnityEngine.Vector3.Cross(global::UnityEngine.Vector3.up, vector);
			global::UnityEngine.Vector3 forward = -vector2;
			if (global::UnityEngine.Vector3.Angle(transform.forward, vector2) <= 90f)
			{
				myShip.SetTargetAngle(myMathf.GetAngleForward(vector2));
			}
			else
			{
				myShip.SetTargetAngle(myMathf.GetAngleForward(forward));
			}
			myShip.isRotateMode = true;
			break;
		}
		case ECombatStyle.Nothing:
		case ECombatStyle.GoFriend:
			break;
		}
	}

	private void FollowTarget(global::UnityEngine.Vector3 targetPos, float magnitude)
	{
		float t = global::UnityEngine.Mathf.InverseLerp(0f, 20f, magnitude);
		float num = global::UnityEngine.Mathf.Lerp(1.1f, 1.5f, t);
		float angleForward = myMathf.GetAngleForward((targetPos - transform.position).normalized);
		float bonusTurnSpeed = global::UnityEngine.Mathf.Lerp(0f, 20f, t);
		myShip.bonusTurnSpeed = bonusTurnSpeed;
		myShip.SetTargetAngle(angleForward);
		myShip.Acceleration(myShip.acceleration * num);
	}

	private void Die()
	{
		if (targetTrans != null)
		{
			ShipController component = targetTrans.GetComponent<ShipController>();
			if (component != null && component.subShipList.Contains(this))
			{
				component.subShipList.Remove(this);
			}
		}
	}
}
