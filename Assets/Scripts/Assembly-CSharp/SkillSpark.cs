public class SkillSpark : Skill
{
	private global::UnityEngine.GameObject sparkSrc;

	private global::UnityEngine.GameObject sparkObj;

	protected override void Awake()
	{
		base.Awake();
		sparkSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/Spark");
	}

	protected override void SetMyCoolTimeKey()
	{
		myCoolTimeKey = "SkillSpark";
	}

	protected override bool IsHaveSkill()
	{
		bool result = false;
		foreach (ESailor item in SailorDataManager.Instance.skill_spark)
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(item);
			if (mySailorData != null && mySailorData.isHave)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void Start()
	{
		AddActiveCallBack(ActiveSkill);
		AddSkillOverCallBack(OverSkill);
	}

	private void ActiveSkill()
	{
		durationTime = 20f;
		coolTime = 5f;
		ShipBase playerShip = GameManager.Instance.playerShip;
		sparkObj = global::UnityEngine.Object.Instantiate(sparkSrc, playerShip.transform.position, global::UnityEngine.Quaternion.identity, playerShip.transform);
		int num = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		float num2 = 1f;
		switch (num)
		{
		case 1:
		case 2:
			num2 = 0.57f;
			break;
		case 3:
		case 4:
			num2 = 0.74f;
			break;
		case 5:
		case 6:
			num2 = 0.91f;
			break;
		case 7:
		case 8:
		case 9:
		case 10:
			num2 = 1f;
			break;
		}
		EShipMain level = (EShipMain)num;
		int level2 = PlayerInfo.Instance.myData.mainShipLevel;
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(EShipType.Main, level2);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num3 = (int)((float)(playerShip.Damage * mainData.cannonCount) / shipSheetData.fireCoolTime);
		float num4 = 3f;
		float num5 = 1f;
		if (SailorDataManager.Instance.skill_spark.Count > 0)
		{
			switch (MyInventoryDataManager.Instance.GetMySailorData(SailorDataManager.Instance.skill_spark[0]).level)
			{
			case 1:
				num4 = 0.8f;
				durationTime = 15f;
				coolTime = 120f;
				num5 = 1.6f;
				break;
			case 2:
				num4 = 0.9f;
				durationTime = 15f;
				coolTime = 115f;
				num5 = 1.7f;
				break;
			case 3:
				num4 = 1f;
				durationTime = 15f;
				coolTime = 110f;
				num5 = 1.8f;
				break;
			case 4:
				num4 = 1.1f;
				durationTime = 15f;
				coolTime = 105f;
				num5 = 1.9f;
				break;
			case 5:
				num4 = 1.2f;
				durationTime = 15f;
				coolTime = 100f;
				num5 = 2f;
				break;
			}
		}
		playerShip.accelerationFactor = num5;
		playerShip.turnSpeedFactor = num5;
		playerShip.rigidbody.velocity *= num5;
		sparkObj.GetComponent<Spark>().damage = (int)((float)num3 * num4);
		sparkObj.transform.localScale = global::UnityEngine.Vector3.one * num2;
	}

	private void OverSkill()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		playerShip.accelerationFactor = 1f;
		playerShip.turnSpeedFactor = 1f;
		if (sparkObj != null)
		{
			global::UnityEngine.Object.Destroy(sparkObj);
		}
	}
}
