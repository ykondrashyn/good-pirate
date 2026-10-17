public class Tree : Boss
{
	private global::UnityEngine.Animator anim;

	public global::UnityEngine.GameObject circleWarnObj;

	public Hit circleHitObj;

	public global::UnityEngine.GameObject quadWarnObj;

	public Hit quadHitObj;

	public global::UnityEngine.GameObject sawWarnObj;

	public Hit sawHitObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject branchSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject branchQuadWarnSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject branchCircleWarnSrc;

	public int damage;

	private int branchDamageVertical;

	private int branchDamageHorizontal;

	private int sawDamage;

	private int punchDamage;

	protected override void Awake()
	{
		base.Awake();
		anim = GetComponentInChildren<global::UnityEngine.Animator>();
		AddPattern(18f, 100f, TripleAttack, 10f);
		AddPattern(18f, 100f, SwipeAttack, 5f);
		AddPattern(13f, 100f, SpawnBranchOnWater, 6.5f);
		AddPattern(13f, 100f, SpawnBranchFromMe);
	}

	private void DataCalculation()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(status.bossLevel / 100 + 1, 1, 10);
		MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num];
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = (int)((float)(mainShipDataData.Attack * mainData.cannonCount) / mainShipDataData.Firecooltime);
		branchDamageVertical = mainShipDataData.Health / 10;
		branchDamageHorizontal = mainShipDataData.Health / 10;
		sawDamage = mainShipDataData.Health / 7;
		punchDamage = mainShipDataData.Health / 9;
		float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
		myHealth.SetHealthPoint((int)((float)num2 * 1.9f * 100f * enemyHealthFactor));
		sawHitObj.damage = sawDamage;
		quadHitObj.damage = sawDamage;
		circleHitObj.damage = punchDamage;
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
				transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime * 1.5f);
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
		yield return new global::UnityEngine.WaitForSeconds(4f);
		global::UnityEngine.Object.Destroy(base.gameObject);
		SpawnReward();
	}

	public void SetDamage(int dmg)
	{
		damage = dmg;
		circleHitObj.damage = damage;
		quadHitObj.damage = damage;
		sawHitObj.damage = damage;
	}

	private void AttackTrigger(int type)
	{
		anim.SetTrigger("Attack");
		anim.SetInteger("AttackType", type);
	}

	private void TripleAttack()
	{
		AttackTrigger(1);
	}

	private void SwipeAttack()
	{
		AttackTrigger(2);
	}

	private TreeBranch SpawnBranch(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot)
	{
		return global::UnityEngine.Object.Instantiate(branchSrc, pos, rot).GetComponent<TreeBranch>();
	}

	private void SpawnBranchFromMe()
	{
		StartCoroutine("FromMeRutine");
	}

	private global::System.Collections.IEnumerator FromMeRutine()
	{
		int count = global::UnityEngine.Random.Range(3, 6);
		global::UnityEngine.Quaternion[] rot = new global::UnityEngine.Quaternion[count];
		for (int i = 0; i < count; i++)
		{
			rot[i] = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			global::UnityEngine.Vector3 vector = rot[i] * global::UnityEngine.Vector3.forward;
			global::UnityEngine.Object.Destroy(global::UnityEngine.Object.Instantiate(branchQuadWarnSrc, transform.position + vector * 60f + global::UnityEngine.Vector3.up * 0.1f, rot[i] * global::UnityEngine.Quaternion.Euler(90f, 0f, 0f)), 2.2f);
		}
		yield return new global::UnityEngine.WaitForSeconds(2.2f);
		AudioManager.Instance.PlaySFX("tree_branch");
		for (int j = 0; j < count; j++)
		{
			SpawnBranch(transform.position + global::UnityEngine.Vector3.up * 2f, rot[j]).SetDamage(branchDamageHorizontal);
		}
	}

	private void SpawnBranchOnWater()
	{
		StartCoroutine("FromWater");
	}

	private global::System.Collections.IEnumerator FromWater()
	{
		int count = global::UnityEngine.Random.Range(5, 8);
		global::UnityEngine.Vector3[] spawnPos = new global::UnityEngine.Vector3[count];
		for (int i = 0; i < count; i++)
		{
			global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward;
			spawnPos[i] = GameManager.Instance.playerShip.transform.position + vector * global::UnityEngine.Random.Range(0f, 45f);
			spawnPos[i].y = 0.1f;
			global::UnityEngine.Object.Destroy(global::UnityEngine.Object.Instantiate(branchCircleWarnSrc, spawnPos[i], global::UnityEngine.Quaternion.Euler(90f, 0f, 0f)), 2.2f);
		}
		yield return new global::UnityEngine.WaitForSeconds(2.2f);
		AudioManager.Instance.PlaySFX("tree_branch");
		for (int j = 0; j < count; j++)
		{
			SpawnBranch(spawnPos[j] + global::UnityEngine.Vector3.up * -1f, global::UnityEngine.Quaternion.Euler(-90f, global::UnityEngine.Random.Range(0f, 360f), 0f)).SetDamage(branchDamageVertical);
		}
	}
}
