public class UI_IslandStatus : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image icon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftCountText;

	private Island selectedIsland;

	private int enemyMaxCount;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup rewardPopCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text rewardPriceText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image otherRewardImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite diaSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite crewSprite;

	private IslandSheetData sheetData;

	private void Awake()
	{
		if (sheetData == null)
		{
			sheetData = global::UnityEngine.Resources.Load<IslandSheetData>("Data/IslandSheetData");
		}
	}

	private void OnEnable()
	{
		rewardPopCanvas.gameObject.SetActive(value: true);
		global::DG.Tweening.ShortcutExtensions.DOKill(rewardPopCanvas);
		rewardPopCanvas.alpha = 1f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOFade(rewardPopCanvas, 0f, 3f), 5f), delegate
		{
			rewardPopCanvas.gameObject.SetActive(value: false);
		});
		if (UI_Manager.Instance.ui_mainGame.ui_chestPanel.IsOpen)
		{
			UI_Manager.Instance.ui_mainGame.ui_chestPanel.Close();
		}
	}

	public void SetIsland(Island island)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (sheetData == null)
		{
			sheetData = global::UnityEngine.Resources.Load<IslandSheetData>("Data/IslandSheetData");
		}
		selectedIsland = island;
		islandNameText.text = string.Format("{0} {1}", LanguageManager.Instance.translate("key_island"), selectedIsland.status.islandId + 1);
		enemyMaxCount = 0;
		global::System.Collections.Generic.List<IslandSpawnData> spawnList = selectedIsland.status.spawnList;
		for (int i = 0; i < spawnList.Count; i++)
		{
			enemyMaxCount += spawnList[i].count;
		}
		leftCountText.text = $"{selectedIsland.GetCurrentEnemyCount} / {enemyMaxCount}";
		IslandSheetDataData islandSheetDataData = sheetData.dataArray[selectedIsland.status.islandId];
		rewardPriceText.text = Numbering.Convert(islandSheetDataData.Gold);
		if (islandSheetDataData.Diamond == 0)
		{
			otherRewardImage.sprite = crewSprite;
			otherRewardImage.SetNativeSize();
			if (islandSheetDataData.Skillcrewid != 0)
			{
				otherRewardImage.color = myColor.GetColor(196, 0, 29);
				return;
			}
			switch (islandSheetDataData.Crewgrade)
			{
			case 0:
				otherRewardImage.color = myColor.GetColor(255, 197, 0);
				break;
			case 1:
				otherRewardImage.color = global::UnityEngine.Color.white;
				break;
			case 2:
				otherRewardImage.color = myColor.GetColor(255, 197, 0);
				break;
			case 3:
				otherRewardImage.color = global::UnityEngine.Color.white;
				break;
			}
		}
		else
		{
			otherRewardImage.sprite = diaSprite;
			otherRewardImage.color = global::UnityEngine.Color.white;
			otherRewardImage.SetNativeSize();
		}
	}

	public void RefreshLeftEnemyCount()
	{
		if (selectedIsland != null)
		{
			leftCountText.text = $"{selectedIsland.GetCurrentEnemyCount} / {enemyMaxCount}";
		}
	}
}
