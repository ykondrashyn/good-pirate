[global::UnityEngine.RequireComponent(typeof(ShipBase))]
public class ShipEnemyController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private ShipBase myShip;

	private ShipState state;

	public global::System.Collections.Generic.List<ShipSubController> subShipList = new global::System.Collections.Generic.List<ShipSubController>();

	public Health targetEnemy;

	private global::UnityEngine.Coroutine followCoroutine;

	private global::UnityEngine.Coroutine rotateCoroutine;

	private global::UnityEngine.Coroutine combatMoveCoroutine;

	private float combatMoveTime = 4f;

	private float combatRestTime = 7f;

	private global::UnityEngine.Vector3 placeGoBack = global::UnityEngine.Vector3.zero;

	private bool isGoingBack;

	[global::UnityEngine.HideInInspector]
	public bool isNeverGoBack;

	private global::UnityEngine.Vector3 vecZero = global::UnityEngine.Vector3.zero;

	private EShipMoveType moveType;

	private UI_WorldMapSameScale patrolWorldIcon;

	private global::UnityEngine.Coroutine goBackCorutine;

	private global::System.Collections.Generic.List<global::UnityEngine.Vector3> patrolCourseList = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();

	private int coruseNum;

	private global::System.Collections.Generic.List<ShipBase> patrolSubShipList;

	private PatrolStatus myPatrolStatus;

	public int listNum;

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
	}

	private void OnEnable()
	{
		StartCoroutine(NearTargetCheck());
		myShip.myHealth.AddDieAction(RemoveEnemySpawnManager);
	}

	public void StartGoBackStartPositionCoroutine(float waitTime = 60f)
	{
		if (goBackCorutine != null)
		{
			StopCoroutine(goBackCorutine);
			goBackCorutine = null;
		}
		goBackCorutine = StartCoroutine(GoBackStartPosition(waitTime));
	}

	private global::System.Collections.IEnumerator GoBackStartPosition(float time)
	{
		yield return null;
		global::UnityEngine.Vector3 startPosition = base.transform.position;
		yield return new global::UnityEngine.WaitForSeconds(time);
		isGoingBack = true;
		while (true)
		{
			global::UnityEngine.Vector3 forward = startPosition - base.transform.position;
			forward.y = 0f;
			float angleForward = myMathf.GetAngleForward(forward);
			myShip.SetTargetAngle(angleForward);
			myShip.Acceleration(myShip.acceleration);
			if (forward.sqrMagnitude < 36f && !GetComponentInChildren<global::UnityEngine.MeshRenderer>().isVisible)
			{
				myShip.ShipDestroy(dropItem: false);
			}
			yield return null;
		}
	}

	private void RemoveEnemySpawnManager()
	{
		EnemySpawnManager.Instance.RemoveEnemyList(myShip);
		for (int i = 0; i < subShipList.Count; i++)
		{
			if (!(subShipList[i] == null))
			{
				subShipList[i].targetTrans = null;
				ShipSubController component = subShipList[i].gameObject.GetComponent<ShipSubController>();
				if (component != null)
				{
					global::UnityEngine.Object.Destroy(component);
					subShipList[i].gameObject.AddComponent<ShipEnemyController>();
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (myShip.IsDie)
		{
			if (followCoroutine != null)
			{
				StopCoroutine(followCoroutine);
			}
			if (rotateCoroutine != null)
			{
				StopCoroutine(rotateCoroutine);
			}
			if (combatMoveCoroutine != null)
			{
				StopCoroutine(combatMoveCoroutine);
			}
		}
		else
		{
			if (isGoingBack)
			{
				return;
			}
			if (placeGoBack != vecZero && !isNeverGoBack && (placeGoBack - base.transform.position).sqrMagnitude > 25600f)
			{
				bool flag = false;
				bool flag2 = false;
				if (targetEnemy == null)
				{
					flag = true;
					flag2 = true;
				}
				if (!flag && (targetEnemy.transform.position - base.transform.position).sqrMagnitude > 400f)
				{
					flag2 = true;
				}
				if (flag2)
				{
					isGoingBack = true;
					StartCoroutine(GoBackRutine());
					if (followCoroutine != null)
					{
						StopCoroutine(followCoroutine);
						followCoroutine = null;
					}
					state = ShipState.Follow;
					myShip.SetState(state);
					foreach (ShipSubController subShip in subShipList)
					{
						subShip.SetState(state);
					}
					myShip.SetState(state);
					ECombatStyle combatStyle = myShip.baseData.combatStyle;
					if (combatStyle == ECombatStyle.Vertical)
					{
						if (rotateCoroutine != null)
						{
							StopCoroutine(rotateCoroutine);
							rotateCoroutine = null;
						}
						if (combatMoveCoroutine != null)
						{
							StopCoroutine(combatMoveCoroutine);
							combatMoveCoroutine = null;
						}
					}
					return;
				}
			}
			if (targetEnemy != null)
			{
				global::UnityEngine.Vector3 vector = base.transform.position - targetEnemy.transform.position;
				if (vector.magnitude < ConstValue.CannonRange - 5f)
				{
					if (state != ShipState.Combat)
					{
						state = ShipState.Combat;
						myShip.SetState(state);
						foreach (ShipSubController subShip2 in subShipList)
						{
							subShip2.SetState(state);
						}
					}
					vector.y = 0f;
					vector.Normalize();
					if (followCoroutine != null)
					{
						StopCoroutine(followCoroutine);
						followCoroutine = null;
					}
					switch (myShip.baseData.combatStyle)
					{
					case ECombatStyle.Rush:
						CombatRush(-vector);
						break;
					case ECombatStyle.Vertical:
						CombatVertical(vector);
						break;
					case ECombatStyle.LootAt:
						CombatLookAt(-vector);
						break;
					}
				}
				else
				{
					if (state != ShipState.Follow)
					{
						state = ShipState.Follow;
						myShip.SetState(state);
						foreach (ShipSubController subShip3 in subShipList)
						{
							subShip3.SetState(state);
						}
						myShip.SetState(state);
					}
					ECombatStyle combatStyle = myShip.baseData.combatStyle;
					if (combatStyle == ECombatStyle.Vertical)
					{
						if (rotateCoroutine != null)
						{
							StopCoroutine(rotateCoroutine);
							rotateCoroutine = null;
						}
						if (combatMoveCoroutine != null)
						{
							StopCoroutine(combatMoveCoroutine);
							combatMoveCoroutine = null;
						}
					}
					if (placeGoBack == global::UnityEngine.Vector3.zero)
					{
						SetGoBackPlace(base.transform.position);
					}
					if (followCoroutine == null)
					{
						followCoroutine = StartCoroutine(TargetFollowRutine());
					}
				}
			}
			else
			{
				if (followCoroutine == null)
				{
					EShipMoveType eShipMoveType = moveType;
					if (eShipMoveType == EShipMoveType.PATROL)
					{
						followCoroutine = StartCoroutine(PatrolRutine());
					}
				}
				if (state != ShipState.Follow)
				{
					state = ShipState.Follow;
					myShip.SetState(state);
					foreach (ShipSubController subShip4 in subShipList)
					{
						subShip4.SetState(state);
					}
					myShip.SetState(state);
				}
				ECombatStyle combatStyle = myShip.baseData.combatStyle;
				if (combatStyle == ECombatStyle.Vertical)
				{
					if (rotateCoroutine != null)
					{
						StopCoroutine(rotateCoroutine);
						rotateCoroutine = null;
					}
					if (combatMoveCoroutine != null)
					{
						StopCoroutine(combatMoveCoroutine);
						combatMoveCoroutine = null;
					}
				}
			}
			if (patrolWorldIcon != null && patrolWorldIcon.gameObject.activeSelf)
			{
				patrolWorldIcon.SetPosition(base.transform.position);
			}
		}
	}

	private global::System.Collections.IEnumerator GoBackRutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			global::UnityEngine.Vector3 forward = placeGoBack - base.transform.position;
			if (forward.sqrMagnitude < 100f)
			{
				break;
			}
			forward.y = 0f;
			forward.Normalize();
			float angleForward = myMathf.GetAngleForward(forward);
			if (myShip.isRotateMode)
			{
				myShip.isRotateMode = false;
			}
			myShip.Acceleration(myShip.acceleration);
			myShip.SetTargetAngle(angleForward);
			yield return wait;
		}
		if (moveType == EShipMoveType.PATROL)
		{
			SetGoBackPlace(global::UnityEngine.Vector3.zero);
			if (followCoroutine != null)
			{
				StopCoroutine(followCoroutine);
			}
			followCoroutine = StartCoroutine(PatrolRutine());
		}
		targetEnemy = null;
		isGoingBack = false;
	}

	public void SetGoBackPlace(global::UnityEngine.Vector3 pos)
	{
		placeGoBack = pos;
	}

	private void CombatRush(global::UnityEngine.Vector3 dir)
	{
		myShip.isRotateMode = false;
		myShip.SetTargetAngle(myMathf.GetAngleForward(dir));
		myShip.Acceleration(myShip.acceleration);
	}

	private void CombatVertical(global::UnityEngine.Vector3 dir)
	{
		if (rotateCoroutine == null)
		{
			rotateCoroutine = StartCoroutine(RotateVertical());
		}
		if (combatMoveCoroutine == null)
		{
			combatMoveCoroutine = StartCoroutine(CombatMove());
		}
	}

	private void CombatLookAt(global::UnityEngine.Vector3 dir)
	{
		myShip.isRotateMode = true;
		myShip.SetTargetAngle(myMathf.GetAngleForward(dir));
		if (combatMoveCoroutine == null)
		{
			combatMoveCoroutine = StartCoroutine(CombatMove());
		}
	}

	private global::System.Collections.IEnumerator CombatMove()
	{
		bool isMove = false;
		float totalTime = 0f;
		if (global::UnityEngine.Random.value <= 0.3f)
		{
			combatRestTime = 2f;
		}
		else
		{
			combatRestTime = global::UnityEngine.Random.Range(6f, 9f);
		}
		if (global::UnityEngine.Random.value <= 0.3f)
		{
			combatMoveTime = 15f;
		}
		else
		{
			combatMoveTime = global::UnityEngine.Random.Range(7f, 5f);
		}
		float coolTime = combatRestTime;
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			totalTime += global::UnityEngine.Time.fixedDeltaTime;
			if (isMove)
			{
				myShip.Acceleration(myShip.acceleration);
			}
			if (totalTime >= coolTime)
			{
				totalTime -= coolTime;
				isMove = !isMove;
				coolTime = ((!isMove) ? combatRestTime : combatMoveTime);
			}
			yield return wait;
		}
	}

	private global::System.Collections.IEnumerator TargetFollowRutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			if (targetEnemy != null)
			{
				FollowTarget(targetEnemy.transform.position);
			}
			yield return wait;
		}
	}

	private global::System.Collections.IEnumerator PatrolRutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		if (patrolCourseList.Count <= 0)
		{
			yield break;
		}
		bool goBack = false;
		while (true)
		{
			if (GameManager.Instance != null && GameManager.Instance.IsInIsland)
			{
				yield return wait;
				continue;
			}
			global::UnityEngine.Vector3 vector = patrolCourseList[coruseNum];
			FollowTarget(vector);
			if ((vector - base.transform.position).sqrMagnitude < 25f)
			{
				if (!goBack)
				{
					coruseNum++;
					if (coruseNum >= patrolCourseList.Count)
					{
						goBack = true;
						coruseNum = patrolCourseList.Count - 2;
					}
				}
				else
				{
					coruseNum--;
					if (coruseNum <= -1)
					{
						goBack = false;
						coruseNum = 1;
					}
				}
			}
			yield return wait;
		}
	}

	public void PatrolReset()
	{
		myShip.myHealth.SetFullHp();
		if (patrolSubShipList != null)
		{
			foreach (ShipBase patrolSubShip in patrolSubShipList)
			{
				if (patrolSubShip != null)
				{
					global::UnityEngine.Object.Destroy(patrolSubShip.gameObject);
				}
			}
			patrolSubShipList.Clear();
		}
		SpawnPatrolSubShip(myPatrolStatus);
	}

	public void SetPatrolData(PatrolStatus status)
	{
		myPatrolStatus = status;
		base.transform.position = status.startPosition;
		base.transform.rotation = status.startRotation;
		patrolCourseList = status.patrolCourseList;
		moveType = EShipMoveType.PATROL;
		patrolWorldIcon = UI_Manager.Instance.ui_worldMap.GeneratePatrol();
		myShip.myHealth.AddDieAction(delegate
		{
			if ((bool)patrolWorldIcon)
			{
				global::UnityEngine.Object.Destroy(patrolWorldIcon.gameObject);
			}
			GameManager.Instance.worldManager.AddPatrolCoolTimeList(status);
			CoolTimeManager.AddCooltimeSecondFromNow(status.patrolKey, status.coolTime);
			UI_PatrolCombatStatus ui_patrolCombatStatus = UI_Manager.Instance.ui_mainGame.ui_patrolCombatStatus;
			if (ui_patrolCombatStatus.gameObject.activeSelf)
			{
				ui_patrolCombatStatus.gameObject.SetActive(value: false);
			}
			StopCoroutine("CombatStatusCheck");
			UI_PatrolCombatStatus ui_patrolCombatStatus2 = UI_Manager.Instance.ui_mainGame.ui_patrolCombatStatus;
			if (ui_patrolCombatStatus2.gameObject.activeSelf)
			{
				ui_patrolCombatStatus2.gameObject.SetActive(value: false);
			}
		});
		SpawnPatrolSubShip(myPatrolStatus);
		StartCoroutine("CombatStatusCheck");
	}

	private void OnDestory()
	{
		if (subShipList.Count > 0)
		{
			for (int i = 0; i < subShipList.Count; i++)
			{
				if (subShipList[i] != null)
				{
					global::UnityEngine.Object.Destroy(subShipList[i].gameObject);
				}
			}
		}
		if (patrolSubShipList.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < patrolSubShipList.Count; j++)
		{
			if (patrolSubShipList[j] != null)
			{
				global::UnityEngine.Object.Destroy(patrolSubShipList[j].gameObject);
			}
		}
	}

	public void RemovePatrolSubShip()
	{
		if (patrolSubShipList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < patrolSubShipList.Count; i++)
		{
			if (patrolSubShipList[i] != null)
			{
				global::UnityEngine.Object.Destroy(patrolSubShipList[i].gameObject);
			}
		}
	}

	public void PatrolIconDestroy()
	{
		if ((bool)patrolWorldIcon)
		{
			global::UnityEngine.Object.Destroy(patrolWorldIcon.gameObject);
		}
	}

	private void SpawnPatrolSubShip(PatrolStatus status)
	{
		if (patrolSubShipList == null)
		{
			patrolSubShipList = new global::System.Collections.Generic.List<ShipBase>();
		}
		listNum = 0;
		foreach (IslandSpawnData spawn in status.spawnList)
		{
			SpawnSubShip(spawn.count, spawn.shipType, delegate(ShipBase ship)
			{
				patrolSubShipList.Add(ship);
			});
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
				_ = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			if ((player.transform.position - base.transform.position).sqrMagnitude < 2500f && !patrolStatus.gameObject.activeSelf)
			{
				patrolStatus.gameObject.SetActive(value: true);
				patrolStatus.SetEnemy(myShip.myHealth);
				patrolStatus.SetPatrol();
				while (true)
				{
					if (player == null)
					{
						player = GameManager.Instance.playerShip;
						yield return null;
						continue;
					}
					if ((player.transform.position - base.transform.position).sqrMagnitude > 3600f)
					{
						break;
					}
					if (!patrolStatus.gameObject.activeSelf)
					{
						patrolStatus.gameObject.SetActive(value: true);
						patrolStatus.SetEnemy(myShip.myHealth);
						patrolStatus.SetPatrol();
					}
					yield return null;
				}
				patrolStatus.gameObject.SetActive(value: false);
			}
			yield return null;
		}
	}

	private global::System.Collections.IEnumerator RotateVertical()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			if (targetEnemy == null)
			{
				yield return wait;
				continue;
			}
			global::UnityEngine.Vector3 rhs = targetEnemy.transform.position - base.transform.position;
			rhs.y = 0f;
			rhs.Normalize();
			global::UnityEngine.Vector3 vector = global::UnityEngine.Vector3.Cross(global::UnityEngine.Vector3.up, rhs);
			global::UnityEngine.Vector3 forward = -vector;
			if (global::UnityEngine.Vector3.Angle(base.transform.forward, vector) <= 90f)
			{
				myShip.SetTargetAngle(myMathf.GetAngleForward(vector));
			}
			else
			{
				myShip.SetTargetAngle(myMathf.GetAngleForward(forward));
			}
			yield return wait;
		}
	}

	private void FollowTarget(global::UnityEngine.Vector3 targetPos)
	{
		float angleForward = myMathf.GetAngleForward((targetPos - base.transform.position).normalized);
		myShip.SetTargetAngle(angleForward);
		myShip.Acceleration(myShip.acceleration);
	}

	private global::System.Collections.IEnumerator NearTargetCheck()
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(1.5f);
		global::System.Collections.Generic.HashSet<Health> list = myShip.targetFinder.HealthHashSet;
		while (true)
		{
			float num = float.PositiveInfinity;
			Health health = null;
			foreach (Health item in list)
			{
				if (item != null)
				{
					float magnitude = (base.transform.position - item.transform.position).magnitude;
					if (magnitude < num)
					{
						health = item;
						num = magnitude;
					}
				}
			}
			if (health != null)
			{
				targetEnemy = health;
			}
			yield return wait;
		}
	}

	public void SpawnSubShip(int count, EShipType SpawnType, global::UnityEngine.Events.UnityAction<ShipBase> callBack)
	{
		global::System.Collections.Generic.List<global::UnityEngine.Transform> randomLineData = ShipLineManager.Instance.GetRandomLineData();
		for (int i = 0; i < count; i++)
		{
			int level = global::UnityEngine.Mathf.Clamp(myShip.myLevel / 100 + 1, 1, 10);
			global::UnityEngine.Vector3 pos = base.transform.TransformVector(randomLineData[listNum].localPosition) + base.transform.position;
			ShipSubController shipSubController = ShipGenerator.Instance.SpawnSubShip(pos, base.transform.rotation, myShip, randomLineData[listNum].localPosition);
			ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(SpawnType, (EShipSub)level);
			if (subShipData == null || subShipData.modelSrc == null)
			{
				subShipData = ShipDataManager.Instance.GetSubShipData(SpawnType, EShipSub.LEVEL_01);
			}
			ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(SpawnType);
			if (subShipData.enemyModelSrc != null)
			{
				shipSubController.myShip.SpawnShipModel(subShipData.enemyModelSrc, ELayerType.Enemy);
			}
			else
			{
				shipSubController.myShip.SpawnShipModel(subShipData.modelSrc, ELayerType.Enemy);
			}
			shipSubController.SetTarget(myShip, base.transform.TransformVector(randomLineData[listNum].localPosition));
			shipSubController.SetBaseData(shipBaseData);
			shipSubController.transform.SetParent(base.transform.parent);
			shipSubController.transform.forward = base.transform.forward;
			shipSubController.myShip.SetLevel(myShip.myLevel);
			AddSubShip(shipSubController);
			listNum++;
			callBack?.Invoke(shipSubController.myShip);
			EnemySpawnManager.Instance.AddOnlyHealth(shipSubController.myShip.myHealth);
		}
	}

	protected void AddSubShip(ShipSubController ship)
	{
		if (!subShipList.Contains(ship))
		{
			subShipList.Add(ship);
		}
	}

	protected void RemoveSubShip(ShipSubController ship)
	{
		if (subShipList.Contains(ship))
		{
			subShipList.Remove(ship);
		}
	}
}
