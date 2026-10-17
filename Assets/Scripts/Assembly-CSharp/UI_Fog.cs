public class UI_Fog : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup myCanvas;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] leftFogs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] rightFogs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] topFogs;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] bottomFogs;

	private global::UnityEngine.Vector3[] leftFogsStartPos;

	private global::UnityEngine.Vector3[] rightFogsStartPos;

	private global::UnityEngine.Vector3[] topFogsStartPos;

	private global::UnityEngine.Vector3[] bottomFogsStartPos;

	private void Awake()
	{
		leftFogsStartPos = new global::UnityEngine.Vector3[leftFogs.Length];
		for (int i = 0; i < leftFogs.Length; i++)
		{
			leftFogsStartPos[i] = leftFogs[i].anchoredPosition;
		}
		rightFogsStartPos = new global::UnityEngine.Vector3[rightFogs.Length];
		for (int j = 0; j < rightFogs.Length; j++)
		{
			rightFogsStartPos[j] = rightFogs[j].anchoredPosition;
		}
		topFogsStartPos = new global::UnityEngine.Vector3[topFogs.Length];
		for (int k = 0; k < topFogs.Length; k++)
		{
			topFogsStartPos[k] = topFogs[k].anchoredPosition;
		}
		bottomFogsStartPos = new global::UnityEngine.Vector3[bottomFogs.Length];
		for (int l = 0; l < bottomFogs.Length; l++)
		{
			bottomFogsStartPos[l] = bottomFogs[l].anchoredPosition;
		}
	}

	private void OnEnable()
	{
		myCanvas.alpha = 1f;
		global::DG.Tweening.ShortcutExtensions.DOKill(base.transform);
		base.transform.localScale = global::UnityEngine.Vector3.one * 3f;
		global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, 1f, 0.7f);
		global::System.Collections.Generic.List<ESailor> skill_avoidEnvironment = SailorDataManager.Instance.skill_avoidEnvironment;
		bool flag = false;
		if (skill_avoidEnvironment.Count > 0)
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_avoidEnvironment[0]);
			if (mySailorData.isHave && mySailorData.level >= 1)
			{
				flag = true;
			}
		}
		for (int i = 0; i < leftFogs.Length; i++)
		{
			leftFogs[i].anchoredPosition = leftFogsStartPos[i] + (flag ? new global::UnityEngine.Vector3(-267f, 0f, 0f) : global::UnityEngine.Vector3.zero);
		}
		for (int j = 0; j < rightFogs.Length; j++)
		{
			rightFogs[j].anchoredPosition = rightFogsStartPos[j] + (flag ? new global::UnityEngine.Vector3(267f, 0f, 0f) : global::UnityEngine.Vector3.zero);
		}
		for (int k = 0; k < topFogs.Length; k++)
		{
			topFogs[k].anchoredPosition = topFogsStartPos[k] + (flag ? new global::UnityEngine.Vector3(0f, 267f, 0f) : global::UnityEngine.Vector3.zero);
		}
		for (int l = 0; l < bottomFogs.Length; l++)
		{
			bottomFogs[l].anchoredPosition = bottomFogsStartPos[l] + (flag ? new global::UnityEngine.Vector3(0f, -267f, 0f) : global::UnityEngine.Vector3.zero);
		}
	}

	public void Off()
	{
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(myCanvas, 0f, 0.6f), delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}
}
