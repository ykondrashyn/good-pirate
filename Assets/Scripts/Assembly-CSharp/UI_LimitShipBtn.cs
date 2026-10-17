public class UI_LimitShipBtn : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform infoPanel;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup infoPanelGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] canvasGroups;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	private int prevClearIsland = -1;

	private bool isActive;

	public bool IsActive => isActive;

	public void Toggle()
	{
		isActive = !isActive;
		if (isActive)
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(infoPanelGroup);
			global::DG.Tweening.ShortcutExtensions.DOKill(infoPanel);
			infoPanel.gameObject.SetActive(value: true);
			global::DG.Tweening.ShortcutExtensions.DOScale(infoPanel, 1f, 0.3f);
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(infoPanel, 280f, 0.3f);
			global::DG.Tweening.DOTweenModuleUI.DOFade(infoPanelGroup, 1f, 0.3f);
		}
		else
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(infoPanelGroup);
			global::DG.Tweening.ShortcutExtensions.DOKill(infoPanel);
			global::DG.Tweening.DOTweenModuleUI.DOAnchorPosY(infoPanel, 0f, 0.3f);
			global::DG.Tweening.ShortcutExtensions.DOScale(infoPanel, 0f, 0.3f);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(infoPanelGroup, 0f, 0.3f), delegate
			{
				infoPanel.gameObject.SetActive(value: false);
			});
		}
	}

	private void AutoDisable()
	{
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void Refresh()
	{
		int clearIslandCount = PlayerInfo.Instance.ClearIslandCount;
		myText.text = clearIslandCount.ToString();
		global::UnityEngine.CanvasGroup[] array = canvasGroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].alpha = 1f;
		}
		int num = clearIslandCount;
		if (num >= ConstValue.island_clear_4 && num < ConstValue.island_clear_5)
		{
			canvasGroups[0].alpha = 0.4f;
		}
		if (num >= ConstValue.island_clear_5 && num < ConstValue.island_clear_6)
		{
			canvasGroups[1].alpha = 0.4f;
		}
		if (num >= ConstValue.island_clear_6 && num < ConstValue.island_clear_7)
		{
			canvasGroups[2].alpha = 0.4f;
		}
		if (num >= ConstValue.island_clear_7)
		{
			canvasGroups[3].alpha = 0.4f;
		}
	}
}
