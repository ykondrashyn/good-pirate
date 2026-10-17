public class UI_ShipSelectSailorPopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup myCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform panelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image shipIconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject spawnCompleteObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text shipNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text hpText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text attackText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text cannonText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text speedText;

	[global::UnityEngine.SerializeField]
	private UI_CrewSelectSlotManager crewSelectSlotManager;

	private global::System.Collections.Generic.Queue<MyShipInventory> shipQueue = new global::System.Collections.Generic.Queue<MyShipInventory>();

	public MyShipInventory selectedShip { get; private set; }

	private void OnEnable()
	{
		Open();
	}

	private void Open()
	{
		global::UnityEngine.Time.timeScale = 0f;
		global::DG.Tweening.ShortcutExtensions.DOKill(myCanvasGroup);
		myCanvasGroup.alpha = 0f;
		global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 1f, 0.2f), isIndependentUpdate: true);
		panelTrans.localScale = global::UnityEngine.Vector3.one * 0.6f;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 1f, 0.3f), isIndependentUpdate: true), global::DG.Tweening.Ease.OutQuad);
	}

	public void SelectShip(MyShipInventory select, bool isSpawn = false)
	{
		if (selectedShip != null)
		{
			shipQueue.Enqueue(select);
			return;
		}
		selectedShip = select;
		if (selectedShip.equipSailerId[0] != -1)
		{
			crewSelectSlotManager.MakeEquippedSlot(selectedShip);
		}
		int shipLevel = PlayerInfo.Instance.GetShipLevel(selectedShip.shipType);
		ShipBaseData shipBaseData = ShipDataManager.Instance.GetShipBaseData(selectedShip.shipType);
		int level = global::UnityEngine.Mathf.Clamp(shipLevel / 100 + 1, 1, 10);
		ShipSubData subShipData = ShipDataManager.Instance.GetSubShipData(selectedShip.shipType, (EShipSub)level);
		if (subShipData == null || subShipData.modelSrc == null)
		{
			subShipData = ShipDataManager.Instance.GetSubShipData(selectedShip.shipType, EShipSub.LEVEL_01);
		}
		shipNameText.text = $"{shipBaseData.shipName} Lv.{shipLevel}";
		shipIconImage.sprite = subShipData.icon;
		spawnCompleteObj.SetActive(isSpawn);
	}

	public void ClosePopup()
	{
		selectedShip = null;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(panelTrans, 0.6f, 0.4f), isIndependentUpdate: true), global::DG.Tweening.Ease.InQuad);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvasGroup, 0f, 0.2f), isIndependentUpdate: true), 0.2f), delegate
		{
			if (shipQueue.Count > 0)
			{
				MyShipInventory myShipInventory = shipQueue.Dequeue();
				SelectShip(myShipInventory);
				Open();
			}
			else
			{
				global::UnityEngine.Time.timeScale = 1f;
				base.gameObject.SetActive(value: false);
			}
		});
	}

	public void Refresh()
	{
		UI_Manager.Instance.ui_mainGame.ui_popupCrewShip.Refresh();
	}

	public void SelectSailor(MySailorInventory select)
	{
	}

	public void UnEquipped(MySailorInventory select)
	{
	}
}
