public class Siren : Boss
{
	public Hit harpHit;

	public global::UnityEngine.GameObject harpWarnObj;

	public global::UnityEngine.GameObject empWarnObj;

	public global::UnityEngine.GameObject circleWarnObj;

	public SirenEmp sirenEmp;

	private global::UnityEngine.Animator anim;

	private int harpThrowDamage;

	private int harpDownDamage;

	private int shockDamage;

	private int playHarpDamage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem harpParticleSystem;

	private float neerDist = 3025f;

	private bool isNeverComeNear = true;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject tornadoSrc;

	protected override void Awake()
	{
		base.Awake();
		anim = GetComponentInChildren<global::UnityEngine.Animator>();
		AddPattern(12f, 100f, HarpAttack, 8f, isNeer);
		AddPattern(15f, 100f, ThrowHarp, 0f, isNeer);
		AddPattern(20f, 100f, Emp, 0f, isNeer);
		AddPattern(1f, 300f, PlayHarp, 0f, canPlayHarp);
		AddPattern(35f, 100f, MakeTornado);
	}

	private bool isNeer()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		if (playerShip != null)
		{
			if ((transform.position - playerShip.transform.position).sqrMagnitude < neerDist)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private bool canPlayHarp()
	{
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			ShipBase playerShip = GameManager.Instance.playerShip;
			if (!(playerShip != null))
			{
				return false;
			}
			global::UnityEngine.Vector3 vector = transform.position - playerShip.transform.position;
			if (!isNeverComeNear)
			{
				if (vector.sqrMagnitude > neerDist)
				{
					return true;
				}
				return false;
			}
			if (vector.sqrMagnitude < 1600f)
			{
				isNeverComeNear = false;
			}
		}
		return false;
	}

	private void DataCalculation()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(status.bossLevel / 100 + 1, 1, 10);
		MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num];
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = (int)((float)(mainShipDataData.Attack * mainData.cannonCount) / mainShipDataData.Firecooltime);
		harpThrowDamage = mainShipDataData.Health / 12;
		harpDownDamage = mainShipDataData.Health / 12;
		shockDamage = mainShipDataData.Health / 20;
		playHarpDamage = mainShipDataData.Health / 30;
		float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
		myHealth.SetHealthPoint((int)((float)num2 * 1.9f * 120f * enemyHealthFactor));
	}

	private global::System.Collections.IEnumerator Start()
	{
		DataCalculation();
		ShipBase playerShip = GameManager.Instance.playerShip;
		while (true)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				global::UnityEngine.Vector3 vector = playerShip.transform.position - transform.position;
				transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime * 2f);
			}
			yield return null;
		}
	}

	protected override void Die()
	{
		base.Die();
		if (!isDie)
		{
			isDie = true;
			anim.SetTrigger("Die");
			StartCoroutine(DieRutine());
		}
	}

	private global::System.Collections.IEnumerator DieRutine()
	{
		yield return new global::UnityEngine.WaitForSeconds(0.5f);
		while (!(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		yield return new global::UnityEngine.WaitForSeconds(1f);
		global::UnityEngine.Object.Destroy(base.gameObject);
		SpawnReward();
	}

	private void ThrowHarp()
	{
		harpHit.damage = harpThrowDamage;
		anim.SetTrigger("Attack");
		anim.SetInteger("AttackType", 1);
	}

	private void HarpAttack()
	{
		harpHit.damage = harpDownDamage;
		anim.SetTrigger("Attack");
		anim.SetInteger("AttackType", 2);
	}

	private void Emp()
	{
		harpHit.damage = shockDamage;
		anim.SetTrigger("Attack");
		anim.SetInteger("AttackType", 3);
	}

	private void PlayHarp()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		if (playerShip != null)
		{
			AudioManager.Instance.PlaySFX("boss_harp");
			playerShip.myHealth.Hit(playHarpDamage);
			harpParticleSystem.Play();
		}
	}

	private void MakeTornado()
	{
		int num = global::UnityEngine.Random.Range(4, 7);
		for (int i = 0; i < num; i++)
		{
			global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward;
			global::UnityEngine.Vector3 position = transform.position + vector * 10f;
			global::UnityEngine.Object.Instantiate(tornadoSrc, position, global::UnityEngine.Quaternion.identity).GetComponent<Tornado>().SetDirection(vector);
		}
	}
}
