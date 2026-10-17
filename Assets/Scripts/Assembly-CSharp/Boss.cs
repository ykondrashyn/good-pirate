[global::UnityEngine.RequireComponent(typeof(Health))]
[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.Rigidbody))]
public abstract class Boss : global::UnityEngine.MonoBehaviour
{
	protected class Pattern
	{
		public float totalTime;

		public float coolTime;

		public bool isActive;

		public global::UnityEngine.Events.UnityAction action;

		public float minDistance;

		public global::System.Func<bool> IsCan;
	}

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	[global::UnityEngine.HideInInspector]
	public Health myHealth;

	public BossStatus status;

	protected bool isActive;

	protected bool isDie;

	protected global::System.Action GetOutAction;

	private global::UnityEngine.GameObject bossRangeObj;

	protected global::System.Collections.Generic.List<Boss.Pattern> patternList = new global::System.Collections.Generic.List<Boss.Pattern>();

	private global::UnityEngine.GameObject myIcon;

	private HpBar hpBar;

	private int startCombatDist = 90000;

	private int disappearCombatDist = 93025;

	protected virtual void OnEnable()
	{
		StopCoroutine("CheckPattern");
		StartCoroutine("CheckPattern");
	}

	private void HpBarRefresh()
	{
		if ((bool)hpBar)
		{
			hpBar.SetGauge(myHealth.HpRatio);
		}
	}

	protected virtual void Awake()
	{
		myHealth = GetComponent<Health>();
		transform = GetComponent<global::UnityEngine.Transform>();
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		myHealth.AddDieAction(Die);
		myIcon = UI_Manager.Instance.ui_worldMap.GenerateBoss();
		global::UnityEngine.Vector3 position = transform.position;
		myIcon.GetComponent<global::UnityEngine.RectTransform>().anchoredPosition = new global::UnityEngine.Vector2(position.x, position.z);
		global::UnityEngine.UI.Image[] componentsInChildren = myIcon.GetComponentsInChildren<global::UnityEngine.UI.Image>();
		if (componentsInChildren.Length != 0)
		{
			componentsInChildren[componentsInChildren.Length - 1].sprite = BossDataManager.Instance.GetBossData(status.bossType).bossIcon;
		}
		myHealth.AddDieAction(delegate
		{
			if (myIcon != null)
			{
				global::UnityEngine.Object.Destroy(myIcon.gameObject);
			}
			global::UnityEngine.GameObject obj = UI_Manager.Instance.ui_worldMap.GenerateBossClear();
			global::UnityEngine.Vector3 position2 = status.position;
			obj.GetComponent<global::UnityEngine.RectTransform>().anchoredPosition = new global::UnityEngine.Vector2(position2.x, position2.z);
			obj.GetComponent<global::UnityEngine.UI.Image>().sprite = BossDataManager.Instance.GetBossData(status.bossType).bossIcon;
			if (GameManager.Instance.IsBossFighting)
			{
				GameManager.Instance.IsBossFighting = false;
			}
		});
		if (hpBar == null)
		{
			hpBar = HpBarManager.Instance.Generate(ELayerType.Enemy, isBig: true);
			if (hpBar != null)
			{
				hpBar.SetGauge(myHealth.HpRatio);
			}
			myHealth.AddHealAction(HpBarRefresh);
			myHealth.AddHitAction(HpBarRefresh);
			myHealth.AddHpSetAction(HpBarRefresh);
		}
		EnemySpawnManager.Instance.AddOnlyHealth(myHealth);
		StartCoroutine(GenerateBossRange());
		StartCoroutine(CombatStatusCheck());
	}

	private void LateUpdate()
	{
		if ((bool)hpBar)
		{
			hpBar.SetPosition(transform.position + new global::UnityEngine.Vector3(0f, -1f, 0f));
		}
	}

	private global::System.Collections.IEnumerator GenerateBossRange()
	{
		yield return null;
		global::UnityEngine.GameObject original = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Boss Range Particle");
		bossRangeObj = global::UnityEngine.Object.Instantiate(original, status.position, global::UnityEngine.Quaternion.Euler(90f, 0f, 0f));
		myHealth.AddDieAction(delegate
		{
			if (bossRangeObj != null)
			{
				global::UnityEngine.Object.Destroy(bossRangeObj);
			}
		});
	}

	private void OnDestroy()
	{
		if (bossRangeObj != null)
		{
			global::UnityEngine.Object.Destroy(bossRangeObj);
		}
		if (GameManager.Instance != null && GameManager.Instance.IsBossFighting)
		{
			GameManager.Instance.IsBossFighting = false;
		}
		if ((bool)hpBar)
		{
			global::UnityEngine.Object.Destroy(hpBar.gameObject);
		}
	}

	public void IconDestroy()
	{
		if (myIcon != null)
		{
			global::UnityEngine.Object.Destroy(myIcon.gameObject);
		}
	}

	public virtual void Update()
	{
		if (isDie || !isActive)
		{
			return;
		}
		for (int i = 0; i < patternList.Count; i++)
		{
			patternList[i].totalTime += global::UnityEngine.Time.deltaTime;
			if (patternList[i].totalTime >= patternList[i].coolTime)
			{
				patternList[i].totalTime -= patternList[i].coolTime;
				if ((patternList[i].IsCan == null || patternList[i].IsCan()) && patternList[i].action != null && patternList[i].isActive)
				{
					patternList[i].action();
				}
			}
		}
	}

