public class CannonManager : WeaponBase
{
	[global::UnityEngine.SerializeField]
	private float limitAngleH;

	private float coolTimeTotal;

	private float coolTime = 2f;

	private Cannon[] myCannons;

	[global::UnityEngine.SerializeField]
	private bool isAnotherPoolName;

	[global::UnityEngine.SerializeField]
	private string poolName = "";

	private UI_FireRange myFireRange;

	private float range = 35f;

	private bool isFireReady = true;

	protected override void Awake()
	{
		base.Awake();
		myCannons = GetComponentsInChildren<Cannon>();
	}

	public void MakeCannon(int count)
	{
		if (myCannons.Length == 0)
		{
			myCannons = new Cannon[count];
			float num = 0.75f;
			float num2 = num / 2f;
			bool flag = false;
			if (count % 2 == 1)
			{
				flag = true;
			}
			global::UnityEngine.GameObject original = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/Default Cannon");
			for (int i = 0; i < count; i++)
			{
				int num3 = -(count / 2) + i;
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(original, transform);
				global::UnityEngine.Vector3 localPosition = new global::UnityEngine.Vector3((float)num3 * num + (flag ? 0f : num2), 0f, 0f);
				global::UnityEngine.Transform obj = gameObject.transform;
				obj.localPosition = localPosition;
				obj.localScale = global::UnityEngine.Vector3.one;
				obj.localRotation = global::UnityEngine.Quaternion.identity;
				myCannons[i] = gameObject.GetComponent<Cannon>();
			}
		}
	}

