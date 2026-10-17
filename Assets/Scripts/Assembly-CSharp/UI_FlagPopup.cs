public class UI_FlagPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject flagSlotSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform flagGridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject askPopup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform shipModelPositionTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] btnObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text needIslandText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] abilityTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject adsTimeObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject buyObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button buyButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text timeText;

	private global::System.Collections.Generic.List<UI_FlagSlot> flagSlotList = new global::System.Collections.Generic.List<UI_FlagSlot>();

	private Flag shipFlag;

	private global::UnityEngine.GameObject modelObj;

	private UI_FlagSlot selectedFlagSlot;

	private int uiLayer;

	[global::UnityEngine.SerializeField]
	private UI_FlagSlot askFlagSlot;

	private global::System.Collections.Generic.List<int> orderList = new global::System.Collections.Generic.List<int>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] panelCanvasGroups;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] tabButtons;

	private void Awake()
	{
		OrderInit();
		uiLayer = global::UnityEngine.LayerMask.NameToLayer("UI");
		needIslandText.gameObject.SetActive(value: false);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(EShipMain.LEVEL_01);
		modelObj = global::UnityEngine.Object.Instantiate(mainData.modelSrc, shipModelPositionTrans);
		modelObj.transform.localScale = global::UnityEngine.Vector3.one;
		modelObj.transform.localRotation = global::UnityEngine.Quaternion.identity;
		modelObj.transform.localPosition = global::UnityEngine.Vector3.zero;
		global::UnityEngine.Transform[] componentsInChildren = modelObj.GetComponentsInChildren<global::UnityEngine.Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = uiLayer;
		}
		shipFlag = modelObj.GetComponentInChildren<Flag>();
		global::System.Collections.Generic.List<FlagInfo> flagList = FlagManager.Instance.flagList;
		for (int j = 0; j < orderList.Count; j++)
		{
			UI_FlagSlot component = global::UnityEngine.Object.Instantiate(flagSlotSrc, flagGridTrans).GetComponent<UI_FlagSlot>();
			component.SetFlagData(flagList[orderList[j]]);
			component.popupManager = this;
			flagSlotList.Add(component);
		}
	}

	private void OnEnable()
	{
		shipFlag = modelObj.GetComponentInChildren<Flag>();
		if (FlagManager.Instance.GetFlagInfo(PlayerInfo.Instance.myData.selectedFlagId, out var info))
		{
			shipFlag.SetFlag(info.flagMaterial);
		}
		Refresh();
		TabOpen(1);
	}

	private void LateUpdate()
	{
		if (!(selectedFlagSlot == null) && adsTimeObj.activeSelf)
		{
			int num = (int)CoolTimeManager.GetLeftCoolTime("ADSFLAGCOOLTIME" + selectedFlagSlot.myData.id).TotalSeconds;
			if (num > 0)
			{
				timeText.text = $"{num / 60:D2}:{num % 60:D2}";
				return;
			}
			adsTimeObj.SetActive(value: false);
			buyObj.SetActive(value: true);
			buyButton.enabled = true;
		}
	}

	public void SelectedThis(UI_FlagSlot slot)
	{
		if ((int)PlayerInfo.Instance.myData.selectedFlagId == slot.myData.id)
		{
			return;
		}
		selectedFlagSlot = slot;
		shipFlag.SetFlag(slot.myData.flagMaterial);
		if (PlayerInfo.Instance.IsHaveFlag(slot.myData.id))
		{
			PlayerInfo.Instance.myData.selectedFlagId = slot.myData.id;
			GameManager.Instance.playerShipController.ChangeFlag(PlayerInfo.Instance.myData.selectedFlagId);
			GameManager.Instance.DockAndTaxShipFlag();
			PlayerInfo.Instance.SaveData();
			Refresh();
			return;
		}
		askFlagSlot.SetFlagData(selectedFlagSlot.myData);
		switch (selectedFlagSlot.myData.priceType)
		{
		case EFlagPriceType.CLAIM:
		{
			global::UnityEngine.GameObject[] array = btnObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			needIslandText.gameObject.SetActive(value: true);
			needIslandText.text = string.Format("{2} ({0}/{1})", PlayerInfo.Instance.ClearIslandClaimCount, selectedFlagSlot.myData.price, LanguageManager.Instance.translate("key_claim"));
			break;
		}
		case EFlagPriceType.ISLAND:
		{
			global::UnityEngine.GameObject[] array = btnObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			needIslandText.gameObject.SetActive(value: true);
			needIslandText.text = string.Format("{2} ({0}/{1})", PlayerInfo.Instance.ClearIslandCount, selectedFlagSlot.myData.price, LanguageManager.Instance.translate("key_island"));
			break;
		}
		default:
		{
			global::UnityEngine.GameObject[] array = btnObjs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			needIslandText.gameObject.SetActive(value: false);
			break;
		}
		}
		askPopup.SetActive(value: true);
	}

	private void ReturnEquippedFlag()
	{
		if (FlagManager.Instance.GetFlagInfo(PlayerInfo.Instance.myData.selectedFlagId, out var info))
		{
			shipFlag.SetFlag(info.flagMaterial);
		}
	}

	public void Buy()
	{
		switch (selectedFlagSlot.myData.priceType)
		{
		case EFlagPriceType.DIAMOND:
			if ((long)PlayerInfo.Instance.myData.diamond >= selectedFlagSlot.myData.price)
			{
				PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -selectedFlagSlot.myData.price);
				AudioManager.Instance.PlaySFX("use_diamond");
				BuySuccess();
				GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "깃발 구매", "깃발 구매", 1L);
			}
			break;
		case EFlagPriceType.ADS:
			if ((bool)PlayerInfo.Instance.myData.isRemoveADS)
			{
				ShowAdsSuccess();
				break;
			}
			MoPubAdManager.Instance.ShowRewardedVideo(ShowAdsSuccess);
			GoogleAnalyticsV3.instance.LogEvent("광고", "광고 본 횟수", "깃발", 1L);
			break;
		case EFlagPriceType.ISLAND:
			break;
		}
	}

	private void ShowAdsSuccess()
	{
		AudioManager.PlayButtonClick();
		int flagAdsCount = PlayerInfo.Instance.GetFlagAdsCount(selectedFlagSlot.myData.id);
		flagAdsCount++;
		PlayerInfo.Instance.SetFlagAdsCount(selectedFlagSlot.myData.id, flagAdsCount);
		CoolTimeManager.AddCooltimeSecondFromNow("ADSFLAGCOOLTIME" + selectedFlagSlot.myData.id, 601);
		if (flagAdsCount >= selectedFlagSlot.myData.price)
		{
			BuySuccess();
			return;
		}
		askFlagSlot.Refresh();
		Refresh();
	}

	private void BuySuccess()
	{
		PlayerInfo.Instance.myData.selectedFlagId = selectedFlagSlot.myData.id;
		PlayerInfo.Instance.BuyFlag(PlayerInfo.Instance.myData.selectedFlagId);
		shipFlag.SetFlag(selectedFlagSlot.myData.flagMaterial);
		GameManager.Instance.playerShipController.ChangeFlag(PlayerInfo.Instance.myData.selectedFlagId);
		GameManager.Instance.DockAndTaxShipFlag();
		PlayerInfo.Instance.SaveData();
		askPopup.GetComponent<PopupAction>().Close();
		Refresh();
		FlagManager.Instance.RefreshFlagAbility();
	}

	public void Cancel()
	{
		selectedFlagSlot = null;
		askPopup.GetComponent<PopupAction>().Close();
		ReturnEquippedFlag();
	}

	private void Refresh()
	{
		foreach (UI_FlagSlot flagSlot in flagSlotList)
		{
			flagSlot.Refresh();
		}
		FlagManager.Instance.RefreshFlagAbility();
		abilityTexts[0].text = FlagManager.Instance.AbilityAttack.ToString("F1") + "%";
		abilityTexts[1].text = FlagManager.Instance.AbilityCoolTime.ToString("F1") + "%";
		abilityTexts[2].text = FlagManager.Instance.AbilityAcceleration.ToString("F1") + "%";
		abilityTexts[3].text = FlagManager.Instance.AbilityRotation.ToString("F1") + "%";
		if (!(selectedFlagSlot != null))
		{
			return;
		}
		EFlagPriceType priceType = selectedFlagSlot.myData.priceType;
		if (priceType == EFlagPriceType.ADS)
		{
			buyObj.SetActive(value: false);
			if (CoolTimeManager.GetLeftCoolTime("ADSFLAGCOOLTIME" + selectedFlagSlot.myData.id).TotalSeconds > 0.0)
			{
				adsTimeObj.SetActive(value: true);
				buyObj.SetActive(value: false);
				buyButton.enabled = false;
			}
			else
			{
				adsTimeObj.SetActive(value: false);
				buyObj.SetActive(value: true);
				buyButton.enabled = true;
			}
		}
	}

	private void OrderInit()
	{
		orderList.Add(36);
		orderList.Add(37);
		orderList.Add(38);
		orderList.Add(39);
		orderList.Add(0);
		orderList.Add(34);
		orderList.Add(9);
		orderList.Add(27);
		orderList.Add(32);
		orderList.Add(26);
		orderList.Add(24);
		orderList.Add(31);
		orderList.Add(15);
		orderList.Add(6);
		orderList.Add(10);
		orderList.Add(11);
		orderList.Add(2);
		orderList.Add(20);
		orderList.Add(21);
		orderList.Add(16);
		orderList.Add(8);
		orderList.Add(19);
		orderList.Add(28);
		orderList.Add(4);
		orderList.Add(33);
		orderList.Add(35);
		orderList.Add(30);
		orderList.Add(17);
		orderList.Add(23);
		orderList.Add(14);
		orderList.Add(25);
		orderList.Add(1);
		orderList.Add(22);
		orderList.Add(12);
		orderList.Add(5);
		orderList.Add(3);
		orderList.Add(7);
		orderList.Add(29);
		orderList.Add(13);
		orderList.Add(18);
	}

	public void TabOpen(int num)
	{
		for (int i = 0; i < panelCanvasGroups.Length; i++)
		{
			if (i == num)
			{
				if (!panelCanvasGroups[i].gameObject.activeSelf)
				{
					panelCanvasGroups[i].gameObject.SetActive(value: true);
					global::DG.Tweening.ShortcutExtensions.DOKill(panelCanvasGroups[i]);
					panelCanvasGroups[i].alpha = 0f;
					global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(panelCanvasGroups[i], 1f, 0.2f), isIndependentUpdate: true);
				}
			}
			else if (panelCanvasGroups[i].gameObject.activeSelf)
			{
				panelCanvasGroups[i].gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < tabButtons.Length; j++)
		{
			if (j == num)
			{
				tabButtons[j].color = global::UnityEngine.Color.white;
			}
			else
			{
				tabButtons[j].color = myColor.GetColor(255, 255, 255, 128);
			}
		}
		AudioManager.PlayButtonClick();
	}
}
