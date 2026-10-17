[global::UnityEngine.RequireComponent(typeof(ShipBase))]
public class ShipAllyController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private ShipBase myShip;

	private ShipState state;

	public Health targetEnemy;

	private global::UnityEngine.Coroutine followCoroutine;

	private global::UnityEngine.Coroutine rotateCoroutine;

	private global::UnityEngine.Coroutine combatMoveCoroutine;

	private global::UnityEngine.Coroutine IdleCoroutine;

	private float combatMoveTime = 4f;

	private float combatRestTime = 7f;

	private global::UnityEngine.Vector3 placeGoBack = global::UnityEngine.Vector3.zero;

	private bool isGoingBack;

	[global::UnityEngine.HideInInspector]
	public bool isNeverGoBack;

	private global::UnityEngine.Vector3 vecZero = global::UnityEngine.Vector3.zero;

	public string userName;

	private eAllyMoveType myMoveType;

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
	}

	private void OnEnable()
	{
		StartCoroutine(NearTargetCheck());
		StartCoroutine(Emoji());
	}

	public void SetMoveType(eAllyMoveType movetype)
	{
		myMoveType = movetype;
		switch (myMoveType)
		{
		case eAllyMoveType.SameEnemy:
			isNeverGoBack = true;
			break;
		case eAllyMoveType.DefendTarget:
			isNeverGoBack = false;
			break;
		case eAllyMoveType.Disconnection:
			StartCoroutine(HPBARWait());
			break;
		}
	}

	private global::System.Collections.IEnumerator HPBARWait()
	{
		yield return new global::UnityEngine.WaitUntil(() => myShip.HPBAR != null);
		myShip.HPBAR.SetDisconnetedIconActive(active: true);
		yield return new global::UnityEngine.WaitForSecondsRealtime(global::UnityEngine.Random.Range(10f, 15f));
		myShip.ShipDestroy(dropItem: false);
	}

	private global::System.Collections.IEnumerator Emoji()
	{
		yield return new global::UnityEngine.WaitUntil(() => myShip.myHealth.HpRatio <= 0.7f);
		if (global::UnityEngine.Random.value <= 0.4f)
		{
			if (global::UnityEngine.Random.value <= 0.8f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.Sad, base.transform);
			}
			else
			{
				eEmoji emojiType = (eEmoji)global::UnityEngine.Random.Range(0, 10);
				EmojiManager.Instance.MakeEmoji(emojiType, base.transform);
			}
		}
		yield return new global::UnityEngine.WaitUntil(() => myShip.myHealth.HpRatio <= 0.4f);
		if (global::UnityEngine.Random.value <= 0.5f)
		{
			float value = global::UnityEngine.Random.value;
			if (value <= 0.2f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.Angry, base.transform);
			}
			else if (value <= 0.8f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.Sad, base.transform);
			}
			else
			{
				eEmoji emojiType2 = (eEmoji)global::UnityEngine.Random.Range(0, 10);
				EmojiManager.Instance.MakeEmoji(emojiType2, base.transform);
			}
		}
		yield return new global::UnityEngine.WaitUntil(() => myShip.myHealth.HpRatio <= 0.2f);
		if (global::UnityEngine.Random.value <= 0.5f)
		{
			float value2 = global::UnityEngine.Random.value;
			if (value2 <= 0.2f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.Angry, base.transform);
				yield break;
			}
			if (value2 <= 0.8f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.Sad, base.transform);
				yield break;
			}
			eEmoji emojiType3 = (eEmoji)global::UnityEngine.Random.Range(0, 10);
			EmojiManager.Instance.MakeEmoji(emojiType3, base.transform);
		}
	}

	public void VictoryEmoji()
	{
		StartCoroutine(VictoryEmojiRutine());
	}

	private global::System.Collections.IEnumerator VictoryEmojiRutine()
	{
		yield return new global::UnityEngine.WaitForSeconds(global::UnityEngine.Random.Range(0f, 3f));
		if (global::UnityEngine.Random.value <= 0.6f)
		{
			float value = global::UnityEngine.Random.value;
			if (value <= 0.3f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.NiceGuy, base.transform);
				yield break;
			}
			if (value <= 0.6f)
			{
				EmojiManager.Instance.MakeEmoji(eEmoji.Love, base.transform);
				yield break;
			}
			eEmoji emojiType = (eEmoji)global::UnityEngine.Random.Range(0, 10);
			EmojiManager.Instance.MakeEmoji(emojiType, base.transform);
		}
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
			if (isGoingBack || myMoveType == eAllyMoveType.Disconnection)
			{
				return;
			}
			ECombatStyle combatStyle;
			if (!isNeverGoBack && (placeGoBack - base.transform.position).sqrMagnitude > 25600f)
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
					combatStyle = myShip.baseData.combatStyle;
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
				if (IdleCoroutine != null)
				{
					StopCoroutine(IdleCoroutine);
					IdleCoroutine = null;
				}
				global::UnityEngine.Vector3 vector = base.transform.position - targetEnemy.transform.position;
				if (vector.magnitude < ConstValue.CannonRange - 5f)
				{
					if (state != ShipState.Combat)
					{
						state = ShipState.Combat;
						myShip.SetState(state);
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
					case ECombatStyle.Nothing:
						break;
					}
					return;
				}
				if (state != ShipState.Follow)
				{
					state = ShipState.Follow;
					myShip.SetState(state);
				}
				combatStyle = myShip.baseData.combatStyle;
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
				if (followCoroutine == null)
				{
					followCoroutine = StartCoroutine(TargetFollowRutine());
				}
				return;
			}
			if (state != ShipState.Follow)
			{
				state = ShipState.Follow;
				myShip.SetState(state);
			}
			combatStyle = myShip.baseData.combatStyle;
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
			eAllyMoveType eAllyMoveType2 = myMoveType;
			if (eAllyMoveType2 != eAllyMoveType.SameEnemy)
			{
				_ = 2;
			}
			else if (IdleCoroutine == null)
			{
				IdleCoroutine = StartCoroutine(IdleMove());
			}
		}
	}

	public void GoCenter()
	{
		if (IdleCoroutine == null)
		{
			IdleCoroutine = StartCoroutine(GoCenterRutine());
		}
	}

	private global::System.Collections.IEnumerator GoCenterRutine()
	{
		global::UnityEngine.Random.Range(3f, 10f);
		float angleForward = myMathf.GetAngleForward((new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-90f, 90f), 0f, global::UnityEngine.Random.Range(-90f, 90f)) - base.transform.position).normalized);
		myShip.SetTargetAngle(angleForward);
		yield return new global::UnityEngine.WaitForSecondsRealtime(global::UnityEngine.Random.Range(5f, 8f));
		while (true)
		{
			myShip.Acceleration(myShip.acceleration);
			yield return null;
		}
	}

	private global::System.Collections.IEnumerator IdleMove()
	{
		float num = global::UnityEngine.Random.Range(0f, 100f);
		float totalTime = 0f;
		if (num <= 30f)
		{
			float duration = global::UnityEngine.Random.Range(3f, 5f);
			float angleForward = myMathf.GetAngleForward((global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * base.transform.forward).normalized);
			myShip.SetTargetAngle(angleForward);
			while (true)
			{
				totalTime += global::UnityEngine.Time.deltaTime;
				if (!(totalTime > duration))
				{
					myShip.Acceleration(myShip.acceleration);
					yield return null;
					continue;
				}
				break;
			}
		}
		else
		{
			if (!(num <= 80f))
			{
				yield break;
			}
			float duration = global::UnityEngine.Random.Range(3f, 5f);
			float angleForward2 = myMathf.GetAngleForward((global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * base.transform.forward).normalized);
			myShip.SetTargetAngle(angleForward2);
			while (true)
			{
				totalTime += global::UnityEngine.Time.deltaTime;
				if (totalTime > duration)
				{
					totalTime = 0f;
					angleForward2 = myMathf.GetAngleForward((global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * base.transform.forward).normalized);
					myShip.SetTargetAngle(angleForward2);
					duration = global::UnityEngine.Random.Range(0.5f, 4f);
				}
				myShip.Acceleration(myShip.acceleration);
				yield return null;
			}
		}
	}

	private global::System.Collections.IEnumerator GoBackRutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			global::UnityEngine.Vector3 forward = placeGoBack - base.transform.position;
			if (forward.sqrMagnitude < 6400f)
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
			combatMoveTime = 25f;
		}
		else
		{
			combatMoveTime = global::UnityEngine.Random.Range(12f, 5f);
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
		yield return new global::UnityEngine.WaitForSeconds(global::UnityEngine.Random.Range(0f, 4f));
		while (true)
		{
			if (targetEnemy != null)
			{
				FollowTarget(targetEnemy.transform.position);
			}
			yield return wait;
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
}
