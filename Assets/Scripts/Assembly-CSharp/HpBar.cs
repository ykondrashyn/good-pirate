public class HpBar : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image topBarImage;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.RectTransform rectTransform;

	private global::UnityEngine.Color playerColor = myColor.GetColor(0, 188, 64);

	private global::UnityEngine.Color enemyColor = myColor.GetColor(255, 0, 0);

	private global::UnityEngine.Camera mainCamera;

	private global::UnityEngine.Vector2 sizeDelta;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myNameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject disconnectedObj;

	private void Awake()
	{
		rectTransform = GetComponent<global::UnityEngine.RectTransform>();
		mainCamera = global::UnityEngine.Camera.main;
		sizeDelta = GetComponentInParent<global::UnityEngine.Canvas>().GetComponent<global::UnityEngine.RectTransform>().sizeDelta;
		if (mainCamera == null)
		{
			StartCoroutine(CameraWait());
		}
	}

	private global::System.Collections.IEnumerator CameraWait()
	{
		while (true)
		{
			mainCamera = global::UnityEngine.Camera.main;
			if (!(mainCamera != null))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public void SetType(ELayerType type)
	{
		switch (type)
		{
		case ELayerType.Enemy:
			topBarImage.color = enemyColor;
			break;
		case ELayerType.Player:
			topBarImage.color = playerColor;
			break;
		}
	}

	public void SetGauge(float ratio)
	{
		topBarImage.fillAmount = ratio;
	}

	public void SetPosition(global::UnityEngine.Vector3 pos)
	{
		if (!(mainCamera == null) && mainCamera.gameObject.activeSelf)
		{
			global::UnityEngine.Vector3 vector = mainCamera.WorldToViewportPoint(pos);
			global::UnityEngine.Vector2 anchoredPosition = new global::UnityEngine.Vector2(vector.x * sizeDelta.x, vector.y * sizeDelta.y);
			rectTransform.anchoredPosition = anchoredPosition;
		}
	}

	public void SetNameTextActive(bool active)
	{
		if ((bool)myNameText)
		{
			myNameText.gameObject.SetActive(active);
		}
	}

	public void SetNameText(string str)
	{
		if ((bool)myNameText)
		{
			myNameText.text = str;
		}
	}

	public void SetDisconnetedIconActive(bool active)
	{
		if ((bool)disconnectedObj)
		{
			disconnectedObj.SetActive(active);
		}
	}
}
