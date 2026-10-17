public class TutorialSceneManager : global::UnityEngine.MonoBehaviour
{
	public static TutorialSceneManager Instance;

	public WorldMaker worldMaker;

	public WorldManager worldManager;

	public Docking anchoredDock;

	public FollowTarget followTarget;

	public bool IsInIsland;

	public ShipBase playerShip { get; private set; }

	public ShipController playerShipController { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		BGMManager.Instance.masterVolume = 0.6f;
		BGMManager.Instance.PlayBGM("wave_loop_freesound");
		if (worldMaker != null && worldManager != null)
		{
			worldManager.GenerateWorld(0, 0);
		}
		ShipBase ship = null;
		ShipController shipController = null;
		ShipGenerator.Instance.SpawnMain(global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.Euler(0f, 135f, 0f), out ship, out shipController);
		if (ship != null)
		{
			playerShip = ship;
		}
		if (shipController != null)
		{
			playerShipController = shipController;
		}
		Island islandData = worldManager.GetIslandData(9999);
		if (islandData != null)
		{
			InIsland(islandData.myDock);
		}
		else
		{
			UI_Manager.Instance.TouchPanelActive(active: false);
			IsInIsland = false;
		}
		worldManager.GetIndex(playerShip.transform.position, out var indexX, out var indexZ);
		worldManager.Check(indexX, indexZ);
		ShipMainData mainData = ShipDataManager.Instance.GetMainData(EShipMain.LEVEL_10);
		playerShip.SetLevel(PlayerInfo.Instance.myData.mainShipLevel);
		playerShip.SpawnShipModel(mainData.modelSrc, ELayerType.Player);
	}

	public void InIsland(Docking dock)
	{
		StopCoroutine("CheckFarToIsland");
		global::UnityEngine.Camera.main.orthographicSize = 45f;
		UI_Manager.Instance.UIActive(active: false);
		UI_Manager.Instance.ui_mainGame.SetIslandName(dock.myIsland.status.nameText);
		anchoredDock = dock;
		anchoredDock.DockingActive(active: false);
		IsInIsland = true;
		followTarget.IslandCameraSetting(anchoredDock.transform);
		ShipController playerController = playerShipController;
		playerController.SetAutoDriveMode(isActive: false);
		playerController.rigidbody.velocity = global::UnityEngine.Vector3.zero;
		playerController.rigidbody.angularVelocity = global::UnityEngine.Vector3.zero;
		playerController.transform.rotation = global::UnityEngine.Quaternion.Euler(0f, anchoredDock.transform.rotation.eulerAngles.y + 180f, 0f);
		TouchPad.Instance.ForceTouchUp();
		global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(-22f, -2f, -22f);
		global::UnityEngine.Vector3 vector2 = anchoredDock.transform.TransformVector(vector) + anchoredDock.transform.position;
		global::UnityEngine.Vector3 position = vector2 + anchoredDock.transform.forward * 10f;
		playerController.transform.position = position;
		playerController.SubShipsTeleport(0.6f);
		foreach (ShipSubController subShip in playerController.subShipList)
		{
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOMove(subShip.transform, anchoredDock.transform.forward * -10f, 3f), isIndependentUpdate: true), isRelative: true), global::DG.Tweening.Ease.OutQuad);
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOMove(playerController.transform, vector2, 3f), isIndependentUpdate: true), global::DG.Tweening.Ease.OutQuad), delegate
		{
			playerController.InIsland();
		});
		UI_Manager.Instance.BlackScreenOff(0.6f, delegate
		{
			UI_Manager.Instance.UIActiveFade(active: true);
			UI_Manager.Instance.ui_mainGame.UIMode(EUIMode.InIsland);
		});
		PlayerInfo.Instance.myData.selectedIslandId = dock.myIsland.status.islandId;
		PlayerInfo.Instance.SaveData();
	}

	public void OutIsland()
	{
		StartCoroutine(OutIslandControlPlayerShip());
		playerShipController.OutIsland();
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DORotate(followTarget.transform, global::UnityEngine.Vector3.zero, 5f), global::DG.Tweening.Ease.OutQuart), delegate
		{
			UI_Manager.Instance.TouchPanelActive(active: true);
		});
		followTarget.isSmooth = true;
		followTarget.isPause = false;
	}

	private global::System.Collections.IEnumerator OutIslandControlPlayerShip()
	{
		ShipController shipController = playerShipController;
		global::DG.Tweening.ShortcutExtensions.DOKill(shipController.transform);
		global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOMove(shipController.transform, playerShip.transform.forward * 28f, 6f), global::DG.Tweening.Ease.InOutQuad), isRelative: true);
		IsInIsland = false;
		yield return new global::UnityEngine.WaitForSeconds(6f);
		if (anchoredDock != null)
		{
			anchoredDock.DockingActive(active: true);
		}
		UI_Manager.Instance.UIActiveFade(active: true);
	}

	public void TutorialOver()
	{
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
