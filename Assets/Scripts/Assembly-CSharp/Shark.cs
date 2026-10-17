public class Shark : Boss
{
	private global::UnityEngine.Animator anim;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform mouseFirePivot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject dashWarningObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject warnCircleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject mouseSrc;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<global::UnityEngine.Vector3> roamPosList = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();

	private global::UnityEngine.Vector3 spawnPos;

	private const float MAX_ROAMDISTANCE = 70f;

	private float speedPower = 10f;

	private float turnSpeed = 45f;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Collider myCol;

	private SharkCannon myCannon;

	[global::UnityEngine.SerializeField]
	private Hit hitFin;

	[global::UnityEngine.SerializeField]
	private Hit hitJump;

	private int cannonDamage;

	private int teethDamage;

	private int dashDamage;

	private int jumpDamage;

	private int teethHealth;

	public bool isOnTheWater = true;

	private float targetAngle = -999f;

	private bool IsOnTheWater()
	{
		return isOnTheWater;
	}

	protected override void Awake()
	{
		if (gameObject.scene.name == "Intro")
		{
			enabled = false;
			return;
		}
		base.Awake();
		anim = GetComponentInChildren<global::UnityEngine.Animator>();
		dashWarningObj.SetActive(value: false);
		myCannon = GetComponentInChildren<SharkCannon>();
		Hit hit = hitJump;
		hit.hitAction = (global::System.Action<Health>)global::System.Delegate.Combine(hit.hitAction, new global::System.Action<Health>(PushJumpHit));
		AddPattern(10f, 300f, ShootCannon, 0f, IsOnTheWater);
		AddPattern(30f, 300f, ShootMouse, 5f, IsOnTheWater);
	}

	private void DataCalculation()
	{
		int num = global::UnityEngine.Mathf.Clamp(status.bossLevel - 1, 0, 999);
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(status.bossLevel / 100 + 1, 1, 10);
		MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num];
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = mainShipDataData.Attack * mainData.cannonCount;
		int num3 = (int)((float)num2 / mainShipDataData.Firecooltime);
		cannonDamage = mainShipDataData.Health / 10;
		teethDamage = mainShipDataData.Health / 8;
		dashDamage = mainShipDataData.Health / 8;
		jumpDamage = mainShipDataData.Health / 5;
		float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
		teethHealth = (int)((float)num2 * 1.5f * enemyHealthFactor);
		myHealth.SetHealthPoint((int)((float)num3 * 1.9f * 95f * enemyHealthFactor));
		myCannon.damage = cannonDamage;
		hitFin.damage = dashDamage;
		hitJump.damage = jumpDamage;
	}

	private void Start()
	{
		spawnPos = transform.position;
		RoamStart();
		DataCalculation();
	}

	protected override void Die()
	{
		base.Die();
		if (!isDie)
		{
			isDie = true;
			anim.SetInteger("Rotate", 0);
			turnSpeed = 0f;
			targetAngle = -999f;
			anim.SetFloat("moveSpeedMultiple", 1f);
			anim.SetLayerWeight(1, 0f);
			anim.SetTrigger("Die");
			StartCoroutine(DieAction());
		}
	}

	private global::System.Collections.IEnumerator DieAction()
	{
		yield return new global::UnityEngine.WaitForSeconds(4f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(transform, 0f, 0.4f), delegate
		{
			SpawnReward();
			global::UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	private void FixedUpdate()
	{
		float t = global::UnityEngine.Mathf.InverseLerp(5f, 50f, rigidbody.velocity.magnitude);
		float value = global::UnityEngine.Mathf.Lerp(0.7f, 5f, t);
		anim.SetFloat("moveSpeedMultiple", value);
		SharkRotate();
	}

	private void SharkRotate()
	{
		if (!(rigidbody.velocity.sqrMagnitude > 0f) || isDie)
		{
			return;
		}
		if (targetAngle != -999f)
		{
			float num = myMathf.GetAngleForward(transform.forward) - targetAngle;
			bool flag = false;
			flag = (num >= 0f && num < 180f) || ((!(num <= 0f) || !(num > -180f)) && ((num <= -180f) ? true : false));
			if (num < 2f + global::UnityEngine.Mathf.Epsilon && num > -2f - global::UnityEngine.Mathf.Epsilon)
			{
				anim.SetInteger("Rotate", 0);
			}
			else if (flag)
			{
				anim.SetInteger("Rotate", 1);
			}
			else
			{
				anim.SetInteger("Rotate", 2);
			}
			float num2 = turnSpeed * global::UnityEngine.Time.fixedDeltaTime;
			float num3 = global::UnityEngine.Mathf.Abs(num);
			if (num3 > num2)
			{
				transform.Rotate(0f, flag ? num2 : (0f - num2), 0f, global::UnityEngine.Space.World);
			}
			else if (num3 < global::UnityEngine.Mathf.Epsilon && num3 > 0f - global::UnityEngine.Mathf.Epsilon)
			{
				transform.Rotate(0f, flag ? num3 : (0f - num3), 0f, global::UnityEngine.Space.World);
			}
		}
		else
		{
			anim.SetInteger("Rotate", 0);
		}
	}

	private void GoBack()
	{
		StopCoroutine(ChangeForcePower());
		StopCoroutine(Roam());
		roamPosList.Clear();
		roamPosList.Add(status.position);
		StartCoroutine(Roam());
		if (modelTrans.localPosition.y < -6f)
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(modelTrans);
			global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, 0f, 0.3f);
		}
		myCol.enabled = true;
	}

	[global::UnityEngine.ContextMenu("RoamStart")]
	private void RoamStart()
	{
		MakeRoamList();
		StartCoroutine(ChangeForcePower());
		StartCoroutine(Roam());
	}

	private void MakeRoamList()
	{
		roamPosList.Clear();
		int num = global::UnityEngine.Random.Range(6, 7);
		for (int i = 0; i < num; i++)
		{
			global::UnityEngine.Vector3 item = spawnPos + global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward * global::UnityEngine.Random.Range(30f, 70f);
			item.y = 0f;
			roamPosList.Add(item);
		}
	}

	private global::System.Collections.IEnumerator ChangeForcePower()
	{
		speedPower = 10f;
		while (!isDie)
		{
			yield return new global::UnityEngine.WaitForSeconds(global::UnityEngine.Random.Range(6f, 10f));
			speedPower = global::UnityEngine.Random.Range(5f, 7f);
			float t = global::UnityEngine.Mathf.InverseLerp(5f, 7f, speedPower);
			turnSpeed = global::UnityEngine.Mathf.Lerp(60f, 120f, t);
		}
	}

	private global::System.Collections.IEnumerator Roam()
	{
		anim.SetLayerWeight(1, 1f);
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			if (isDie)
			{
				yield break;
			}
			if (roamPosList.Count <= 0)
			{
				break;
			}
			global::UnityEngine.Vector3 targetPos = roamPosList[0];
			while (true)
			{
				if (isDie)
				{
					yield break;
				}
				global::UnityEngine.Vector3 vector = targetPos - transform.position;
				float angleForward = myMathf.GetAngleForward(vector.normalized);
				targetAngle = angleForward;
				if (global::UnityEngine.Mathf.Abs(vector.magnitude) < 8f)
				{
					break;
				}
				rigidbody.AddForce(transform.forward * speedPower, global::UnityEngine.ForceMode.Acceleration);
				yield return wait;
			}
			roamPosList.RemoveAt(0);
			yield return wait;
		}
		StopCoroutine(ChangeForcePower());
		anim.SetLayerWeight(1, 0f);
		targetAngle = -999f;
		DashStart();
	}

	[global::UnityEngine.ContextMenu("dash")]
	private void DashStart()
	{
		StartCoroutine(DashRutine());
	}

	private global::System.Collections.IEnumerator DashRutine()
	{
		yield return new global::UnityEngine.WaitUntil(() => GameManager.Instance.playerShip != null);
		if ((status.position - GameManager.Instance.playerShip.transform.position).magnitude > 300f)
		{
			RoamStart();
			yield break;
		}
		global::UnityEngine.WaitForFixedUpdate waitForFixedUpdate = new global::UnityEngine.WaitForFixedUpdate();
		while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			yield return null;
		}
		bool ready = false;
		myCol.enabled = false;
		isOnTheWater = false;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, -12f, 3f), delegate
		{
			ready = true;
		});
		yield return new global::UnityEngine.WaitUntil(() => ready);
		int count = global::UnityEngine.Random.Range(3, 4);
		for (int i = 0; i < count; i++)
		{
			dashWarningObj.SetActive(value: true);
			anim.SetTrigger("DashReady");
			float totalTime = 0f;
			float duration = 2f;
			ShipBase playerShip = GameManager.Instance.playerShip;
			while (!(playerShip == null))
			{
				totalTime += global::UnityEngine.Time.deltaTime;
				if (totalTime >= duration)
				{
					break;
				}
				global::UnityEngine.Vector3 vector = playerShip.transform.position - transform.position;
				vector.y = 0f;
				transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime * 2f);
				global::UnityEngine.Vector3 position = dashWarningObj.transform.position;
				position.y = 0.1f;
				dashWarningObj.transform.position = position;
				yield return null;
			}
			yield return new global::UnityEngine.WaitForSeconds(1.5f);
			anim.SetTrigger("Dash");
			yield return new global::UnityEngine.WaitForSeconds(0.2f);
			AudioManager.Instance.PlaySFX("shark_dash");
			dashWarningObj.SetActive(value: false);
			hitFin.gameObject.SetActive(value: true);
			float weight = anim.GetLayerWeight(1);
			anim.SetLayerWeight(1, 0f);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.ShortcutExtensions.DOMove(transform, transform.forward * 90f, 1.5f)), delegate
			{
				hitFin.gameObject.SetActive(value: false);
			});
			yield return new global::UnityEngine.WaitForSeconds(1.3f);
			anim.SetLayerWeight(1, weight);
			anim.CrossFade("Idle", 1f, 0);
			yield return new global::UnityEngine.WaitForSeconds(0.6f);
			global::UnityEngine.Vector3 vector2 = ((global::UnityEngine.Random.value <= 0.5f) ? transform.right : (-transform.right));
			global::UnityEngine.Vector3 movePos = transform.position + vector2 * 25f;
			global::UnityEngine.Vector3 vector3 = GameManager.Instance.playerShip.transform.position - movePos;
			vector3.y = 0f;
			float magnitude = vector3.magnitude;
			if (magnitude > 60f)
			{
				movePos += vector3.normalized * (magnitude - 60f);
			}
			while (true)
			{
				global::UnityEngine.Vector3 vector4 = movePos - transform.position;
				float angleForward = myMathf.GetAngleForward(vector4.normalized);
				turnSpeed = 75f;
				speedPower = 40f;
				targetAngle = angleForward;
				rigidbody.AddForce(transform.forward * speedPower, global::UnityEngine.ForceMode.Acceleration);
				if (global::UnityEngine.Mathf.Abs(vector4.magnitude) <= 8f)
				{
					break;
				}
				yield return waitForFixedUpdate;
			}
			targetAngle = -999f;
		}
		StartCoroutine(JumpAttack());
	}

	private global::System.Collections.IEnumerator JumpAttack()
	{
		global::UnityEngine.Vector3 targetPos = transform.position;
		if (GameManager.Instance.playerShip != null)
		{
			targetPos = GameManager.Instance.playerShip.transform.position;
		}
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		warnCircleObj.SetActive(value: true);
		global::UnityEngine.Vector3 position = warnCircleObj.transform.position;
		position.y = 0.1f;
		warnCircleObj.transform.position = position;
		while (true)
		{
			if (GameManager.Instance.playerShip != null)
			{
				targetPos = GameManager.Instance.playerShip.transform.position;
			}
			global::UnityEngine.Vector3 position2 = transform.position;
			position2.y = 0f;
			global::UnityEngine.Vector3 vector = targetPos - position2;
			float angleForward = myMathf.GetAngleForward(vector.normalized);
			targetAngle = angleForward;
			turnSpeed = 75f;
			speedPower = 40f;
			if (!(vector.magnitude >= 10f))
			{
				break;
			}
			rigidbody.AddForce(transform.forward * speedPower, global::UnityEngine.ForceMode.Acceleration);
			yield return wait;
		}
		yield return new global::UnityEngine.WaitForSeconds(1f);
		warnCircleObj.SetActive(value: false);
		anim.SetLayerWeight(1, 0f);
		anim.SetTrigger("Jump");
		yield return new global::UnityEngine.WaitForSeconds(0.79f);
		hitJump.gameObject.SetActive(value: true);
		global::UnityEngine.Vector3 position3 = transform.position;
		position3.y = 0.05f;
		global::UnityEngine.GameObject obj = null;
		AudioManager.Instance.PlaySFX("shark_jump");
		ObjectPoolManager.TryGetNextObject("WaterSplash03", position3, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj);
		obj.transform.localScale = global::UnityEngine.Vector3.one * 12f;
		yield return new global::UnityEngine.WaitForSeconds(0.5f);
		float totalTime = 0f;
		float duration = 2.4f;
		bool waterSplash = false;
		while (true)
		{
			totalTime += global::UnityEngine.Time.fixedDeltaTime;
			if (!waterSplash && totalTime >= 1.05f)
			{
				waterSplash = true;
				position3 = transform.position;
				position3.y = 0.05f;
				ObjectPoolManager.TryGetNextObject("WaterSplash03", position3, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj);
				obj.transform.localScale = global::UnityEngine.Vector3.one * 12f;
				AudioManager.Instance.PlaySFX("boss_water_splash");
			}
			if (totalTime >= duration)
			{
				break;
			}
			rigidbody.AddForce(transform.forward * speedPower, global::UnityEngine.ForceMode.Acceleration);
			yield return wait;
		}
		hitJump.gameObject.SetActive(value: false);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, 0f, 2f), delegate
		{
			RoamStart();
			myCol.enabled = true;
			isOnTheWater = true;
		});
	}

	private void PushJumpHit(Health health)
	{
		if (health.myShip != null)
		{
			ShipBase myShip = health.myShip;
			global::UnityEngine.Vector3 vector = myShip.transform.position - transform.position;
			vector.y = 0f;
			if (global::UnityEngine.Mathf.Abs(vector.x) <= 0.1f)
			{
				vector.x = global::UnityEngine.Random.Range(-0.7f, 0.7f);
			}
			vector.Normalize();
			myShip.rigidbody.AddForce(vector * 30f, global::UnityEngine.ForceMode.VelocityChange);
		}
	}

	private void ShootMouse()
	{
		anim.SetTrigger("Mouse");
		StartCoroutine(MouseAnimWait());
	}

	private global::System.Collections.IEnumerator MouseAnimWait()
	{
		yield return null;
		SharkMouse component = global::UnityEngine.Object.Instantiate(mouseSrc, mouseFirePivot.transform.position, mouseFirePivot.rotation).GetComponent<SharkMouse>();
		component.myHealth.SetHealthPoint(teethHealth);
		component.SetDamage(teethDamage);
		AudioManager.Instance.PlaySFX("shark_tooth");
	}

	private void ShootCannon()
	{
		myCannon.Shoot();
	}
}
