public class UI_MainMenu : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.RectTransform rectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.GridLayoutGroup grid;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject closeButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject openButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button[] buttons;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] dotImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] inImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject[] notificationObjs;

	private global::UnityEngine.Coroutine twinCoroutine;

	private void Awake()
	{
		rectTransform = base.transform as global::UnityEngine.RectTransform;
		if (grid == null)
		{
			grid = GetComponent<global::UnityEngine.UI.GridLayoutGroup>();
		}
		global::UnityEngine.UI.Button[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	private void Start()
	{
		NotificationCheck();
	}

	public void AddNotification(MainMenuNotification type)
	{
		switch (type)
		{
		case MainMenuNotification.Crew:
			PlayerInfo.Instance.myData.isHaveCrewNotification = true;
			break;
		case MainMenuNotification.Inventory:
			PlayerInfo.Instance.myData.isHaveInventoryNotification = true;
			break;
		case MainMenuNotification.Message:
			PlayerInfo.Instance.myData.isHaveMessageNotification = true;
			break;
		case MainMenuNotification.Setting:
			PlayerInfo.Instance.myData.isHaveSettingNotification = true;
			break;
		}
		NotificationCheck();
		PlayerInfo.Instance.SaveData();
	}

	public void CheckNotification(int num)
	{
		CheckNotification((MainMenuNotification)num);
	}

	public void CheckNotification(MainMenuNotification type)
	{
		switch (type)
		{
		case MainMenuNotification.Crew:
			PlayerInfo.Instance.myData.isHaveCrewNotification = false;
			break;
		case MainMenuNotification.Inventory:
			PlayerInfo.Instance.myData.isHaveInventoryNotification = false;
			break;
		case MainMenuNotification.Message:
			PlayerInfo.Instance.myData.isHaveMessageNotification = false;
			break;
		case MainMenuNotification.Setting:
			PlayerInfo.Instance.myData.isHaveSettingNotification = false;
			break;
		}
		NotificationCheck();
		PlayerInfo.Instance.SaveData();
	}

	private void NotificationCheck()
	{
		if ((bool)PlayerInfo.Instance.myData.isHaveSettingNotification)
		{
			notificationObjs[0].SetActive(value: true);
		}
		else
		{
			notificationObjs[0].SetActive(value: false);
		}
		if ((bool)PlayerInfo.Instance.myData.isHaveMessageNotification)
		{
			notificationObjs[1].SetActive(value: true);
		}
		else
		{
			notificationObjs[1].SetActive(value: false);
		}
		if ((bool)PlayerInfo.Instance.myData.isHaveInventoryNotification)
		{
			notificationObjs[2].SetActive(value: true);
		}
		else
		{
			notificationObjs[2].SetActive(value: false);
		}
		if ((bool)PlayerInfo.Instance.myData.isHaveCrewNotification)
		{
			notificationObjs[3].SetActive(value: true);
		}
		else
		{
			notificationObjs[3].SetActive(value: false);
		}
	}

	public void MenuOpen()
	{
		if (twinCoroutine != null)
		{
			StopCoroutine(twinCoroutine);
		}
		twinCoroutine = StartCoroutine(MenuTwin(isOpen: true, delegate
		{
			global::UnityEngine.UI.Button[] array = buttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}));
	}

	public void MenuClose()
	{
		if (twinCoroutine != null)
		{
			StopCoroutine(twinCoroutine);
		}
		twinCoroutine = StartCoroutine(MenuTwin(isOpen: false));
		global::UnityEngine.UI.Button[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	private global::System.Collections.IEnumerator MenuTwin(bool isOpen, global::UnityEngine.Events.UnityAction endCallback = null)
	{
		float totaltime = 0f;
		float duration = 0.5f;
		float startSize = grid.cellSize.x;
		float startSpacing = grid.spacing.x;
		float startAnchoredPosX = rectTransform.anchoredPosition.x;
		if (isOpen)
		{
			global::UnityEngine.UI.Image[] array = inImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: true);
			}
		}
		float endSpacing;
		float endSize;
		float startAlpha;
		float endAlpha;
		float endAnchoredPosX;
		if (isOpen)
		{
			endSpacing = 31f;
			endSize = 130f;
			startAlpha = 0f;
			endAlpha = 1f;
			endAnchoredPosX = 123f;
			closeButton.SetActive(value: true);
			openButton.SetActive(value: false);
		}
		else
		{
			endSpacing = 23.3f;
			endSize = 33f;
			startAlpha = 1f;
			endAlpha = 0f;
			endAnchoredPosX = 93f;
			closeButton.SetActive(value: false);
			openButton.SetActive(value: true);
		}
		while (true)
		{
			totaltime += global::UnityEngine.Time.deltaTime;
			float time = global::UnityEngine.Mathf.Clamp01(totaltime / duration);
			time = animCurve.Evaluate(time);
			float num = global::UnityEngine.Mathf.Lerp(startSize, endSize, time);
			float num2 = global::UnityEngine.Mathf.Lerp(startSpacing, endSpacing, time);
			float num3 = global::UnityEngine.Mathf.Lerp(startAlpha, endAlpha, time);
			float x = global::UnityEngine.Mathf.Lerp(startAnchoredPosX, endAnchoredPosX, time);
			global::UnityEngine.Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.x = x;
			rectTransform.anchoredPosition = anchoredPosition;
			grid.cellSize = new global::UnityEngine.Vector2(num, num);
			grid.spacing = new global::UnityEngine.Vector2(num2, num2);
			global::UnityEngine.UI.Image[] array = inImages;
			foreach (global::UnityEngine.UI.Image obj in array)
			{
				global::UnityEngine.Color color = obj.color;
				color.a = num3;
				obj.color = color;
			}
			array = dotImages;
			foreach (global::UnityEngine.UI.Image obj2 in array)
			{
				global::UnityEngine.Color color2 = obj2.color;
				color2.a = global::UnityEngine.Mathf.Lerp(0f, 0.75f, 1f - num3);
				obj2.color = color2;
			}
			if (time >= 1f)
			{
				break;
			}
			yield return null;
		}
		if (!isOpen)
		{
			global::UnityEngine.UI.Image[] array = inImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}
		endCallback?.Invoke();
	}
}
