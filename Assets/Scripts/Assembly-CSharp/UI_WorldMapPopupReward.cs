public class UI_WorldMapPopupReward : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myIcon;

	public void SetData(IslandRewardData data)
	{
		myText.text = Numbering.Convert(data.rewardValue);
		myIcon.sprite = UI_Manager.Instance.GetMoneyIcon(data.rewardType);
	}
}
