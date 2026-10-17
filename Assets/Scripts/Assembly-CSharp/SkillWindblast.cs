public class SkillWindblast : Skill
{
	private global::UnityEngine.GameObject windblastSrc;

	private global::UnityEngine.GameObject windObj;

	protected override void Awake()
	{
		base.Awake();
		windblastSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/Windblast");
	}

	protected override void SetMyCoolTimeKey()
	{
		myCoolTimeKey = "SkillWindblast";
	}

	protected override bool IsHaveSkill()
	{
		bool result = false;
		foreach (ESailor item in SailorDataManager.Instance.skill_windblast)
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
		windObj = global::UnityEngine.Object.Instantiate(windblastSrc, playerShip.transform.position, global::UnityEngine.Quaternion.identity, playerShip.transform);
		Windblast component = windObj.GetComponent<Windblast>();
		int num = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		float shieldScale = 1f;
		switch (num)
		{
		case 1:
			shieldScale = 0.57f;
			break;
		case 2:
			shieldScale = 0.68f;
			break;
		case 3:
			shieldScale = 0.74f;
			break;
		case 4:
		case 5:
			shieldScale = 0.8f;
			break;
		case 6:
		case 7:
		case 8:
			shieldScale = 1f;
			break;
		case 9:
			shieldScale = 1.15f;
			break;
		case 10:
			shieldScale = 1.26f;
			break;
		}
		EShipMain level = (EShipMain)num;
		int level2 = PlayerInfo.Instance.myData.mainShipLevel;
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(EShipType.Main, level2);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num2 = (int)((float)(playerShip.Damage * mainData.cannonCount) / shipSheetData.fireCoolTime);
		float num3 = 3f;
		float num4 = 0.1f;
		if (SailorDataManager.Instance.skill_windblast.Count > 0)
		{
			switch (MyInventoryDataManager.Instance.GetMySailorData(SailorDataManager.Instance.skill_windblast[0]).level)
			{
			case 1:
				num3 = 3f;
				durationTime = 30f;
				coolTime = 120f;
				num4 = 0.1f;
				break;
			case 2:
				num3 = 3.25f;
				durationTime = 30f;
				coolTime = 115f;
				num4 = 0.15f;
				break;
			case 3:
				num3 = 3.5f;
				durationTime = 30f;
				coolTime = 110f;
				num4 = 0.2f;
				break;
			case 4:
				num3 = 3.75f;
				durationTime = 30f;
				coolTime = 105f;
				num4 = 0.25f;
				break;
			case 5:
				num3 = 4f;
				durationTime = 30f;
				coolTime = 100f;
				num4 = 0.3f;
				break;
			}
		}
		component.damage = (int)((float)num2 * num3);
		component.SetShieldScale(shieldScale);
		playerShip.myHealth.AddShieldEnergy((int)((float)playerShip.myHealth.HP_MAX * num4));
		AudioManager.Instance.PlaySFX("skill_windblast");
	}

	private void OverSkill()
	{
		GameManager.Instance.playerShip.myHealth.RemoveShield();
		if (windObj != null)
		{
			global::UnityEngine.Object.Destroy(windObj);
		}
	}
}
