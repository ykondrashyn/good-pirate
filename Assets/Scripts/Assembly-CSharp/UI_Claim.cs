public class UI_Claim : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text spectateUserNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject spectateUIObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text missionTimeLeftText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text missionPercentText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image missionPercentGaugeImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject loseNormalObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject loseTimeOutObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject missionStatusPanel;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject winPopup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject losePopup;

	public void SetName(string str)
	{
		spectateUserNameText.text = str;
	}

	public void SetSpectateModeActive(bool active)
	{
		spectateUIObj.SetActive(active);
	}

	public void SetLeftTimeColor(global::UnityEngine.Color color)
	{
		missionTimeLeftText.color = color;
	}

	public void SetLeftTime(string str)
	{
		missionTimeLeftText.text = str;
	}

	public void SetMissionPercent(float ratio)
	{
		ratio = global::UnityEngine.Mathf.Clamp01(ratio);
		int num = (int)(ratio * 100f);
		missionPercentGaugeImage.fillAmount = ratio;
		missionPercentText.text = num + "%";
	}

	public void Reset()
	{
		SetMissitonStatusActive(active: false);
		SetSpectateModeActive(active: false);
		winPopup.SetActive(value: false);
		losePopup.SetActive(value: false);
	}

	public void SetMissitonStatusActive(bool active)
	{
		missionStatusPanel.SetActive(active);
	}

	public void Win()
	{
		winPopup.SetActive(value: true);
	}

	public void Lose(bool isTime = false)
	{
		losePopup.SetActive(value: true);
		loseNormalObj.SetActive(!isTime);
		loseTimeOutObj.SetActive(isTime);
	}
}
