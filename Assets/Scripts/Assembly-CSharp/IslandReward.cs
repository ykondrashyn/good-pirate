public class IslandReward : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text amountText;

	public void SetReward(EMonenyType moneyType, int amount)
	{
		amountText.text = Numbering.Convert(amount);
		iconImage.sprite = UI_Manager.Instance.GetMoneyIcon(moneyType);
	}
}
