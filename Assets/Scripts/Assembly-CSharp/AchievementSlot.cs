public class AchievementSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text achievementName;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text contentText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasBtnGet;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button btnGet;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image gaugeTop;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject alphaBG;

	private EAchievement myType;

	public bool IsCanGetReward { get; private set; }

	private void Awake()
	{
		achievementName.font = LanguageManager.Instance.getLanguageFont();
		contentText.font = LanguageManager.Instance.getLanguageFont();
	}

	public void SetAchievement(EAchievement achievementType)
	{
		myType = achievementType;
		switch (myType)
		{
		case EAchievement.Attendance:
			achievementName.text = LanguageManager.Instance.translate("achievement_01");
			break;
		case EAchievement.GetCrew:
			achievementName.text = LanguageManager.Instance.translate("achievement_03");
			break;
		case EAchievement.GetFlag:
			achievementName.text = LanguageManager.Instance.translate("achievement_06");
			break;
		case EAchievement.GetIsland:
			achievementName.text = LanguageManager.Instance.translate("achievement_04");
			break;
		case EAchievement.KillBoss:
			achievementName.text = LanguageManager.Instance.translate("achievement_07");
			break;
		case EAchievement.KillEnemy:
			achievementName.text = LanguageManager.Instance.translate("achievement_05");
			break;
		case EAchievement.Level:
			achievementName.text = LanguageManager.Instance.translate("achievement_02");
			break;
		}
		Refresh();
	}

	public void Refresh()
	{
		int num2;
		int num = (num2 = 0);
		switch (myType)
		{
		case EAchievement.Attendance:
			num = PlayerInfo.Instance.myData.attendanceCount;
			num2 = PlayerInfo.Instance.myData.achievement_lvl_attend;
			break;
		case EAchievement.GetCrew:
		{
			int num3 = 0;
			foreach (global::System.Collections.Generic.KeyValuePair<ESailor, MySailorInventory> item in MyInventoryDataManager.Instance.mySailorDataDic)
			{
				if (item.Value.isHave)
				{
					num3++;
				}
			}
			num = num3;
			num2 = PlayerInfo.Instance.myData.achievement_lvl_getcrew * 5;
			break;
		}
		case EAchievement.GetFlag:
		{
			int num4 = 0;
			foreach (FlagInfo flag in FlagManager.Instance.flagList)
			{
				if (PlayerInfo.Instance.IsHaveFlag(flag.id))
				{
					num4++;
				}
			}
			num2 = PlayerInfo.Instance.myData.achievement_lvl_getflag * 3;
			num = num4;
			break;
		}
		case EAchievement.GetIsland:
			num = PlayerInfo.Instance.ClearIslandCount;
			num2 = PlayerInfo.Instance.myData.achievement_lvl_getisland * 5;
			break;
		case EAchievement.KillBoss:
			num = PlayerInfo.Instance.myData.bossKillCount;
			num2 = PlayerInfo.Instance.myData.achievement_lvl_killboss;
			break;
		case EAchievement.KillEnemy:
			num = PlayerInfo.Instance.myData.enemyKillCount;
			if (PlayerInfo.Instance.myData.achievement_lvl_killenemy < 3)
			{
				num2 = 20 + PlayerInfo.Instance.myData.achievement_lvl_killenemy * 30;
			}
			num2 = PlayerInfo.Instance.myData.achievement_lvl_killenemy * 20;
			break;
		case EAchievement.Level:
			num = PlayerInfo.Instance.myData.mainShipLevel;
			break;
		}
		float fillAmount = (float)num / (float)num2;
		contentText.text = $"({num} / {num2})";
		gaugeTop.fillAmount = fillAmount;
	}

	public void SetAlphaBackgroundActive(bool active)
	{
		alphaBG.SetActive(active);
	}
}
