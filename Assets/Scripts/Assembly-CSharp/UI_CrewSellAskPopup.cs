public class UI_CrewSellAskPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform pivotTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cardSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text sellMessage;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> cardList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	private void OnDisable()
	{
		foreach (global::UnityEngine.GameObject card in cardList)
		{
			global::UnityEngine.Object.Destroy(card);
		}
		cardList.Clear();
	}

	public void SetList(global::System.Collections.Generic.List<MySailorInventory> list, long price)
	{
		for (int i = 0; i < list.Count; i++)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(cardSrc, pivotTrans);
			gameObject.GetComponent<UI_InvenCrewSlot>().SetData(list[i]);
			global::UnityEngine.RectTransform component = gameObject.GetComponent<global::UnityEngine.RectTransform>();
			if (list.Count == 1)
			{
				component.anchoredPosition = global::UnityEngine.Vector2.zero;
			}
			else
			{
				component.anchoredPosition = new global::UnityEngine.Vector2(-265 + 530 / (list.Count - 1) * i, 0f);
			}
			cardList.Add(gameObject);
		}
		if (list.Count > 1)
		{
			sellMessage.text = $"Sell {list.Count} Crews?";
		}
		else
		{
			sellMessage.text = "Sell this Crew?";
		}
		priceText.text = Numbering.Convert(price);
	}

	public void Sell()
	{
		UI_Manager.Instance.ui_mainGame.ui_crewPopup.Sell();
		GetComponent<PopupAction>().Close();
	}
}
