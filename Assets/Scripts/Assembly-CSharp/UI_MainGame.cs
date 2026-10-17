public class UI_MainGame : global::UnityEngine.MonoBehaviour
{
	public delegate void showUIToggleDelegate(bool active);

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] portraitObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] landscapeObjs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform mainMenuRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform minimapRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup outIslandCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup inIslandCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup hpCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image hpTopGauge;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform hpTopRedRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image watchHpToggleImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject popupCrewShip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup claimCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button claimBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandNameText;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.GameObject conquestClearNeedMsg;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.UI.Text conquestHaveTaxTime;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.UI.Text conquestTaxText;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.UI.Text conquestTaxValueText;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.UI.Text needClearTaxText;

	[global::UnityEngine.SerializeField]
	public global::UnityEngine.UI.Text needClearTaxValueText;

	public UI_PopupCrewShip ui_popupCrewShip;

	public UI_IslandStatus ui_islandStatus;

	public UI_PatrolCombatStatus ui_patrolCombatStatus;

	public UI_GateEnemyStatus ui_gateEnemyStatus;

	public UI_IslandRewardPopup ui_islandRewardPopup;

	public UI_ShipSelectSailorPopup ui_shipSelectSailorPopup;

	public UI_MainUpgradeMenu ui_mainUpgradeMenu;

	public UI_ExploreCheckPopupManager ui_exploreCheckPopupManager;

	public UI_ShipInfoPopup ui_shipInfoPopup;

	public UI_CrewInfoPopup ui_crewInfoPopup;

	public UI_CrewPopup ui_crewPopup;

	public UI_ChestPanel ui_chestPanel;

	public UI_LimitShipBtn ui_limitShipBtn;

	public UI_DeadAskPopup deadAskPopupObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup shipsPanelCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup tipsPanelCanvas;

	public HealSpeech healSpeech;

	public UI_MainGame.showUIToggleDelegate uiToggleDelegate;

	public bool IsShowGuideUIToggle = true;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform rewardboxTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image boxImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text boxLevel;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite boxBronze;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite boxSilver;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite boxGold;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite boxBlack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	public EUIMode uiMode { get; private set; }

	private void Start()
	{
		OrientationCheck.Instance.AddPortraitEvent(SetPortrait);
		OrientationCheck.Instance.AddLandscapeEvent(SetLandscape);
		if ((bool)ui_islandStatus)
		{
			ui_islandStatus.gameObject.SetActive(value: false);
		}
		if ((bool)ui_islandRewardPopup)
		{
			ui_islandRewardPopup.gameObject.SetActive(value: false);
		}
		uiMode = EUIMode.InIsland;
		ShowUIToggle();
		conquestTaxText.text = LanguageManager.Instance.translate("key_tax") + ":";
		needClearTaxText.text = LanguageManager.Instance.translate("key_tax") + "+";
	}

	private void SetPortrait()
	{
		global::UnityEngine.GameObject[] array = portraitObjs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		array = landscapeObjs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		mainMenuRect.anchoredPosition = new global::UnityEngine.Vector2(93f, -150f);
		minimapRect.anchoredPosition = new global::UnityEngine.Vector2(-193f, -350f);
	}

	private void SetLandscape()
	{
		global::UnityEngine.GameObject[] array = portraitObjs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = landscapeObjs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: true);
		}
		mainMenuRect.anchoredPosition = new global::UnityEngine.Vector2(93f, -456f);
		minimapRect.anchoredPosition = new global::UnityEngine.Vector2(-193f, -193f);
	}

	public void UIMode(EUIMode mode)
	{
		uiMode = mode;
		global::DG.Tweening.ShortcutExtensions.DOKill(outIslandCanvas);
		if ((bool)outIslandCanvas)
		{
			outIslandCanvas.gameObject.SetActive(value: false);
		}
		if ((bool)inIslandCanvas)
		{
			inIslandCanvas.gameObject.SetActive(value: false);
		}
		switch (mode)
		{
		case EUIMode.InIsland:
			if ((bool)inIslandCanvas)
			{
				inIslandCanvas.gameObject.SetActive(value: true);
				inIslandCanvas.alpha = 0f;
				global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(inIslandCanvas, 1f, 0.6f), isIndependentUpdate: true);
			}
			TouchPad.Instance.gameObject.SetActive(value: false);
			break;
		case EUIMode.OutIsland:
			outIslandCanvas.gameObject.SetActive(value: true);
			outIslandCanvas.alpha = 0f;
			global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(outIslandCanvas, 1f, 0.6f), isIndependentUpdate: true);
			break;
		case EUIMode.Formation:
			break;
		}
	}

	public void OutIsLand()
	{
		GameManager.Instance.OutIsland();
		UI_Manager.Instance.UIActiveFade(active: false, OutIsLandUI);
		AudioManager.Instance.PlaySFX("journey");
	}

	private void OutIsLandUI()
	{
		UI_Manager.Instance.UIActiveFade(active: true);
		UIMode(EUIMode.OutIsland);
	}

	public void PopupSetActive(bool active)
	{
		if (active)
		{
			global::UnityEngine.Time.timeScale = 0f;
		}
		else
		{
			global::UnityEngine.Time.timeScale = 1f;
		}
		popupCrewShip.SetActive(active);
	}

	public void HpBar(float ratio)
	{
		float num = hpTopGauge.fillAmount - ratio;
		if (num > 0f)
		{
			float width = hpTopGauge.rectTransform.rect.width;
			global::UnityEngine.Vector2 sizeDelta = hpTopRedRect.sizeDelta;
			sizeDelta.x = width * num;
			hpTopRedRect.gameObject.SetActive(value: true);
			global::DG.Tweening.ShortcutExtensions.DOKill(hpTopRedRect);
			hpTopRedRect.sizeDelta = sizeDelta;
			hpTopRedRect.anchoredPosition = new global::UnityEngine.Vector2(width * ratio, 0f);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.DOTweenModuleUI.DOSizeDelta(hpTopRedRect, new global::UnityEngine.Vector2(0f, sizeDelta.y), 0.8f), global::DG.Tweening.Ease.Linear), delegate
			{
				hpTopRedRect.gameObject.SetActive(value: false);
			});
		}
		hpTopGauge.fillAmount = ratio;
	}

	public void ShowUIToggle()
	{
		IsShowGuideUIToggle = !IsShowGuideUIToggle;
		if (IsShowGuideUIToggle)
		{
			watchHpToggleImage.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.5f);
			HpBarManager.Instance.SetActive(active: true);
			if (TutorialManager.Instance.IsHadTutorial("doubletap"))
			{
				TutorialManager.Instance.StartTutorial("battleguide");
			}
		}
		else
		{
			watchHpToggleImage.color = global::UnityEngine.Color.white;
			HpBarManager.Instance.SetActive(active: false);
		}
		if (uiToggleDelegate != null)
		{
			uiToggleDelegate(IsShowGuideUIToggle);
		}
	}

	public void ShipsPanel()
	{
		if (shipsPanelCanvas.gameObject.activeSelf)
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(shipsPanelCanvas);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(shipsPanelCanvas, 0f, 0.2f), delegate
			{
				tipsPanelCanvas.gameObject.SetActive(value: true);
				tipsPanelCanvas.alpha = 0f;
				global::DG.Tweening.DOTweenModuleUI.DOFade(tipsPanelCanvas, 1f, 0.3f);
				shipsPanelCanvas.gameObject.SetActive(value: false);
			});
		}
		else
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(tipsPanelCanvas);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(tipsPanelCanvas, 0f, 0.2f), delegate
			{
				shipsPanelCanvas.gameObject.SetActive(value: true);
				shipsPanelCanvas.alpha = 0f;
				global::DG.Tweening.DOTweenModuleUI.DOFade(shipsPanelCanvas, 1f, 0.3f);
				tipsPanelCanvas.gameObject.SetActive(value: false);
			});
		}
	}

	public void OpenShipInfomationPopup(MyShipInventory shipData)
	{
		ui_shipInfoPopup.gameObject.SetActive(value: true);
		ui_shipInfoPopup.SetData(shipData);
	}

	public global::UnityEngine.Sprite BoxSprite(ERewardBox type)
	{
		global::UnityEngine.Sprite result = null;
		switch (type)
		{
		case ERewardBox.BRONZE:
			result = boxBronze;
			break;
		case ERewardBox.SILVER:
			result = boxSilver;
			break;
		case ERewardBox.GOLD:
			result = boxGold;
			break;
		case ERewardBox.BLACK:
			result = boxBlack;
			break;
		}
		return result;
	}

	public void AddRewardBox(RewardBoxStatus boxData)
	{
		boxImage.sprite = BoxSprite(boxData.boxType);
		boxLevel.text = "Lv." + boxData.level;
		rewardboxTrans.gameObject.SetActive(value: true);
		rewardboxTrans.SetParent(base.transform);
		rewardboxTrans.anchoredPosition = global::UnityEngine.Vector3.zero;
		rewardboxTrans.localScale = global::UnityEngine.Vector3.one * 0.2f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(rewardboxTrans, 2.5f, 0.6f), delegate
		{
			StartCoroutine(NewBoxMove(boxData));
		});
	}

	private void OnEnable()
	{
		if (rewardboxTrans.gameObject.activeSelf)
		{
			rewardboxTrans.gameObject.SetActive(value: false);
		}
		ui_chestPanel.BoxRefresh();
		ClaimTaxManager.Instance.RefreshTaxValue();
		conquestTaxValueText.text = Numbering.Convert(ClaimTaxManager.Instance.TaxValue);
	}

	private global::System.Collections.IEnumerator NewBoxMove(RewardBoxStatus boxData)
	{
		float totalTime = 0f;
		float duration = 0.9f;
		bool isPrevOpen = ui_chestPanel.IsOpen;
		float startScale = rewardboxTrans.localScale.x;
		if (!ui_chestPanel.IsOpen)
		{
			ui_chestPanel.Open();
		}
		yield return new global::UnityEngine.WaitForSeconds(0.6f);
		UI_ItemRewardBox emptyBox = null;
		ui_chestPanel.IsHaveEmptySlot(ref emptyBox);
		emptyBox.SetData(boxData);
		PlayerInfo.Instance.myData.rewardBoxStatus[emptyBox.boxSlotNumber - 1] = boxData;
		PlayerInfo.Instance.SaveData();
		rewardboxTrans.SetParent(emptyBox.transform);
		global::UnityEngine.Vector3 startPos = rewardboxTrans.anchoredPosition;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float time = totalTime / duration;
			time = animCurve.Evaluate(time);
			float num = global::UnityEngine.Mathf.Lerp(startScale, 1f, time);
			rewardboxTrans.anchoredPosition = global::UnityEngine.Vector3.Lerp(startPos, global::UnityEngine.Vector3.zero, time);
			rewardboxTrans.localScale = global::UnityEngine.Vector3.one * num;
			if (time >= 1f)
			{
				break;
			}
			yield return null;
		}
		rewardboxTrans.gameObject.SetActive(value: false);
		emptyBox.RefreshStatus();
		AudioManager.Instance.PlaySFX("ui_get_rewardbox");
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(emptyBox.transform, 1.1f, 0.15f), delegate
		{
			global::DG.Tweening.ShortcutExtensions.DOScale(emptyBox.transform, 1f, 0.15f);
		});
		TutorialManager.Instance.StartTutorial("rewardbox");
		yield return new global::UnityEngine.WaitForSeconds(1.3f);
		if (!isPrevOpen)
		{
			ui_chestPanel.Close();
		}
	}

	public void SetClaimActive(bool active)
	{
		claimBtn.enabled = active;
		if (active)
		{
			claimCanvas.alpha = 1f;
		}
		else
		{
			claimCanvas.alpha = 0.3f;
		}
	}

	public void SetIslandName(string name)
	{
		islandNameText.text = name;
	}
}
