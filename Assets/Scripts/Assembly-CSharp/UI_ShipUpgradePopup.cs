public class UI_ShipUpgradePopup : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] tabBtns;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup[] canvasGroups;

	private int selectNum;

	public void OpenPanel(int num)
	{
		if (selectNum == num)
		{
			return;
		}
		global::UnityEngine.CanvasGroup[] array = canvasGroups;
		foreach (global::UnityEngine.CanvasGroup canvasGroup in array)
		{
			if (canvasGroup.gameObject.activeSelf)
			{
				canvasGroup.gameObject.SetActive(value: false);
			}
		}
		canvasGroups[num].gameObject.SetActive(value: true);
		tabBtns[selectNum].targetGraphic.color = global::UnityEngine.Color.gray;
		tabBtns[num].targetGraphic.color = global::UnityEngine.Color.white;
		selectNum = num;
	}
}
