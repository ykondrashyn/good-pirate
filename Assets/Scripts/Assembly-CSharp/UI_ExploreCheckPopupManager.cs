public class UI_ExploreCheckPopupManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.Header("check it !")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image rewardBoxImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image crewImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewTypeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text crewName;

	[global::UnityEngine.Header("box")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image rewardBoxImage2;

	[global::UnityEngine.Header("reward")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text rewardItemNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text rewardItemContentText;

	[global::UnityEngine.Header("roulette")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform rouletteTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] rouletteInactives;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] spinCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] spinButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject spinBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject getBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject againBtnObj;

	[global::UnityEngine.Header("etc")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image blackBgImage;

	[global::UnityEngine.Header("panels")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject panelCheck;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject panelBox;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject panelReward;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject panelRoulette;

	[global::UnityEngine.Header("my")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup myCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform panelsTrans;

	private MySailorInventory selectedSailorData;

	private void OnEnable()
	{
		panelCheck.SetActive(value: true);
		panelBox.SetActive(value: false);
		panelReward.SetActive(value: false);
		panelRoulette.SetActive(value: false);
		myCanvasGroup.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 1f, 0.2f), isIndependentUpdate: true);
		panelsTrans.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelsTrans, 1f, 0.4f), isIndependentUpdate: true);
		global::UnityEngine.CanvasGroup[] array = spinCanvas;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].alpha = 1f;
		}
		global::UnityEngine.UI.Button[] array2 = spinButton;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
		spinBtnObj.SetActive(value: true);
		getBtnObj.SetActive(value: false);
		againBtnObj.SetActive(value: false);
	}

	public void SetCrewData(MySailorInventory data)
	{
		selectedSailorData = data;
		crewName.text = data.baseData.defaultName;
	}

	public void Close()
	{
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelsTrans, 0.6f, 0.4f), isIndependentUpdate: true);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 0f, 0.2f), isIndependentUpdate: true), 0.2f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void CheckIt()
	{
		panelCheck.SetActive(value: false);
		panelBox.SetActive(value: true);
	}

	public void BoxOpen()
	{
		panelBox.SetActive(value: false);
		panelReward.SetActive(value: true);
	}

	public void Roulette()
	{
		panelReward.SetActive(value: false);
		panelRoulette.SetActive(value: true);
	}

	public void RouletteBack()
	{
		panelRoulette.SetActive(value: false);
		panelReward.SetActive(value: true);
	}

	public void Spin()
	{
		global::UnityEngine.GameObject[] array = rouletteInactives;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		global::UnityEngine.UI.Button[] array2 = spinButton;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		global::UnityEngine.CanvasGroup[] array3 = spinCanvas;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].alpha = 0.5f;
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DORotate(rouletteTrans, new global::UnityEngine.Vector3(0f, 0f, 1860f), 5f, global::DG.Tweening.RotateMode.FastBeyond360), global::DG.Tweening.Ease.OutCirc), isIndependentUpdate: true), delegate
		{
			global::UnityEngine.UI.Button[] array4 = spinButton;
			for (int j = 0; j < array4.Length; j++)
			{
				array4[j].enabled = true;
			}
			global::UnityEngine.CanvasGroup[] array5 = spinCanvas;
			for (int j = 0; j < array5.Length; j++)
			{
				array5[j].alpha = 1f;
			}
			spinBtnObj.SetActive(value: false);
			getBtnObj.SetActive(value: true);
			againBtnObj.SetActive(value: true);
		});
	}

	public void Get()
	{
		selectedSailorData.status = ESailorStatus.OnCall;
		selectedSailorData.exploreEndTick = 0L;
		selectedSailorData.exploreStartTick = 0L;
		UI_Manager.Instance.ui_mainGame.ui_popupCrewShip.Refresh();
		Close();
	}
}