	protected virtual void Die()
	{
		if (!isDie)
		{
			UI_PatrolCombatStatus ui_patrolCombatStatus = UI_Manager.Instance.ui_mainGame.ui_patrolCombatStatus;
			if (ui_patrolCombatStatus.gameObject.activeSelf)
			{
				ui_patrolCombatStatus.gameObject.SetActive(value: false);
			}
			if (EnemySpawnManager.Instance.isSpawnPause)
			{
				EnemySpawnManager.Instance.isSpawnPause = false;
			}
			PlayerInfo.Instance.SetClearBoss(status.bossKey, isClear: true);
		}
	}

	protected void SpawnReward()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EnemyRewardData obj = ShipDataManager.Instance.enemyReward.dataArray[num];
		long num2 = 0L;
		num2 = obj.Normalgold;
		if ((bool)PlayerInfo.Instance.myData.isGetDouble)
		{
			num2 *= 2;
		}
		for (int i = 0; i < status.reward.goldBoxCount; i++)
		{
			global::UnityEngine.Vector3 position = transform.position;
			if (i != 0)
			{
				position += new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-1f, 1f), 0f, global::UnityEngine.Random.Range(-1f, 1f)).normalized * global::UnityEngine.Random.Range(3f, 15f);
			}
			position.y = 0f;
			RootBox rootBox = null;
			if (GameManager.Instance.GenerateRootBox(position, out rootBox))
			{
				rootBox.AddReward(new Reward(EMonenyType.gold, num2));
			}
		}
		if (status.reward.isOpenPopup)
		{
			global::UnityEngine.Time.timeScale = 0f;
			UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.gameObject.SetActive(value: true);
			switch (status.bossType)
			{
			case EBoss.Island:
				UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetTitle("Island");
				break;
			case EBoss.Kraken:
				UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetTitle("Kraken");
				break;
			case EBoss.Shark:
				UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetTitle("Shark");
				break;
			case EBoss.Siren:
				UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetTitle("Siren");
				break;
			case EBoss.Tree:
				UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetTitle("Tree");
				break;
			case EBoss.Turtle:
				UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.SetTitle("Turtle");
				break;
			}
			IslandRewardGrid rewardGrid = UI_Manager.Instance.ui_mainGame.ui_islandRewardPopup.RewardGrid;
			new RewardBoxStatus
			{
				boxType = status.reward.rewardType,
				level = status.bossLevel
			};
			rewardGrid.AddItemSlot(EMonenyType.diamond, status.reward.diaCount);
			rewardGrid.AddItemSlot(status.reward.rewardType, status.bossLevel, status.reward.rewardboxCount);
			for (int j = 0; j < status.reward.sailor.Count; j++)
			{
				rewardGrid.AddItemSlot(status.reward.sailor[j], 1);
			}
			PlayerInfo.Instance.SaveData();
		}
	}

	private global::System.Collections.IEnumerator CombatStatusCheck()
	{
		GetComponentInChildren<global::UnityEngine.MeshRenderer>();
		UI_PatrolCombatStatus patrolStatus = UI_Manager.Instance.ui_mainGame.ui_patrolCombatStatus;
		while (true)
		{
			ShipBase player = GameManager.Instance.playerShip;
			if (player == null)
			{
				yield return null;
				continue;
			}
			if ((player.transform.position - status.position).sqrMagnitude < (float)startCombatDist && !patrolStatus.gameObject.activeSelf)
			{
				GameManager.Instance.IsBossFighting = true;
				EnemySpawnManager.Instance.AllEnemiesGobackNow();
				EnemySpawnManager.Instance.isSpawnPause = true;
				patrolStatus.gameObject.SetActive(value: true);
				patrolStatus.SetEnemy(myHealth);
				global::UnityEngine.Sprite bossIcon = BossDataManager.Instance.GetBossData(status.bossType).bossIcon;
				patrolStatus.SetImage(bossIcon);
				while (true)
				{
					if (player == null)
					{
						yield return null;
						continue;
					}
					if ((player.transform.position - status.position).sqrMagnitude > (float)disappearCombatDist)
					{
						break;
					}
					yield return null;
				}
				GameManager.Instance.IsBossFighting = false;
				if (GetOutAction != null)
				{
					GetOutAction();
				}
				patrolStatus.gameObject.SetActive(value: false);
				Reset();
				if (EnemySpawnManager.Instance.isSpawnPause)
				{
					EnemySpawnManager.Instance.isSpawnPause = false;
				}
			}
			yield return null;
		}
	}

	protected virtual void Reset()
	{
		myHealth.SetFullHp();
	}

	private global::System.Collections.IEnumerator CheckPattern()
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.3f);
		ShipBase playerShip = GameManager.Instance.playerShip;
		while (!isDie)
		{
			if (playerShip == null)
			{
				yield return null;
				playerShip = GameManager.Instance.playerShip;
				continue;
			}
			float magnitude = (playerShip.transform.position - transform.position).magnitude;
			for (int i = 0; i < patternList.Count; i++)
			{
				if (magnitude < patternList[i].minDistance)
				{
					if (!patternList[i].isActive)
					{
						patternList[i].isActive = true;
					}
					if (!isActive)
					{
						isActive = true;
					}
				}
				else
				{
					if (i == 0 && isActive)
					{
						isActive = false;
					}
					if (patternList[i].isActive)
					{
						patternList[i].isActive = false;
					}
				}
			}
			yield return wait;
		}
	}

	protected void AddPattern(float coolTime, float minDistance, global::UnityEngine.Events.UnityAction action, float startCoolOffset = 0f, global::System.Func<bool> isCan = null)
	{
		Boss.Pattern pattern = new Boss.Pattern();
		pattern.totalTime = startCoolOffset;
		pattern.coolTime = coolTime;
		pattern.isActive = false;
		pattern.minDistance = minDistance;
		pattern.action = action;
		pattern.IsCan = isCan;
		patternList.Add(pattern);
	}
}
