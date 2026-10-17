public class UI_WorldMap : global::UnityEngine.MonoBehaviour, global::UnityEngine.EventSystems.IPointerDownHandler, global::UnityEngine.EventSystems.IEventSystemHandler, global::UnityEngine.EventSystems.IPointerUpHandler
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private global::UnityEngine.RectTransform rectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject mapObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject dockObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject playerPositionObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject patrolPositionObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject bossPositionObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject bossClearSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject patrolPositionCoolTimeObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject destinationIconSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject playerDeadPositionSrc;

	[global::UnityEngine.SerializeField]
	private UI_WorldMapPopup worldMapPopup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform pivotTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform objectsTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform playerTrans;

	private global::UnityEngine.RectTransform playerRectTransform;

	private float offsetScale;

	private global::UnityEngine.Vector2 canvasSizeDelta;

	private const float minScale = 0.35f;

	private const float maxScale = 5f;

	private global::UnityEngine.Camera uiCamera;

	public bool isCanMove = true;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] upDoor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] upWall;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] rightDoor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] rightWall;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] downDoor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] downWall;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] leftDoor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform[] leftWall;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text upWorldText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text rightWorldText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text downWorldText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftWorldText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	public global::System.Collections.Generic.Dictionary<int, UI_WorldMapIsland> islandWorldmapDic = new global::System.Collections.Generic.Dictionary<int, UI_WorldMapIsland>();

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> worldmapIconList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nowWorldText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text rewardClearIslandCount;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image rewardBoxImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button rewardButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject rewardButtonObj;

	private global::UnityEngine.GameObject playerDeadObj;

	private global::UnityEngine.Vector3[] touchStartPos = new global::UnityEngine.Vector3[2];

	private float prevDist;

	private global::UnityEngine.Vector2 prevTouchPosition;

	private global::UnityEngine.Vector2 touchCenter;

	private global::UnityEngine.Coroutine scaleCoroutine;

	private global::UnityEngine.EventSystems.PointerEventData eventData;

	private int pointerId = -99;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform tutorialObjectPivot;

	private global::UnityEngine.GameObject cloneObj;

	public global::UnityEngine.Transform PivotTransform => pivotTrans;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		rectTransform = transform as global::UnityEngine.RectTransform;
		uiCamera = GetComponentInParent<global::UnityEngine.Camera>();
	}

	private void OnEnable()
	{
		int num = 0;
		int num2 = PlayerInfo.Instance.myData.selectedWorldNum;
		if (PlayerInfo.Instance.GetIsGotWorldReward(num2))
		{
			rewardButtonObj.SetActive(value: false);
		}
		else
		{
			rewardButtonObj.SetActive(value: true);
			int num3 = 0;
			int num4 = 0;
			switch (num2)
			{
			case 1:
				num3 = 0;
				num4 = 19;
				break;
			case 2:
				num3 = 20;
				num4 = 39;
				break;
			case 3:
				num3 = 40;
				num4 = 79;
				break;
			case 4:
				num3 = 80;
				num4 = 119;
				break;
			case 5:
				num3 = 120;
				num4 = 159;
				break;
			case 6:
				num3 = 160;
				num4 = 209;
				break;
			}
			int num5 = 0;
			for (int i = num3; i <= num4; i++)
			{
				if (PlayerInfo.Instance.GetIsClearIsland(i))
				{
					num5++;
				}
			}
			num = num4 - num3 + 1;
			nowWorldText.text = string.Format("{0} {1}", LanguageManager.Instance.translate("key_world"), num2);
			rewardClearIslandCount.text = $"({num5}/{num})";
			if (num5 >= num)
			{
				global::UnityEngine.Color color = rewardBoxImage.color;
				color.a = 1f;
				rewardBoxImage.color = color;
				color = rewardButton.targetGraphic.color;
				color.a = 1f;
				rewardButton.targetGraphic.color = color;
				rewardButton.enabled = true;
			}
			else
			{
				global::UnityEngine.Color color2 = rewardBoxImage.color;
				color2.a = 0.5f;
				rewardBoxImage.color = color2;
				color2 = rewardButton.targetGraphic.color;
				color2.a = 0.5f;
				rewardButton.targetGraphic.color = color2;
				rewardButton.enabled = false;
			}
		}
		GameManager.Instance.worldManager.DestinationIconLastSilblingCheck();
	}

	public void GetReward()
	{
		PlayerInfo.Instance.myData.storeGetRewardBoxList.Insert(0, new RewardBoxStatus(ERewardBox.BLACK, PlayerInfo.Instance.myData.mainShipLevel));
		PlayerInfo.Instance.SaveData();
		UI_Manager.Instance.ui_storeGetRewardBox.Refresh();
		PlayerInfo.Instance.SetIsGotWorldReward(PlayerInfo.Instance.myData.selectedWorldNum);
		rewardButtonObj.SetActive(value: false);
	}

	private void Start()
	{
		global::UnityEngine.RectTransform rectTransform = GetComponentInParent<global::UnityEngine.Canvas>().transform as global::UnityEngine.RectTransform;
		canvasSizeDelta = rectTransform.sizeDelta;
		offsetScale = canvasSizeDelta.x / (float)global::UnityEngine.Screen.width;
	}

	public void ClearWorldMap()
	{
		foreach (global::UnityEngine.GameObject worldmapIcon in worldmapIconList)
		{
			if (worldmapIcon != null)
			{
				global::UnityEngine.Object.Destroy(worldmapIcon);
			}
		}
		worldmapIconList.Clear();
	}

	public void SetSize(float width, float height)
	{
		float num = width / 2f;
		float num2 = height / 2f;
		upWall[0].sizeDelta = new global::UnityEngine.Vector2(30f, num - 110f + 15f);
		upWall[0].anchoredPosition = new global::UnityEngine.Vector2(num2, 110f);
		upWall[1].sizeDelta = new global::UnityEngine.Vector2(30f, num - 110f + 15f);
		upWall[1].anchoredPosition = new global::UnityEngine.Vector2(num2, -110f);
		upDoor[0].anchoredPosition = new global::UnityEngine.Vector2(num2, 80f);
		upDoor[1].anchoredPosition = new global::UnityEngine.Vector2(num2, -80f);
		downWall[0].sizeDelta = new global::UnityEngine.Vector2(30f, num - 110f + 15f);
		downWall[0].anchoredPosition = new global::UnityEngine.Vector2(num2, 110f);
		downWall[1].sizeDelta = new global::UnityEngine.Vector2(30f, num - 110f + 15f);
		downWall[1].anchoredPosition = new global::UnityEngine.Vector2(num2, -110f);
		downDoor[0].anchoredPosition = new global::UnityEngine.Vector2(num2, 80f);
		downDoor[1].anchoredPosition = new global::UnityEngine.Vector2(num2, -80f);
		rightWall[0].sizeDelta = new global::UnityEngine.Vector2(30f, num2 - 110f + 15f);
		rightWall[0].anchoredPosition = new global::UnityEngine.Vector2(num, 110f);
		rightWall[1].sizeDelta = new global::UnityEngine.Vector2(30f, num2 - 110f + 15f);
		rightWall[1].anchoredPosition = new global::UnityEngine.Vector2(num, -110f);
		rightDoor[0].anchoredPosition = new global::UnityEngine.Vector2(num, 80f);
		rightDoor[1].anchoredPosition = new global::UnityEngine.Vector2(num, -80f);
		leftWall[0].sizeDelta = new global::UnityEngine.Vector2(30f, num2 - 110f + 15f);
		leftWall[0].anchoredPosition = new global::UnityEngine.Vector2(num, 110f);
		leftWall[1].sizeDelta = new global::UnityEngine.Vector2(30f, num2 - 110f + 15f);
		leftWall[1].anchoredPosition = new global::UnityEngine.Vector2(num, -110f);
		leftDoor[0].anchoredPosition = new global::UnityEngine.Vector2(num, 80f);
		leftDoor[1].anchoredPosition = new global::UnityEngine.Vector2(num, -80f);
		upWorldText.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, num2 + 80f);
		rightWorldText.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(num, 0f);
		downWorldText.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 0f - num2 - 80f);
		leftWorldText.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f - num, 0f);
		width = global::UnityEngine.Mathf.Max(5000f, width);
		height = global::UnityEngine.Mathf.Max(5000f, height);
		objectsTrans.sizeDelta = new global::UnityEngine.Vector2(width + 1000f, height + 1000f);
		pivotTrans.sizeDelta = new global::UnityEngine.Vector2(width + 1000f, height + 1000f);
	}

	public void SetGateWorldText(Gate[] gates)
	{
		if (gates.Length >= 4)
		{
			upWorldText.text = string.Format("{1} {0}", gates[0].gateStatus.worldNum, LanguageManager.Instance.translate("key_world"));
			downWorldText.text = string.Format("{1} {0}", gates[1].gateStatus.worldNum, LanguageManager.Instance.translate("key_world"));
			leftWorldText.text = string.Format("{1} {0}", gates[2].gateStatus.worldNum, LanguageManager.Instance.translate("key_world"));
			rightWorldText.text = string.Format("{1} {0}", gates[3].gateStatus.worldNum, LanguageManager.Instance.translate("key_world"));
		}
	}

	public void GenerateDock(global::UnityEngine.Vector3 pos, global::UnityEngine.Transform trans)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(dockObjectSrc, objectsTrans);
		worldmapIconList.Add(gameObject);
		global::UnityEngine.RectTransform component = gameObject.GetComponent<global::UnityEngine.RectTransform>();
		component.anchoredPosition = new global::UnityEngine.Vector2(pos.x, pos.z);
		component.localScale = global::UnityEngine.Vector3.one * 0.2726298f;
		component.SetParent(trans);
	}

	public UI_WorldMapSameScale GenerateSameScale()
	{
		UI_WorldMapSameScale component = global::UnityEngine.Object.Instantiate(playerPositionObjectSrc, playerTrans).GetComponent<UI_WorldMapSameScale>();
		component.targetScalePivot = pivotTrans;
		return component;
	}

	public UI_WorldMapSameScale GeneratePatrol()
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(patrolPositionObjectSrc, playerTrans);
		worldmapIconList.Add(gameObject);
		UI_WorldMapSameScale component = gameObject.GetComponent<UI_WorldMapSameScale>();
		component.targetScalePivot = pivotTrans;
		return component;
	}

	public global::UnityEngine.GameObject GenerateBoss()
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(bossPositionObjectSrc, playerTrans);
		worldmapIconList.Add(gameObject);
		gameObject.GetComponentInChildren<UI_WorldMapSameScale>().targetScalePivot = pivotTrans;
		return gameObject;
	}

	public global::UnityEngine.GameObject GenerateBossClear()
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(bossClearSrc, playerTrans);
		worldmapIconList.Add(gameObject);
		gameObject.GetComponent<UI_WorldMapSameScale>().targetScalePivot = pivotTrans;
		return gameObject;
	}

	public UI_WorldMapSameScale GeneratePatrolCoolTime()
	{
		UI_WorldMapSameScale component = global::UnityEngine.Object.Instantiate(patrolPositionCoolTimeObjectSrc, playerTrans).GetComponent<UI_WorldMapSameScale>();
		component.targetScalePivot = pivotTrans;
		return component;
	}

	public UI_WorldMapSameScale GeneratePlayerDeadSign()
	{
		if (playerDeadObj != null)
		{
			global::UnityEngine.Object.Destroy(playerDeadObj);
		}
		playerDeadObj = global::UnityEngine.Object.Instantiate(playerDeadPositionSrc, playerTrans);
		UI_WorldMapSameScale component = playerDeadObj.GetComponent<UI_WorldMapSameScale>();
		component.targetScalePivot = pivotTrans;
		return component;
	}

	public global::UnityEngine.Transform GenerateObject(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, global::UnityEngine.Sprite sprite, float scale = 1f)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(mapObjectSrc, objectsTrans);
		worldmapIconList.Add(gameObject);
		global::UnityEngine.UI.Image component = gameObject.GetComponent<global::UnityEngine.UI.Image>();
		component.sprite = sprite;
		component.SetNativeSize();
		component.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(pos.x, pos.z);
		component.rectTransform.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f - rot.eulerAngles.y);
		component.rectTransform.localScale = global::UnityEngine.Vector3.one * 0.2726298f * scale;
		return component.rectTransform;
	}

	public global::UnityEngine.GameObject GenerateDestinationIcon(global::UnityEngine.Vector3 pos, global::UnityEngine.Transform trans)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(destinationIconSrc, objectsTrans);
		worldmapIconList.Add(gameObject);
		global::UnityEngine.RectTransform component = gameObject.GetComponent<global::UnityEngine.RectTransform>();
		component.anchoredPosition = new global::UnityEngine.Vector2(pos.x, pos.z);
		component.localScale = global::UnityEngine.Vector3.one * 0.2726298f;
		component.SetAsLastSibling();
		return gameObject;
	}

	public void GeneratePlayer(global::UnityEngine.Vector3 pos, global::UnityEngine.Sprite sprite)
	{
		global::UnityEngine.UI.Image component = global::UnityEngine.Object.Instantiate(mapObjectSrc, playerTrans).GetComponent<global::UnityEngine.UI.Image>();
		component.sprite = sprite;
		component.SetNativeSize();
		playerRectTransform = component.rectTransform;
		playerRectTransform.anchoredPosition = new global::UnityEngine.Vector2(pos.x, pos.z);
	}

	public void OpenSetPosition(global::UnityEngine.Vector3 playerPos)
	{
		global::UnityEngine.Vector2 vector = new global::UnityEngine.Vector2(playerPos.x, playerPos.z);
		float num = pivotTrans.sizeDelta.x * 0.5f;
		float num2 = pivotTrans.sizeDelta.y * 0.5f;
		float x = global::UnityEngine.Mathf.InverseLerp(0f - num, num, vector.x);
		float y = global::UnityEngine.Mathf.InverseLerp(0f - num2, num2, vector.y);
		pivotTrans.localScale = global::UnityEngine.Vector3.one;
		pivotTrans.pivot = new global::UnityEngine.Vector2(x, y);
		pivotTrans.anchoredPosition = global::UnityEngine.Vector3.zero;
	}

	public void SetPlayerPos(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, float angle)
	{
		if (base.gameObject.activeSelf && playerRectTransform != null)
		{
			playerRectTransform.anchoredPosition = pos;
			playerRectTransform.rotation = global::UnityEngine.Quaternion.Euler(0f, 0f, rot.eulerAngles.y);
		}
	}

	public void IslandDataPopupOn(IslandStatus status, global::UnityEngine.Vector3 dockPos)
	{
		worldMapPopup.gameObject.SetActive(value: true);
		worldMapPopup.SetData(status, dockPos);
	}

	public void IslandDataPopupOff()
	{
		worldMapPopup.gameObject.SetActive(value: false);
	}

	public void SetTouchActive(bool isActive)
	{
		isCanMove = isActive;
	}

	public void ZoomIn()
	{
		float x = pivotTrans.transform.localScale.x;
		if (x < 5f)
		{
			x = global::UnityEngine.Mathf.Clamp(x + 0.1f, 0.35f, 5f);
			SetScale(x);
		}
	}

	public void ZoomOut()
	{
		float x = pivotTrans.transform.localScale.x;
		if (x > 0.35f)
		{
			x = global::UnityEngine.Mathf.Clamp(x - 0.1f, 0.35f, 5f);
			SetScale(x);
		}
	}

	private void Update()
	{
		if (!isCanMove || worldMapPopup.gameObject.activeSelf)
		{
			return;
		}
		float x = pivotTrans.transform.localScale.x;
		for (int i = 0; i < global::UnityEngine.Input.touchCount; i++)
		{
			if (i < touchStartPos.Length && global::UnityEngine.Input.GetTouch(i).phase == global::UnityEngine.TouchPhase.Began)
			{
				touchStartPos[i] = global::UnityEngine.Input.GetTouch(i).position;
			}
		}
		if (global::UnityEngine.Input.touchCount == 2)
		{
			prevTouchPosition = global::UnityEngine.Vector3.zero;
			global::UnityEngine.Vector2 position = global::UnityEngine.Input.touches[0].position;
			global::UnityEngine.Vector2 position2 = global::UnityEngine.Input.touches[1].position;
			if (prevDist == 0f)
			{
				touchCenter = position + (position2 - position) * 0.5f;
				WorldMapPivotChange(touchCenter);
				float magnitude = (position - position2).magnitude;
				prevDist = magnitude;
			}
			if (global::UnityEngine.Input.GetTouch(0).phase == global::UnityEngine.TouchPhase.Moved || global::UnityEngine.Input.GetTouch(1).phase == global::UnityEngine.TouchPhase.Moved)
			{
				float magnitude2 = (position - position2).magnitude;
				float num = magnitude2 - prevDist;
				if (num > global::UnityEngine.Mathf.Epsilon || num < 0f - global::UnityEngine.Mathf.Epsilon)
				{
					x = global::UnityEngine.Mathf.Clamp(x + num * 0.002f, 0.35f, 5f);
					if (num < 0f)
					{
						if (x < 5f)
						{
							SetScale(x);
						}
					}
					else if (x > 0.35f)
					{
						SetScale(x);
					}
				}
				prevDist = magnitude2;
			}
			if (global::UnityEngine.Input.GetTouch(0).phase == global::UnityEngine.TouchPhase.Ended || global::UnityEngine.Input.GetTouch(1).phase == global::UnityEngine.TouchPhase.Ended)
			{
				PivotReset();
				prevTouchPosition = global::UnityEngine.Vector2.zero;
				prevDist = 0f;
			}
		}
		else if (global::UnityEngine.Input.touchCount == 1 && eventData != null)
		{
			global::UnityEngine.Touch touch = global::UnityEngine.Input.GetTouch(0);
			switch (global::UnityEngine.Input.GetTouch(0).phase)
			{
			case global::UnityEngine.TouchPhase.Began:
				Began(touch.position);
				break;
			case global::UnityEngine.TouchPhase.Ended:
				End(touch.position);
				break;
			case global::UnityEngine.TouchPhase.Moved:
				Move(touch.position);
				break;
			case global::UnityEngine.TouchPhase.Stationary:
				break;
			}
		}
	}

	private void WorldMapPivotChange(global::UnityEngine.Vector2 screenPos)
	{
		if (global::UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(objectsTrans, screenPos, uiCamera, out var localPoint))
		{
			float x = global::UnityEngine.Mathf.Clamp01((localPoint.x + pivotTrans.sizeDelta.x * 0.5f) / pivotTrans.sizeDelta.x);
			float y = global::UnityEngine.Mathf.Clamp01((localPoint.y + pivotTrans.sizeDelta.y * 0.5f) / pivotTrans.sizeDelta.y);
			global::UnityEngine.Vector2 pivot = pivotTrans.pivot;
			global::UnityEngine.Vector2 vector = new global::UnityEngine.Vector2(x, y);
			global::UnityEngine.Vector2 vector2 = vector - pivot;
			float x2 = pivotTrans.sizeDelta.x * vector2.x;
			float y2 = pivotTrans.sizeDelta.y * vector2.y;
			global::UnityEngine.Vector2 vector3 = new global::UnityEngine.Vector2(x2, y2) * pivotTrans.localScale.x;
			pivotTrans.pivot = vector;
			pivotTrans.anchoredPosition += vector3;
		}
	}

	private void SetScale(float scale)
	{
		if (scaleCoroutine != null)
		{
			StopCoroutine(scaleCoroutine);
			scaleCoroutine = null;
		}
		scaleCoroutine = StartCoroutine(ScaleCoroutine(scale));
	}

	private global::System.Collections.IEnumerator ScaleCoroutine(float scale)
	{
		float startScale = pivotTrans.localScale.x;
		float totalTime = 0f;
		float duration = 0.15f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float time = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			time = animCurve.Evaluate(time);
			pivotTrans.localScale = global::UnityEngine.Vector3.one * global::UnityEngine.Mathf.Lerp(startScale, scale, time);
			WorldMapPositionClamp();
			yield return null;
		}
	}

	private void PivotReset()
	{
		float x = 0.5f;
		float y = 0.5f;
		global::UnityEngine.Vector2 pivot = pivotTrans.pivot;
		global::UnityEngine.Vector2 vector = new global::UnityEngine.Vector2(x, y);
		global::UnityEngine.Vector2 vector2 = vector - pivot;
		float x2 = pivotTrans.sizeDelta.x * vector2.x;
		float y2 = pivotTrans.sizeDelta.y * vector2.y;
		global::UnityEngine.Vector2 vector3 = new global::UnityEngine.Vector2(x2, y2) * pivotTrans.localScale.x;
		pivotTrans.pivot = vector;
		pivotTrans.anchoredPosition += vector3;
	}

	private void Began(global::UnityEngine.Vector2 screenPos)
	{
		prevTouchPosition = screenPos;
	}

	private void Move(global::UnityEngine.Vector2 screenPos)
	{
		if (prevTouchPosition == global::UnityEngine.Vector2.zero)
		{
			prevTouchPosition = screenPos;
			return;
		}
		_ = pivotTrans.localScale;
		global::UnityEngine.Vector2 vector = (screenPos - prevTouchPosition) * offsetScale;
		global::UnityEngine.Vector2 anchoredPosition = pivotTrans.anchoredPosition;
		anchoredPosition += vector;
		pivotTrans.anchoredPosition = anchoredPosition;
		WorldMapPositionClamp();
		prevTouchPosition = screenPos;
	}

	private void WorldMapPositionClamp()
	{
		float x = pivotTrans.localScale.x;
		float num = (0.5f - pivotTrans.pivot.x) * pivotTrans.sizeDelta.x * x;
		float num2 = (0.5f - pivotTrans.pivot.y) * pivotTrans.sizeDelta.y * x;
		float max = objectsTrans.sizeDelta.x * 0.5f * x - canvasSizeDelta.x / 2f - num;
		float min = (0f - objectsTrans.sizeDelta.x) * 0.5f * x + canvasSizeDelta.x / 2f - num;
		float num3 = objectsTrans.sizeDelta.y * 0.5f * x - canvasSizeDelta.y / 2f - num2;
		float num4 = (0f - objectsTrans.sizeDelta.y) * 0.5f * x + canvasSizeDelta.y / 2f - num2;
		if (num3 < num4)
		{
			num3 = 0f - num2;
			num4 = 0f - num2;
		}
		global::UnityEngine.Vector2 anchoredPosition = pivotTrans.anchoredPosition;
		anchoredPosition = new global::UnityEngine.Vector2(global::UnityEngine.Mathf.Clamp(anchoredPosition.x, min, max), global::UnityEngine.Mathf.Clamp(anchoredPosition.y, num4, num3));
		pivotTrans.anchoredPosition = anchoredPosition;
	}

	private void End(global::UnityEngine.Vector2 screenPos)
	{
		prevTouchPosition = global::UnityEngine.Vector3.zero;
	}

	private void ZoomIn(global::UnityEngine.Vector2 screen)
	{
		global::UnityEngine.Vector3 vector = global::UnityEngine.Camera.main.ScreenToViewportPoint(screen);
		float x = global::UnityEngine.Mathf.Lerp(-0.5f, 0.5f, vector.x) * canvasSizeDelta.x;
		float y = global::UnityEngine.Mathf.Lerp(-0.5f, 0.5f, vector.y) * canvasSizeDelta.y;
		global::UnityEngine.Vector2 vector2 = new global::UnityEngine.Vector2(x, y);
		vector2 *= 0.5f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(pivotTrans, -vector2, 0.1f), isRelative: true), delegate
		{
			WorldMapPositionClamp();
		});
	}

	private void ZoomOut(global::UnityEngine.Vector2 screen)
	{
		global::UnityEngine.Vector3 vector = global::UnityEngine.Camera.main.ScreenToViewportPoint(screen);
		float x = global::UnityEngine.Mathf.Lerp(-0.5f, 0.5f, vector.x) * canvasSizeDelta.x;
		float y = global::UnityEngine.Mathf.Lerp(-0.5f, 0.5f, vector.y) * canvasSizeDelta.y;
		global::UnityEngine.Vector2 endValue = new global::UnityEngine.Vector2(x, y);
		endValue *= 0.5f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetRelative(global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(pivotTrans, endValue, 0.1f), isRelative: true), delegate
		{
			WorldMapPositionClamp();
		});
	}

	public void OnPointerDown(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (pointerId == -99)
		{
			this.eventData = eventData;
			pointerId = eventData.pointerId;
		}
	}

	public void OnPointerUp(global::UnityEngine.EventSystems.PointerEventData eventData)
	{
		if (eventData.pointerId == pointerId)
		{
			this.eventData = null;
			pointerId = -99;
		}
	}

	public void GenerateTutorialIslandTarget()
	{
		if (islandWorldmapDic.ContainsKey(1))
		{
			UI_WorldMapIsland uI_WorldMapIsland = islandWorldmapDic[1];
			cloneObj = global::UnityEngine.Object.Instantiate(uI_WorldMapIsland.gameObject, uI_WorldMapIsland.transform.parent);
			UI_WorldMapIsland component = cloneObj.GetComponent<UI_WorldMapIsland>();
			component.transform.parent = tutorialObjectPivot;
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.TweenSettingsExtensions.SetLoops(global::DG.Tweening.ShortcutExtensions.DOScale(component.transform, component.transform.localScale.x * 1.06f, 0.4f), -1, global::DG.Tweening.LoopType.Yoyo), isIndependentUpdate: true), global::DG.Tweening.Ease.Linear);
			component.myBtn.enabled = true;
			component.myBtn.onClick.AddListener(delegate
			{
				TutorialManager.Instance.Next();
			});
			global::UnityEngine.RectTransform component2 = component.GetComponent<global::UnityEngine.RectTransform>();
			global::UnityEngine.Vector3 anchoredPosition3D = component2.anchoredPosition3D;
			anchoredPosition3D.z = 0f;
			component2.anchoredPosition3D = anchoredPosition3D;
			component.island = uI_WorldMapIsland.island;
		}
	}

	public void TutorialNext()
	{
		if ((bool)cloneObj)
		{
			global::UnityEngine.Object.Destroy(cloneObj);
		}
	}
}
