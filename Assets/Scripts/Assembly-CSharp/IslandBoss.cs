public class IslandBoss : Boss
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem volcanoParticle;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject volcanoSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject warnTorpedoSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject torpedoSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject circleWarnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject gasParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject gasSmokeSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cannonExploSrc;

	private IslandBossCannon[] myCannons;

	private global::System.Collections.Generic.List<ShipBase> mySpawnShipList = new global::System.Collections.Generic.List<ShipBase>();

	private int cannonDamage;

	private int volcanoDamage;

	private int volcanoGasDamage;

	private int torpedoDamage;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> warnObjList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	protected override void Awake()
	{
		base.Awake();
		myCannons = GetComponentsInChildren<IslandBossCannon>();
		AddPattern(13f, 150f, VolcanoFire);
		AddPattern(35f, 150f, SpawnShip);
		AddPattern(25f, 150f, Torpedo);
		AddPattern(50f, 150f, EmitGas);
	}

	private void DataCalculation()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(status.bossLevel / 100 + 1, 1, 10);
		MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num];
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = (int)((float)(mainShipDataData.Attack * mainData.cannonCount) / mainShipDataData.Firecooltime);
		cannonDamage = mainShipDataData.Health / 30;
		volcanoDamage = mainShipDataData.Health / 25;
		volcanoGasDamage = mainShipDataData.Health / 30;
		torpedoDamage = mainShipDataData.Health / 28;
		IslandBossCannon[] array = myCannons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].damage = cannonDamage;
		}
		float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
		myHealth.SetHealthPoint((int)((float)num2 * 1.9f * 150f * enemyHealthFactor));
	}

	private void Start()
	{
		DataCalculation();
	}

	protected override void Die()
	{
		base.Die();
		if (!isDie)
		{
			isDie = true;
			StartCoroutine(DieRutine());
		}
	}

	private global::System.Collections.IEnumerator DieRutine()
	{
		volcanoParticle.Play();
		for (int i = 0; i < myCannons.Length; i++)
		{
			if (myCannons[i] != null)
			{
				global::UnityEngine.Object.Instantiate(cannonExploSrc, myCannons[i].transform.position, global::UnityEngine.Quaternion.identity);
				global::UnityEngine.Object.Destroy(myCannons[i]);
				yield return new global::UnityEngine.WaitForSeconds(0.2f);
			}
		}
		yield return new global::UnityEngine.WaitForSeconds(0.7f);
		global::UnityEngine.Object.Destroy(base.gameObject);
		global::UnityEngine.Object.Instantiate(cannonExploSrc, transform.position + global::UnityEngine.Vector3.up * 5f, global::UnityEngine.Quaternion.identity).transform.localScale = global::UnityEngine.Vector3.one * 20f;
		SpawnReward();
	}

	private void VolcanoFire()
	{
		volcanoParticle.Play();
		int num = global::UnityEngine.Random.Range(20, 30);
		for (int i = 0; i < num; i++)
		{
			global::UnityEngine.Vector3 position = transform.position;
			global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward;
			position += vector * global::UnityEngine.Random.Range(35f, 90f);
			global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(volcanoSrc, position, global::UnityEngine.Quaternion.identity);
			Meteor component = obj.GetComponent<Meteor>();
			obj.GetComponentInChildren<Hit>().damage = volcanoDamage;
			component.StartFallingDown(global::UnityEngine.Random.Range(2f, 8f));
		}
	}

	private void SpawnShip()
	{
		global::UnityEngine.Vector3 position = transform.position;
		global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward;
		position += vector * 30f;
		position.y = 0f;
		ShipBase ship = EnemySpawnManager.Instance.Spawn(EShipType.Normal, 1000);
		ship.transform.position = position;
		ship.transform.forward = vector;
		ShipBase shipBase = ship;
		shipBase.beforeDestroyAction = (global::System.Action)global::System.Delegate.Combine(shipBase.beforeDestroyAction, (global::System.Action)delegate
		{
			if (this != null && mySpawnShipList.Contains(ship))
			{
				mySpawnShipList.Remove(ship);
			}
		});
		mySpawnShipList.Add(ship);
	}

	private void Torpedo()
	{
		StartCoroutine("TorpedoRutine");
	}

	private global::System.Collections.IEnumerator TorpedoRutine()
	{
		float offsetAngle = 15f;
		ShipBase playerShip = GameManager.Instance.playerShip;
		if (playerShip == null)
		{
			yield break;
		}
		global::UnityEngine.Vector3 centerDir = playerShip.transform.position - transform.position;
		centerDir.Normalize();
		for (int i = -2; i <= 2; i++)
		{
			global::UnityEngine.Vector3 forward = global::UnityEngine.Quaternion.Euler(0f, offsetAngle * (float)i, 0f) * centerDir;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(warnTorpedoSrc, transform.position + global::UnityEngine.Vector3.up * 0.1f, global::UnityEngine.Quaternion.identity);
			gameObject.transform.forward = forward;
			warnObjList.Add(gameObject);
		}
		yield return new global::UnityEngine.WaitForSeconds(1.5f);
		foreach (global::UnityEngine.GameObject warnObj in warnObjList)
		{
			global::UnityEngine.Object.Destroy(warnObj);
		}
		for (int j = -2; j <= 2; j++)
		{
			global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, offsetAngle * (float)j, 0f) * centerDir;
			global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(torpedoSrc, transform.position + vector * 17f, global::UnityEngine.Quaternion.identity);
			obj.GetComponent<Hit>().damage = torpedoDamage;
			obj.transform.forward = vector;
		}
	}

	private void EmitGas()
	{
		StartCoroutine(GasRutine());
	}

	private global::System.Collections.IEnumerator GasRutine()
	{
		circleWarnObj.SetActive(value: true);
		yield return new global::UnityEngine.WaitForSeconds(2f);
		circleWarnObj.SetActive(value: false);
		gasParticleObj.SetActive(value: true);
		global::UnityEngine.Object.Instantiate(gasSmokeSrc, transform.position, global::UnityEngine.Quaternion.identity).GetComponent<HitContinuous>().damage = volcanoGasDamage;
	}
}
