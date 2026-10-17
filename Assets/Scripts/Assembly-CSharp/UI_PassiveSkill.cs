public class UI_PassiveSkill : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] bushs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] fogs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] rocks;

	public void BushSetActive(bool active, out bool isSkillActive)
	{
		isSkillActive = false;
		if (active)
		{
			global::System.Collections.Generic.List<ESailor> skill_avoidEnvironment = SailorDataManager.Instance.skill_avoidEnvironment;
			if (skill_avoidEnvironment.Count > 0)
			{
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_avoidEnvironment[0]);
				if (mySailorData.isHave && mySailorData.level >= 3)
				{
					bushs[0].SetActive(value: false);
					bushs[1].SetActive(value: true);
					isSkillActive = true;
				}
				else
				{
					bushs[0].SetActive(value: true);
					bushs[1].SetActive(value: false);
				}
			}
		}
		else
		{
			global::UnityEngine.GameObject[] array = bushs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}

	public void FogSetActive(bool active, out bool isSkillActive)
	{
		isSkillActive = false;
		if (active)
		{
			global::System.Collections.Generic.List<ESailor> skill_avoidEnvironment = SailorDataManager.Instance.skill_avoidEnvironment;
			if (skill_avoidEnvironment.Count > 0)
			{
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_avoidEnvironment[0]);
				if (mySailorData.isHave && mySailorData.level >= 1)
				{
					fogs[0].SetActive(value: false);
					fogs[1].SetActive(value: true);
					isSkillActive = true;
				}
				else
				{
					fogs[0].SetActive(value: true);
					fogs[1].SetActive(value: false);
				}
			}
		}
		else
		{
			global::UnityEngine.GameObject[] array = fogs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}

	public void RockSetActive(bool active, out bool isSkillActive)
	{
		isSkillActive = false;
		if (active)
		{
			global::System.Collections.Generic.List<ESailor> skill_avoidEnvironment = SailorDataManager.Instance.skill_avoidEnvironment;
			if (skill_avoidEnvironment.Count > 0)
			{
				MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_avoidEnvironment[0]);
				if (mySailorData.isHave && mySailorData.level >= 2)
				{
					rocks[0].SetActive(value: false);
					rocks[1].SetActive(value: true);
					isSkillActive = true;
				}
				else
				{
					rocks[0].SetActive(value: true);
					rocks[1].SetActive(value: false);
				}
			}
		}
		else
		{
			global::UnityEngine.GameObject[] array = rocks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}
}
