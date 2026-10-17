public class Kraken : Boss
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject warningSrc;

	private const float firstPattern = 100f;

	private const float secondPattern = 60f;

	private const float thridPattern = 30f;

	private global::System.Collections.Generic.List<KrakenLeg> legList = new global::System.Collections.Generic.List<KrakenLeg>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.Header("=================")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject legSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject legHammerSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject legCannonSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject krakenMiniSrc;

	[global::UnityEngine.Header("=================")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject destroyParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject destroyParticleSrc;

	private int krakenLegHealth;

	private int swipeDamage;

	private int hammerDamage;

	private int cannonDamage;

	private int miniKrakenDamage;

	private int miniKrakenHealth;

	protected override void Awake()
	{
		base.Awake();
		AddPattern(12f, 100f, SpawnBelowPlayer);
		AddPattern(30f, 60f, SpawnFrontPlayer);
		AddPattern(7f, 60f, SpawnMiniKraken);
	}

	private void DataCalculation()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(status.bossLevel / 100 + 1, 1, 10);
		MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num];
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = mainShipDataData.Attack * mainData.cannonCount;
		int num3 = (int)((float)num2 / mainShipDataData.Firecooltime);
		cannonDamage = mainShipDataData.Health / 35;
		swipeDamage = mainShipDataData.Health / 12;
		hammerDamage = mainShipDataData.Health / 10;
		miniKrakenDamage = mainShipDataData.Health / 25;
		float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
		krakenLegHealth = (int)((float)num2 * 8f * enemyHealthFactor);
		miniKrakenHealth = (int)((float)num2 * 1.5f * enemyHealthFactor);
		myHealth.SetHealthPoint((int)((float)num3 * 1.9f * 80f * enemyHealthFactor));
	}

	private global::System.Collections.IEnumerator Start()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		DataCalculation();
		SpawnKrakennon();
		while (true)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
			}
			else
			{
				global::UnityEngine.Vector3 vector = playerShip.transform.position - transform.position;
				transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime);
				yield return null;
			}
		}
	}

	public void SpawnKrakennon()
	{
		KrakenLeg krakenLeg = SpawnLeg(transform.position + new global::UnityEngine.Vector3(-15f, 0f, -15f), global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f), 3);
		krakenLeg.SetDamage(cannonDamage);
		legList.Add(krakenLeg);
		krakenLeg = SpawnLeg(transform.position + new global::UnityEngine.Vector3(-15f, 0f, 15f), global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f), 3);
		krakenLeg.SetDamage(cannonDamage);
		legList.Add(krakenLeg);
		krakenLeg = SpawnLeg(transform.position + new global::UnityEngine.Vector3(15f, 0f, -15f), global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f), 3);
		krakenLeg.SetDamage(cannonDamage);
		legList.Add(krakenLeg);
		krakenLeg = SpawnLeg(transform.position + new global::UnityEngine.Vector3(15f, 0f, 15f), global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f), 3);
		krakenLeg.SetDamage(cannonDamage);
		legList.Add(krakenLeg);
	}

	protected override void Reset()
	{
		base.Reset();
		foreach (KrakenLeg leg in legList)
		{
			if (leg != null)
			{
				global::UnityEngine.Object.Destroy(leg.gameObject);
			}
		}
		legList.Clear();
		SpawnKrakennon();
	}

	protected override void Die()
	{
		base.Die();
		if (isDie)
		{
			return;
		}
		isDie = true;
		StopCoroutine("CheckPattern");
		foreach (KrakenLeg leg in legList)
		{
			if (leg != null)
			{
				leg.Die();
			}
		}
		StartCoroutine(DieAction());
	}

	private void OnDestroy()
	{
		foreach (KrakenLeg leg in legList)
		{
			if (leg != null)
			{
				global::UnityEngine.Object.Destroy(leg.gameObject);
			}
		}
	}

	private global::System.Collections.IEnumerator DieAction()
	{
		global::DG.Tweening.ShortcutExtensions.DOShakePosition(modelTrans, 3f, 0.7f, 100);
		for (int i = 0; i < 10; i++)
		{
			global::UnityEngine.Vector3 position = transform.position + new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-15f, 15f), global::UnityEngine.Random.Range(10f, 20f), global::UnityEngine.Random.Range(-15f, 15f));
			global::UnityEngine.Object.Instantiate(destroyParticleSrc, position, global::UnityEngine.Quaternion.identity).transform.localScale = global::UnityEngine.Vector3.one * 5f;
			yield return new global::UnityEngine.WaitForSeconds(global::UnityEngine.Random.Range(0.2f, 0.4f));
		}
		modelTrans.gameObject.SetActive(value: false);
		destroyParticleObj.SetActive(value: true);
		yield return new global::UnityEngine.WaitForSeconds(3f);
		SpawnReward();
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void SpawnMiniKraken()
	{
		AudioManager.Instance.PlaySFX("kraken_mini_spawn");
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.Vector3 vector = -playerShip.transform.forward * 15f + playerShip.transform.right * global::UnityEngine.Random.Range(-15, 15);
		global::UnityEngine.Vector3 position = playerShip.transform.position + vector;
		global::UnityEngine.Object.Instantiate(krakenMiniSrc, position, global::UnityEngine.Quaternion.identity);
	}

	private void SpawnFrontPlayer()
	{
		StartCoroutine(FrontPlayerRutine());
	}

	private global::System.Collections.IEnumerator FrontPlayerRutine()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.GameObject[] warningObjs = new global::UnityEngine.GameObject[3];
		global::UnityEngine.Vector3[] pos = new global::UnityEngine.Vector3[3];
		global::UnityEngine.Quaternion[] rot = new global::UnityEngine.Quaternion[3];
		KrakenLeg[] leg = new KrakenLeg[3];
		float num = 30f;
		pos[0] = playerShip.transform.position + playerShip.transform.forward * num;
		pos[1] = playerShip.transform.position + playerShip.transform.forward * global::UnityEngine.Random.Range(num, num + 20f) + playerShip.transform.right * global::UnityEngine.Random.Range(20f, 15f);
		pos[2] = playerShip.transform.position + playerShip.transform.forward * global::UnityEngine.Random.Range(num, num + 20f) + playerShip.transform.right * (0f - global::UnityEngine.Random.Range(20f, 15f));
		rot[0] = playerShip.transform.rotation * global::UnityEngine.Quaternion.Euler(0f, 180f, 0f);
		rot[1] = playerShip.transform.rotation * global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
		rot[2] = playerShip.transform.rotation * global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
		for (int i = 0; i < 3; i++)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(warningSrc, pos[i] + global::UnityEngine.Vector3.up * 0.1f, global::UnityEngine.Quaternion.Euler(90f, 0f, 0f));
			warningObjs[i] = gameObject;
		}
		yield return new global::UnityEngine.WaitForSeconds(3f);
		global::UnityEngine.GameObject[] array = warningObjs;
		for (int j = 0; j < array.Length; j++)
		{
			global::UnityEngine.Object.Destroy(array[j]);
		}
		for (int k = 0; k < pos.Length; k++)
		{
			leg[k] = SpawnLeg(pos[k], rot[k], (k != 0) ? 1 : 2);
			if (k == 0)
			{
				leg[k].SetDamage(swipeDamage);
			}
			else
			{
				leg[k].SetDamage(hammerDamage);
			}
		}
		yield return new global::UnityEngine.WaitForSeconds(0.7f);
		for (int l = 0; l < leg.Length; l++)
		{
			leg[l].AttackTrigger((l != 0) ? 1 : 2);
		}
	}

	private void SpawnBelowPlayer()
	{
		StartCoroutine(BelowPlayerRutine());
	}

	private global::System.Collections.IEnumerator BelowPlayerRutine()
	{
		float seconds = 3f;
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(warningSrc, playerShip.transform.position + global::UnityEngine.Vector3.up * 0.1f, global::UnityEngine.Quaternion.Euler(90f, 0f, 0f));
		global::UnityEngine.Transform transform = obj.transform;
		global::UnityEngine.Vector3 vector = (transform.position = playerShip.transform.position + global::UnityEngine.Vector3.up * 0.1f);
		global::UnityEngine.Vector3 lastPos = vector;
		global::UnityEngine.Quaternion lastRot = playerShip.transform.rotation;
		yield return new global::UnityEngine.WaitForSeconds(seconds);
		global::UnityEngine.Object.Destroy(obj);
		int type = global::UnityEngine.Random.Range(1, 3);
		KrakenLeg leg = SpawnLeg(lastPos, lastRot, type);
		switch (type)
		{
		case 1:
			leg.SetDamage(swipeDamage);
			break;
		case 2:
			leg.SetDamage(hammerDamage);
			break;
		case 3:
			leg.SetDamage(cannonDamage);
			break;
		}
		yield return new global::UnityEngine.WaitForSeconds(0.7f);
		leg.AttackTrigger(type);
	}

	private KrakenLeg SpawnLeg(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, int type)
	{
		AudioManager.Instance.PlaySFX("kraken_leg_spawn");
		global::UnityEngine.GameObject original = null;
		switch (type)
		{
		case 1:
			original = legSrc;
			break;
		case 2:
			original = legHammerSrc;
			break;
		case 3:
			original = legCannonSrc;
			break;
		}
		KrakenLeg component = global::UnityEngine.Object.Instantiate(original, pos, rot).GetComponent<KrakenLeg>();
		component.myHealth.SetHealthPoint(krakenLegHealth);
		legList.Add(component);
		return component;
	}
}
