public class UI_Formation : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private FormationHelper helper;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] formationCanvasGroups;

	public FormationPos selectedFormation;

	public int selectedShipId = -1;

	private global::UnityEngine.Vector3 offset;

	private int nowSelectedFormationNum;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform titleRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform bottomRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform cancelRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform saveRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] abilityTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] abilityGridTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] abilityPointTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text attackText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text healthText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text speedText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftAbilityPointText;

	private int[,] originalPoint;

	private int[,] tempPoint;

	[global::UnityEngine.Header("새로운 슬롯")]
	public UI_InvenShipSlotManager invenShipSlotManager;

	[global::UnityEngine.SerializeField]
	private UI_InvenShipSlotOutline invenOutline;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button backBtn;

	private ShipController playerShipController;

	private global::UnityEngine.Vector3 clickPos = global::UnityEngine.Vector3.zero;

	private void Awake()
	{
		if ((bool)helper)
		{
			helper.uiFormation = this;
		}
		originalPoint = new int[3, 3];
		tempPoint = new int[3, 3];
	}

	private void OnEnable()
	{
		selectedShipId = -1;
		nowSelectedFormationNum = PlayerInfo.Instance.myData.selectedFormationNum;
		ButtonCheck();
		PointRefresh();
		if (OrientationCheck.Instance.IsPortrait)
		{
			SetPortrait();
		}
		else
		{
			SetLandscape();
		}
		playerShipController = GameManager.Instance.playerShipController;
	}

	private void Start()
	{
		OrientationCheck.Instance.AddLandscapeEvent(SetLandscape);
		OrientationCheck.Instance.AddPortraitEvent(SetPortrait);
	}

	private void SetLandscape()
	{
		bottomRectTransform.pivot = new global::UnityEngine.Vector2(1f, 0.5f);
		bottomRectTransform.sizeDelta = new global::UnityEngine.Vector2(920f, 1242f);
		bottomRectTransform.anchorMin = new global::UnityEngine.Vector2(1f, 0.5f);
		bottomRectTransform.anchorMax = new global::UnityEngine.Vector2(1f, 0.5f);
		cancelRectTransform.anchoredPosition = new global::UnityEngine.Vector2(-1333f, 192f);
		saveRectTransform.anchoredPosition = new global::UnityEngine.Vector2(-1103f, 192f);
		bottomRectTransform.anchoredPosition = global::UnityEngine.Vector3.zero;
		titleRectTransform.anchoredPosition = new global::UnityEngine.Vector2(-449f, -80f);
		int num = 0;
		global::UnityEngine.RectTransform[] array = abilityTransform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchoredPosition = new global::UnityEngine.Vector2(215f, 790 - 235 * num++);
		}
		array = abilityGridTransform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchoredPosition = new global::UnityEngine.Vector2(-115f, -115f);
		}
		array = abilityPointTransform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchoredPosition = new global::UnityEngine.Vector2(562f, -115f);
		}
	}

	private void SetPortrait()
	{
		bottomRectTransform.pivot = new global::UnityEngine.Vector2(0.5f, 0f);
		bottomRectTransform.sizeDelta = new global::UnityEngine.Vector2(1250f, 900f);
		bottomRectTransform.anchorMin = new global::UnityEngine.Vector2(0.5f, 0f);
		bottomRectTransform.anchorMax = new global::UnityEngine.Vector2(0.5f, 0f);
		bottomRectTransform.anchoredPosition = global::UnityEngine.Vector3.zero;
		cancelRectTransform.anchoredPosition = new global::UnityEngine.Vector2(-636f, 994f);
		saveRectTransform.anchoredPosition = new global::UnityEngine.Vector2(-218f, 994f);
		titleRectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, -115f);
		int num = 0;
		global::UnityEngine.RectTransform[] array = abilityTransform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchoredPosition = new global::UnityEngine.Vector2(142f, 412 - 123 * num++);
		}
		array = abilityGridTransform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchoredPosition = new global::UnityEngine.Vector2(105.7f, 0f);
		}
		array = abilityPointTransform;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchoredPosition = new global::UnityEngine.Vector2(889f, 0f);
		}
	}

	public void Clean()
	{
		ShipController shipController = null;
		if (GameManager.Instance != null)
		{
			shipController = GameManager.Instance.playerShipController;
		}
		global::System.Collections.Generic.List<FormationPos> formationList = shipController.formationList;
		foreach (ShipSubController subShip in shipController.subShipList)
		{
			for (int i = 0; i < subShip.myShip.InvenShipData.lineIndex.Length; i++)
			{
				subShip.transform.position = formationList[subShip.myShip.InvenShipData.lineTempIndex[nowSelectedFormationNum]].transform.position;
				subShip.myShip.InvenShipData.lineTempIndex[i] = subShip.myShip.InvenShipData.lineIndex[i];
			}
			subShip.SetTarget(subShip.targetTrans, shipController.formationList[subShip.myShip.InvenShipData.lineIndex[(int)PlayerInfo.Instance.myData.selectedFormationNum]].transform.localPosition);
		}
		for (int j = 0; j < 3; j++)
		{
			tempPoint[j, 0] = (originalPoint[j, 0] = MyInventoryDataManager.Instance.myData.myFormationList[j].attackPoint);
			tempPoint[j, 1] = (originalPoint[j, 1] = MyInventoryDataManager.Instance.myData.myFormationList[j].healthPoint);
			tempPoint[j, 2] = (originalPoint[j, 2] = MyInventoryDataManager.Instance.myData.myFormationList[j].speedPoint);
		}
		PointRefresh();
	}

	public void ChangeFormationNum(int num)
	{
		num = global::UnityEngine.Mathf.Clamp(num, 0, 3);
		if (num == nowSelectedFormationNum)
		{
			return;
		}
		nowSelectedFormationNum = num;
		global::System.Collections.Generic.List<FormationPos> formationList = playerShipController.formationList;
		foreach (ShipSubController subShip in playerShipController.subShipList)
		{
			subShip.transform.position = formationList[subShip.myShip.InvenShipData.lineTempIndex[nowSelectedFormationNum]].transform.position;
			subShip.SetTarget(subShip.targetTrans, playerShipController.formationList[subShip.myShip.InvenShipData.lineTempIndex[nowSelectedFormationNum]].transform.localPosition);
		}
		ResetFormationStatus(isTemp: true);
		ButtonCheck();
		PointRefresh();
	}

	public void Save()
	{
		foreach (ShipSubController subShip in playerShipController.subShipList)
		{
			for (int i = 0; i < subShip.myShip.InvenShipData.lineIndex.Length; i++)
			{
				subShip.myShip.InvenShipData.lineIndex[i] = subShip.myShip.InvenShipData.lineTempIndex[i];
			}
			subShip.SetTarget(subShip.targetTrans, playerShipController.formationList[subShip.myShip.InvenShipData.lineIndex[(int)PlayerInfo.Instance.myData.selectedFormationNum]].transform.localPosition);
		}
		for (int j = 0; j < 3; j++)
		{
			MyInventoryDataManager.Instance.myData.myFormationList[j].attackPoint = (originalPoint[j, 0] = tempPoint[j, 0]);
			MyInventoryDataManager.Instance.myData.myFormationList[j].healthPoint = (originalPoint[j, 1] = tempPoint[j, 1]);
			MyInventoryDataManager.Instance.myData.myFormationList[j].speedPoint = (originalPoint[j, 2] = tempPoint[j, 2]);
		}
		MyInventoryDataManager.Instance.SaveData();
		UI_Manager.Instance.FormationMode(active: false);
	}

	public void Cancel()
	{
		Clean();
		ResetFormationStatus();
		UI_Manager.Instance.FormationMode(active: false);
		for (int i = 0; i < 3; i++)
		{
			MyInventoryDataManager.Instance.myData.myFormationList[i].attackPoint = (tempPoint[i, 0] = originalPoint[i, 0]);
			MyInventoryDataManager.Instance.myData.myFormationList[i].healthPoint = (tempPoint[i, 1] = originalPoint[i, 1]);
			MyInventoryDataManager.Instance.myData.myFormationList[i].speedPoint = (tempPoint[i, 2] = originalPoint[i, 2]);
		}
	}

	private void ResetFormationStatus(bool isTemp = false)
	{
		int num = ((!isTemp) ? ((int)PlayerInfo.Instance.myData.selectedFormationNum) : nowSelectedFormationNum);
		global::System.Collections.Generic.List<FormationPos> formationList = playerShipController.formationList;
		foreach (FormationPos item in formationList)
		{
			item.SetStatus(null);
		}
		foreach (ShipSubController subShip in playerShipController.subShipList)
		{
			if (isTemp)
			{
				formationList[subShip.myShip.InvenShipData.lineTempIndex[num]].SetStatus(subShip);
			}
			else
			{
				formationList[subShip.myShip.InvenShipData.lineIndex[num]].SetStatus(subShip);
			}
		}
	}

	public void PointRefresh()
	{
		_ = MyInventoryDataManager.Instance.myData.myFormationList[nowSelectedFormationNum];
		int num = tempPoint[nowSelectedFormationNum, 0] + tempPoint[nowSelectedFormationNum, 1] + tempPoint[nowSelectedFormationNum, 2];
		attackText.text = tempPoint[nowSelectedFormationNum, 0].ToString();
		healthText.text = tempPoint[nowSelectedFormationNum, 1].ToString();
		speedText.text = tempPoint[nowSelectedFormationNum, 2].ToString();
		leftAbilityPointText.text = (30 - num).ToString();
	}

	public void Minus(int num)
	{
		if (tempPoint[nowSelectedFormationNum, num] > 0)
		{
			tempPoint[nowSelectedFormationNum, num]--;
			PointRefresh();
		}
	}

	public void Plus(int num)
	{
		if (tempPoint[nowSelectedFormationNum, 0] + tempPoint[nowSelectedFormationNum, 1] + tempPoint[nowSelectedFormationNum, 2] < 30)
		{
			tempPoint[nowSelectedFormationNum, num]++;
			PointRefresh();
		}
	}

	private void ButtonCheck()
	{
		for (int i = 0; i < formationCanvasGroups.Length; i++)
		{
			if (i == nowSelectedFormationNum)
			{
				formationCanvasGroups[i].alpha = 1f;
			}
			else
			{
				formationCanvasGroups[i].alpha = 0.5f;
			}
		}
	}

	public void Select(MyShipInventory shipData, global::UnityEngine.Transform trans)
	{
		invenOutline.gameObject.SetActive(value: true);
		invenOutline.SetColor(myColor.GetColor(0, 188, 64));
		invenOutline.SetTarget(trans);
		if (shipData.lineIndex[0] != -1)
		{
			(selectedFormation = playerShipController.formationList[shipData.lineIndex[0]]).SetSelect(active: true);
		}
	}

	public void JustSelect(MyShipInventory shipData, global::UnityEngine.Transform trans)
	{
		invenOutline.gameObject.SetActive(value: true);
		invenOutline.SetColor(myColor.GetColor(255, 197, 0));
		invenOutline.SetTarget(trans);
		playerShipController.formationList[shipData.lineIndex[0]].SetJustSelect(active: true);
	}

	public void UnJustSelect(MyShipInventory shipData)
	{
		invenOutline.gameObject.SetActive(value: false);
		invenOutline.SetTarget(null);
		if (shipData.lineIndex[0] != -1)
		{
			playerShipController.formationList[shipData.lineIndex[0]].SetJustSelect(active: false);
		}
	}

	public void UnSelect(MyShipInventory shipData)
	{
		invenOutline.gameObject.SetActive(value: false);
		invenOutline.SetTarget(null);
		playerShipController.formationList[shipData.lineIndex[0]].SetSelect(active: false);
	}

	private void Update()
	{
		if (global::UnityEngine.Input.GetMouseButtonDown(0))
		{
			TouchBegan(global::UnityEngine.Input.mousePosition);
		}
		else if (global::UnityEngine.Input.GetMouseButton(0))
		{
			TouchDrag(global::UnityEngine.Input.mousePosition);
		}
		else if (global::UnityEngine.Input.GetMouseButtonUp(0))
		{
			TouchEnd(global::UnityEngine.Input.mousePosition);
		}
	}

	private void TouchBegan(global::UnityEngine.Vector2 screenPos)
	{
	}

	public void Reset()
	{
		selectedFormation = null;
		selectedShipId = -1;
		invenShipSlotManager.SetStatusMode();
		invenShipSlotManager.Refresh();
		invenOutline.gameObject.SetActive(value: false);
		invenShipSlotManager.AllUnJustSelect();
		foreach (FormationPos formation in playerShipController.formationList)
		{
			formation.SetJustSelect(active: false);
			formation.SetSelect(active: false);
			formation.MoveMode(active: false);
		}
		BackBtnCantPressMode();
	}

	private void TouchDrag(global::UnityEngine.Vector2 screenPos)
	{
	}

	public void BackBtnCanPressMode()
	{
		backBtn.targetGraphic.color = global::UnityEngine.Color.white;
		backBtn.enabled = true;
	}

	public void BackBtnCantPressMode()
	{
		backBtn.enabled = false;
		backBtn.targetGraphic.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.3f);
	}

	private void TouchEnd(global::UnityEngine.Vector2 screenPos)
	{
		global::UnityEngine.Ray ray = global::UnityEngine.Camera.main.ScreenPointToRay(screenPos);
		if (global::UnityEngine.Physics.Raycast(ray, out var hitInfo, 300f, global::UnityEngine.LayerMask.GetMask("FormationBack")))
		{
			Reset();
		}
		else
		{
			if (!global::UnityEngine.Physics.Raycast(ray, out hitInfo, 300f, global::UnityEngine.LayerMask.GetMask("Formation")))
			{
				return;
			}
			FormationPos component = hitInfo.collider.GetComponent<FormationPos>();
			if (!(component != null))
			{
				return;
			}
			if (selectedFormation != null && (bool)selectedFormation.shipController)
			{
				if (component.isCanSelect)
				{
					ShipSubController shipController = selectedFormation.shipController;
					global::UnityEngine.GameObject obj = shipController.gameObject;
					MyShipInventory invenShipData = selectedFormation.shipController.myShip.InvenShipData;
					if (playerShipController.subShipList.Contains(shipController))
					{
						playerShipController.subShipList.Remove(shipController);
					}
					selectedFormation.SetStatus(null);
					global::UnityEngine.Object.Destroy(obj);
					invenShipData.lineIndex[0] = component.index;
					playerShipController.AddSubShip(invenShipData);
					Reset();
					return;
				}
			}
			else if (selectedShipId != -1)
			{
				MyShipInventory myShipData = MyInventoryDataManager.Instance.GetMyShipData(selectedShipId);
				myShipData.lineIndex[0] = component.index;
				playerShipController.AddSubShip(myShipData);
				Reset();
				return;
			}
			Reset();
			selectedFormation = component;
			if (component.shipController != null)
			{
				invenShipSlotManager.SetFormationMode(component.shipController.myShip.InvenShipData.id);
				foreach (FormationPos formation in playerShipController.formationList)
				{
					formation.MoveMode(active: true);
				}
				BackBtnCanPressMode();
				return;
			}
			invenShipSlotManager.SetFormationMode(-1);
			foreach (FormationPos formation2 in playerShipController.formationList)
			{
				formation2.SetSelect(active: false);
			}
			component.SetSelect(active: true);
			BackBtnCanPressMode();
		}
	}
}
