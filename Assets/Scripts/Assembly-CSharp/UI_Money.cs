public class UI_Money : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconimage;

	private void Awake()
	{
		myText = GetComponent<global::UnityEngine.UI.Text>();
		if (myText == null)
		{
			myText = GetComponentInChildren<global::UnityEngine.UI.Text>();
		}
		iconimage = GetComponent<global::UnityEngine.UI.Image>();
		if (iconimage == null)
		{
			iconimage = GetComponentInChildren<global::UnityEngine.UI.Image>();
		}
	}

	public void SetMoney(EMonenyType type, long moneyAmount)
	{
		iconimage.sprite = UI_Manager.Instance.GetMoneyIcon(type);
		myText.text = Numbering.Convert(moneyAmount);
	}
}
