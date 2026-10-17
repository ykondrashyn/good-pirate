public class UI_ShipSkinManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform gridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	private global::System.Collections.Generic.List<UI_ShipSkinSlot> slotList = new global::System.Collections.Generic.List<UI_ShipSkinSlot>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelPivot;

	private global::UnityEngine.GameObject modelObj;

	private int uiLayer;

	private ShipSkinData selectedSkinData;

	private void Awake()
	{
		uiLayer = global::UnityEngine.LayerMask.NameToLayer("UI");
		AddSkinSlot(eShipSkin.로빈슨의땟목);
		for (int i = 0; i < 21; i++)
		{
			if (i != 20)
			{
				AddSkinSlot((eShipSkin)i);
			}
		}
	}

	private void AddSkinSlot(eShipSkin type)
	{
		UI_ShipSkinSlot component = global::UnityEngine.Object.Instantiate(slotSrc, gridTrans).GetComponent<UI_ShipSkinSlot>();
		component.SetData(SkinDataManager.Instance.GetSkinData(type));
		component.manager = this;
		slotList.Add(component);
	}

	private void OnEnable()
	{
		ModelRefresh();
		AllRefresh();
	}

	public void Preview(ShipSkinData data)
	{
		selectedSkinData = data;
		ModelRefresh(isPreview: true);
	}

	public void AllRefresh()
	{
		foreach (UI_ShipSkinSlot slot in slotList)
		{
			slot.Refresh();
		}
	}

	public void ModelRefresh(bool isPreview = false)
	{
		if (modelObj != null)
		{
			global::UnityEngine.Object.Destroy(modelObj);
		}
		EShipMain level = (EShipMain)global::UnityEngine.Mathf.Clamp((int)PlayerInfo.Instance.myData.mainShipLevel / 100 + 1, 1, 10);
		int num = (int)PlayerInfo.Instance.myData.mainShipLevel % 100 / 25;
		global::UnityEngine.GameObject gameObject = null;
		if (isPreview)
		{
			modelObj = global::UnityEngine.Object.Instantiate(selectedSkinData.modelSrc, modelPivot);
		}
		else if (PlayerInfo.Instance.myData.selectedSkin == eShipSkin.Normal)
		{
			ShipMainData mainData = ShipDataManager.Instance.GetMainData(level);
			switch (num)
			{
			case 0:
				gameObject = mainData.modelSrc;
				break;
			case 1:
				gameObject = mainData.playerModelType01;
				break;
			case 2:
				gameObject = mainData.playerModelType02;
				break;
			case 3:
				gameObject = mainData.playerModelType03;
				break;
			}
			if ((int)PlayerInfo.Instance.myData.mainShipLevel >= ConstValue.ShipLevelMax)
			{
				gameObject = mainData.playerModelType03;
			}
			if (gameObject != null)
			{
				modelObj = global::UnityEngine.Object.Instantiate(gameObject, modelPivot);
			}
			else
			{
				modelObj = global::UnityEngine.Object.Instantiate(mainData.modelSrc, modelPivot);
			}
		}
		else
		{
			ShipSkinData skinData = SkinDataManager.Instance.GetSkinData(PlayerInfo.Instance.myData.selectedSkin);
			modelObj = global::UnityEngine.Object.Instantiate(skinData.modelSrc, modelPivot);
		}
		global::UnityEngine.Transform obj = modelObj.transform;
		obj.localScale = global::UnityEngine.Vector3.one;
		obj.localRotation = global::UnityEngine.Quaternion.identity;
		obj.localPosition = global::UnityEngine.Vector3.zero;
		global::UnityEngine.Transform[] componentsInChildren = modelObj.GetComponentsInChildren<global::UnityEngine.Transform>();
		Flag componentInChildren = modelObj.GetComponentInChildren<Flag>();
		global::UnityEngine.Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = uiLayer;
		}
		if (FlagManager.Instance.GetFlagInfo(PlayerInfo.Instance.myData.selectedFlagId, out var info))
		{
			componentInChildren.SetFlag(info.flagMaterial);
		}
		GameManager.Instance.playerShipController.GenerateCannonRange();
	}
}
