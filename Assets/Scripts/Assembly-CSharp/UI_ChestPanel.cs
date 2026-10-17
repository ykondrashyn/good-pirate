public class UI_ChestPanel : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.GridLayoutGroup grid;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button myBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<UI_ItemRewardBox> itemRewardBox;

	[global::UnityEngine.SerializeField]
	private UI_ItemRewardBox freeBox;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lockSlotObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform chestTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image[] chestImages;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image outLineImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image iconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite boxSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite triArrowSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject signObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject fullTextObj;

	public bool IsOpen { get; private set; }

	private void Awake()
	{
		myBtn.onClick.RemoveAllListeners();
		myBtn.onClick.AddListener(Open);
		for (int i = 0; i < PlayerInfo.Instance.myData.rewardBoxStatus.Length; i++)
		{
			if (PlayerInfo.Instance.myData.rewardBoxStatus[i] != null)
			{
				itemRewardBox[i].SetData(PlayerInfo.Instance.myData.rewardBoxStatus[i]);
			}
		}
		CheckHaveThridBox();
	}

	public void CheckOverCoolTime()
	{
		bool flag = false;
		foreach (UI_ItemRewardBox item in itemRewardBox)
		{
			if (item.myData != null && item.myData.boxType != ERewardBox.NONE && CoolTimeManager.GetLeftCoolTime(item.MyCoolTimeKey).TotalSeconds <= 0.0)
			{
				flag = true;
				break;
			}
		}
		if (freeBox != null && CoolTimeManager.GetLeftCoolTime(freeBox.MyCoolTimeKey).TotalSeconds <= 0.0)
		{
			flag = true;
		}
		if (flag)
		{
			outLineImage.color = myColor.GetColor(255, 0, 66);
			if (!signObj.activeSelf)
			{
				AudioManager.Instance.PlaySFX("ui_complete_rewardbox");
				signObj.SetActive(value: true);
			}
		}
		else
		{
			outLineImage.color = global::UnityEngine.Color.white;
			if (signObj.activeSelf)
			{
				signObj.SetActive(value: false);
			}
		}
	}

	public void CheckHaveThridBox()
	{
		if (PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot)
		{
			lockSlotObj.SetActive(value: false);
			itemRewardBox[2].gameObject.SetActive(value: true);
		}
		else
		{
			lockSlotObj.SetActive(value: true);
			itemRewardBox[2].gameObject.SetActive(value: false);
		}
	}

	public void Open()
	{
		IsOpen = true;
		canvasGroup.gameObject.SetActive(value: true);
		canvasGroup.alpha = 0f;
		global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
		global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.3f);
		iconImage.sprite = triArrowSprite;
		iconImage.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(-8f, 0f);
		StopCoroutine("SpacingClose");
		StartCoroutine("SpacingOpen");
		myBtn.onClick.RemoveAllListeners();
		myBtn.onClick.AddListener(Close);
		if (GameManager.Instance != null && GameManager.Instance.playerShip.myHealth.HpRatio < 1f)
		{
			UI_Manager.Instance.ui_mainGame.healSpeech.Inactive();
		}
		if (fullTextObj.activeSelf)
		{
			fullTextObj.SetActive(value: false);
		}
	}

	private global::System.Collections.IEnumerator SpacingOpen()
	{
		float totalTime = 0f;
		float duration = 0.3f;
		float startX = -156f;
		float endX = 16.5f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float time = totalTime / duration;
			time = animCurve.Evaluate(time);
			float x = global::UnityEngine.Mathf.Lerp(startX, endX, time);
			global::UnityEngine.Vector2 spacing = grid.spacing;
			spacing.x = x;
			grid.spacing = spacing;
			if (!(time > 1f))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public void Alarm(bool soundActive)
	{
		if (soundActive)
		{
			AudioManager.Instance.PlaySFX("error");
		}
		global::UnityEngine.UI.Image[] array = chestImages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = new global::UnityEngine.Color(1f, 0f, 0f);
		}
		float durationTime = 0.05f;
		if (!IsOpen)
		{
			fullTextObj.SetActive(value: true);
		}
		global::DG.Tweening.ShortcutExtensions.DOKill(chestTrans);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(chestTrans, 15f, durationTime), delegate
		{
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(chestTrans, -6f, durationTime), delegate
			{
				global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(chestTrans, 15f, durationTime), delegate
				{
					global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(chestTrans, -6f, durationTime), delegate
					{
						global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(chestTrans, 15f, durationTime), delegate
						{
							global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(chestTrans, 0f, durationTime), delegate
							{
								if (fullTextObj.activeSelf)
								{
									fullTextObj.SetActive(value: false);
								}
								global::UnityEngine.UI.Image[] array2 = chestImages;
								for (int j = 0; j < array2.Length; j++)
								{
									array2[j].color = global::UnityEngine.Color.white;
								}
							});
						});
					});
				});
			});
		});
	}

	public void Close()
	{
		IsOpen = false;
		global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.3f), delegate
		{
			canvasGroup.gameObject.SetActive(value: false);
		});
		StopCoroutine("SpacingOpen");
		StartCoroutine("SpacingClose");
		myBtn.onClick.RemoveAllListeners();
		myBtn.onClick.AddListener(Open);
		iconImage.sprite = boxSprite;
		iconImage.rectTransform.anchoredPosition = global::UnityEngine.Vector2.zero;
		if (GameManager.Instance != null && GameManager.Instance.playerShip.myHealth.HpRatio < 1f)
		{
			UI_Manager.Instance.ui_mainGame.healSpeech.gameObject.SetActive(value: true);
		}
	}

	private global::System.Collections.IEnumerator SpacingClose()
	{
		float totalTime = 0f;
		float duration = 0.3f;
		float startX = 16.5f;
		float endX = -156f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float time = totalTime / duration;
			time = animCurve.Evaluate(time);
			float x = global::UnityEngine.Mathf.Lerp(startX, endX, time);
			global::UnityEngine.Vector2 spacing = grid.spacing;
			spacing.x = x;
			grid.spacing = spacing;
			if (!(time > 1f))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public bool IsHaveEmptySlot(ref UI_ItemRewardBox emptyItemRewardBox)
	{
		bool result = false;
		int num = 2;
		if (PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot)
		{
			num = 3;
		}
		for (int i = 0; i < num; i++)
		{
			if (itemRewardBox[i].myData == null)
			{
				emptyItemRewardBox = itemRewardBox[i];
				result = true;
				break;
			}
		}
		return result;
	}

	public void BoxRefresh()
	{
		int num = 2;
		if (PlayerInfo.Instance.myData.isHaveThirdRewardBoxSlot)
		{
			num = 3;
		}
		for (int i = 0; i < num; i++)
		{
			if ((bool)itemRewardBox[i])
			{
				itemRewardBox[i].RefreshStatus();
			}
		}
	}

	private void LateUpdate()
	{
		CheckOverCoolTime();
	}
}
