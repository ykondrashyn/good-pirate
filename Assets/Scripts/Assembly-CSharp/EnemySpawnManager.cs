public class EnemySpawnManager : global::UnityEngine.MonoBehaviour
{
	public static EnemySpawnManager Instance;

	public global::System.Collections.Generic.List<Health> allEnemyHealthList = new global::System.Collections.Generic.List<Health>();

	public global::System.Collections.Generic.List<ShipBase> spawnEnemyList = new global::System.Collections.Generic.List<ShipBase>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject ghostParticleSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject ghostHandSrc;

	public bool isSpawnPause = true;

	private int islandLayer;

	[global::UnityEngine.SerializeField]
	public IslandSheetData islandSheet;

	private MainShipData mainShipData;

	private int spawnLevel;

	private global::System.Collections.Generic.List<EShipType> shipList = new global::System.Collections.Generic.List<EShipType>();

	public int SpawnLevel
	{
		get
		{
			return spawnLevel;
		}
		set
		{
			if (value != spawnLevel)
			{
				spawnLevel = value;
			}
		}
	}

	public float EnemyHealthFactor
	{
		get
		{
			float result = 1f;
			global::System.Collections.Generic.List<ESailor> skill_enemyHealthDown = SailorDataManager.Instance.skill_enemyHealthDown;
			if (skill_enemyHealthDown.Count > 0)
			{
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_enemyHealthDown[0]);
				if (mySailorData.isHave)
				{
					switch (mySailorData.level)
					{
					case 1:
						result = 0.9f;
						break;
					case 2:
						result = 0.85f;
						break;
					case 3:
						result = 0.8f;
						break;
					}
				}
			}
			return result;
		}
	}

	public bool IsCanSpawnLimit => spawnEnemyList.Count < 15;

	private void Awake()
	{
		Instance = this;
		mainShipData = global::UnityEngine.Resources.Load<MainShipData>("Data/MainShipData");
	}

	private void Start()
	{
		StartCoroutine(SpawnEnemy());
		islandLayer = global::UnityEngine.LayerMask.GetMask("Island");
		shipList.Add(EShipType.Normal);
		shipList.Add(EShipType.Cannon);
		shipList.Add(EShipType.Rapid);
		shipList.Add(EShipType.SuicideBomb);
		shipList.Add(EShipType.Guide);
		shipList.Add(EShipType.Multi);
	}

	private global::System.Collections.IEnumerator SpawnEnemy()
	{
		yield return null;
		yield return new global::UnityEngine.WaitUntil(() => GameManager.Instance.playerShip != null);
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.Transform playerTrans = playerShip.transform;
		float coolTime = 34f;
		float num = 5f;
		float totalTime = coolTime - num;
		while (true)
		{
			if (!PlayerInfo.Instance.myData.isWatchedTutorial)
			{
				yield return null;
				continue;
			}
			if ((GameManager.Instance != null && GameManager.Instance.IsInIsland) || isSpawnPause)
			{
				yield return null;
				continue;
			}
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
			}
			if (playerTrans == null)
			{
				playerTrans = playerShip.transform;
			}
			if (playerTrans == null)
			{
				yield return null;
				continue;
			}
			totalTime += global::UnityEngine.Time.deltaTime;
			int level = global::UnityEngine.Mathf.Clamp(SpawnLevel / 100 + 1, 1, 10);
			if (totalTime >= coolTime)
			{
				totalTime -= coolTime;
				float levelfactor = mainShipData.dataArray[(int)PlayerInfo.Instance.myData.mainShipLevel - 1].Levelfactor;
				int num2 = (int)(levelfactor / 1f);
				float num3 = levelfactor - 1f * (float)num2;
				num3 *= 1000f;
				if (global::UnityEngine.Random.Range(0f, 1000f) <= num3)
				{
					num2++;
				}
				bool flag = false;
				for (int num4 = 0; num4 < num2; num4++)
				{
					if (!IsCanSpawnLimit)
					{
						float num5 = 0f;
						ShipBase shipBase = null;
						foreach (ShipBase spawnEnemy in spawnEnemyList)
						{
							float sqrMagnitude = (playerTrans.position - spawnEnemy.transform.position).sqrMagnitude;
							if (num5 < sqrMagnitude)
							{
								shipBase = spawnEnemy;
								num5 = sqrMagnitude;
							}
						}
						if (shipBase != null)
						{
							shipBase.ShipDestroy(dropItem: false);
						}
					}
					float num6 = 70f;
					ShipBase shipBase2 = null;
					bool flag2 = false;
					float num7 = global::UnityEngine.Random.Range(0f, 100f);
					float num8 = 12f;
					float num9 = 23f;
					float num10 = 19f;
					float num11 = 18f;
					float num12 = 6f;
					float num13 = 11f;
					EShipType shipType;
					if (!(num7 <= num8) || flag || num2 < 3)
					{
						shipType = ((num7 <= num8 + num9) ? EShipType.Normal : ((num7 <= num8 + num9 + num10) ? EShipType.Rapid : ((num7 <= num8 + num9 + num10 + num11) ? EShipType.Cannon : ((num7 <= num8 + num9 + num10 + num11 + num12) ? EShipType.Gas : ((!(num7 <= num8 + num9 + num10 + num11 + num12 + num13)) ? EShipType.Multi : EShipType.Guide)))));
					}
					else
					{
						shipType = EShipType.Main;
						flag = true;
						flag2 = true;
					}
					global::UnityEngine.Vector3 vector = global::UnityEngine.Vector3.zero;
					if (num4 != 0)
					{
						vector = global::UnityEngine.Random.Range(8, 30) * num4 * playerTrans.right * (((double)global::UnityEngine.Random.value < 0.5) ? 1f : (-1f));
					}
					global::UnityEngine.Vector3 targetPos = playerTrans.position + playerTrans.forward * num6 + vector;
					SpawnEnemyModel(playerTrans, num6, out shipBase2, targetPos);
					if (flag2)
					{
						ShipMainData mainData = ShipDataManager.Instance.GetMainData((EShipMain)level);
						if (mainData.modelSrc == null)
						{
							mainData = ShipDataManager.Instance.GetMainData(EShipMain.LEVEL_01);
						}
						ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Main);
						if (mainData.enemyModelSrc != null)
						{
							shipBase2.SpawnShipModel(mainData.enemyModelSrc, ELayerType.Enemy);
						}
						else
						{
							shipBase2.SpawnShipModel(mainData.modelSrc, ELayerType.Enemy);
						}
						shipBase2.SetBaseData(shipBaseData);
						shipBase2.SetLevel(SpawnLevel);
						ShipEnemyController component = shipBase2.GetComponent<ShipEnemyController>();
						int num14 = global::UnityEngine.Mathf.Min(num2 - num4 - 1, 4);
						num4 += num14;
						component.listNum = 0;
						for (int num15 = 0; num15 < num14; num15++)
						{
							float num16 = global::UnityEngine.Random.Range(0f, 100f);
							EShipType spawnType = ((num16 <= num8 + num9) ? EShipType.Normal : ((!(num16 <= num8 + num9 + num10)) ? ((!(num16 <= num8 + num9 + num10 + num11)) ? ((!(num16 <= num8 + num9 + num10 + num11 + num12)) ? ((!(num16 <= num8 + num9 + num10 + num11 + num12 + num13)) ? EShipType.Multi : EShipType.Guide) : EShipType.Gas) : EShipType.Cannon) : EShipType.Rapid));
							component.SpawnSubShip(1, spawnType, delegate(ShipBase ship)
							{
								ship.SetLevel(SpawnLevel);
								AddEnemyList(ship);
							});
						}
					}
					if (!flag2)
					{
						global::UnityEngine.Random.Range(0, shipList.Count);
						ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(shipType, (EShipSub)level);
						if (subShipData == null || subShipData.modelSrc == null)
						{
							subShipData = ShipDataManager.Instance.GetSubShipData(shipType, EShipSub.LEVEL_01);
						}
						ShipBaseData shipBaseData2 = ShipDataManager.Instance.GetShipBaseData(shipType);
						if (subShipData.enemyModelSrc != null)
						{
							shipBase2.SpawnShipModel(subShipData.enemyModelSrc, ELayerType.Enemy);
						}
						else
						{
							shipBase2.SpawnShipModel(subShipData.modelSrc, ELayerType.Enemy);
						}
						shipBase2.SetBaseData(shipBaseData2);
						shipBase2.SetLevel(SpawnLevel);
					}
					ShipEnemyController component2 = shipBase2.GetComponent<ShipEnemyController>();
					if (component2 != null)
					{
						component2.StartGoBackStartPositionCoroutine();
					}
					AddEnemyList(shipBase2);
				}
			}
			yield return null;
		}
	}

	public void AllEnemiesGobackNow()
	{
		foreach (ShipBase spawnEnemy in spawnEnemyList)
		{
			ShipEnemyController component = spawnEnemy.GetComponent<ShipEnemyController>();
			if (component != null)
			{
				component.StartGoBackStartPositionCoroutine(0.5f);
			}
		}
	}

	public ShipBase Spawn(EShipType spawnType, int level)
	{
		ShipBase ship = null;
		int num = level;
		num = global::UnityEngine.Mathf.Clamp(num / 100 + 1, 1, 10);
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(spawnType);
		ShipGenerator.Instance.SpawnEnemy(global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.identity, out ship);
		if (spawnType == EShipType.Main || EShipType.Elite == spawnType)
		{
			ShipMainData mainData = ShipDataManager.Instance.GetMainData((EShipMain)num);
			if (mainData.modelSrc == null)
			{
				mainData = ShipDataManager.Instance.GetMainData(EShipMain.LEVEL_01);
			}
			if (mainData.enemyModelSrc != null)
			{
				ship.SpawnShipModel(mainData.enemyModelSrc, ELayerType.Enemy);
			}
			else
			{
				ship.SpawnShipModel(mainData.modelSrc, ELayerType.Enemy);
			}
		}
		else
		{
			ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(spawnType, (EShipSub)num);
			if (subShipData == null || subShipData.modelSrc == null)
			{
				subShipData = ShipDataManager.Instance.GetSubShipData(spawnType, EShipSub.LEVEL_01);
			}
			if (subShipData.enemyModelSrc != null)
			{
				ship.SpawnShipModel(subShipData.enemyModelSrc, ELayerType.Enemy);
			}
			else
			{
				ship.SpawnShipModel(subShipData.modelSrc, ELayerType.Enemy);
			}
		}
		ship.SetBaseData(shipBaseData);
		ship.SetLevel(level);
		AddOnlyHealth(ship.myHealth);
		return ship;
	}

	private void SpawnEnemyModel(global::UnityEngine.Transform playerTrans, float spawnRange, out ShipBase shipBase, global::UnityEngine.Vector3 targetPos)
	{
		global::UnityEngine.Vector3 pos = ((!(targetPos != global::UnityEngine.Vector3.zero)) ? (playerTrans.position + playerTrans.forward * spawnRange) : targetPos);
		if (global::UnityEngine.Physics.Raycast(playerTrans.position, playerTrans.forward, out var hitInfo, spawnRange, islandLayer))
		{
			pos.x = hitInfo.point.x;
			pos.z = hitInfo.point.z;
			pos += hitInfo.normal * 10f;
		}
		pos.y = 0f;
		global::UnityEngine.Quaternion rot = global::UnityEngine.Quaternion.Euler(0f, playerTrans.rotation.eulerAngles.y + 180f, 0f);
		ShipGenerator.Instance.SpawnEnemy(pos, rot, out shipBase);
	}

	public void DestroyAllEnemies()
	{
		for (int i = 0; i < spawnEnemyList.Count; i++)
		{
			if (spawnEnemyList[i] != null)
			{
				spawnEnemyList[i].ShipDestroy(dropItem: false);
				i--;
			}
		}
		StopCoroutine(SpawnGhostRutine());
		spawnEnemyList.Clear();
	}

	public void AddEnemyList(ShipBase enemy)
	{
		if (!spawnEnemyList.Contains(enemy))
		{
			spawnEnemyList.Add(enemy);
		}
		if (!allEnemyHealthList.Contains(enemy.myHealth))
		{
			allEnemyHealthList.Add(enemy.myHealth);
		}
	}

	public void AddOnlyHealth(Health health)
	{
		if (!allEnemyHealthList.Contains(health))
		{
			allEnemyHealthList.Add(health);
		}
	}

	public void RemoveEnemyList(ShipBase enemy)
	{
		if (spawnEnemyList.Contains(enemy))
		{
			spawnEnemyList.Remove(enemy);
		}
		if (allEnemyHealthList.Contains(enemy.myHealth))
		{
			allEnemyHealthList.Remove(enemy.myHealth);
		}
	}

	public void SpawnGhost()
	{
		StartCoroutine(SpawnGhostRutine());
	}

	private global::System.Collections.IEnumerator SpawnGhostRutine()
	{
		if (!(GameManager.Instance.playerShip == null))
		{
			global::UnityEngine.Vector3 position = GameManager.Instance.playerShip.transform.position;
			global::UnityEngine.Vector3 direction = global::UnityEngine.Quaternion.Euler(0f, -10f, 0f) * global::UnityEngine.Vector3.forward;
			global::UnityEngine.Vector3 spawnPos = position + new global::UnityEngine.Vector3(-8f, 0f, 48f);
			if (global::UnityEngine.Physics.Raycast(position, direction, out var hitInfo, 70f, global::UnityEngine.LayerMask.GetMask("Island")))
			{
				global::UnityEngine.Vector3 vector = position - hitInfo.point;
				spawnPos = hitInfo.point + vector.normalized * 30f;
			}
			global::UnityEngine.Vector3 position2 = spawnPos + global::UnityEngine.Quaternion.Euler(0f, -180f, 0f) * global::UnityEngine.Vector3.forward * 10f + global::UnityEngine.Vector3.up * 11f;
			global::UnityEngine.GameObject smokeObj = global::UnityEngine.Object.Instantiate(ghostParticleSrc, position2, global::UnityEngine.Quaternion.Euler(0f, -180f, 0f));
			GameManager.Instance.AddObject(smokeObj);
			yield return new global::UnityEngine.WaitForSeconds(2f);
			if (GameManager.Instance.IsInIsland)
			{
				global::UnityEngine.Object.Destroy(smokeObj);
				yield break;
			}
			global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(ghostHandSrc, spawnPos, global::UnityEngine.Quaternion.Euler(0f, -180f, 0f));
			GameManager.Instance.AddObject(obj);
		}
	}

	public void TutorialEnemySpawn()
	{
		if (!TutorialManager.Instance.IsHadTutorial("goldbox"))
		{
			TutorialManager.Instance.SetHadTutorial("goldbox");
		}
		global::UnityEngine.Transform playerTrans = GameManager.Instance.playerShip.transform;
		ShipBase shipBase = null;
		SpawnEnemyModel(playerTrans, 22f, out shipBase, global::UnityEngine.Vector3.zero);
		ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(EShipType.Normal, EShipSub.LEVEL_01);
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Normal);
		shipBase.SpawnShipModel(subShipData.enemyModelSrc, ELayerType.Enemy);
		shipBase.SetBaseData(shipBaseData);
		shipBase.SetLevel(1);
		int num = (int)((float)shipBase.myHealth.HP_MAX * 0.5f);
		shipBase.myHealth.SetHealthPoint(num, num);
		shipBase.DamageFactor = 0f;
	}

	public void TutorialSecondEnemySpawn()
	{
		global::UnityEngine.Transform playerTrans = GameManager.Instance.playerShip.transform;
		for (int i = 0; i < 2; i++)
		{
			global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-1f, 1f), 0f, global::UnityEngine.Random.Range(-1f, 1f)).normalized * 3f;
			ShipBase shipBase = null;
			SpawnEnemyModel(playerTrans, 22f, out shipBase, global::UnityEngine.Vector3.zero);
			shipBase.transform.position += vector;
			ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(EShipType.Normal, EShipSub.LEVEL_01);
			ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(EShipType.Normal);
			shipBase.SpawnShipModel(subShipData.enemyModelSrc, ELayerType.Enemy);
			shipBase.SetBaseData(shipBaseData);
			shipBase.SetLevel(1);
			int num = (int)((float)shipBase.myHealth.HP_MAX * 0.6f);
			shipBase.myHealth.SetHealthPoint(num, num);
			shipBase.DamageFactor = 0f;
			AddEnemyList(shipBase);
		}
	}
}
