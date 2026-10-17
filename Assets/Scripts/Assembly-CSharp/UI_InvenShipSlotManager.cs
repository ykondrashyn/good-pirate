public class UI_InvenShipSlotManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.RectTransform rectTransform;

	private global::System.Collections.Generic.List<UI_InvenShipSlot> invenShipSlotList = new global::System.Collections.Generic.List<UI_InvenShipSlot>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform contentTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject slotSrc;

	private void Awake()
	{
		rectTransform = base.transform as global::UnityEngine.RectTransform;
	}

	private void OnEnable()
	{
		ReGenerate();
	}

	public void ReGenerate()
	{
		foreach (UI_InvenShipSlot invenShipSlot in invenShipSlotList)
		{
			global::UnityEngine.Object.Destroy(invenShipSlot.gameObject);
		}
		invenShipSlotList.Clear();
		int num = 0;
		foreach (global::System.Collections.Generic.KeyValuePair<int, MyShipInventory> item in MyInventoryDataManager.Instance.myShipDataDic)
		{
			UI_InvenShipSlot component = global::UnityEngine.Object.Instantiate(slotSrc, contentTrans).GetComponent<UI_InvenShipSlot>();
			if (num % 2 == 0)
			{
				component.bgObj.SetActive(value: false);
			}
			else
			{
				component.bgObj.SetActive(value: true);
			}
			component.SetData(item.Value);
			invenShipSlotList.Add(component);
			num++;
		}
		RefreshHeight();
	}

	public void Refresh()
	{
		foreach (UI_InvenShipSlot invenShipSlot in invenShipSlotList)
		{
			invenShipSlot.Refresh();
		}
	}

	public void MoveContent(UI_InvenShipSlot targetSlot)
	{
		float endValue = global::UnityEngine.Mathf.Max(0f, 0f - targetSlot.rectTransform.anchoredPosition.y - 150f);
		global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(contentTrans, endValue, 0.3f);
	}

	public void RefreshHeight()
	{
		float num = 0f;
		for (int i = 0; i < invenShipSlotList.Count; i++)
		{
			invenShipSlotList[i].rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f - num);
			num += invenShipSlotList[i].rectTransform.sizeDelta.y;
		}
		global::UnityEngine.Vector2 sizeDelta = contentTrans.sizeDelta;
		sizeDelta.y = num + 200f;
		contentTrans.sizeDelta = sizeDelta;
	}

	public void AllUnSelect()
	{
		foreach (UI_InvenShipSlot invenShipSlot in invenShipSlotList)
		{
			if (invenShipSlot.isSelected)
			{
				invenShipSlot.Select(active: false);
			}
		}
	}

	public void AllUnJustSelect()
	{
		foreach (UI_InvenShipSlot invenShipSlot in invenShipSlotList)
		{
			invenShipSlot.UnJustSelect();
		}
	}

	public void SetFormationMode(int selectedFormationShipId)
	{
		foreach (UI_InvenShipSlot invenShipSlot in invenShipSlotList)
		{
			invenShipSlot.SetFormationMode(selectedFormationShipId);
		}
	}

	public void SetStatusMode()
	{
		foreach (UI_InvenShipSlot invenShipSlot in invenShipSlotList)
		{
			invenShipSlot.isSelected = false;
			invenShipSlot.SetStatusMode();
		}
	}
}
