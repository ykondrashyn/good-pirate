public class UI_SpawnShipListManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject shipSrc;

	private global::System.Collections.Generic.List<UI_GeneratorSlot> spawnSlotList = new global::System.Collections.Generic.List<UI_GeneratorSlot>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] graphic;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform shipCountRectTransform;

	private global::System.Collections.Generic.List<EShipType> canSpawnShipType = new global::System.Collections.Generic.List<EShipType>();

	public void Alarm()
	{
		global::UnityEngine.UI.Graphic[] array = graphic;
		foreach (global::UnityEngine.UI.Graphic target in array)
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(target);
			global::DG.Tweening.DOTweenModuleUI.DOColor(target, global::UnityEngine.Color.red, 0.1f);
		}
		global::DG.Tweening.ShortcutExtensions.DOKill(shipCountRectTransform);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(shipCountRectTransform, -365f, 0.07f), global::DG.Tweening.Ease.Linear), delegate
		{
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(shipCountRectTransform, -350f, 0.07f), global::DG.Tweening.Ease.Linear), delegate
			{
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(shipCountRectTransform, -365f, 0.07f), global::DG.Tweening.Ease.Linear), delegate
				{
					global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(shipCountRectTransform, -350f, 0.07f), global::DG.Tweening.Ease.Linear);
					global::UnityEngine.UI.Graphic[] array2 = graphic;
					for (int j = 0; j < array2.Length; j++)
					{
						global::DG.Tweening.DOTweenModuleUI.DOColor(array2[j], global::UnityEngine.Color.white, 0.1f);
					}
				});
			});
		});
	}

	private void Awake()
	{
		canSpawnShipType.Add(EShipType.Normal);
		canSpawnShipType.Add(EShipType.Rapid);
		canSpawnShipType.Add(EShipType.Cannon);
		canSpawnShipType.Add(EShipType.Gas);
		canSpawnShipType.Add(EShipType.Guide);
		canSpawnShipType.Add(EShipType.Multi);
		for (int i = 0; i < canSpawnShipType.Count; i++)
		{
			UI_GeneratorSlot component = global::UnityEngine.Object.Instantiate(shipSrc, base.transform).GetComponent<UI_GeneratorSlot>();
			component.SetData(canSpawnShipType[i]);
			spawnSlotList.Add(component);
		}
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		foreach (UI_GeneratorSlot spawnSlot in spawnSlotList)
		{
			spawnSlot.Refresh();
		}
	}
}
