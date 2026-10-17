public class UI_IslandRewardPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform popupTrans;

	[global::UnityEngine.SerializeField]
	private IslandRewardGrid rewardGrid;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandNameText;

	private bool closeClick;

	private bool isIsland;

	public IslandRewardGrid RewardGrid => rewardGrid;

	private void OnEnable()
	{
		popupTrans.localScale = global::UnityEngine.Vector3.one * 0.4f;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(popupTrans, 1f, 0.4f), isIndependentUpdate: true), global::DG.Tweening.Ease.OutCubic);
		closeClick = false;
		isIsland = false;
	}

	public void SetIslandData(IslandStatus status)
	{
		rewardGrid.SetRewardData(status, isAddRealData: true);
		islandNameText.text = string.Format("{0} {1}", LanguageManager.Instance.translate("key_island"), status.islandId + 1);
		isIsland = true;
	}

	public void SetTitle(string title)
	{
		islandNameText.text = title;
	}

	public void Close()
	{
		if (!closeClick)
		{
			closeClick = true;
			if (rewardGrid.number != 0)
			{
				UI_Manager.Instance.ui_storeGetRewardBox.Refresh();
			}
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(popupTrans, 0.4f, 0.3f), isIndependentUpdate: true), global::DG.Tweening.Ease.Linear), delegate
			{
				global::UnityEngine.Time.timeScale = 1f;
				base.gameObject.SetActive(value: false);
			});
			if (isIsland && PlayerInfo.Instance.ClearIslandCount >= 5)
			{
				UI_Manager.Instance.RateUs();
			}
		}
	}
}
