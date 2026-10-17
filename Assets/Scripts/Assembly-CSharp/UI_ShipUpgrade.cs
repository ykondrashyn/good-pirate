public class UI_ShipUpgrade : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelNowText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text levelNextText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text upgradePriceText;

	private global::UnityEngine.UI.Button myButton;

	private global::UnityEngine.UI.Image btnImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] whiteGraphics;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] yellowGraphics;

	private void Awake()
	{
		myButton = GetComponent<global::UnityEngine.UI.Button>();
		btnImage = GetComponent<global::UnityEngine.UI.Image>();
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Upgrade()
	{
		int num = PlayerInfo.Instance.myData.mainShipLevel;
		long gold = ShipDataManager.Instance.subShipUpgradePrice.dataArray[num - 1].Gold;
		if ((long)PlayerInfo.Instance.myData.gold >= gold)
		{
			PlayerInfo.Instance.myData.mainShipLevel = num + 1;
			PlayerInfo.Instance.AddMoney(EMonenyType.gold, -gold);
			Refresh();
		}
	}

	private void Refresh()
	{
		int num = PlayerInfo.Instance.myData.mainShipLevel;
		if ((bool)levelNowText)
		{
			levelNowText.text = $"Lv.{num}";
		}
		if ((bool)levelNextText)
		{
			levelNextText.text = $"Lv.{num + 1}";
		}
		long gold = ShipDataManager.Instance.subShipUpgradePrice.dataArray[num - 1].Gold;
		if ((bool)upgradePriceText)
		{
			upgradePriceText.text = Numbering.Convert(gold);
		}
		if ((long)PlayerInfo.Instance.myData.gold < gold)
		{
			myButton.enabled = false;
			btnImage.color = myColor.GetColor(111, 111, 111);
			global::UnityEngine.UI.Graphic[] array = yellowGraphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = myColor.GetColor(170, 170, 170);
			}
			array = whiteGraphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = myColor.GetColor(170, 170, 170);
			}
		}
		else
		{
			myButton.enabled = true;
			btnImage.color = myColor.GetColor(0, 188, 64);
			global::UnityEngine.UI.Graphic[] array = yellowGraphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = myColor.GetColor(255, 255, 0);
			}
			array = whiteGraphics;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = global::UnityEngine.Color.white;
			}
		}
	}
}
