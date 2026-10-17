public class UI_PopupCrewShip : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image blackImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] rectCanvas;

	private int prevSelectedNum = -1;

	[global::UnityEngine.SerializeField]
	private UI_ShipSlotManager shipSlotManger;

	[global::UnityEngine.SerializeField]
	private UI_CrewSlotManager crewSlotManger;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] tabButtonRects;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] tabButtonImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup myCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform panelTrans;

	public UI_ExploreSailorPopup ui_exploreSailorPopup;

	private void OnEnable()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(myCanvasGroup);
		myCanvasGroup.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 1f, 0.3f), isIndependentUpdate: true);
		panelTrans.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 1f, 0.2f), isIndependentUpdate: true), global::DG.Tweening.Ease.OutQuad);
	}

	public void OpenRect(int num)
	{
		num = global::UnityEngine.Mathf.Clamp(num, 0, rectCanvas.Length);
		if (num == prevSelectedNum)
		{
			return;
		}
		global::UnityEngine.CanvasGroup[] array = rectCanvas;
		foreach (global::UnityEngine.CanvasGroup canvas in array)
		{
			if (canvas.gameObject.activeSelf)
			{
				global::DG.Tweening.ShortcutExtensions.DOKill(canvas);
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(canvas, 0f, 0.2f), isIndependentUpdate: true), delegate
				{
					canvas.gameObject.SetActive(value: false);
				});
			}
		}
		rectCanvas[num].gameObject.SetActive(value: true);
		global::DG.Tweening.ShortcutExtensions.DOKill(rectCanvas[num]);
		rectCanvas[num].alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(rectCanvas[num], 1f, 0.2f), isIndependentUpdate: true);
		for (int num2 = 0; num2 < tabButtonRects.Length; num2++)
		{
			if (num2 == num)
			{
				global::UnityEngine.Vector2 sizeDelta = tabButtonRects[num2].sizeDelta;
				sizeDelta.y = 128f;
				tabButtonRects[num2].sizeDelta = sizeDelta;
				tabButtonImages[num2].color = myColor.GetColor(197, 214, 226);
			}
			else
			{
				global::UnityEngine.Vector2 sizeDelta2 = tabButtonRects[num2].sizeDelta;
				sizeDelta2.y = 118f;
				tabButtonRects[num2].sizeDelta = sizeDelta2;
				tabButtonImages[num2].color = myColor.GetColor(166, 190, 204);
			}
		}
		prevSelectedNum = num;
	}

	public void Refresh()
	{
		shipSlotManger.Refresh();
	}
}
