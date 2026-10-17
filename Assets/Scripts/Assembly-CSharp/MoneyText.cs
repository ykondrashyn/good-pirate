public class MoneyText : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private EMonenyType moneyType;

	private global::UnityEngine.UI.Text myText;

	private long prevMoney;

	private long current = -1L;

	private void Awake()
	{
		myText = GetComponent<global::UnityEngine.UI.Text>();
		if (myText == null)
		{
			myText = GetComponentInChildren<global::UnityEngine.UI.Text>();
		}
	}

	private void Start()
	{
		switch (moneyType)
		{
		case EMonenyType.diamond:
			prevMoney = PlayerInfo.Instance.SHOWDIAMOND;
			break;
		case EMonenyType.fish:
			prevMoney = PlayerInfo.Instance.SHOWFISH;
			break;
		case EMonenyType.gold:
			prevMoney = PlayerInfo.Instance.SHOWGOLD;
			break;
		case EMonenyType.iron:
			prevMoney = PlayerInfo.Instance.SHOWIRON;
			break;
		case EMonenyType.wood:
			prevMoney = PlayerInfo.Instance.SHOWWOOD;
			break;
		}
		myText.text = Numbering.Convert(prevMoney);
	}

	private void LateUpdate()
	{
		switch (moneyType)
		{
		case EMonenyType.diamond:
			current = PlayerInfo.Instance.SHOWDIAMOND;
			break;
		case EMonenyType.fish:
			current = PlayerInfo.Instance.SHOWFISH;
			break;
		case EMonenyType.gold:
			current = PlayerInfo.Instance.SHOWGOLD;
			break;
		case EMonenyType.iron:
			current = PlayerInfo.Instance.SHOWIRON;
			break;
		case EMonenyType.wood:
			current = PlayerInfo.Instance.SHOWWOOD;
			break;
		}
		if (current != -1 && current != prevMoney)
		{
			myText.text = Numbering.Convert(current);
		}
		switch (moneyType)
		{
		case EMonenyType.diamond:
			prevMoney = PlayerInfo.Instance.SHOWDIAMOND;
			break;
		case EMonenyType.fish:
			prevMoney = PlayerInfo.Instance.SHOWFISH;
			break;
		case EMonenyType.gold:
			prevMoney = PlayerInfo.Instance.SHOWGOLD;
			break;
		case EMonenyType.iron:
			prevMoney = PlayerInfo.Instance.SHOWIRON;
			break;
		case EMonenyType.wood:
			prevMoney = PlayerInfo.Instance.SHOWWOOD;
			break;
		}
	}
}
