[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.Rigidbody))]
public class ShipBase : global::UnityEngine.MonoBehaviour
{
	public delegate void EnemyIslandRemoveCheck(ShipBase shipBase);

	public ShipBase.EnemyIslandRemoveCheck checkDelegate;

	[global::UnityEngine.SerializeField]
	private EShipType[] orderList;

	public ShipBaseData baseData;

	public bool isBoundaryNoUse;

	public float accelerationFactor = 1f;

	public float followingWindFactor;

	public float turnSpeedFactor = 1f;

	[global::UnityEngine.HideInInspector]
	public float bonusTurnSpeed;

	public float DamageFactor = 1f;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Transform modelTrans;

	private float targetAngle = -999f;

	public bool isMove;

	public global::System.Collections.Generic.List<WeaponBase> weaponList = new global::System.Collections.Generic.List<WeaponBase>();

	public TargetFinder targetFinder;

	public bool isRotateMode;

	[global::UnityEngine.SerializeField]
	private bool isHolding;

	private int enemyLayer;

	private int enemyProjectileLayer;

	private int playerLayer;

	private int playerProjectileLayer;

	[global::UnityEngine.SerializeField]
	private ELayerType myType;

	private HpBar hpBar;

	private MyShipInventory invenShipData;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject seaWaveParticle;

	private global::UnityEngine.GameObject shipObj;

	public global::System.Action beforeDestroyAction;

	public float acceleration
	{
		get
		{
			float num = 1f + (MyInventoryDataManager.Instance.AbilityAcceleration + FlagManager.Instance.AbilityAcceleration + SkinDataManager.Instance.AbilityAcceleration) * 0.01f;
			if (invenShipData != null)
			{
				int num2 = PlayerInfo.Instance.myData.mainShipLevel;
				num2 = global::UnityEngine.Mathf.Clamp(num2 - 1, 0, 999);
				return ShipDataManager.Instance.mainShipData.dataArray[num2].Acceleration * num * accelerationFactor;
			}
			if (GameManager.Instance != null && this == GameManager.Instance.playerShip)
			{
				int num3 = PlayerInfo.Instance.myData.mainShipLevel;
				num3 = global::UnityEngine.Mathf.Clamp(num3 - 1, 0, 999);
				return ShipDataManager.Instance.mainShipData.dataArray[num3].Acceleration * num * accelerationFactor * followingWindFactor;
			}
			int num4 = PlayerInfo.Instance.myData.mainShipLevel;
			num4 = global::UnityEngine.Mathf.Clamp(num4 - 1, 0, 999);
			float num5 = ShipDataManager.Instance.mainShipData.dataArray[num4].Acceleration;
			num5 = ((baseData.id == EShipType.Elite) ? (num5 * 0.6f) : ((baseData.id != EShipType.SuicideBomb) ? (num5 * 0.85f) : (num5 * 1.1f)));
			return num5 * accelerationFactor;
		}
	}

	public float turnSpeed
	{
		get
		{
			float num = 1f + (MyInventoryDataManager.Instance.AbilityRotation + FlagManager.Instance.AbilityRotation + SkinDataManager.Instance.AbilityRotation) * 0.01f;
			if (invenShipData != null)
			{
				int num2 = PlayerInfo.Instance.myData.mainShipLevel;
				num2 = global::UnityEngine.Mathf.Clamp(num2 - 1, 0, 999);
				return (ShipDataManager.Instance.mainShipData.dataArray[num2].Rotatepersec + bonusTurnSpeed) * num * turnSpeedFactor;
			}
			if (this == GameManager.Instance.playerShip)
			{
				int num3 = PlayerInfo.Instance.myData.mainShipLevel;
				num3 = global::UnityEngine.Mathf.Clamp(num3 - 1, 0, 999);
				return ShipDataManager.Instance.mainShipData.dataArray[num3].Rotatepersec * num * turnSpeedFactor;
			}
			int num4 = PlayerInfo.Instance.myData.mainShipLevel;
			num4 = global::UnityEngine.Mathf.Clamp(num4 - 1, 0, 999);
			float num5 = ShipDataManager.Instance.mainShipData.dataArray[num4].Rotatepersec;
			if (baseData.id == EShipType.Bomber)
			{
				num5 *= 1.4f;
			}
			if (baseData.id == EShipType.Ghost)
			{
				num5 *= 0.9f;
			}
			return num5 * turnSpeedFactor;
		}
	}

