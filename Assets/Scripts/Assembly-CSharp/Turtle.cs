public class Turtle : Boss
{
	private TurtleLeaf[] myLeafs;

	private TurtleFlower myFlower;

	[global::UnityEngine.SerializeField]
	private TurtleSmallFlower[] mySmallFlowers;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private HitContinuous solarBeam;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Collider[] myColliders;

	private int seedDamage;

	private int leafDamage;

	private int myGasDamage;

	private int solarBeamDamage;

	private bool asd;

	protected override void Awake()
	{
		base.Awake();
		myLeafs = GetComponentsInChildren<TurtleLeaf>();
		myFlower = GetComponentInChildren<TurtleFlower>();
		myColliders = GetComponentsInChildren<global::UnityEngine.Collider>();
		if (mySmallFlowers == null || mySmallFlowers.Length == 0)
		{
			mySmallFlowers = GetComponentsInChildren<TurtleSmallFlower>();
		}
		AddPattern(20f, 60f, AttackLeaf);
		AddPattern(40f, 80f, SolarBeam, 10f);
		AddPattern(30f, 80f, GasBurst, 20f);
		AddPattern(40f, 80f, FireFlowerSeed, 30f);
	}

	private void Start()
	{
		DataCalculation();
	}

	private void DataCalculation()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(status.bossLevel / 100 + 1, 1, 10);
		MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num];
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = (int)((float)(mainShipDataData.Attack * mainData.cannonCount) / mainShipDataData.Firecooltime);
		seedDamage = mainShipDataData.Health / 20;
		leafDamage = mainShipDataData.Health / 25;
		myGasDamage = mainShipDataData.Health / 35;
		solarBeamDamage = mainShipDataData.Health / 10;
		float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
		myHealth.SetHealthPoint((int)((float)num2 * 1.9f * 100f * enemyHealthFactor));
		myFlower.SetDamage(myGasDamage);
		TurtleSmallFlower[] array = mySmallFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].damage = seedDamage;
		}
		TurtleLeaf[] array2 = myLeafs;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SetDamage(leafDamage);
		}
		solarBeam.damage = solarBeamDamage;
	}

	protected override void Die()
	{
		base.Die();
		if (!isDie)
		{
			isDie = true;
			StartCoroutine("DieRutine");
		}
	}

	private global::System.Collections.IEnumerator DieRutine()
	{
		global::UnityEngine.Collider[] array = myColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		global::DG.Tweening.ShortcutExtensions.DOShakeScale(modelTrans, 3f, 1f, 100);
		yield return new global::UnityEngine.WaitForSeconds(4f);
		global::DG.Tweening.ShortcutExtensions.DOLocalMoveY(modelTrans, -16.7f, 2f);
		yield return new global::UnityEngine.WaitForSeconds(5f);
		global::DG.Tweening.ShortcutExtensions.DOLocalMoveY(modelTrans, -33f, 2f);
		yield return new global::UnityEngine.WaitForSeconds(1.4f);
		global::UnityEngine.GameObject obj = null;
		if (ObjectPoolManager.TryGetNextObject("WaterSplash03", transform.position + new global::UnityEngine.Vector3(0f, 0.05f, 0f), global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj))
		{
			obj.transform.localScale = global::UnityEngine.Vector3.one * 5f;
		}
		yield return new global::UnityEngine.WaitForSeconds(2f);
		SpawnReward();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void FireFlowerSeed()
	{
		asd = !asd;
		if (asd)
		{
			StartCoroutine("FireFlowerSeedRutine");
			return;
		}
		TurtleSmallFlower[] array = mySmallFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FireTrigger();
		}
	}

	private global::System.Collections.IEnumerator FireFlowerSeedRutine()
	{
		TurtleSmallFlower[] array = mySmallFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FireTrigger();
			yield return new global::UnityEngine.WaitForSeconds(0.2f);
		}
	}

	private void GasBurst()
	{
		myFlower.SetTrigger("Gas");
	}

	private void AttackLeaf()
	{
		StartCoroutine(AttackLeafRutine());
	}

	private global::System.Collections.IEnumerator AttackLeafRutine()
	{
		for (int i = 0; i < myLeafs.Length; i++)
		{
			myLeafs[i].Attack();
			yield return new global::UnityEngine.WaitForSeconds(0.4f);
		}
	}

	private void SolarBeam()
	{
		StartCoroutine("SolarBeamRutine");
	}

	private global::System.Collections.IEnumerator SolarBeamRutine()
	{
		float totalTime = 0f;
		float duration = 4f;
		ShipBase playerShip = GameManager.Instance.playerShip;
		myFlower.SetTrigger("Charging");
		while (!myFlower.anim.GetCurrentAnimatorStateInfo(0).IsName("Charging"))
		{
			yield return null;
		}
		while (true)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			totalTime += global::UnityEngine.Time.deltaTime;
			global::UnityEngine.Vector3 vector = playerShip.transform.position - transform.position;
			transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime * 3f);
			if (totalTime >= duration)
			{
				break;
			}
			yield return null;
		}
		yield return new global::UnityEngine.WaitForSeconds(1.5f);
		myFlower.SetTrigger("Attack");
		yield return new global::UnityEngine.WaitForSeconds(5f);
		myFlower.SetTrigger("End");
	}
}
