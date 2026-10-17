public class UI_ItemBoxPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private UI_ItemBox itemBox;

	[global::UnityEngine.SerializeField]
	private UI_ItemBox freeItemBox;

	private global::System.Collections.Generic.List<UI_ItemSlot> itemSlotList = new global::System.Collections.Generic.List<UI_ItemSlot>();

	private global::System.Collections.Generic.List<ItemSlotValue> itemListValueList = new global::System.Collections.Generic.List<ItemSlotValue>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject moneySrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject crewSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] gridSlots;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup buttonCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button skipBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button getButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup getButtonCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform gridRectTransform;

	private UI_ItemBox nowItemBox;

	[global::UnityEngine.Header("etc")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.GridLayoutGroup gridLayout;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform gridRecttransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform btnRecttransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] moneyTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform crewIconTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform goldIconTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform diaIconTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject goldSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject diaSrc;

	private RewardBoxData rewardData;

	public bool isFree { get; private set; }

	private void Awake()
	{
		if (rewardData == null)
		{
			rewardData = global::UnityEngine.Resources.Load<RewardBoxData>("Data/RewardBoxData");
		}
	}

	private void OnEnable()
	{
		foreach (UI_ItemSlot itemSlot in itemSlotList)
		{
			global::UnityEngine.Object.Destroy(itemSlot.gameObject);
		}
		itemSlotList.Clear();
		itemListValueList.Clear();
		global::UnityEngine.CanvasGroup[] array = gridSlots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		buttonCanvas.gameObject.SetActive(value: false);
		getButton.enabled = true;
		getButtonCanvas.alpha = 1f;
		global::UnityEngine.Time.timeScale = 0f;
	}

	private void OnDisable()
	{
		global::UnityEngine.Time.timeScale = 1f;
	}

	public void SetIsFreeBox(bool isTrue)
	{
		isFree = isTrue;
		freeItemBox.gameObject.SetActive(value: false);
		itemBox.gameObject.SetActive(value: false);
		if (isFree)
		{
			nowItemBox = freeItemBox;
			gridRectTransform.anchoredPosition = new global::UnityEngine.Vector3(0f, 0f, 0f);
		}
		else
		{
			nowItemBox = itemBox;
			gridRectTransform.anchoredPosition = new global::UnityEngine.Vector3(0f, -278f, 0f);
		}
		nowItemBox.gameObject.SetActive(value: true);
	}

	public void AddItemList(ItemSlotValue value)
	{
		itemListValueList.Add(value);
	}

	public void Open(ERewardBox boxType)
	{
		skipBtn.enabled = true;
		skipBtn.gameObject.SetActive(value: true);
		nowItemBox.SetBox(boxType);
		nowItemBox.SetItemList(itemListValueList, ItemOpen);
		if (itemListValueList.Count <= 4)
		{
			gridLayout.enabled = true;
			gridRecttransform.anchoredPosition = new global::UnityEngine.Vector2(0f, -88f);
			btnRecttransform.anchoredPosition = new global::UnityEngine.Vector2(0f, -539f);
		}
		else
		{
			gridLayout.enabled = false;
			gridRecttransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 58f);
			btnRecttransform.anchoredPosition = new global::UnityEngine.Vector2(0f, -633f);
			switch (itemListValueList.Count)
			{
			case 5:
			{
				for (int k = 0; k < 3; k++)
				{
					(gridSlots[k].transform as global::UnityEngine.RectTransform).anchoredPosition = new global::UnityEngine.Vector3(-224f + (float)k * 274f, -50f, 0f);
				}
				for (int l = 3; l < 5; l++)
				{
					(gridSlots[l].transform as global::UnityEngine.RectTransform).anchoredPosition = new global::UnityEngine.Vector3(-87f + (float)(l - 3) * 274f, -390f, 0f);
				}
				break;
			}
			case 6:
			{
				for (int m = 0; m < 3; m++)
				{
					(gridSlots[m].transform as global::UnityEngine.RectTransform).anchoredPosition = new global::UnityEngine.Vector3(-224f + (float)m * 274f, -50f, 0f);
				}
				for (int n = 3; n < 6; n++)
				{
					(gridSlots[n].transform as global::UnityEngine.RectTransform).anchoredPosition = new global::UnityEngine.Vector3(-224f + (float)(n - 3) * 274f, -390f, 0f);
				}
				break;
			}
			case 7:
			{
				for (int i = 0; i < 3; i++)
				{
					(gridSlots[i].transform as global::UnityEngine.RectTransform).anchoredPosition = new global::UnityEngine.Vector3(-224f + (float)i * 274f, -50f, 0f);
				}
				for (int j = 3; j < 7; j++)
				{
					(gridSlots[j].transform as global::UnityEngine.RectTransform).anchoredPosition = new global::UnityEngine.Vector3(-361f + (float)(j - 3) * 274f, -390f, 0f);
				}
				break;
			}
			}
		}
		for (int num = 0; num < gridSlots.Length; num++)
		{
			if (num >= itemListValueList.Count)
			{
				gridSlots[num].gameObject.SetActive(value: false);
				continue;
			}
			gridSlots[num].alpha = 0f;
			gridSlots[num].gameObject.SetActive(value: true);
			global::UnityEngine.Transform parent = gridSlots[num].transform;
			switch (itemListValueList[num].itemType)
			{
			case EItemBoxSlotType.Crew:
			{
				UI_ItemSlot component2 = global::UnityEngine.Object.Instantiate(crewSrc, parent).GetComponent<UI_ItemSlot>();
				component2.SetData(itemListValueList[num].sailorData, itemListValueList[num].sailorGetCount);
				itemSlotList.Add(component2);
				break;
			}
			case EItemBoxSlotType.Money:
			{
				UI_ItemSlot component = global::UnityEngine.Object.Instantiate(moneySrc, parent).GetComponent<UI_ItemSlot>();
				component.SetData(itemListValueList[num].moneyType, itemListValueList[num].moneyAmount);
				itemSlotList.Add(component);
				break;
			}
			}
		}
	}

	public void SetItem(RewardBoxStatus data)
	{
		int num = 0;
		float num2 = 1f;
		int num3 = 0;
		switch (data.boxType)
		{
		case ERewardBox.NONE:
			num = 1;
			num2 = 1f;
			num3 = global::UnityEngine.Random.Range(1, 3);
			break;
		case ERewardBox.BRONZE:
			num = 2;
			num2 = 2f;
			num3 = global::UnityEngine.Random.Range(1, 3);
			break;
		case ERewardBox.SILVER:
			num = 2;
			num2 = 4f;
			num3 = global::UnityEngine.Random.Range(2, 4);
			break;
		case ERewardBox.GOLD:
			num = 4;
			num2 = 8f;
			num3 = global::UnityEngine.Random.Range(3, 7);
			break;
		case ERewardBox.BLACK:
			num = 4;
			num2 = 12f;
			num3 = global::UnityEngine.Random.Range(5, 11);
			break;
		}
		ItemSlotValue value = new ItemSlotValue
		{
			itemType = EItemBoxSlotType.Money,
			moneyType = EMonenyType.gold
		};
		if (isFree)
		{
			value.moneyAmount = rewardData.dataArray[(int)PlayerInfo.Instance.myData.mainShipLevel - 1].Gold;
			PlayerInfo.Instance.myData.isWatchedFreeBoxAds = false;
		}
		else
		{
			value.moneyAmount = (int)((float)rewardData.dataArray[data.level - 1].Gold * num2);
		}
		AddItemList(value);
		PlayerInfo.Instance.waitActionGold = true;
		PlayerInfo.Instance.AddMoney(value.moneyType, value.moneyAmount);
		value = new ItemSlotValue
		{
			itemType = EItemBoxSlotType.Money,
			moneyType = EMonenyType.diamond,
			moneyAmount = num3
		};
		AddItemList(value);
		PlayerInfo.Instance.waitActionDiamond = true;
		PlayerInfo.Instance.AddMoney(value.moneyType, value.moneyAmount);
		foreach (ESailor rewardSailor in SailorDataManager.Instance.GetRewardSailorList(data.boxType))
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(rewardSailor);
			value = default(ItemSlotValue);
			if (rewardSailor != ESailor.END)
			{
				value.itemType = EItemBoxSlotType.Crew;
				value.sailorData = mySailorData;
				value.sailorGetCount = num;
				MyInventoryDataManager.Instance.AddSailor(rewardSailor, num);
			}
			else
			{
				value.itemType = EItemBoxSlotType.Money;
				value.moneyType = EMonenyType.diamond;
				value.moneyAmount = num;
				PlayerInfo.Instance.AddMoney(value.moneyType, value.moneyAmount);
			}
			AddItemList(value);
		}
	}

	private void ItemOpen()
	{
		for (int i = 0; i < itemSlotList.Count; i++)
		{
			global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(gridSlots[i], 1f, 0.4f), isIndependentUpdate: true);
		}
		StartCoroutine("ItemOpenCoroutine");
	}

	public void Get()
	{
		getButton.enabled = false;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(getButtonCanvas, 0f, 0.3f), isIndependentUpdate: true);
		for (int i = 0; i < itemSlotList.Count; i++)
		{
			if (itemSlotList[i].crewData == null)
			{
				global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(itemSlotList[i].canvas, 0f, 0.2f), isIndependentUpdate: true);
				global::UnityEngine.CanvasGroup slotCanvasGroup = gridSlots[i];
				global::UnityEngine.Transform targetTrans = ((i == 0) ? goldIconTrans : diaIconTrans);
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate((i == 0) ? goldSrc : diaSrc, itemSlotList[i].transform.position, global::UnityEngine.Quaternion.identity, base.transform);
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(slotCanvasGroup, 0f, 0.3f), isIndependentUpdate: true), delegate
				{
					slotCanvasGroup.gameObject.SetActive(value: false);
				});
				global::UnityEngine.RectTransform rect = gameObject.GetComponent<global::UnityEngine.RectTransform>();
				rect.anchoredPosition3D += new global::UnityEngine.Vector3(0f, 36f, 0f);
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(rect, 1.1f, 0.3f), isIndependentUpdate: true), delegate
				{
					global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(rect, 0.58f, 0.7f), isIndependentUpdate: true), 0.15f);
					global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOMove(rect, targetTrans.position, 0.7f), isIndependentUpdate: true), 0.15f), delegate
					{
						global::UnityEngine.Object.Destroy(rect.gameObject);
						global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(targetTrans, 1.2f, 0.1f), isIndependentUpdate: true), delegate
						{
							global::DG.Tweening.ShortcutExtensions.DOScale(targetTrans, 1f, 0.1f);
							PlayerInfo.Instance.waitActionDiamond = false;
							PlayerInfo.Instance.waitActionGold = false;
							global::UnityEngine.UI.Text[] array = moneyTexts;
							foreach (global::UnityEngine.UI.Text text in array)
							{
								global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(text.transform, 1.5f, 0.2f), isIndependentUpdate: true), delegate
								{
									global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(text.transform, 1f, 0.2f), isIndependentUpdate: true), 0.8f);
								});
								global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOColor(text, myColor.GetColor(255, 197, 0), 0.2f), isIndependentUpdate: true), delegate
								{
									global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOColor(text, global::UnityEngine.Color.white, 0.2f), isIndependentUpdate: true), 0.8f);
								});
							}
						});
					});
				});
				continue;
			}
			UI_ItemSlot slot = itemSlotList[i];
			global::UnityEngine.CanvasGroup slotCanvasGroup2 = gridSlots[i];
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(slot.transform, 1.1f, 0.3f), isIndependentUpdate: true), delegate
			{
				slot.transform.SetParent(base.transform);
				global::UnityEngine.RectTransform obj = slot.transform as global::UnityEngine.RectTransform;
				obj.anchorMin = global::UnityEngine.Vector2.one * 0.5f;
				obj.anchorMax = global::UnityEngine.Vector2.one * 0.5f;
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(slotCanvasGroup2, 0f, 0.3f), isIndependentUpdate: true), delegate
				{
					slotCanvasGroup2.gameObject.SetActive(value: false);
				});
				global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOMove(slot.transform, crewIconTrans.position, 0.7f), isIndependentUpdate: true), 0.2f);
				global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(slot.transform, 0.2f, 0.7f), isIndependentUpdate: true), 0.2f);
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(slot.canvas, 0f, 0.2f), isIndependentUpdate: true), 0.7f), delegate
				{
					global::DG.Tweening.ShortcutExtensions.DOKill(crewIconTrans);
					global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(crewIconTrans, 1.2f, 0.1f), isIndependentUpdate: true), delegate
					{
						global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(crewIconTrans, 1f, 0.1f), delegate
						{
						});
					});
				});
			});
		}
		StartCoroutine(GetClose());
		AudioManager.Instance.PlaySFX("ui_get_rewardbox");
	}

	private global::System.Collections.IEnumerator GetClose()
	{
		yield return new global::UnityEngine.WaitForSecondsRealtime((float)itemSlotList.Count * 0.2f + 1.1f);
		GetComponent<PopupAction>().Close();
	}

	private global::System.Collections.IEnumerator ItemOpenCoroutine()
	{
		yield return new global::UnityEngine.WaitForSecondsRealtime(0.4f);
		float time = 1f / (float)itemSlotList.Count;
		for (int i = 0; i < itemSlotList.Count; i++)
		{
			if (!isFree)
			{
				yield return new global::UnityEngine.WaitForSecondsRealtime(time);
				itemSlotList[i].Open();
			}
			else
			{
				itemSlotList[i].FreeBoxOpen();
			}
		}
		yield return new global::UnityEngine.WaitForSecondsRealtime(0.4f);
		skipBtn.enabled = false;
		skipBtn.gameObject.SetActive(value: false);
		buttonCanvas.gameObject.SetActive(value: true);
		buttonCanvas.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(buttonCanvas, 1f, 0.2f), isIndependentUpdate: true);
	}

	public void Skip()
	{
		StopCoroutine("ItemOpenCoroutine");
		skipBtn.enabled = false;
		skipBtn.gameObject.SetActive(value: false);
		nowItemBox.Skip();
		for (int i = 0; i < itemSlotList.Count; i++)
		{
			global::UnityEngine.CanvasGroup obj = gridSlots[i];
			global::DG.Tweening.ShortcutExtensions.DOKill(obj);
			obj.alpha = 1f;
		}
		for (int j = 0; j < itemSlotList.Count; j++)
		{
			itemSlotList[j].Skip();
		}
		buttonCanvas.gameObject.SetActive(value: true);
		global::DG.Tweening.ShortcutExtensions.DOKill(buttonCanvas);
		buttonCanvas.alpha = 1f;
	}
}