	public int Damage
	{
		get
		{
			float num = 1f + (MyInventoryDataManager.Instance.AbilityAttack + FlagManager.Instance.AbilityAttack + SkinDataManager.Instance.AbilityAttack) * 0.01f;
			if (invenShipData != null)
			{
				int shipLevel = PlayerInfo.Instance.GetShipLevel(invenShipData.shipType);
				return (int)((float)ShipDataManager.Instance.GetShipSheetData(invenShipData.shipType, shipLevel).attack * num);
			}
			if (this == GameManager.Instance.playerShip)
			{
				int num2 = PlayerInfo.Instance.myData.mainShipLevel;
				num2 = global::UnityEngine.Mathf.Clamp(num2 - 1, 0, 999);
				int num3 = (int)((float)ShipDataManager.Instance.mainShipData.dataArray[num2].Attack * num);
				if (MyInventoryDataManager.Instance.GetMySailorData(ESailor.boatswain_s_inappp).isHave)
				{
					num3 += 150;
				}
				return num3;
			}
			int num4 = myLevel;
			int result = (int)((float)ShipDataManager.Instance.GetShipSheetData(baseData.id, num4).enemyAttack * DamageFactor);
			if (baseData.id == EShipType.Elite || baseData.id == EShipType.Ghost || baseData.id == EShipType.Bomber)
			{
				int num5 = PlayerInfo.Instance.myData.mainShipLevel;
				num5 = global::UnityEngine.Mathf.Clamp(num4 - 1, 0, 999);
				MainShipDataData mainShipDataData = ShipDataManager.Instance.mainShipData.dataArray[num5];
				EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp(myLevel / 1000 + 1, 1, 10);
				ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
				int num6 = (int)((float)(mainShipDataData.Attack * mainData.cannonCount) / mainShipDataData.Firecooltime);
				if (baseData.id == EShipType.Elite)
				{
					EShipMain level2 = (EShipMain)global::UnityEngine.Mathf.Clamp(myLevel / 1000 + 1, 1, 10);
					ShipMainData mainData2 = ShipDataManager.Instance.GetMainData(level2);
					result = (int)((float)num6 * 1f / (float)mainData2.cannonCount * 6f * DamageFactor);
				}
				else if (baseData.id == EShipType.Ghost || baseData.id == EShipType.Bomber)
				{
					result = (int)((float)num6 / 4f * 6f * DamageFactor);
				}
			}
			return result;
		}
	}

	public Health myHealth { get; private set; }

	public bool IsDie { get; private set; }

	public int myLevel { get; private set; }

	public HpBar HPBAR => hpBar;

	public MyShipInventory InvenShipData
	{
		get
		{
			return invenShipData;
		}
		set
		{
			invenShipData = value;
			MyInventoryDataManager.Instance.AddSubShip(this);
		}
	}

	protected virtual void Awake()
	{
		IsDie = false;
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		transform = GetComponent<global::UnityEngine.Transform>();
		modelTrans = transform.Find("Model");
		targetFinder = GetComponentInChildren<TargetFinder>();
		myHealth = GetComponent<Health>();
		rigidbody.angularDrag = 0.2f;
		rigidbody.drag = 0.8f;
		rigidbody.mass = 1000f;
		rigidbody.constraints = (global::UnityEngine.RigidbodyConstraints)116;
		enemyLayer = global::UnityEngine.LayerMask.NameToLayer("Enemy");
		enemyProjectileLayer = global::UnityEngine.LayerMask.NameToLayer("EnemyProjectile");
		playerLayer = global::UnityEngine.LayerMask.NameToLayer("Player");
		playerProjectileLayer = global::UnityEngine.LayerMask.NameToLayer("PlayerProjectile");
		if (myHealth == null)
		{
			myHealth = base.gameObject.AddComponent<Health>();
		}
		myHealth.AddHealAction(HpBarRefresh);
		myHealth.AddHitAction(HpBarRefresh);
		myHealth.AddHpSetAction(HpBarRefresh);
		myHealth.AddDieAction(Die);
	}

