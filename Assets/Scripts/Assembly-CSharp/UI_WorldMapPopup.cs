public class UI_WorldMapPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image islandImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image dockImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite dockEnemy;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite dockPlayer;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform islandDockRecttransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandLevelText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text distanceText;

	[global::UnityEngine.SerializeField]
	private IslandRewardGrid rewardGrid;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject setDestinationObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject cancelDestinationObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject teleportBtnObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject teleportCantObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text[] teleportPriceText;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> rewardSlotList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	private IslandStatus selectedIsland;

	private int TeleportPrice
	{
		get
		{
			Island islandData = GameManager.Instance.worldManager.GetIslandData(selectedIsland.islandId);
			int num = 3;
			int num2 = 2;
			global::UnityEngine.Vector3 vector = GameManager.Instance.playerShip.transform.position - islandData.myDock.transform.position;
			float num3 = 300f;
			float magnitude = vector.magnitude;
			switch (GameManager.Instance.worldManager.selectedWorldData.worldNum)
			{
			case 1:
				num3 = 300f;
				break;
			case 2:
				num3 = 400f;
				break;
			case 3:
				num3 = 500f;
				break;
			case 4:
				num3 = 550f;
				break;
			case 5:
				num3 = 600f;
				break;
			case 6:
				num3 = 600f;
				break;
			}
			return (int)((float)num2 * (magnitude / num3)) + num;
		}
	}

	public void TelePort()
	{
		Island island = GameManager.Instance.worldManager.GetIslandData(selectedIsland.islandId);
		int teleportPrice = TeleportPrice;
		if (teleportPrice <= (long)PlayerInfo.Instance.myData.diamond)
		{
			AudioManager.Instance.PlaySFX("use_diamond");
			UI_Manager.Instance.BlackScreenOn(0.8f, delegate
			{
				GameManager.Instance.InIsland(island.myDock);
			});
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, -teleportPrice);
			GetComponent<PopupAction>().Close();
			UI_Manager.Instance.WorldMapSetActive(active: false);
			GoogleAnalyticsV3.instance.LogEvent("다이아 사용", "섬 즉시이동", "섬 즉시이동", 1L);
		}
	}

	public void SetData(IslandStatus status, global::UnityEngine.Vector3 dockPos)
	{
		for (int i = 0; i < rewardSlotList.Count; i++)
		{
			global::UnityEngine.Object.Destroy(rewardSlotList[i]);
		}
		rewardSlotList.Clear();
		selectedIsland = status;
		rewardGrid.SetRewardData(selectedIsland);
		IslandData islandData = IslandDataManager.Instance.GetIslandData(selectedIsland.islandType);
		islandImage.sprite = islandData.minimapIcon;
		islandImage.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f - selectedIsland.islandRotation.eulerAngles.y);
		islandImage.SetNativeSize();
		Island islandData2 = GameManager.Instance.worldManager.GetIslandData(selectedIsland.islandId);
		islandDockRecttransform.anchoredPosition = dockPos;
		if (PlayerInfo.Instance.GetIsClearIsland(selectedIsland.islandId) && ((int)PlayerInfo.Instance.myData.selectedIslandId != selectedIsland.islandId || !GameManager.Instance.IsInIsland))
		{
			teleportBtnObj.SetActive(value: true);
			teleportCantObj.SetActive(value: false);
			int teleportPrice = TeleportPrice;
			global::UnityEngine.UI.Text[] array = teleportPriceText;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].text = teleportPrice.ToString();
			}
		}
		else
		{
			teleportBtnObj.SetActive(value: false);
			teleportCantObj.SetActive(value: true);
		}
		islandNameText.text = string.Format("{0} {1}", LanguageManager.Instance.translate("key_island"), selectedIsland.islandId + 1);
		_ = EnemySpawnManager.Instance.islandSheet.dataArray[selectedIsland.islandId].Enemylevelmin;
		_ = EnemySpawnManager.Instance.islandSheet.dataArray[selectedIsland.islandId].Enemylevelmax;
		if (PlayerInfo.Instance.myData.destinationIslandId == islandData2.status.islandId)
		{
			cancelDestinationObj.SetActive(value: true);
			setDestinationObj.SetActive(value: false);
		}
		else
		{
			cancelDestinationObj.SetActive(value: false);
			setDestinationObj.SetActive(value: true);
		}
	}

	public void SetDestination()
	{
		Island islandData = GameManager.Instance.worldManager.GetIslandData(selectedIsland.islandId);
		GameManager.Instance.playerShipController.SetDestination(islandData);
		PlayerInfo.Instance.myData.destinationIslandId = islandData.status.islandId;
		GameManager.Instance.worldManager.RefreshDestination(islandData);
		GetComponent<PopupAction>().Close();
		cancelDestinationObj.SetActive(value: true);
		setDestinationObj.SetActive(value: false);
		PlayerInfo.Instance.SaveData();
	}

	public void CancelDestination()
	{
		PlayerInfo.Instance.myData.destinationIslandId = -1;
		GameManager.Instance.playerShipController.SetDestination(null);
		GameManager.Instance.worldManager.RefreshDestination(null);
		GetComponent<PopupAction>().Close();
		PlayerInfo.Instance.SaveData();
	}
}
