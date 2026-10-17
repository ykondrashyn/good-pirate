[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.Rigidbody))]
public class Projectile : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	[global::UnityEngine.HideInInspector]
	public int damage;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Collider collider;

	[global::UnityEngine.SerializeField]
	protected global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem ps;

	private global::UnityEngine.Transform[] trans;

	private int enemyProjectileLayer;

	private int playerProjectileLayer;

	private ELayerType myLayerType;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		collider = GetComponent<global::UnityEngine.Collider>();
		if (modelTrans == null)
		{
			modelTrans = transform.Find("Model");
		}
		ps = GetComponentInChildren<global::UnityEngine.ParticleSystem>();
		trans = GetComponentsInChildren<global::UnityEngine.Transform>();
		enemyProjectileLayer = global::UnityEngine.LayerMask.NameToLayer("EnemyProjectile");
		playerProjectileLayer = global::UnityEngine.LayerMask.NameToLayer("PlayerProjectile");
	}

	protected virtual void OnEnable()
	{
		if (IsInvoking("SelfInactive"))
		{
			CancelInvoke("SelfInactive");
		}
		if ((bool)ps)
		{
			ps.Play();
		}
		if ((bool)modelTrans)
		{
			modelTrans.gameObject.SetActive(value: true);
		}
		if ((bool)collider)
		{
			collider.enabled = true;
		}
		if ((bool)rigidbody)
		{
			rigidbody.isKinematic = false;
		}
	}

	public virtual void Fire(global::UnityEngine.Vector3 startPos, global::UnityEngine.Vector3 endPos)
	{
	}

	public virtual void Fire()
	{
	}

	public virtual void Fire(global::UnityEngine.Vector3 dir, float power)
	{
		rigidbody.velocity = global::UnityEngine.Vector3.zero;
		rigidbody.AddForce(dir * power, global::UnityEngine.ForceMode.VelocityChange);
		CheckFloor();
	}

	protected void CheckFloor()
	{
		StartCoroutine(CheckFloorCoroutine());
	}

	protected virtual void BeforeWaterDisappear()
	{
	}

	private global::System.Collections.IEnumerator CheckFloorCoroutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (!(transform.position.y <= ConstValue.floorHeight))
		{
			yield return wait;
		}
		BeforeWaterDisappear();
		string text = "";
		switch (global::UnityEngine.Random.Range(1, 3))
		{
		case 1:
			text = "WaterSplash02";
			break;
		case 2:
			text = "WaterSplash03";
			break;
		}
		global::UnityEngine.Vector3 position = transform.position;
		position.y = 0.1f;
		global::UnityEngine.GameObject obj = null;
		ObjectPoolManager.TryGetNextObject(text, position, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj);
		obj.transform.localScale = global::UnityEngine.Vector3.one * 1.5f;
		yield return new global::UnityEngine.WaitForSeconds(2f);
		BeforeInactive();
		base.gameObject.SetActive(value: false);
	}

	protected virtual void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		Health health = other.GetComponent<Health>();
		if (health == null)
		{
			health = other.GetComponentInParent<Health>();
		}
		if ((bool)health)
		{
			Hit(transform.position, health);
		}
		else
		{
			global::UnityEngine.Debug.Log("health is NULL");
		}
	}

	protected virtual void OnCollisionEnter(global::UnityEngine.Collision other)
	{
		Health health = other.transform.GetComponent<Health>();
		if (health == null)
		{
			health = other.transform.GetComponentInParent<Health>();
		}
		if ((bool)health)
		{
			Hit(other.contacts[0].point, health);
		}
		else
		{
			global::UnityEngine.Debug.Log("health is NULL");
		}
	}

	private void Hit(global::UnityEngine.Vector3 hitPos, Health health)
	{
		if (IsInvoking("SelfInactive"))
		{
			CancelInvoke("SelfInactive");
		}
		Invoke("SelfInactive", 2f);
		CameraShakeManager.Instance.CameraShake(0.1f, 0.2f);
		if ((bool)health)
		{
			bool flag = false;
			bool flag2 = false;
			if (myLayerType == ELayerType.Player)
			{
				flag = true;
			}
			if (flag && global::UnityEngine.Random.value >= 0.7f)
			{
				flag2 = true;
				damage += damage / 2;
			}
			damage = global::UnityEngine.Mathf.Max(1, damage + (int)((float)damage * global::UnityEngine.Random.Range(-0.1f, 0.1f)));
			health.Hit(damage);
			hitPos += new global::UnityEngine.Vector3(0f, 3f, 0f);
			switch (myLayerType)
			{
			case ELayerType.Enemy:
				UI_Manager.Instance.DamageUIGenerate(EDamageType.HITME, hitPos, damage);
				break;
			case ELayerType.Player:
				if (flag2)
				{
					UI_Manager.Instance.DamageUIGenerate(EDamageType.CRITICAL, hitPos, damage);
				}
				else
				{
					UI_Manager.Instance.DamageUIGenerate(EDamageType.NORMAL, hitPos, damage);
				}
				break;
			}
			Particle();
			if ((bool)modelTrans)
			{
				modelTrans.gameObject.SetActive(value: false);
			}
			if ((bool)ps)
			{
				ps.Stop();
			}
			collider.enabled = false;
			rigidbody.velocity = global::UnityEngine.Vector3.zero;
			rigidbody.isKinematic = true;
		}
		else
		{
			global::UnityEngine.Debug.Log("health is NULL");
		}
	}

	protected virtual void BeforeInactive()
	{
	}

	private void SelfInactive()
	{
		BeforeInactive();
		base.gameObject.SetActive(value: false);
	}

	protected virtual void Particle()
	{
		ObjectPoolManager.TryGetNextObject("HitFireParticle", transform.position, global::UnityEngine.Quaternion.identity);
	}

	public void SetProejctile(ELayerType layer)
	{
		myLayerType = layer;
		int layer2 = 0;
		switch (layer)
		{
		case ELayerType.Enemy:
			layer2 = enemyProjectileLayer;
			break;
		case ELayerType.Player:
			layer2 = playerProjectileLayer;
			break;
		}
		global::UnityEngine.Transform[] array = trans;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = layer2;
		}
	}
}
