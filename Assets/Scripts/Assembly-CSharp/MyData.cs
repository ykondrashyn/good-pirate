[global::System.Serializable]
public class MyData
{
	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isRemoveADS = false;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isCoinDouble = false;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isSoundOn = true;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isMusicOn = true;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredLong gold = 0L;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredLong fish = 0L;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredLong wood = 0L;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredLong iron = 0L;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredLong diamond = 0L;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt mainShipLevel = 1;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt reserveShipCount = 0;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt selectedCubeNum = 1;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt selectedWorldNum = 1;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt selectedIslandId = 0;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt selectedFormationNum = 0;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt selectedFlagId = 0;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isHaveSettingNotification;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isHaveMessageNotification;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isHaveInventoryNotification;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isHaveCrewNotification;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredInt resurrectionTicket = 0;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isWatchDieAskPopup = false;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isHaveEagle;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isHavePirrot;

	public bool isWatchedFreeBoxAds;

	public float tornadoEventTotalTime;

	public float ghostEventTotalTime;

	public int myBestStage;

	public global::CodeStage.AntiCheat.ObscuredTypes.ObscuredBool isGetDouble = false;

	public int lastLeftHp;

	public int destinationIslandId = -1;

	public bool isHaveThirdRewardBoxSlot;

	public RewardBoxStatus[] rewardBoxStatus = new RewardBoxStatus[3];

	public global::System.Collections.Generic.List<RewardBoxStatus> storeGetRewardBoxList = new global::System.Collections.Generic.List<RewardBoxStatus>();

	public global::System.Collections.Generic.List<EGrade> storeCardList = new global::System.Collections.Generic.List<EGrade>();

	public int rewardboxBonusPercentCount;

	public int lifeBeadBonusPercentCount;

	public bool isWatchedTutorial;

	public bool isShowIntro;

	public bool isChangeLanguage;

	public int canUseSkillCount;

	public global::UnityEngine.SystemLanguage selectedLanguage;

	public bool isTodayPackCanBuy;

	public bool isRefreshTodayPack;

	public int todayPackRefreshCount;

	public ESailor todaySelectedLegendarySailor;

	public int lastConnectedMonth;

	public int lastConnectedDay;

	public bool isBuyRaft;

	public bool isBuyStarter;

	public bool isBuyIntermediate;

	public bool isBuyExpert;

	public bool isTargetFps60 = true;

	public bool isRateUs;

	public long lastQuitTimeTick;

	public int enemyKillCount;

	public int bossKillCount;

	public int attendanceCount;

	public int achievement_lvl_attend = 1;

	public int achievement_lvl_level = 1;

	public int achievement_lvl_getcrew = 1;

	public int achievement_lvl_getisland = 1;

	public int achievement_lvl_killenemy = 1;

	public int achievement_lvl_getflag = 1;

	public int achievement_lvl_killboss = 1;

	public eShipSkin selectedSkin = eShipSkin.Normal;

	public bool isHadClaimClear;

	public float taxTimeTotal;

	public int showRvCount;

	public bool send3RvEvent;
}
