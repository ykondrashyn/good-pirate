public class UI_ShipInfoPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.Header("ship info")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text shipNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text shipTypeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text shipLevelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image shipImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform statusGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject statusObjSrc;

	[global::UnityEngine.Header("equipped crew list")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform equippedCrewGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject equippedCrewObjSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject blankCrewSlotObjSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewSlotCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewSlotCountBtnText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewListCountText;

	[global::UnityEngine.Header("content objs")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject contentStatus;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject contentOrder;

	[global::UnityEngine.Header("crews list")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform crewSlotGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject crewSlotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Toggle availableToggle;

	[global::UnityEngine.Header("askPopup")]
	[global::UnityEngine.SerializeField]
	private UI_DYWTCPopup ui_dywtcPopup;

	[global::UnityEngine.SerializeField]
	private UI_ShipSellAskPopup ui_shipSellAskPopup;

	[global::UnityEngine.Header("etc")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnDelectObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject btnBackObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button commitBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image commitBtnImage;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> reserveResetObjList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	private global::System.Collections.Generic.List<UI_ShipSelectCrewSlot> crewSlotList = new global::System.Collections.Generic.List<UI_ShipSelectCrewSlot>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<MySailorInventory> selectCrewList = new global::System.Collections.Generic.List<MySailorInventory>();

	private bool wasAnyAction;

	public MyShipInventory selectedShipInvenData { get; private set; }

	private void OnEnable()
	{
		MyShipInventory data = MyInventoryDataManager.Instance.myData.myShipDataList[0];
		SetData(data);
		SetCrewSelectMode(active: false);
	}

	private void OnDisable()
	{
		ObjectsClear();
	}

	private void ObjectsClear()
	{
		foreach (UI_ShipSelectCrewSlot crewSlot in crewSlotList)
		{
			global::UnityEngine.Object.Destroy(crewSlot.gameObject);
		}
		foreach (global::UnityEngine.GameObject reserveResetObj in reserveResetObjList)
		{
			global::UnityEngine.Object.Destroy(reserveResetObj);
		}
		reserveResetObjList.Clear();
		crewSlotList.Clear();
		selectCrewList.Clear();
	}

	public void SetData(MyShipInventory shipData)
	{
		selectedShipInvenData = shipData;
		TextRefresh();
		DataRefresh();
	}

	private void DataRefresh()
	{
		ObjectsClear();
		for (int i = 0; i < 3; i++)
		{
			if (selectedShipInvenData.equipSailerId[i] != -1)
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(equippedCrewObjSrc, equippedCrewGridTrans);
				gameObject.GetComponent<UI_InvenCrewSlot>();
				reserveResetObjList.Add(gameObject);
			}
			else
			{
				global::UnityEngine.GameObject item = global::UnityEngine.Object.Instantiate(blankCrewSlotObjSrc, equippedCrewGridTrans);
				reserveResetObjList.Add(item);
			}
		}
		btnBackObj.SetActive(value: false);
		commitBtn.gameObject.SetActive(value: false);
	}

	public void SetCrewSelectMode(bool active)
	{
		if (active)
		{
			contentStatus.SetActive(value: false);
			contentOrder.SetActive(value: true);
			btnBackObj.SetActive(value: true);
			commitBtn.gameObject.SetActive(value: true);
			commitBtn.enabled = false;
			commitBtnImage.color = new global::UnityEngine.Color(0.3f, 0.3f, 0.3f);
			availableToggle.isOn = true;
			ObjectsClear();
			GenerateCrewList();
		}
		else
		{
			btnBackObj.SetActive(value: false);
			commitBtn.gameObject.SetActive(value: false);
			contentStatus.SetActive(value: true);
			contentOrder.SetActive(value: false);
			DataRefresh();
		}
	}

	private void GenerateCrewList()
	{
		foreach (global::System.Collections.Generic.KeyValuePair<ESailor, MySailorInventory> item in MyInventoryDataManager.Instance.mySailorDataDic)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(crewSlotSrc, crewSlotGridTrans);
			UI_ShipSelectCrewSlot component = gameObject.GetComponent<UI_ShipSelectCrewSlot>();
			component.SetData(item.Value);
			component.SetCheckBoxMode(active: true);
			crewSlotList.Add(component);
			reserveResetObjList.Add(gameObject);
		}
		commitBtn.enabled = false;
		commitBtnImage.color = new global::UnityEngine.Color(0.3f, 0.3f, 0.3f);
		wasAnyAction = false;
		TextRefresh();
	}

	public void AvailableToggle(bool isActive)
	{
	}

	public void Commit()
	{
		SetCrewSelectMode(active: false);
	}

	public bool AddSelectList(MySailorInventory crewData)
	{
		if (selectCrewList.Count >= 3)
		{
			return false;
		}
		if (!selectCrewList.Contains(crewData))
		{
			selectCrewList.Add(crewData);
			TextRefresh();
			AnyAction();
			return true;
		}
		return false;
	}

	public bool RemoveSelectList(MySailorInventory crewData)
	{
		if (selectCrewList.Contains(crewData))
		{
			selectCrewList.Remove(crewData);
			TextRefresh();
			AnyAction();
			return true;
		}
		return false;
	}

	private void TextRefresh()
	{
		crewListCountText.text = $"{selectCrewList.Count}/3";
		crewSlotCountBtnText.text = $"{selectCrewList.Count}/3";
	}

	private void AnyAction()
	{
		if (!wasAnyAction)
		{
			commitBtn.enabled = true;
			commitBtnImage.color = global::UnityEngine.Color.white;
			wasAnyAction = true;
		}
	}

	public void AskChangePopup(UI_ShipSelectCrewSlot data)
	{
		ui_dywtcPopup.gameObject.SetActive(value: true);
		ui_dywtcPopup.SetCrewData(data);
	}

	public void AskSell()
	{
		ui_shipSellAskPopup.gameObject.SetActive(value: true);
		ui_shipSellAskPopup.SetData(selectedShipInvenData);
	}
}
