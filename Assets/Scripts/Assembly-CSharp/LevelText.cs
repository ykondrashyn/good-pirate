public class LevelText : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Text myText;

	private int prevLevel;

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
		if ((int)PlayerInfo.Instance.myData.mainShipLevel >= ConstValue.ShipLevelMax)
		{
			myText.text = "Lv.MAX";
			return;
		}
		prevLevel = PlayerInfo.Instance.myData.mainShipLevel;
		myText.text = $"Lv.{prevLevel}";
	}

	private void LateUpdate()
	{
		int num = PlayerInfo.Instance.myData.mainShipLevel;
		if (num != -1 && num != prevLevel)
		{
			if ((int)PlayerInfo.Instance.myData.mainShipLevel >= ConstValue.ShipLevelMax)
			{
				myText.text = "Lv.MAX";
			}
			else
			{
				myText.text = $"Lv.{num}";
			}
		}
		prevLevel = num;
	}
}
