public class KrakenLeg : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Animator anim;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.SpriteRenderer attackRange;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Collider attackCollider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject waterSplashObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private Hit hit;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem burstParticle;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem swipeParticle;

	private Krakennon krakennon;

	private HpBar hpBar;

	public bool IsDie { get; private set; }

	public Health myHealth { get; private set; }

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		anim = GetComponentInChildren<global::UnityEngine.Animator>();
		if ((bool)attackRange)
		{
			attackRange.gameObject.SetActive(value: false);
		}
		myHealth = GetComponent<Health>();
		krakennon = GetComponentInChildren<Krakennon>();
		myHealth.AddDieAction(Die);
		if (hit != null)
		{
			Hit obj = hit;
			obj.hitAction = (global::System.Action<Health>)global::System.Delegate.Combine(obj.hitAction, new global::System.Action<Health>(HitAddForce));
		}
		if (EnemySpawnManager.Instance != null)
		{
			EnemySpawnManager.Instance.AddOnlyHealth(myHealth);
		}
	}

	private void Start()
	{
		if (hpBar == null && krakennon != null)
		{
			hpBar = HpBarManager.Instance.Generate(ELayerType.Enemy);
			if (hpBar != null)
			{
				hpBar.SetGauge(myHealth.HpRatio);
			}
			myHealth.AddHealAction(HpBarRefresh);
			myHealth.AddHitAction(HpBarRefresh);
			myHealth.AddHpSetAction(HpBarRefresh);
		}
	}

	private void OnDestroy()
	{
		if ((bool)hpBar)
		{
			global::UnityEngine.Object.Destroy(hpBar.gameObject);
		}
	}

	private void LateUpdate()
	{
		if ((bool)hpBar)
		{
			hpBar.SetPosition(transform.position + new global::UnityEngine.Vector3(0f, -1f, 0f));
		}
	}

	private void HpBarRefresh()
	{
		if ((bool)hpBar)
		{
			hpBar.SetGauge(myHealth.HpRatio);
		}
	}

	public void SetDamage(int damage)
	{
		if (hit != null)
		{
			hit.damage = damage;
		}
		if (krakennon != null)
		{
			krakennon.damage = damage;
			StartCoroutine(LookAtPlayer());
		}
	}

	private global::System.Collections.IEnumerator LookAtPlayer()
	{
		float totalTime = 0f;
		float coolTime = 4f;
		while (true)
		{
			if (GameManager.Instance.playerShip != null)
			{
				global::UnityEngine.Vector3 vector = GameManager.Instance.playerShip.transform.position - transform.position;
				if (vector.magnitude > 45f)
				{
					yield return null;
					continue;
				}
				vector.y = 0f;
				transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime * 3f);
				totalTime += global::UnityEngine.Time.deltaTime;
				if (totalTime >= coolTime)
				{
					totalTime -= coolTime;
					AttackTrigger(3);
				}
			}
			yield return null;
		}
	}

	private void HitAddForce(Health health)
	{
		if ((bool)health.myShip)
		{
			health.myShip.rigidbody.velocity = global::UnityEngine.Vector3.zero;
			health.myShip.rigidbody.AddForce(transform.forward * 10f, global::UnityEngine.ForceMode.VelocityChange);
		}
	}

	[global::UnityEngine.ContextMenu("attack")]
	public void AttackTrigger(int type)
	{
		anim.SetTrigger("Attack");
		anim.SetInteger("AttackType", type);
		if ((bool)attackRange)
		{
			attackRange.gameObject.SetActive(value: true);
		}
	}

	public void ColliderOn()
	{
		if ((bool)attackCollider)
		{
			attackCollider.enabled = true;
		}
		if ((bool)attackRange)
		{
			global::UnityEngine.Color color = attackRange.color;
			color.a = 0f;
			global::DG.Tweening.DOTweenModuleSprite.DOColor(attackRange, color, 0.2f);
		}
		if (burstParticle != null)
		{
			burstParticle.Emit(60);
		}
	}

	public void AttackOff()
	{
		if (attackRange != null)
		{
			global::UnityEngine.Color color = attackRange.color;
			color.a = 1f;
			attackRange.color = color;
			attackRange.gameObject.SetActive(value: false);
		}
		if (attackCollider != null && attackCollider.enabled)
		{
			attackCollider.enabled = false;
		}
		StartCoroutine(DestroyRutine());
	}

	private global::System.Collections.IEnumerator DestroyRutine()
	{
		yield return new global::UnityEngine.WaitForSeconds(3f);
		global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, -40f, 0.8f);
		yield return new global::UnityEngine.WaitForSeconds(0.3f);
		waterSplashObj.SetActive(value: true);
		yield return new global::UnityEngine.WaitForSeconds(1f);
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Die()
	{
		if (!IsDie)
		{
			IsDie = true;
			global::DG.Tweening.ShortcutExtensions.DOKill(modelTrans);
			anim.SetTrigger("Die");
			if (attackRange != null)
			{
				attackRange.enabled = false;
				attackRange.gameObject.SetActive(value: false);
			}
			StartCoroutine(DieAction());
		}
	}

	private global::System.Collections.IEnumerator DieAction()
	{
		while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
		{
			yield return null;
		}
		while (!(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		yield return new global::UnityEngine.WaitForSeconds(1f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, -10f, 0.6f), delegate
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		});
	}
}
