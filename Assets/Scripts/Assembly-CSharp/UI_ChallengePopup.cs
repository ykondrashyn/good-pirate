public class UI_ChallengePopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] titleTexts;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup myCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup waitPanel;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup loadingPanel;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text timeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text priatesCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text objectiveMessageText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform gridTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject userSlotSrc;

	private global::System.Collections.Generic.List<UI_UserSlot> userSlotList = new global::System.Collections.Generic.List<UI_UserSlot>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject connectingObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject connectedObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nowConnectedPlayerText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cancelBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject countdownBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text countdownText;

	private ClaimStatus selectedClaimData = new ClaimStatus();

	private int selectedMyNumber;

	private string strFmUserCount = "{2} ({0}/{1})";

	[global::UnityEngine.SerializeField]
	private ClaimMode sheetData;

	private void OnEnable()
	{
		waitPanel.gameObject.SetActive(value: true);
		waitPanel.alpha = 0f;
		global::DG.Tweening.ShortcutExtensions.DOKill(waitPanel);
		global::DG.Tweening.DOTweenModuleUI.DOFade(waitPanel, 1f, 0.2f);
		waitPanel.transform.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(waitPanel.transform, 1f, 0.2f), isIndependentUpdate: true);
		myCanvas.alpha = 1f;
		loadingPanel.gameObject.SetActive(value: false);
		foreach (UI_UserSlot userSlot in userSlotList)
		{
			if (userSlot != null)
			{
				global::UnityEngine.Object.Destroy(userSlot.gameObject);
			}
		}
		userSlotList.Clear();
		int num = PlayerInfo.Instance.myData.selectedIslandId;
		string text = LanguageManager.Instance.translate("key_island") + " " + (num + 1);
		global::UnityEngine.UI.Text[] array = titleTexts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = text;
		}
		ClaimModeData claimModeData = sheetData.dataArray[num];
		selectedClaimData.claimType = (eClaimMode)claimModeData.Claimtype;
		selectedClaimData.islandId = num;
		selectedClaimData.spawnIntervalTime = claimModeData.Enemyspawnintervaltime;
		selectedClaimData.allyCount = claimModeData.Allycount;
		selectedClaimData.spawnCount = claimModeData.Enemyspawncount;
		selectedClaimData.spawnGroupCount = claimModeData.Enemyspawngroupcount;
		selectedClaimData.limitTime = claimModeData.Limittime;
		selectedClaimData.bossType = claimModeData.Bosstype;
		selectedClaimData.bossSubCount = claimModeData.Bosssub;
		selectedClaimData.maxLevel = global::UnityEngine.Mathf.Clamp(claimModeData.Enemylevelmax, 10, 1000);
		int num2 = (int)(claimModeData.Limittime / 60f);
		int num3 = (int)(claimModeData.Limittime % 60f);
		timeText.text = $"{num2}:{num3:00}";
		priatesCountText.text = "x" + claimModeData.Allycount;
		switch (selectedClaimData.claimType)
		{
		case eClaimMode.DefendTheSupplyShip:
			objectiveMessageText.text = LanguageManager.Instance.translate("key_claim_message_defend");
			break;
		case eClaimMode.KillAllEnemy:
			objectiveMessageText.text = LanguageManager.Instance.translate("key_claim_message_killall");
			break;
		case eClaimMode.KillTheBoss:
			objectiveMessageText.text = LanguageManager.Instance.translate("key_claim_message_killboss");
			break;
		}
	}

	public void Close()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(myCanvas);
		global::DG.Tweening.ShortcutExtensions.DOKill(waitPanel);
		global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvas, 0f, 0.2f);
		global::DG.Tweening.DOTweenModuleUI.DOFade(waitPanel, 0f, 0.2f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(waitPanel.transform, 0.6f, 0.2f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void Match()
	{
		foreach (UI_UserSlot userSlot in userSlotList)
		{
			if (userSlot != null)
			{
				global::UnityEngine.Object.Destroy(userSlot.gameObject);
			}
		}
		userSlotList.Clear();
		cancelBtnObj.gameObject.SetActive(value: true);
		countdownBtnObj.gameObject.SetActive(value: false);
		StartCoroutine("LoadingPlayer");
		OpenPanel(loadingPanel, waitPanel);
	}

	private global::System.Collections.IEnumerator LoadingPlayer()
	{
		int maxCount = selectedClaimData.allyCount;
		connectingObj.SetActive(value: true);
		connectedObj.SetActive(value: false);
		yield return new global::UnityEngine.WaitForSecondsRealtime(global::UnityEngine.Random.Range(0.4f, 2.5f));
		connectingObj.SetActive(value: false);
		connectedObj.SetActive(value: true);
		selectedMyNumber = global::UnityEngine.Random.Range(0, maxCount);
		for (int i = 0; i < maxCount; i++)
		{
			UI_UserSlot component = global::UnityEngine.Object.Instantiate(userSlotSrc, gridTrans).GetComponent<UI_UserSlot>();
			userSlotList.Add(component);
			if (selectedMyNumber == i)
			{
				component.SetMe();
			}
			else if (i < selectedMyNumber)
			{
				component.SetUser();
			}
			else
			{
				component.SetWaiting();
			}
		}
		string captain = LanguageManager.Instance.translate("key_captain");
		nowConnectedPlayerText.text = string.Format(strFmUserCount, selectedMyNumber + 1, maxCount, captain);
		yield return new global::UnityEngine.WaitForSecondsRealtime(global::UnityEngine.Random.Range(0.1f, 0.6f));
		for (int j = selectedMyNumber + 1; j < userSlotList.Count; j++)
		{
			if (global::UnityEngine.Random.value <= 0.2f)
			{
				yield return new global::UnityEngine.WaitForSecondsRealtime(global::UnityEngine.Random.Range(2f, 5f));
			}
			else
			{
				yield return new global::UnityEngine.WaitForSecondsRealtime(global::UnityEngine.Random.Range(0.1f, 0.3f));
			}
			userSlotList[j].SetUser();
			nowConnectedPlayerText.text = string.Format(strFmUserCount, j + 1, maxCount, captain);
		}
		cancelBtnObj.gameObject.SetActive(value: false);
		countdownBtnObj.gameObject.SetActive(value: true);
		countdownText.text = "3";
		yield return new global::UnityEngine.WaitForSecondsRealtime(1f);
		countdownText.text = "2";
		yield return new global::UnityEngine.WaitForSecondsRealtime(1f);
		countdownText.text = "1";
		yield return new global::UnityEngine.WaitForSecondsRealtime(1f);
		UI_Manager.Instance.BlackScreenOn(0.6f, ClaimLoad);
	}

	private void ClaimLoad()
	{
		GameManager.Instance.playerShipController.OutIsland();
		ClaimManager.Instance.SetClaimStatus(selectedClaimData, selectedMyNumber);
		UI_Manager.Instance.BlackScreenOff(0.6f);
		ClaimManager.Instance.LoadClaimMap();
		base.gameObject.SetActive(value: false);
	}

	public void Cancel()
	{
		OpenPanel(waitPanel, loadingPanel);
		StopCoroutine("LoadingPlayer");
	}

	private void OpenPanel(global::UnityEngine.CanvasGroup activePanel, global::UnityEngine.CanvasGroup inactivePanel)
	{
		activePanel.gameObject.SetActive(value: true);
		activePanel.alpha = 0f;
		global::DG.Tweening.ShortcutExtensions.DOKill(activePanel);
		global::DG.Tweening.DOTweenModuleUI.DOFade(activePanel, 1f, 0.2f);
		global::DG.Tweening.ShortcutExtensions.DOKill(activePanel.transform);
		activePanel.transform.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(activePanel.transform, 1f, 0.2f), isIndependentUpdate: true);
		global::DG.Tweening.ShortcutExtensions.DOKill(inactivePanel);
		global::DG.Tweening.DOTweenModuleUI.DOFade(inactivePanel, 0f, 0.2f);
		global::DG.Tweening.ShortcutExtensions.DOKill(inactivePanel.transform);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(inactivePanel.transform, 0.6f, 0.2f), delegate
		{
			inactivePanel.gameObject.SetActive(value: false);
		});
	}
}
