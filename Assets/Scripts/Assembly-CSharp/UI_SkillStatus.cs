public class UI_SkillStatus : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image fillImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button adsButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite fullSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite fullInactiveSprite;

	[global::UnityEngine.SerializeField]
	private Skill[] mySkills;

	private int canUseCount = 5;

	private const int SKILL_MAX_COUNT = 5;

	private float totalTime;

	private float coolTime = 180f;

	private void Awake()
	{
		canUseCount = PlayerInfo.Instance.myData.canUseSkillCount;
		TextRefresh();
	}

	private void OnEnable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Combine(instance.UpgradeCrewAction, new global::System.Action(CheckMySkills));
	}

	private void OnDisable()
	{
		MyInventoryDataManager instance = MyInventoryDataManager.Instance;
		instance.UpgradeCrewAction = (global::System.Action)global::System.Delegate.Remove(instance.UpgradeCrewAction, new global::System.Action(CheckMySkills));
	}

	public bool IsCanUse()
	{
		return canUseCount > 0;
	}

	public void Used()
	{
		adsButton.GetComponent<global::UnityEngine.UI.Image>().sprite = fullSprite;
		adsButton.enabled = true;
		canUseCount--;
		PlayerInfo.Instance.myData.canUseSkillCount = canUseCount;
		PlayerInfo.Instance.SaveData();
		TextRefresh();
		CheckMySkills();
	}

	public void ShowAds()
	{
		if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
		{
			AdsSuccess();
			return;
		}
		MoPubAdManager.Instance.ShowRewardedVideo(AdsSuccess);
		GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "스킬 회복", 1L);
	}

	private void AdsSuccess()
	{
		canUseCount = 5;
		adsButton.GetComponent<global::UnityEngine.UI.Image>().sprite = fullInactiveSprite;
		adsButton.enabled = false;
		fillImage.fillAmount = 1f;
		CheckMySkills();
		TextRefresh();
		totalTime = 0f;
	}

	private void LateUpdate()
	{
		if (canUseCount >= 5)
		{
			adsButton.GetComponent<global::UnityEngine.UI.Image>().sprite = fullInactiveSprite;
			adsButton.enabled = false;
			return;
		}
		totalTime += global::UnityEngine.Time.deltaTime;
		if (totalTime >= coolTime)
		{
			totalTime -= coolTime;
			canUseCount++;
			TextRefresh();
			PlayerInfo.Instance.myData.canUseSkillCount = canUseCount;
			PlayerInfo.Instance.SaveData();
			CheckMySkills();
		}
		float fillAmount = (float)canUseCount * 0.2f + global::UnityEngine.Mathf.Lerp(0f, 0.2f, totalTime / coolTime);
		fillImage.fillAmount = fillAmount;
	}

	private void TextRefresh()
	{
		myText.text = $"{canUseCount}/{5}";
	}

	private void CheckMySkills()
	{
		Skill[] array = mySkills;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Check();
		}
	}
}
