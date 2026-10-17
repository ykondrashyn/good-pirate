public class UI_FormationMenu : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] buttonSubShipLimitCountTextObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] buttonTextRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] buttonRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] buttonCanvasGroups;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform arrowRect;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup settingCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button menuButton;

	private bool isOpend;

	private void OnEnable()
	{
		menuButton.onClick.RemoveAllListeners();
		menuButton.onClick.AddListener(MenuOpen);
		for (int i = 0; i < buttonCanvasGroups.Length; i++)
		{
			if (i == (int)PlayerInfo.Instance.myData.selectedFormationNum)
			{
				buttonCanvasGroups[i].gameObject.SetActive(value: true);
				buttonCanvasGroups[i].alpha = 1f;
			}
			else
			{
				buttonCanvasGroups[i].gameObject.SetActive(value: false);
				buttonCanvasGroups[i].alpha = 0f;
			}
			buttonTextRectTransform[i].anchoredPosition = new global::UnityEngine.Vector3(0f, 31f, 0f);
			buttonSubShipLimitCountTextObj[i].SetActive(value: true);
			buttonRectTransform[i].anchoredPosition = global::UnityEngine.Vector3.zero;
		}
		isOpend = false;
		if (GameManager.Instance.IsInIsland)
		{
			arrowRect.anchoredPosition = new global::UnityEngine.Vector3(-160f, 0f, 0f);
			arrowRect.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 90f);
		}
		else
		{
			arrowRect.anchoredPosition = new global::UnityEngine.Vector3(0f, 160f, 0f);
			arrowRect.rotation = global::UnityEngine.Quaternion.identity;
		}
		settingCanvas.alpha = 0f;
		settingCanvas.gameObject.SetActive(value: true);
	}

	public void MenuOpen()
	{
		if (isOpend)
		{
			return;
		}
		isOpend = true;
		for (int i = 0; i < buttonCanvasGroups.Length; i++)
		{
			if (!buttonCanvasGroups[i].gameObject.activeSelf)
			{
				buttonCanvasGroups[i].gameObject.SetActive(value: true);
				buttonCanvasGroups[i].alpha = 0f;
				global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.DOTweenModuleUI.DOFade(buttonCanvasGroups[i], 0.5f, 0.4f), global::DG.Tweening.Ease.Linear);
			}
			if (GameManager.Instance.IsInIsland)
			{
				global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(buttonRectTransform[i], new global::UnityEngine.Vector3(-650f + (float)i * 216.66f, 0f, 0f), 0.3f);
			}
			else
			{
				global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(buttonRectTransform[i], new global::UnityEngine.Vector3(0f, 650f - (float)i * 216.66f, 0f), 0.3f);
			}
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(buttonTextRectTransform[i], new global::UnityEngine.Vector3(0f, 0f, 0f), 0.3f);
			buttonSubShipLimitCountTextObj[i].SetActive(value: false);
		}
		if (GameManager.Instance.IsInIsland)
		{
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(arrowRect, new global::UnityEngine.Vector3(-805f, 0f, 0f), 0.3f);
			global::DG.Tweening.ShortcutExtensions.DORotate(arrowRect, new global::UnityEngine.Vector3(0f, 0f, 270f), 0.3f);
		}
		else
		{
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(arrowRect, new global::UnityEngine.Vector3(0f, 805f, 0f), 0.3f);
			global::DG.Tweening.ShortcutExtensions.DORotate(arrowRect, new global::UnityEngine.Vector3(0f, 0f, 180f), 0.3f);
		}
		menuButton.onClick.RemoveAllListeners();
		menuButton.onClick.AddListener(MenuClose);
		settingCanvas.gameObject.SetActive(value: true);
		settingCanvas.alpha = 0f;
		global::DG.Tweening.DOTweenModuleUI.DOFade(settingCanvas, 1f, 0.4f);
		UI_Manager.Instance.ui_mainGame.ui_mainUpgradeMenu.Close();
	}

	public void MenuClose()
	{
		if (!isOpend)
		{
			return;
		}
		isOpend = false;
		for (int i = 0; i < buttonCanvasGroups.Length; i++)
		{
			bool flag = false;
			if ((int)PlayerInfo.Instance.myData.selectedFormationNum == i)
			{
				flag = true;
			}
			if (!flag)
			{
				global::UnityEngine.GameObject obj = buttonCanvasGroups[i].gameObject;
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(buttonCanvasGroups[i], 0f, 0.3f), delegate
				{
					obj.SetActive(value: false);
				});
			}
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(buttonRectTransform[i], global::UnityEngine.Vector3.zero, 0.3f);
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(buttonTextRectTransform[i], new global::UnityEngine.Vector3(0f, 31f, 0f), 0.3f);
			buttonSubShipLimitCountTextObj[i].SetActive(value: true);
		}
		if (GameManager.Instance.IsInIsland)
		{
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(arrowRect, new global::UnityEngine.Vector3(-160f, 0f, 0f), 0.3f);
			global::DG.Tweening.ShortcutExtensions.DORotate(arrowRect, new global::UnityEngine.Vector3(0f, 0f, 90f), 0.3f);
		}
		else
		{
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(arrowRect, new global::UnityEngine.Vector3(0f, 160f, 0f), 0.3f);
			global::DG.Tweening.ShortcutExtensions.DORotate(arrowRect, global::UnityEngine.Vector3.zero, 0.3f);
		}
		menuButton.onClick.RemoveAllListeners();
		menuButton.onClick.AddListener(MenuOpen);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(settingCanvas, 0f, 0.3f), delegate
		{
			settingCanvas.gameObject.SetActive(value: true);
		});
	}

	public void SetFormation(int num)
	{
		if ((int)PlayerInfo.Instance.myData.selectedFormationNum == num)
		{
			MenuOpen();
			return;
		}
		for (int i = 0; i < buttonCanvasGroups.Length; i++)
		{
			if (i == num)
			{
				buttonCanvasGroups[i].alpha = 1f;
			}
			else
			{
				buttonCanvasGroups[i].alpha = 0.5f;
			}
		}
		PlayerInfo.Instance.myData.selectedFormationNum = num;
		PlayerInfo.Instance.SaveData();
		ShipController playerShipController = GameManager.Instance.playerShipController;
		global::System.Collections.Generic.List<FormationPos> formationList = playerShipController.formationList;
		foreach (ShipSubController subShip in playerShipController.subShipList)
		{
			subShip.SetTarget(subShip.targetTrans, formationList[subShip.myShip.InvenShipData.lineTempIndex[num]].transform.localPosition);
		}
	}
}