	public void SpawnShipModel(global::UnityEngine.GameObject obj, ELayerType type)
	{
		if (shipObj != null)
		{
			global::UnityEngine.Object.Destroy(shipObj);
		}
		shipObj = global::UnityEngine.Object.Instantiate(obj, modelTrans);
		global::UnityEngine.Transform obj2 = shipObj.transform;
		obj2.localPosition = global::UnityEngine.Vector3.zero;
		obj2.localRotation = global::UnityEngine.Quaternion.identity;
		myType = type;
		switch (myType)
		{
		case ELayerType.Player:
		{
			SetLayer(playerLayer, "Player");
			targetFinder.gameObject.layer = playerProjectileLayer;
			int flagId = PlayerInfo.Instance.myData.selectedFlagId;
			if (FlagManager.Instance.GetFlagInfo(flagId, out var info))
			{
				Flag[] componentsInChildren = GetComponentsInChildren<Flag>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].SetFlag(info.flagMaterial);
				}
			}
			break;
		}
		case ELayerType.Enemy:
			SetLayer(enemyLayer, "Enemy");
			targetFinder.gameObject.layer = enemyProjectileLayer;
			break;
		}
		weaponList = global::System.Linq.Enumerable.ToList(shipObj.GetComponentsInChildren<WeaponBase>());
		foreach (WeaponBase weapon in weaponList)
		{
			weapon.myShip = this;
			weapon.SetLayer(myType);
		}
		if ((bool)seaWaveParticle)
		{
			seaWaveParticle.tag = "Untagged";
			seaWaveParticle.layer = global::UnityEngine.LayerMask.NameToLayer("Default");
		}
		global::UnityEngine.SpriteRenderer[] componentsInChildren2 = GetComponentsInChildren<global::UnityEngine.SpriteRenderer>();
		if (componentsInChildren2.Length == 0)
		{
			return;
		}
		global::UnityEngine.SpriteRenderer[] array = componentsInChildren2;
		foreach (global::UnityEngine.SpriteRenderer spriteRenderer in array)
		{
			if (spriteRenderer.gameObject.name.Equals("AutoDrive Arrow"))
			{
				continue;
			}
			spriteRenderer.gameObject.layer = global::UnityEngine.LayerMask.NameToLayer("MiniMap");
			switch (myType)
			{
			case ELayerType.Player:
				if (this == GameManager.Instance.playerShip)
				{
					spriteRenderer.color = global::UnityEngine.Color.white;
				}
				break;
			case ELayerType.Enemy:
				spriteRenderer.color = myColor.GetColor(255, 71, 71);
				break;
			}
		}
	}

	private void Start()
	{
		if (!GetComponent<TaxShip>() && hpBar == null)
		{
			if (baseData.id == EShipType.Main || baseData.id == EShipType.Elite || baseData.id == EShipType.Bomber)
			{
				hpBar = HpBarManager.Instance.Generate(myType, isBig: true);
			}
			else if (baseData.id != EShipType.Ghost)
			{
				hpBar = HpBarManager.Instance.Generate(myType);
			}
			if (hpBar != null)
			{
				hpBar.SetGauge(myHealth.HpRatio);
			}
		}
	}

	private void HpBarRefresh()
	{
		if ((bool)hpBar)
		{
			hpBar.SetGauge(myHealth.HpRatio);
		}
	}

	public void SetBaseData(ShipBaseData data)
	{
		baseData = data;
	}

	public void SetLevel(int level)
	{
		myLevel = level;
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(baseData.id, myLevel);
		if (shipSheetData.level == myLevel)
		{
			float enemyHealthFactor = EnemySpawnManager.Instance.EnemyHealthFactor;
			if (baseData.id == EShipType.Elite)
			{
				global::UnityEngine.Transform obj = global::UnityEngine.Object.Instantiate(global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/EliteEnemyParticle"), transform).transform;
				obj.localScale = global::UnityEngine.Vector3.one * 7f;
				obj.localPosition = new global::UnityEngine.Vector3(0f, 0.5f, 0f);
				obj.localRotation = global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f);
				modelTrans.localScale = global::UnityEngine.Vector3.one * 1.7f;
				myHealth.SetHealthPoint((int)((float)shipSheetData.health * 1.5f * enemyHealthFactor));
			}
			else if (baseData.id == EShipType.Bomber)
			{
				global::UnityEngine.Transform obj2 = global::UnityEngine.Object.Instantiate(global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/EliteEnemyParticle"), transform).transform;
				obj2.localScale = global::UnityEngine.Vector3.one * 7f;
				obj2.localPosition = new global::UnityEngine.Vector3(0f, 0.5f, 0f);
				obj2.localRotation = global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f);
				myHealth.SetHealthPoint((int)((float)shipSheetData.health * 1.6f * enemyHealthFactor));
			}
			else if (baseData.id == EShipType.Ghost)
			{
				myHealth.SetHealthPoint((int)((float)shipSheetData.health * 1.2f * enemyHealthFactor));
			}
			else if (myType == ELayerType.Player)
			{
				myHealth.SetHealthPoint(shipSheetData.health);
			}
			else
			{
				myHealth.SetHealthPoint((int)((float)shipSheetData.enemyHealth * enemyHealthFactor));
			}
		}
	}

	private void FixedUpdate()
	{
		ShipRotate();
		TargetCheck();
		if (!isBoundaryNoUse)
		{
			Boundary();
		}
	}

	private void LateUpdate()
	{
		if ((bool)hpBar)
		{
			hpBar.SetPosition(transform.position + new global::UnityEngine.Vector3(0f, -1f, 0f));
		}
	}

	private void OnEnable()
	{
		if ((bool)hpBar)
		{
			hpBar.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if ((bool)hpBar)
		{
			hpBar.gameObject.SetActive(value: false);
		}
	}

	public void SetLayer(int layerId, string tagName)
	{
		global::UnityEngine.Transform[] componentsInChildren = this.transform.GetComponentsInChildren<global::UnityEngine.Transform>();
		Eagle componentInChildren = GetComponentInChildren<Eagle>();
		global::UnityEngine.Transform[] array = componentsInChildren;
		foreach (global::UnityEngine.Transform transform in array)
		{
			if (!(componentInChildren != null) || !(componentInChildren.transform == transform))
			{
				transform.gameObject.layer = layerId;
				transform.gameObject.tag = tagName;
			}
		}
	}

	public void SetTargetAngle(float target)
	{
		targetAngle = target;
	}

	public void Acceleration(float speed)
	{
		if (!IsDie && !isHolding && (!(GameManager.Instance != null) || !GameManager.Instance.IsShipPause))
		{
			if (transform == null)
			{
				transform = GetComponent<global::UnityEngine.Transform>();
			}
			rigidbody.AddForce(transform.forward * speed, global::UnityEngine.ForceMode.Acceleration);
		}
	}

	public void SetState(ShipState state)
	{
		foreach (WeaponBase weapon in weaponList)
		{
			weapon.SetState(state);
		}
	}

	private void Boundary()
	{
		float num = 7f;
		float x = global::UnityEngine.Mathf.Clamp(transform.position.x, (float)GameManager.Instance.worldManager.minX + num, (float)GameManager.Instance.worldManager.maxX - num);
		float z = global::UnityEngine.Mathf.Clamp(transform.position.z, (float)GameManager.Instance.worldManager.minZ + num, (float)GameManager.Instance.worldManager.maxZ - num);
		global::UnityEngine.Vector3 position = transform.position;
		position.x = x;
		position.z = z;
		transform.position = position;
	}

	private void TargetCheck()
	{
		if (targetFinder == null)
		{
			return;
		}
		global::System.Collections.Generic.HashSet<Health> healthHashSet = targetFinder.HealthHashSet;
		if (healthHashSet.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < weaponList.Count; i++)
		{
			float num = float.PositiveInfinity;
			Health health = null;
			foreach (Health item in healthHashSet)
			{
				if (item == null)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < orderList.Length; j++)
				{
					if (item.myShip != null && orderList[j] == item.myShip.baseData.id)
					{
						health = item;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				float sqrMagnitude = (weaponList[i].transform.position - item.transform.position).sqrMagnitude;
				if (num > sqrMagnitude && weaponList[i].IsTargetPosible(item.transform))
				{
					num = sqrMagnitude;
					health = item;
				}
			}
			if (health != null)
			{
				weaponList[i].SetTarget(health);
			}
		}
	}

	private void ShipRotate()
	{
		if (isHolding || (GameManager.Instance != null && GameManager.Instance.IsShipPause))
		{
			return;
		}
		if (rigidbody.velocity.sqrMagnitude > 0f || isRotateMode)
		{
			if (IsDie)
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
					ComebackZAngle();
				}
				else
				{
					RotateZAngle(flag);
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
				ComebackZAngle();
			}
		}
		else
		{
			ComebackZAngle();
		}
	}

	private void ComebackZAngle()
	{
		if (modelTrans != null && (modelTrans.localRotation.eulerAngles.z < 0f - global::UnityEngine.Mathf.Epsilon || modelTrans.localRotation.eulerAngles.z > global::UnityEngine.Mathf.Epsilon))
		{
			modelTrans.localRotation = global::UnityEngine.Quaternion.Slerp(modelTrans.localRotation, global::UnityEngine.Quaternion.identity, global::UnityEngine.Time.fixedDeltaTime);
		}
	}

	private void RotateZAngle(bool isRight)
	{
		float num = ((!isRight) ? (-10f) : 10f);
		if (modelTrans != null)
		{
			if (modelTrans.localRotation.eulerAngles.z > num - global::UnityEngine.Mathf.Epsilon && modelTrans.localRotation.eulerAngles.z < num + global::UnityEngine.Mathf.Epsilon)
			{
				modelTrans.localRotation = global::UnityEngine.Quaternion.Slerp(modelTrans.localRotation, global::UnityEngine.Quaternion.identity, global::UnityEngine.Time.fixedDeltaTime);
			}
			else
			{
				modelTrans.localRotation = global::UnityEngine.Quaternion.Slerp(modelTrans.localRotation, global::UnityEngine.Quaternion.Euler(0f, 0f, num), global::UnityEngine.Time.fixedDeltaTime);
			}
		}
	}

	public void SetHolding(bool isActive)
	{
		isHolding = isActive;
		rigidbody.isKinematic = isHolding;
	}

	public void Reset()
	{
		targetAngle = -999f;
	}

	public void Die()
	{
		IsDie = true;
		rigidbody.velocity = global::UnityEngine.Vector3.zero;
		global::UnityEngine.Collider[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		global::DG.Tweening.ShortcutExtensions.DOShakePosition(modelTrans, 0.8f, 0.6f, 50);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, -15f, 2f), 1.2f), global::DG.Tweening.Ease.Linear), delegate
		{
			ShipDestroy();
		});
		if ((bool)hpBar)
		{
			global::UnityEngine.Object.Destroy(hpBar.gameObject);
		}
		ShipBase shipBase = null;
		shipBase = GameManager.Instance.playerShip;
		if (InvenShipData == null && this != shipBase && TutorialManager.Instance.IsPlaying("firstbattle"))
		{
			TutorialManager.Instance.Next();
		}
	}

	public void ShipDestroy(bool dropItem = true)
	{
		if ((bool)hpBar)
		{
			global::UnityEngine.Object.Destroy(hpBar.gameObject);
		}
		if (checkDelegate != null)
		{
			checkDelegate(this);
		}
		EnemySpawnManager.Instance.RemoveEnemyList(this);
		if (beforeDestroyAction != null)
		{
			beforeDestroyAction();
		}
		ShipBase shipBase = null;
		shipBase = GameManager.Instance.playerShip;
		if (InvenShipData != null)
		{
			MyInventoryDataManager.Instance.RemoveShip(InvenShipData.id);
		}
		else if (this != shipBase)
		{
			if (!dropItem || myType == ELayerType.Player)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (ClaimManager.Instance.IsPlaying && global::UnityEngine.Random.value <= 0.8f)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			int num = global::UnityEngine.Mathf.Clamp(myLevel - 1, 0, 999);
			EnemyRewardData obj = ShipDataManager.Instance.enemyReward.dataArray[num];
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			num2 = obj.Normalgold;
			if ((bool)PlayerInfo.Instance.myData.isGetDouble)
			{
				num2 *= 2;
				num3 *= 2;
				num4 *= 2;
			}
			float num5 = ConstValue.rewardBoxGeneratePercent + (float)PlayerInfo.Instance.myData.rewardboxBonusPercentCount * ConstValue.rewardBoxBonusPercent;
			if (baseData.id == EShipType.Elite || baseData.id == EShipType.Bomber)
			{
				num5 = 10000f;
			}
			if (!TutorialManager.Instance.IsHadTutorial("battlewasperfect"))
			{
				num5 = -1f;
			}
			int num6 = 1;
			if (baseData.id == EShipType.Elite || baseData.id == EShipType.Bomber)
			{
				num6 = 10;
				num2 /= 2;
			}
			else if (baseData.id == EShipType.Main || baseData.id == EShipType.Ghost)
			{
				num6 = 4;
			}
			bool flag = false;
			if (global::UnityEngine.Random.Range(0f, 10000f) < num5)
			{
				flag = true;
			}
			if (ClaimManager.Instance.IsPlaying)
			{
				flag = false;
			}
			for (int i = 0; i < num6; i++)
			{
				if (i == 0 && flag)
				{
					RewardBox rewardBox = null;
					if (GameManager.Instance.GenerateRewardBox(transform.position, out rewardBox))
					{
						float num7 = global::UnityEngine.Random.Range(0f, 10000000f);
						float num8;
						float num9;
						float num10;
						if (baseData.id == EShipType.Elite || baseData.id == EShipType.Bomber)
						{
							_ = ConstValue.rewardBoxEliteBronzePercent;
							_ = (int)PlayerInfo.Instance.myData.mainShipLevel;
							_ = ConstValue.rewardBoxEliteBronzeBonusPercent;
							num8 = ConstValue.rewardBoxEliteSilverPercent + (float)(int)PlayerInfo.Instance.myData.mainShipLevel * ConstValue.rewardBoxEliteSilverBonusPercent;
							num9 = ConstValue.rewardBoxEliteGoldPercent + (float)(int)PlayerInfo.Instance.myData.mainShipLevel * ConstValue.rewardBoxEliteGoldBonusPercent;
							num10 = ConstValue.rewardBoxEliteBlackPercent + (float)(int)PlayerInfo.Instance.myData.mainShipLevel * ConstValue.rewardBoxEliteBlackBonusPercent;
						}
						else
						{
							_ = ConstValue.rewardBoxBronzePercent;
							_ = (int)PlayerInfo.Instance.myData.mainShipLevel;
							_ = ConstValue.rewardBoxBronzeBonusPercent;
							num8 = ConstValue.rewardBoxSilverPercent + (float)(int)PlayerInfo.Instance.myData.mainShipLevel * ConstValue.rewardBoxSilverBonusPercent;
							num9 = ConstValue.rewardBoxGoldPercent + (float)(int)PlayerInfo.Instance.myData.mainShipLevel * ConstValue.rewardBoxGoldBonusPercent;
							num10 = ConstValue.rewardBoxBlackPercent + (float)(int)PlayerInfo.Instance.myData.mainShipLevel * ConstValue.rewardBoxBlackBonusPercent;
						}
						ERewardBox grade = ((num7 <= num10) ? ERewardBox.BLACK : ((num7 <= num10 + num9) ? ERewardBox.GOLD : ((!(num7 <= num10 + num9 + num8)) ? ERewardBox.BRONZE : ERewardBox.SILVER)));
						rewardBox.SetRewardBox(grade, myLevel);
					}
					PlayerInfo.Instance.myData.rewardboxBonusPercentCount = 0;
				}
				else
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
			}
			if (!flag)
			{
				PlayerInfo.Instance.myData.rewardboxBonusPercentCount++;
			}
			if (EnemySpawnManager.Instance.spawnEnemyList.Count <= 0 && TutorialManager.Instance.IsHadTutorial("secondbattle"))
			{
				TutorialManager.Instance.StartTutorial("battlewasperfect");
			}
			float num11 = ConstValue.rewardBoxGeneratePercent + (float)PlayerInfo.Instance.myData.lifeBeadBonusPercentCount * ConstValue.rewardBoxBonusPercent;
			bool flag2 = false;
			if (global::UnityEngine.Random.Range(0f, 10000f) < num11)
			{
				flag2 = true;
			}
			if (flag2)
			{
				if (GameManager.Instance.GenerateLifeBead(transform.position, out var lifeBead))
				{
					lifeBead.SetMove(transform.position + new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-20f, 20f), 0f, global::UnityEngine.Random.Range(-20f, 20f)));
				}
				PlayerInfo.Instance.myData.lifeBeadBonusPercentCount = 0;
			}
			else
			{
				PlayerInfo.Instance.myData.lifeBeadBonusPercentCount++;
			}
			PlayerInfo.Instance.SaveData();
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private bool IsUniqueShip(EShipType type)
	{
		bool result = false;
		if (type == EShipType.Repair)
		{
			result = true;
		}
		if (type == EShipType.Multi)
		{
			result = true;
		}
		if (type == EShipType.Harpoon)
		{
			result = true;
		}
		if (type == EShipType.Guide)
		{
			result = true;
		}
		if (type == EShipType.Gas)
		{
			result = true;
		}
		return result;
	}

	private bool IsNormalShip(EShipType type)
	{
		bool result = false;
		if (type == EShipType.Cannon)
		{
			result = true;
		}
		if (type == EShipType.Normal)
		{
			result = true;
		}
		if (type == EShipType.Rapid)
		{
			result = true;
		}
		if (type == EShipType.SuicideBomb)
		{
			result = true;
		}
		if (type == EShipType.Fishing)
		{
			result = true;
		}
		return result;
	}
}
