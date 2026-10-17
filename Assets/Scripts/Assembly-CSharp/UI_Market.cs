public class UI_Market : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text wood_FishText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text wood_WoodText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text iron_FishText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text iron_IronText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Slider woodSlider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Slider ironSlider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform[] moneyGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform originalParent;

	private long woodAmount;

	private long fishToWoodAmount;

	private long ironAmount;

	private long fishToIronAmount;

	private void OnEnable()
	{
		wood_FishText.text = Numbering.Convert(0);
		wood_WoodText.text = Numbering.Convert(0);
		iron_FishText.text = Numbering.Convert(0);
		iron_IronText.text = Numbering.Convert(0);
		if ((long)PlayerInfo.Instance.myData.fish <= 0)
		{
			iron_FishText.color = global::UnityEngine.Color.red;
			wood_FishText.color = global::UnityEngine.Color.red;
		}
		else
		{
			iron_FishText.color = myColor.GetColor(19, 52, 132);
			wood_FishText.color = myColor.GetColor(19, 52, 132);
		}
		woodSlider.value = 0f;
		ironSlider.value = 0f;
		for (int i = 0; i < moneyGridTrans.Length; i++)
		{
			moneyGridTrans[i].SetParent(base.transform);
		}
	}

	public void WoodRefresh()
	{
		fishToWoodAmount = (long)((float)(long)PlayerInfo.Instance.myData.fish * woodSlider.value);
		woodAmount = fishToWoodAmount;
		wood_FishText.text = Numbering.Convert(fishToWoodAmount);
		wood_WoodText.text = Numbering.Convert(woodAmount);
		if ((long)PlayerInfo.Instance.myData.fish <= 0)
		{
			iron_FishText.color = global::UnityEngine.Color.red;
			wood_FishText.color = global::UnityEngine.Color.red;
		}
		else
		{
			iron_FishText.color = myColor.GetColor(19, 52, 132);
			wood_FishText.color = myColor.GetColor(19, 52, 132);
		}
	}

	public void WoodExchange()
	{
		PlayerInfo.Instance.AddMoney(EMonenyType.fish, -fishToWoodAmount, isSave: false);
		PlayerInfo.Instance.AddMoney(EMonenyType.wood, woodAmount, isSave: false);
		PlayerInfo.Instance.SaveData();
		woodSlider.value = 0f;
		WoodRefresh();
		IronRefresh();
	}

	public void WoodPlus()
	{
		float value = woodSlider.value;
		value = global::UnityEngine.Mathf.Clamp01(value + 0.05f);
		woodSlider.value = value;
	}

	public void WoodMinus()
	{
		float value = woodSlider.value;
		value = global::UnityEngine.Mathf.Clamp01(value - 0.05f);
		woodSlider.value = value;
	}

	public void IronRefresh()
	{
		fishToIronAmount = (long)((float)(long)PlayerInfo.Instance.myData.fish * ironSlider.value);
		ironAmount = fishToIronAmount / 3;
		iron_FishText.text = Numbering.Convert(fishToIronAmount);
		iron_IronText.text = Numbering.Convert(ironAmount);
		if ((long)PlayerInfo.Instance.myData.fish <= 0)
		{
			iron_FishText.color = global::UnityEngine.Color.red;
			wood_FishText.color = global::UnityEngine.Color.red;
		}
		else
		{
			iron_FishText.color = myColor.GetColor(19, 52, 132);
			wood_FishText.color = myColor.GetColor(19, 52, 132);
		}
	}

	public void IronExchange()
	{
		PlayerInfo.Instance.AddMoney(EMonenyType.fish, -fishToIronAmount, isSave: false);
		PlayerInfo.Instance.AddMoney(EMonenyType.iron, ironAmount, isSave: false);
		PlayerInfo.Instance.SaveData();
		ironSlider.value = 0f;
		WoodRefresh();
		IronRefresh();
	}

	public void IronPlus()
	{
		float value = ironSlider.value;
		value = global::UnityEngine.Mathf.Clamp01(value + 0.05f);
		ironSlider.value = value;
	}

	public void IronMinus()
	{
		float value = ironSlider.value;
		value = global::UnityEngine.Mathf.Clamp01(value - 0.05f);
		ironSlider.value = value;
	}

	public void BackOriginalTrans()
	{
		for (int i = 0; i < moneyGridTrans.Length; i++)
		{
			moneyGridTrans[i].SetParent(originalParent);
		}
	}
}
