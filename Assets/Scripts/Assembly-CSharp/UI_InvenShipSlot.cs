public class UI_InvenShipSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.RectTransform rectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	public global::UnityEngine.GameObject bgObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text shipTypeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text hpText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text attackText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text speedText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject formationModeObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject statusModeObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup statusModeCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject equipBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject unEquipBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject moveBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject stateEquippedBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject stateUnequippedBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] selectActionObj;

	private ShipController playerShipController;

	private global::UnityEngine.UI.Button myButton;

	[global::UnityEngine.HideInInspector]
	public bool isSelected;

	private bool isJustSelect;

	public MyShipInventory myData { get; private set; }

	private void Awake()
	{
		rectTransform = base.transform as global::UnityEngine.RectTransform;
		myButton = GetComponent<global::UnityEngine.UI.Button>();
	}

	private void OnEnable()
	{
		playerShipController = GameManager.Instance.playerShipController;
	}

	public void SetData(MyShipInventory data)
	{
		myData = data;
		Refresh();
	}

	public void OpenShipInfomationPopup()
	{
		UI_Manager.Instance.ui_mainGame.OpenShipInfomationPopup(myData);
	}

	public void Refresh()
	{
		int shipLevel = PlayerInfo.Instance.GetShipLevel(myData.shipType);
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(myData.shipType);
		SheetData shipSheetData = ShipDataManager.Instance.GetShipSheetData(shipBaseData.id, shipLevel);
		nameText.text = shipBaseData.shipName;
		shipTypeText.text = shipBaseData.shipName;
		hpText.text = shipSheetData.health.ToString();
		attackText.text = shipSheetData.attack.ToString();
		if (myData.lineIndex[0] != -1)
		{
			stateEquippedBtnObj.SetActive(value: true);
			stateUnequippedBtnObj.SetActive(value: false);
			equipBtnObj.SetActive(value: false);
			unEquipBtnObj.SetActive(value: true);
			moveBtnObj.SetActive(value: false);
		}
		else
		{
			equipBtnObj.SetActive(value: true);
			moveBtnObj.SetActive(value: false);
			unEquipBtnObj.SetActive(value: false);
			stateEquippedBtnObj.SetActive(value: false);
			stateUnequippedBtnObj.SetActive(value: true);
		}
		statusModeCanvasGroup.alpha = 1f;
	}

	public void SetFormationMode(int selectedFormationShipId)
	{
		equipBtnObj.SetActive(value: false);
		moveBtnObj.SetActive(value: false);
		unEquipBtnObj.SetActive(value: false);
		formationModeObj.SetActive(value: true);
		statusModeObj.SetActive(value: false);
		if (selectedFormationShipId == -1)
		{
			if (myData.lineIndex[0] != -1)
			{
				moveBtnObj.SetActive(value: true);
			}
			else
			{
				equipBtnObj.SetActive(value: true);
			}
		}
		else if (selectedFormationShipId == myData.id)
		{
			if (myData.lineIndex[0] != -1)
			{
				unEquipBtnObj.SetActive(value: true);
			}
			else
			{
				formationModeObj.SetActive(value: false);
				statusModeObj.SetActive(value: true);
			}
			UI_Manager.Instance.ui_formation.invenShipSlotManager.MoveContent(this);
			UI_Manager.Instance.ui_formation.invenShipSlotManager.AllUnSelect();
			UI_Manager.Instance.ui_formation.Select(myData, base.transform);
			global::UnityEngine.GameObject[] array = selectActionObj;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
		else
		{
			formationModeObj.SetActive(value: false);
			statusModeObj.SetActive(value: true);
			if (myData.lineIndex[0] != -1)
			{
				stateUnequippedBtnObj.SetActive(value: false);
				stateEquippedBtnObj.SetActive(value: true);
			}
			else
			{
				stateEquippedBtnObj.SetActive(value: false);
				stateUnequippedBtnObj.SetActive(value: true);
			}
			statusModeCanvasGroup.alpha = 0.3f;
		}
	}

	public void SetStatusMode()
	{
		statusModeCanvasGroup.alpha = 1f;
		formationModeObj.SetActive(value: false);
		statusModeObj.SetActive(value: true);
	}

	public void UnEquip()
	{
		FormationPos selectedFormation = UI_Manager.Instance.ui_formation.selectedFormation;
		ShipSubController shipController = selectedFormation.shipController;
		global::UnityEngine.GameObject obj = shipController.gameObject;
		if (playerShipController.subShipList.Contains(shipController))
		{
			playerShipController.subShipList.Remove(shipController);
		}
		selectedFormation.SetStatus(null);
		global::UnityEngine.Object.Destroy(obj);
		myData.lineIndex[0] = -1;
		UI_Manager.Instance.ui_formation.Reset();
	}

	public void Equip()
	{
		myData.lineIndex[0] = UI_Manager.Instance.ui_formation.selectedFormation.index;
		playerShipController.AddSubShip(myData);
		UI_Manager.Instance.ui_formation.Reset();
	}

	public void Change()
	{
		int num = myData.lineIndex[0];
		if (num != -1)
		{
			FormationPos formationPos = playerShipController.formationList[num];
			if (formationPos.shipController != null)
			{
				global::UnityEngine.GameObject obj = formationPos.shipController.gameObject;
				if (playerShipController.subShipList.Contains(formationPos.shipController))
				{
					playerShipController.subShipList.Remove(formationPos.shipController);
				}
				global::UnityEngine.Object.Destroy(obj);
				formationPos.shipController.myShip.InvenShipData.lineIndex[0] = -1;
			}
		}
		ShipSubController shipController = UI_Manager.Instance.ui_formation.selectedFormation.shipController;
		if (shipController != null)
		{
			global::UnityEngine.GameObject obj2 = shipController.gameObject;
			if (playerShipController.subShipList.Contains(shipController))
			{
				playerShipController.subShipList.Remove(shipController);
			}
			global::UnityEngine.Object.Destroy(obj2);
			shipController.myShip.InvenShipData.lineIndex[0] = num;
			playerShipController.AddSubShip(shipController.myShip.InvenShipData);
		}
		myData.lineIndex[0] = UI_Manager.Instance.ui_formation.selectedFormation.index;
		playerShipController.AddSubShip(myData);
		UI_Manager.Instance.ui_formation.Reset();
	}

	public void Select(bool active)
	{
		if (myData.lineIndex[0] == -1)
		{
			return;
		}
		if (active)
		{
			if (myData.lineIndex[0] != -1)
			{
				UI_Manager.Instance.ui_formation.invenShipSlotManager.SetFormationMode(myData.id);
				UI_Manager.Instance.ui_formation.BackBtnCanPressMode();
			}
			UI_Manager.Instance.ui_formation.invenShipSlotManager.AllUnSelect();
			UI_Manager.Instance.ui_formation.Select(myData, base.transform);
			global::UnityEngine.GameObject[] array = selectActionObj;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			foreach (FormationPos formation in playerShipController.formationList)
			{
				formation.MoveMode(active: true);
			}
		}
		else
		{
			UI_Manager.Instance.ui_formation.UnSelect(myData);
			UI_Manager.Instance.ui_formation.UnJustSelect(myData);
			global::UnityEngine.GameObject[] array = selectActionObj;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			UI_Manager.Instance.ui_formation.Reset();
			isJustSelect = false;
			UI_Manager.Instance.ui_formation.BackBtnCantPressMode();
		}
		isSelected = active;
	}

	public void Select()
	{
		if (myData.lineIndex[0] == -1)
		{
			UI_Manager.Instance.ui_formation.Reset();
			UI_Manager.Instance.ui_formation.invenShipSlotManager.AllUnSelect();
			UI_Manager.Instance.ui_formation.invenShipSlotManager.SetFormationMode(myData.id);
			UI_Manager.Instance.ui_formation.Select(myData, base.transform);
			UI_Manager.Instance.ui_formation.BackBtnCanPressMode();
			UI_Manager.Instance.ui_formation.selectedShipId = myData.id;
			{
				foreach (FormationPos formation in playerShipController.formationList)
				{
					formation.MoveMode(active: true);
				}
				return;
			}
		}
		isSelected = !isSelected;
		if (UI_Manager.Instance.ui_formation.selectedFormation != null)
		{
			if (UI_Manager.Instance.ui_formation.selectedFormation.shipController != null)
			{
				if (UI_Manager.Instance.ui_formation.selectedFormation.shipController.myShip.InvenShipData == myData)
				{
					Select(active: false);
				}
				return;
			}
			if (!isJustSelect)
			{
				UI_Manager.Instance.ui_formation.invenShipSlotManager.AllUnJustSelect();
				UI_Manager.Instance.ui_formation.JustSelect(myData, base.transform);
			}
			else
			{
				UI_Manager.Instance.ui_formation.UnJustSelect(myData);
			}
			isJustSelect = !isJustSelect;
		}
		else
		{
			Select(isSelected);
		}
	}

	public void UnJustSelect()
	{
		UI_Manager.Instance.ui_formation.UnJustSelect(myData);
		isJustSelect = false;
	}
}
