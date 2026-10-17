public class UI_ExploreSailorPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform panelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text sailorName;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text sailorContent;

	private MySailorInventory selectedSailor;

	private void OnEnable()
	{
		canvasGroup.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.2f), isIndependentUpdate: true);
		panelTrans.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 1f, 0.4f), isIndependentUpdate: true);
	}

	public void SetSailor(MySailorInventory sailor)
	{
		selectedSailor = sailor;
		sailorName.text = selectedSailor.baseData.defaultName;
	}

	public void GoExplore()
	{
		selectedSailor.status = ESailorStatus.Explore;
		float num = global::UnityEngine.Random.Range(0f, 100f);
		selectedSailor.exploreStartTick = global::System.DateTime.Now.Ticks;
		if (num <= 1f)
		{
			selectedSailor.exploreGrade = 5;
			long ticks = global::System.DateTime.Now.AddHours(24.0).Ticks;
			selectedSailor.exploreEndTick = ticks;
		}
		else if (num <= 6f)
		{
			selectedSailor.exploreGrade = 4;
			long ticks2 = global::System.DateTime.Now.AddHours(12.0).Ticks;
			selectedSailor.exploreEndTick = ticks2;
		}
		else if (num <= 16f)
		{
			selectedSailor.exploreGrade = 3;
			long ticks3 = global::System.DateTime.Now.AddHours(6.0).Ticks;
			selectedSailor.exploreEndTick = ticks3;
		}
		else if (num <= 46f)
		{
			selectedSailor.exploreGrade = 2;
			long ticks4 = global::System.DateTime.Now.AddHours(2.0).Ticks;
			selectedSailor.exploreEndTick = ticks4;
		}
		else
		{
			selectedSailor.exploreGrade = 1;
			long ticks5 = global::System.DateTime.Now.AddMinutes(30.0).Ticks;
			selectedSailor.exploreEndTick = ticks5;
		}
		Close();
	}

	public void Close()
	{
		UI_Manager.Instance.ui_mainGame.ui_popupCrewShip.Refresh();
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.2f), isIndependentUpdate: true), 0.2f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 0.6f, 0.4f), isIndependentUpdate: true);
	}
}