	private void Start()
	{
		if (isAnotherPoolName)
		{
			Cannon[] array = myCannons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetPoolName(poolName);
			}
		}
		if (myShip == null)
		{
			myShip = GetComponentInParent<ShipBase>();
		}
		if (!(myShip == null))
		{
			if (myShip.baseData.id == EShipType.Main && GameManager.Instance != null && GameManager.Instance.playerShip == myShip)
			{
				int num = PlayerInfo.Instance.myData.mainShipLevel;
				num = global::UnityEngine.Mathf.Clamp(num - 1, 0, 999);
				coolTime = ShipDataManager.Instance.mainShipData.dataArray[num].Firecooltime;
			}
			else
			{
				coolTime = ShipDataManager.Instance.GetShipSheetData(myShip.baseData.id, myShip.myLevel).fireCoolTime;
			}
			isFireReady = true;
			coolTimeTotal = coolTime;
		}
	}

	public void GenerateFireRange()
	{
		if (myFireRange != null)
		{
			global::UnityEngine.Object.Destroy(myFireRange.gameObject);
		}
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/FireRangeCanvas");
		if (gameObject != null)
		{
			global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject);
			myFireRange = gameObject2.GetComponent<UI_FireRange>();
			myFireRange.SetTargetTransform(transform);
			myFireRange.SetRange(range);
			myFireRange.SetAngle(limitAngleH * 2f);
			myFireRange.SetCoolTime(1f);
		}
	}

	private void OnDestroy()
	{
		if (myFireRange != null)
		{
			global::UnityEngine.Object.Destroy(myFireRange.gameObject);
		}
	}

	protected override void CombatMode()
	{
		StartCoroutine("LookAtTarget");
	}

	protected override void FollowMode()
	{
		StopCoroutine("LookAtTarget");
	}

	public override bool IsTargetPosible(global::UnityEngine.Transform target)
	{
		bool result = false;
		global::UnityEngine.Vector3 vector = target.position - transform.position;
		global::UnityEngine.Vector3 forward = transform.forward;
		forward.y = 0f;
		forward.Normalize();
		vector.y = 0f;
		float num = global::UnityEngine.Vector3.Angle(forward, vector.normalized);
		if (limitAngleH != 0f && num < limitAngleH)
		{
			result = true;
		}
		return result;
	}

	private void LateUpdate()
	{
		if (!isFireReady)
		{
			coolTimeTotal += global::UnityEngine.Time.deltaTime;
			float num = global::UnityEngine.Mathf.Clamp01(coolTimeTotal / coolTime);
			if ((bool)myFireRange)
			{
				myFireRange.SetCoolTime(num);
			}
			if (coolTimeTotal >= coolTime)
			{
				isFireReady = true;
			}
		}
	}

	public override void Fire()
	{
		if (myShip.IsDie)
		{
			return;
		}
		float num = (100f - MyInventoryDataManager.Instance.AbilityCoolTime + FlagManager.Instance.AbilityCoolTime + SkinDataManager.Instance.AbilityCoolTime) * 0.01f;
		if (myShip.baseData.id == EShipType.Main && GameManager.Instance != null && GameManager.Instance.playerShip == myShip)
		{
			int num2 = PlayerInfo.Instance.myData.mainShipLevel;
			num2 = global::UnityEngine.Mathf.Clamp(num2 - 1, 0, 999);
			coolTime = ShipDataManager.Instance.mainShipData.dataArray[num2].Firecooltime * num;
		}
		else if (myShip.InvenShipData != null)
		{
			int shipLevel = PlayerInfo.Instance.GetShipLevel(myShip.baseData.id);
			coolTime = ShipDataManager.Instance.GetShipSheetData(myShip.baseData.id, shipLevel).fireCoolTime * num;
		}
		else
		{
			_ = myShip.myLevel;
			if (myShip.baseData.id == EShipType.Elite || myShip.baseData.id == EShipType.Bomber || myShip.baseData.id == EShipType.Ghost)
			{
				coolTime = 6f;
			}
			else
			{
				coolTime = ShipDataManager.Instance.GetShipSheetData(myShip.baseData.id, myShip.myLevel).fireCoolTime;
			}
		}
		StartCoroutine(FireCoroutine());
	}

	private global::System.Collections.IEnumerator FireCoroutine()
	{
		float magnitude = (transform.position - targetEnemy.transform.position).magnitude;
		float t = global::UnityEngine.Mathf.InverseLerp(2f, ConstValue.CannonRange, magnitude) + 0.05f;
		string text = "ship_cannon_";
		int num;
		if (myShip.InvenShipData != null)
		{
			EShipSub level = (EShipSub)global::UnityEngine.Mathf.Clamp(PlayerInfo.Instance.GetShipLevel(myShip.InvenShipData.shipType) / 1000 + 1, 1, 10);
			num = global::UnityEngine.Mathf.Clamp(ShipDataManager.Instance.GetSubShipData(myShip.InvenShipData.shipType, level).cannonCount, 1, 3);
		}
		else if (myShip == GameManager.Instance.playerShip)
		{
			EShipMain level2 = (EShipMain)global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 1000 + 1, 1, 10);
			num = global::UnityEngine.Mathf.Clamp(ShipDataManager.Instance.GetMainData(level2).cannonCount, 1, 3);
		}
		else
		{
			int myLevel = myShip.myLevel;
			if (myShip.baseData.id != EShipType.Main && myShip.baseData.id != EShipType.Elite && myShip.baseData.id != EShipType.Bomber && myShip.baseData.id != EShipType.Ghost)
			{
				EShipSub level3 = (EShipSub)global::UnityEngine.Mathf.Clamp(myLevel / 1000 + 1, 1, 10);
				num = global::UnityEngine.Mathf.Clamp(ShipDataManager.Instance.GetSubShipData(myShip.baseData.id, level3).cannonCount, 1, 3);
			}
			else
			{
				EShipMain level4 = (EShipMain)global::UnityEngine.Mathf.Clamp(myLevel / 1000 + 1, 1, 10);
				num = global::UnityEngine.Mathf.Clamp(ShipDataManager.Instance.GetMainData(level4).cannonCount, 1, 3);
			}
		}
		AudioManager.Instance.PlaySFX(text + num);
		float forcePower = global::UnityEngine.Mathf.Lerp(10f, 32f, t);
		float interval = 0.4f / (float)myCannons.Length;
		for (int i = myCannons.Length - 1; i >= 0; i--)
		{
			float z = myShip.modelTrans.localRotation.eulerAngles.z;
			float offsetAngle = ((transform.localRotation.y < 0f) ? (0f - z) : z);
			myCannons[i].damage = myShip.Damage;
			myCannons[i].Fire(offsetAngle, myType, forcePower);
			yield return new global::UnityEngine.WaitForSeconds(interval);
		}
	}

	private global::System.Collections.IEnumerator LookAtTarget()
	{
		while (true)
		{
			if (targetEnemy == null)
			{
				yield return null;
				continue;
			}
			if (targetEnemy.myShip != null && targetEnemy.myShip.IsDie)
			{
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 rhs = targetEnemy.transform.position - myShip.transform.position;
			float magnitude = rhs.magnitude;
			rhs.y = 0f;
			rhs.Normalize();
			global::UnityEngine.Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float num = global::UnityEngine.Mathf.Clamp(global::UnityEngine.Vector3.Dot(forward, rhs), -1f, 1f);
			if (num < 0f)
			{
				yield return null;
				continue;
			}
			_ = global::UnityEngine.Vector3.Cross(transform.forward, rhs).y;
			_ = 0f;
			float num2 = global::UnityEngine.Mathf.Acos(num) * 57.29578f;
			if (num2 < limitAngleH && magnitude <= range && isFireReady)
			{
				global::UnityEngine.Vector3 normalized;
				float num3;
				if (targetEnemy.myShip != null)
				{
					normalized = targetEnemy.myShip.rigidbody.velocity.normalized;
					num3 = targetEnemy.myShip.rigidbody.velocity.magnitude * 1f;
				}
				else
				{
					global::UnityEngine.Rigidbody component = targetEnemy.GetComponent<global::UnityEngine.Rigidbody>();
					normalized = component.velocity.normalized;
					num3 = component.velocity.magnitude * 1f;
				}
				rhs = targetEnemy.transform.position + normalized * num3 - myShip.transform.position;
				_ = rhs.magnitude;
				rhs.y = 0f;
				rhs.Normalize();
				forward = transform.forward;
				forward.y = 0f;
				forward.Normalize();
				num = global::UnityEngine.Mathf.Clamp(global::UnityEngine.Vector3.Dot(forward, rhs), -1f, 1f);
				if (num < 0f)
				{
					yield return null;
					continue;
				}
				int num4 = ((global::UnityEngine.Vector3.Cross(transform.forward, rhs).y > 0f) ? 1 : 0);
				num2 = global::UnityEngine.Mathf.Acos(num) * 57.29578f;
				if (limitAngleH != 0f)
				{
					num2 = global::UnityEngine.Mathf.Min(limitAngleH, num2);
				}
				if (num4 == 0)
				{
					num2 *= -1f;
				}
				Cannon[] array = myCannons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetAngle(num2);
				}
				Fire();
				isFireReady = false;
				coolTimeTotal = 0f;
			}
			yield return null;
		}
	}
}
