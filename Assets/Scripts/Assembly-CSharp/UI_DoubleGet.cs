public class UI_DoubleGet : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject inactiveObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject activeObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	private const string Key = "GetDouble";

	private const string TimeFormat = "{0}:{1}";

	private bool isActive;

	public void Active()
	{
		Success();
	}

	private void Success()
	{
		CoolTimeManager.AddCooltimeSecondFromNow("GetDouble", 1801);
		inactiveObj.SetActive(value: false);
		activeObj.SetActive(value: true);
		PlayerInfo.Instance.myData.isGetDouble = true;
		isActive = true;
		PlayerInfo.Instance.SaveData();
	}

	private void Start()
	{
		global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime("GetDouble");
		if (leftCoolTime.TotalSeconds > 0.0)
		{
			isActive = true;
			PlayerInfo.Instance.myData.isGetDouble = true;
			inactiveObj.SetActive(value: false);
			activeObj.SetActive(value: true);
			leftTimeText.text = string.Format("{0}:{1}", ((int)leftCoolTime.TotalMinutes).ToString("D2"), ((int)(leftCoolTime.TotalSeconds % 60.0)).ToString("D2"));
		}
		else
		{
			isActive = false;
			PlayerInfo.Instance.myData.isGetDouble = false;
			inactiveObj.SetActive(value: true);
			activeObj.SetActive(value: false);
		}
		PlayerInfo.Instance.SaveData();
	}

	private void LateUpdate()
	{
		if (isActive)
		{
			global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime("GetDouble");
			leftTimeText.text = string.Format("{0}:{1}", ((int)leftCoolTime.TotalMinutes).ToString("D2"), ((int)(leftCoolTime.TotalSeconds % 60.0)).ToString("D2"));
			if (leftCoolTime.TotalSeconds <= 0.0)
			{
				isActive = false;
				PlayerInfo.Instance.myData.isGetDouble = false;
				inactiveObj.SetActive(value: true);
				activeObj.SetActive(value: false);
				PlayerInfo.Instance.SaveData();
			}
		}
	}
}
