public class UI_WorldMapIsland : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public Island island;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button myButton;

	private global::UnityEngine.RectTransform myButtonRect;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Transform targetScalePivot;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image speechImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image claimImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.RawImage flagImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text islandName;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite redDockSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite greenDockSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite claimClear;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite claimNotClear;

	public global::UnityEngine.UI.Button myBtn;

	private float startScale;

	private float prevScale;

	private void Awake()
	{
		myButtonRect = myButton.GetComponent<global::UnityEngine.RectTransform>();
	}

	public void SetData(Island data)
	{
		island = data;
		myButton.gameObject.SetActive(value: true);
		if (myButtonRect == null)
		{
			myButtonRect = myButton.GetComponent<global::UnityEngine.RectTransform>();
		}
		islandName.gameObject.SetActive(value: true);
		islandName.text = string.Format("{0} {1}", LanguageManager.Instance.translate("key_island"), data.status.islandId + 1);
		islandName.transform.parent = base.transform.parent;
		islandName.transform.localRotation = global::UnityEngine.Quaternion.identity;
		global::UnityEngine.Vector2 vector = new global::UnityEngine.Vector2(data.status.islandPosition.x, data.status.islandPosition.z);
		islandName.rectTransform.anchoredPosition = vector + new global::UnityEngine.Vector2(0f, -170f);
		islandName.rectTransform.parent = base.transform;
		islandName.transform.localScale = global::UnityEngine.Vector3.one;
		global::UnityEngine.UI.Image[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.UI.Image>();
		myBtn.enabled = true;
		if (PlayerInfo.Instance.GetIsClearIsland(island.status.islandId))
		{
			speechImage.gameObject.SetActive(value: true);
			flagImage.color = global::UnityEngine.Color.white;
			if (FlagManager.Instance.GetFlagInfo(PlayerInfo.Instance.myData.selectedFlagId, out var info))
			{
				flagImage.texture = info.flagTexture;
			}
			if (island.status.selectedDockNumber != -1)
			{
				componentsInChildren[componentsInChildren.Length - 1].sprite = greenDockSprite;
			}
			if (PlayerInfo.Instance.GetIsClearIslandClaim(island.status.islandId))
			{
				claimImage.sprite = claimClear;
			}
			else
			{
				claimImage.sprite = claimNotClear;
			}
		}
		else
		{
			if (island.status.selectedDockNumber != -1)
			{
				componentsInChildren[componentsInChildren.Length - 1].sprite = redDockSprite;
			}
			speechImage.gameObject.SetActive(value: false);
		}
		if (!UI_Manager.Instance.ui_worldMap.islandWorldmapDic.ContainsKey(island.status.islandId))
		{
			UI_Manager.Instance.ui_worldMap.islandWorldmapDic.Add(island.status.islandId, this);
		}
	}

	private void OnEnable()
	{
		if (island != null)
		{
			if (PlayerInfo.Instance.GetIsClearIsland(island.status.islandId))
			{
				speechImage.color = global::UnityEngine.Color.white;
				flagImage.color = global::UnityEngine.Color.white;
				if (FlagManager.Instance.GetFlagInfo(PlayerInfo.Instance.myData.selectedFlagId, out var info))
				{
					flagImage.texture = info.flagTexture;
				}
				if (PlayerInfo.Instance.GetIsClearIslandClaim(island.status.islandId))
				{
					claimImage.sprite = claimClear;
				}
				else
				{
					claimImage.sprite = claimNotClear;
				}
			}
			myBtn.enabled = true;
			islandName.gameObject.SetActive(value: true);
		}
		else
		{
			myBtn.enabled = false;
			islandName.gameObject.SetActive(value: false);
		}
		StartCoroutine(WaitEndFrame());
	}

	public void OpenPopup()
	{
		global::UnityEngine.UI.Image[] componentsInChildren = GetComponentsInChildren<global::UnityEngine.UI.Image>();
		UI_Manager.Instance.ui_worldMap.IslandDataPopupOn(island.status, componentsInChildren[componentsInChildren.Length - 1].rectTransform.anchoredPosition);
	}

	private global::System.Collections.IEnumerator WaitEndFrame()
	{
		yield return new global::UnityEngine.WaitForEndOfFrame();
		if (myButton.gameObject.activeSelf)
		{
			myButton.transform.SetParent(base.transform.parent);
			myButton.transform.SetAsLastSibling();
			myButton.transform.localRotation = global::UnityEngine.Quaternion.identity;
			global::UnityEngine.Vector3 position = island.myDock.transform.position;
			myButtonRect.anchoredPosition = new global::UnityEngine.Vector2(position.x, position.z + 130f);
		}
	}

	private void OnDestroy()
	{
		if (myButton != null)
		{
			global::UnityEngine.Object.Destroy(myButton.gameObject);
		}
	}

	private void Start()
	{
		if (island != null)
		{
			startScale = myButtonRect.localScale.x;
		}
	}
}
