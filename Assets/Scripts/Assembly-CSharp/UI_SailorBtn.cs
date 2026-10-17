public class UI_SailorBtn : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] redGraphics;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject signObj;

	private void OnEnable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.AddCrewAction = (global::System.Action)global::System.Delegate.Combine(instance.AddCrewAction, new global::System.Action(CheckIsHaveCanUpgradeCrew));
		MyInventoryDataManager instance2 = MyInventoryDataManager.Instance;
		instance2.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Combine(instance2.UpgradeCrewAction, new global::System.Action(CheckIsHaveCanUpgradeCrew));
		CheckIsHaveCanUpgradeCrew();
	}

	private void OnDisable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.AddCrewAction = (global::System.Action)global::System.Delegate.Remove(instance.AddCrewAction, new global::System.Action(CheckIsHaveCanUpgradeCrew));
		MyInventoryDataManager instance2 = MyInventoryDataManager.Instance;
		instance2.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Remove(instance2.UpgradeCrewAction, new global::System.Action(CheckIsHaveCanUpgradeCrew));
	}

	public void CheckIsHaveCanUpgradeCrew()
	{
		bool flag = false;
		foreach (MySailorInventory value in MyInventoryDataManager.Instance.mySailorDataDic.Values)
		{
			if (value.haveCount >= value.NextNeedCount && value.level < value.baseData.maxCount)
			{
				flag = true;
				break;
			}
		}
		if (flag || !TutorialManager.Instance.IsHadTutorial("crewpopup"))
		{
			global::UnityEngine.UI.Graphic[] array = redGraphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = myColor.GetColor(255, 0, 66);
			}
			if (!signObj.activeSelf)
			{
				signObj.SetActive(value: true);
				AudioManager.Instance.PlaySFX("ui_complete_rewardbox");
			}
		}
		else
		{
			global::UnityEngine.UI.Graphic[] array = redGraphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = global::UnityEngine.Color.white;
			}
			if (signObj.activeSelf)
			{
				signObj.SetActive(value: false);
			}
		}
	}
}
