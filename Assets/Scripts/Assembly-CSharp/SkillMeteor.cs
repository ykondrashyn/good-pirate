public class SkillMeteor : Skill
{
	private global::UnityEngine.GameObject meteorFloorSrc;

	private global::UnityEngine.GameObject meteorEnemySrc;

	private global::UnityEngine.GameObject buffParticleSrc;

	private global::UnityEngine.GameObject buffParticleObj;

	protected override void Awake()
	{
		base.Awake();
		meteorFloorSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/MeteorFloor");
		meteorEnemySrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/MeteorEnemy");
		buffParticleSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/Overheat");
	}

	private void Start()
	{
		AddActiveCallBack(ActiveSkill);
		AddSkillOverCallBack(OverSkill);
	}

	protected override void SetMyCoolTimeKey()
	{
		myCoolTimeKey = "SkillMeteor";
	}

	protected override bool IsHaveSkill()
	{
		bool result = false;
		foreach (ESailor item in SailorDataManager.Instance.skill_meteor)
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

	private void ActiveSkill()
	{
		int num = 30;
		float min = 0.3f;
		float max = 3f;
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::System.Collections.Generic.List<Health> list = new global::System.Collections.Generic.List<Health>();
		foreach (Health allEnemyHealth in EnemySpawnManager.Instance.allEnemyHealthList)
		{
			if (allEnemyHealth != null && (playerShip.transform.position - allEnemyHealth.transform.position).magnitude <= 90f)
			{
				list.Add(allEnemyHealth);
			}
		}
		buffParticleObj = global::UnityEngine.Object.Instantiate(buffParticleSrc, playerShip.transform);
		int num2 = global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		float num3 = 1f;
		switch (num2)
		{
		case 1:
			num3 = 5f;
			break;
		case 2:
		case 3:
			num3 = 5.6f;
			break;
		case 4:
		case 5:
			num3 = 6.7f;
			break;
		case 6:
		case 7:
			num3 = 7.5f;
			break;
		case 8:
		case 9:
			num3 = 8.7f;
			break;
		case 10:
			num3 = 10f;
			break;
		}
		buffParticleObj.transform.localScale = global::UnityEngine.Vector3.one * num3;
		for (int i = 0; i < num; i++)
		{
			global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(meteorFloorSrc);
			Meteor component = obj.GetComponent<Meteor>();
			global::UnityEngine.Transform obj2 = obj.transform;
			global::UnityEngine.Vector3 forward = global::UnityEngine.Vector3.forward;
			global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * forward;
			float num4 = global::UnityEngine.Random.Range(10f, 150f);
			obj2.position = playerShip.transform.position + vector * num4;
			component.StartFallingDown(global::UnityEngine.Random.Range(min, max));
		}
		EShipMain level = (EShipMain)num2;
		int level2 = PlayerInfo.Instance.myData.mainShipLevel;
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(EShipType.Main, level2);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
		int num5 = (int)((float)(playerShip.Damage * mainData.cannonCount) / shipSheetData.fireCoolTime);
		float num6 = 8f;
		if (SailorDataManager.Instance.skill_meteor.Count > 0)
		{
			switch (MyInventoryDataManager.Instance.GetMySailorData(SailorDataManager.Instance.skill_meteor[0]).level)
			{
			case 1:
				num6 = 4f;
				durationTime = 20f;
				coolTime = 300f;
				playerShip.DamageFactor = 1.2f;
				break;
			case 2:
				num6 = 4.5f;
				durationTime = 22f;
				coolTime = 280f;
				playerShip.DamageFactor = 1.25f;
				break;
			case 3:
				num6 = 5f;
				durationTime = 24f;
				coolTime = 260f;
				playerShip.DamageFactor = 1.35f;
				break;
			case 4:
				num6 = 5.5f;
				durationTime = 26f;
				coolTime = 240f;
				playerShip.DamageFactor = 1.45f;
				break;
			case 5:
				num6 = 6f;
				durationTime = 28f;
				coolTime = 220f;
				playerShip.DamageFactor = 1.5f;
				break;
			}
		}
		num5 = (int)((float)num5 * num6);
		foreach (Health item in list)
		{
			Meteor component2 = global::UnityEngine.Object.Instantiate(meteorEnemySrc, item.transform).GetComponent<Meteor>();
			component2.SetHealth(item);
			component2.damage = num5;
			component2.StartFallingDown(global::UnityEngine.Random.Range(min, max));
		}
	}

	private void OverSkill()
	{
		GameManager.Instance.playerShip.DamageFactor = 1f;
		if (buffParticleObj != null)
		{
			global::UnityEngine.Object.Destroy(buffParticleObj);
			buffParticleObj = null;
		}
	}
}
