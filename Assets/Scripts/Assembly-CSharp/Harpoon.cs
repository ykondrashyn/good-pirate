public class Harpoon : WeaponBase
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform lineStartPos;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform harpoonTrans;

	private global::UnityEngine.LineRenderer lineRenderer;

	private bool isConnecting;

	private float delay = 4f;

	private float delayTotalTime;

	protected override void Awake()
	{
		base.Awake();
		lineRenderer = GetComponent<global::UnityEngine.LineRenderer>();
	}

	private void Start()
	{
		StartCoroutine(LookAtTarget());
		ShipBase shipBase = myShip;
		shipBase.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase.beforeDestroyAction, new global::System.Action(DieAction));
	}

	public override bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		bool result = false;
		global::UnityEngine.Vector3 vector = target.position - transform.position;
		global::UnityEngine.Vector3 forward = transform.forward;
		forward.y = 0f;
		forward.Normalize();
		vector.y = 0f;
		if (global::UnityEngine.Vector3.Angle(forward, vector.normalized) < 20f)
		{
			result = true;
		}
		return result;
	}

	private void DieAction()
	{
		if (targetEnemy != null && targetEnemy.myShip != null)
		{
			targetEnemy.myShip.SetHolding(isActive: false);
		}
		if (harpoonTrans != null)
		{
			global::UnityEngine.Object.Destroy(harpoonTrans.gameObject);
		}
	}

	public override void Fire()
	{
		isConnecting = true;
		if (targetEnemy.myShip != null)
		{
			targetEnemy.myShip.SetHolding(isActive: true);
		}
		myShip.SetHolding(isActive: true);
		harpoonTrans.SetParent(null);
		StartCoroutine(Shoot());
		StartCoroutine("LineRender");
		StartCoroutine(WaitTargetDie());
	}

	private global::System.Collections.IEnumerator WaitTargetDie()
	{
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		float totalTime = 0f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.fixedDeltaTime;
			if (!(targetEnemy != null) || (targetEnemy.myShip != null && targetEnemy.myShip.IsDie))
			{
				break;
			}
			if (totalTime >= 8f)
			{
				if (targetEnemy != null && targetEnemy.myShip != null)
				{
					targetEnemy.myShip.SetHolding(isActive: false);
				}
				break;
			}
			yield return wait;
		}
		StartCoroutine(ReturnHarpoon());
	}

	private global::System.Collections.IEnumerator ReturnHarpoon()
	{
		if (harpoonTrans == null)
		{
			yield break;
		}
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		float totalTime = 0f;
		float num = 40f;
		global::UnityEngine.Vector3 startPos = harpoonTrans.position;
		float duration = lineStartPos.position.magnitude / num;
		while (!(harpoonTrans == null))
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num2 = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			global::UnityEngine.Vector3 vector = lineStartPos.transform.position - harpoonTrans.position;
			vector.y = 0f;
			vector.Normalize();
			harpoonTrans.forward = -vector;
			harpoonTrans.position = global::UnityEngine.Vector3.Lerp(startPos, lineStartPos.position, num2);
			if (num2 >= 1f)
			{
				break;
			}
			yield return wait;
		}
		isConnecting = false;
		lineRenderer.SetPosition(0, global::UnityEngine.Vector3.zero);
		lineRenderer.SetPosition(1, global::UnityEngine.Vector3.zero);
		if (harpoonTrans != null)
		{
			harpoonTrans.SetParent(lineStartPos);
		}
		myShip.SetHolding(isActive: false);
		StopCoroutine("LineRender");
	}

	private global::System.Collections.IEnumerator Shoot()
	{
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		float totalTime = 0f;
		float num = 80f;
		global::UnityEngine.Vector3 startPos = harpoonTrans.position;
		float duration = (targetEnemy.transform.position - harpoonTrans.position).magnitude / num;
		while (!(targetEnemy == null))
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num2 = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			global::UnityEngine.Vector3 vector = targetEnemy.transform.position - harpoonTrans.position;
			vector.y = 0f;
			vector.Normalize();
			global::UnityEngine.Vector3 b = targetEnemy.transform.position - vector * 4f;
			b.y = 0.3f;
			harpoonTrans.forward = vector;
			harpoonTrans.position = global::UnityEngine.Vector3.Lerp(startPos, b, num2);
			if (num2 >= 1f)
			{
				break;
			}
			yield return wait;
		}
		if (targetEnemy.myShip != null)
		{
			StartCoroutine(TargetShake(targetEnemy.myShip.modelTrans));
		}
	}

	private global::System.Collections.IEnumerator TargetShake(global::UnityEngine.Transform target)
	{
		float totalTime = 0f;
		float duration = 0.16f;
		float strength = 0.3f;
		int num = 20;
		float localY = target.localPosition.y;
		float interval = duration / (float)num;
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(interval);
		while (!(target == null))
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime >= duration)
			{
				break;
			}
			global::UnityEngine.Vector3 endValue = new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(0f - strength, strength), localY, global::UnityEngine.Random.Range(0f - strength, strength));
			global::DG.Tweening.ShortcutExtensions.DOLocalMove(target, endValue, interval);
			yield return wait;
		}
		if (target != null)
		{
			target.localPosition = new global::UnityEngine.Vector3(0f, localY, 0f);
		}
	}

	private global::System.Collections.IEnumerator LineRender()
	{
		while (true)
		{
			if (lineRenderer != null)
			{
				if ((bool)lineStartPos)
				{
					lineRenderer.SetPosition(0, lineStartPos.position);
				}
				if (harpoonTrans != null)
				{
					lineRenderer.SetPosition(1, harpoonTrans.position);
				}
			}
			yield return null;
		}
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
			if (isConnecting)
			{
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 rhs = targetEnemy.transform.position - transform.position;
			float magnitude = rhs.magnitude;
			rhs.y = 0f;
			rhs.Normalize();
			global::UnityEngine.Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float num = global::UnityEngine.Mathf.Clamp(global::UnityEngine.Vector3.Dot(forward, rhs), -1f, 1f);
			if (num < 0f)
			{
				yield return null;
				continue;
			}
			if (magnitude > 30f)
			{
				yield return null;
				continue;
			}
			_ = global::UnityEngine.Vector3.Cross(transform.forward, rhs).y;
			_ = 0f;
			global::UnityEngine.Mathf.Acos(num);
			delayTotalTime += global::UnityEngine.Time.deltaTime;
			if (delayTotalTime >= delay)
			{
				delayTotalTime -= delay;
				Fire();
			}
			yield return null;
		}
	}
}
