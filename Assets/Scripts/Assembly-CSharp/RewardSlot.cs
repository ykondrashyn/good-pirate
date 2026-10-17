public class RewardSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text amountText;

	public void SetReward(Reward reward)
	{
		amountText.text = Numbering.Convert(reward.amount);
		iconImage.sprite = UI_Manager.Instance.GetMoneyIcon(reward.moneyType);
	}
}
