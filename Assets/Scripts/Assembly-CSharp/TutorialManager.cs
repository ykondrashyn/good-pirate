public class TutorialManager : global::UnityEngine.MonoBehaviour
{
	public static TutorialManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup canvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject backGroundObj;

	[global::UnityEngine.SerializeField]
	private MessageBox msgBox;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<TutorialInfo> tutoList;

	private global::System.Collections.Generic.Dictionary<string, TutorialInfo> tutoDic = new global::System.Collections.Generic.Dictionary<string, TutorialInfo>();

	private const string hadTutorialOverKey = "Tutorial_{0}";

	private TutorialInfo nowPlayingTutorial;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform messageBoxRectTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.RectTransform characterImageRectTransform;

	private void Awake()
	{
		Instance = this;
		foreach (TutorialInfo tuto in tutoList)
		{
			tutoDic.Add(tuto.key, tuto);
		}
		canvasGroup.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		GoogleCloudManager instance = GoogleCloudManager.Instance;
		instance.loadAction = (global::System.Action)global::System.Delegate.Combine(instance.loadAction, new global::System.Action(GoogleCloudLoad));
		GoogleCloudManager instance2 = GoogleCloudManager.Instance;
		instance2.saveAction = (global::System.Action)global::System.Delegate.Combine(instance2.saveAction, new global::System.Action(GoogleCloudSave));
	}

	private void GoogleCloudSave()
	{
		foreach (TutorialInfo tuto in tutoList)
		{
			GoogleCloudManager.Instance.SetBool($"Tutorial_{tuto.key}", IsHadTutorial(tuto));
		}
	}

	private void GoogleCloudLoad()
	{
		foreach (TutorialInfo tuto in tutoList)
		{
			bool isHad = GoogleCloudManager.Instance.GetBool($"Tutorial_{tuto.key}");
			SetHadTutorial(tuto, isHad);
		}
	}

	private void CloudSave()
	{
		foreach (TutorialInfo tuto in tutoList)
		{
			global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.SetBool($"Tutorial_{tuto.key}", IsHadTutorial(tuto));
		}
	}

	private void CloudLoad()
	{
		foreach (TutorialInfo tuto in tutoList)
		{
			global::SA.iOS.Foundation.ISN_NSKeyValueObject iSN_NSKeyValueObject = global::SA.iOS.Foundation.ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey($"Tutorial_{tuto.key}");
			if (iSN_NSKeyValueObject != null)
			{
				SetHadTutorial(tuto, iSN_NSKeyValueObject.BoolValue);
			}
		}
	}

	public void Off()
	{
		msgBox.EndAction();
		SetHadTutorial(nowPlayingTutorial);
		global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 0f, 0.2f), isIndependentUpdate: true), delegate
		{
			canvasGroup.gameObject.SetActive(value: false);
		});
	}

	public void StartTutorial(string key)
	{
		if (!tutoDic.ContainsKey(key))
		{
			return;
		}
		TutorialInfo data = tutoDic[key];
		if (!IsHadTutorial(data))
		{
			global::DG.Tweening.ShortcutExtensions.DOKill(canvasGroup);
			canvasGroup.gameObject.SetActive(value: true);
			backGroundObj.SetActive(data.isHasBackground);
			canvasGroup.alpha = 0f;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOFade(canvasGroup, 1f, 0.3f), isIndependentUpdate: true), data.delay), delegate
			{
				SetTutorialInfo(data);
			});
			switch (data.position)
			{
			case MessagePosition.BOTTOM:
			{
				messageBoxRectTransform.anchorMin = new global::UnityEngine.Vector2(0f, 0f);
				messageBoxRectTransform.anchorMax = new global::UnityEngine.Vector2(1f, 0f);
				messageBoxRectTransform.pivot = new global::UnityEngine.Vector2(0.5f, 0f);
				global::UnityEngine.Vector3 anchoredPosition3D3 = messageBoxRectTransform.anchoredPosition3D;
				anchoredPosition3D3.y = 0f;
				messageBoxRectTransform.anchoredPosition3D = anchoredPosition3D3;
				characterImageRectTransform.anchorMin = new global::UnityEngine.Vector2(0f, 0f);
				characterImageRectTransform.anchorMax = new global::UnityEngine.Vector2(0f, 0f);
				characterImageRectTransform.pivot = new global::UnityEngine.Vector2(0f, 0f);
				global::UnityEngine.Vector3 anchoredPosition3D4 = characterImageRectTransform.anchoredPosition3D;
				anchoredPosition3D4.y = 44.5f;
				characterImageRectTransform.anchoredPosition3D = anchoredPosition3D4;
				break;
			}
			case MessagePosition.TOP:
			{
				messageBoxRectTransform.anchorMin = new global::UnityEngine.Vector2(0f, 1f);
				messageBoxRectTransform.anchorMax = new global::UnityEngine.Vector2(1f, 1f);
				messageBoxRectTransform.pivot = new global::UnityEngine.Vector2(0.5f, 1f);
				global::UnityEngine.Vector3 anchoredPosition3D = messageBoxRectTransform.anchoredPosition3D;
				anchoredPosition3D.y = -151f;
				messageBoxRectTransform.anchoredPosition3D = anchoredPosition3D;
				characterImageRectTransform.anchorMin = new global::UnityEngine.Vector2(0f, 1f);
				characterImageRectTransform.anchorMax = new global::UnityEngine.Vector2(0f, 1f);
				characterImageRectTransform.pivot = new global::UnityEngine.Vector2(0f, 1f);
				global::UnityEngine.Vector3 anchoredPosition3D2 = characterImageRectTransform.anchoredPosition3D;
				anchoredPosition3D2.y = -28f;
				characterImageRectTransform.anchoredPosition3D = anchoredPosition3D2;
				break;
			}
			}
			nowPlayingTutorial = data;
		}
	}

	private void SetTutorialInfo(TutorialInfo data)
	{
		msgBox.SetMessage(data);
	}

	public bool IsHadTutorial(TutorialInfo data)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Tutorial_{data.key}", defaultValue: false);
	}

	public bool IsHadTutorial(string id)
	{
		return global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.GetBool($"Tutorial_{id}", defaultValue: false);
	}

	public void SetHadTutorial(TutorialInfo data, bool isHad = true)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Tutorial_{data.key}", isHad);
	}

	public void SetHadTutorial(string id)
	{
		global::CodeStage.AntiCheat.ObscuredTypes.ObscuredPrefs.SetBool($"Tutorial_{id}", value: true);
	}

	public bool IsPlaying(string key)
	{
		if (tutoDic.ContainsKey(key))
		{
			if (nowPlayingTutorial == tutoDic[key])
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void Next()
	{
		msgBox.Next();
	}
}
