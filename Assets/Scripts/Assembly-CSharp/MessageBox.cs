public class MessageBox : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Button myButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject triObj;

	private TutorialInfo nowInfo;

	private TextActionData[] nowData;

	private int nowMessageNum;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	private void Awake()
	{
		myButton = GetComponent<global::UnityEngine.UI.Button>();
		myText.font = LanguageManager.Instance.getLanguageFont();
	}

	private void OnEnable()
	{
		myText.text = "";
		triObj.SetActive(value: false);
	}

	private void OnDisable()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(triObj.transform);
		if (IsInvoking("Next"))
		{
			CancelInvoke("Next");
		}
	}

	public void SetMessage(TutorialInfo info)
	{
		triObj.gameObject.SetActive(value: false);
		nowInfo = info;
		nowData = info.messageList;
		nowMessageNum = 0;
		Message(nowData[nowMessageNum]);
	}

	private void Message(TextActionData data)
	{
		data.action.Invoke();
		global::DG.Tweening.ShortcutExtensions.DOKill(myText);
		myText.text = "";
		string text = LanguageManager.Instance.translate(data.messageId);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOText(myText, text, (float)text.Length * 0.04f), isIndependentUpdate: true), global::DG.Tweening.Ease.Linear), delegate
		{
			ArrowCheck();
			myButton.enabled = data.isCanNext;
			myButton.onClick.RemoveAllListeners();
			myButton.onClick.AddListener(Next);
			if (data.isAutoNext)
			{
				Invoke("Next", 5f);
			}
		});
		myButton.onClick.RemoveAllListeners();
		myButton.onClick.AddListener(Skip);
	}

	public void Skip()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(myText);
		string text = LanguageManager.Instance.translate(nowData[nowMessageNum].messageId);
		myText.text = text;
		ArrowCheck();
		myButton.onClick.RemoveAllListeners();
		myButton.onClick.AddListener(Next);
		myButton.enabled = nowData[nowMessageNum].isCanNext;
	}

	private bool IsHaveNextMessage()
	{
		if (nowMessageNum < nowData.Length - 1)
		{
			return true;
		}
		return false;
	}

	public void Next()
	{
		if (IsInvoking("Next"))
		{
			CancelInvoke("Next");
		}
		if (IsHaveNextMessage())
		{
			nowData[nowMessageNum].endAction.Invoke();
			triObj.gameObject.SetActive(value: false);
			global::DG.Tweening.ShortcutExtensions.DOKill(triObj.GetComponent<global::UnityEngine.RectTransform>());
			nowMessageNum++;
			Message(nowData[nowMessageNum]);
		}
		else if (nowInfo.isTouchToDisappear)
		{
			nowData[nowMessageNum].endAction.Invoke();
			TutorialManager.Instance.Off();
		}
	}

	public void EndAction()
	{
		nowData[nowMessageNum].endAction.Invoke();
	}

	private void ArrowCheck()
	{
		if ((IsHaveNextMessage() || nowInfo.isTouchToDisappear) && nowData[nowMessageNum].isCanNext)
		{
			triObj.gameObject.SetActive(value: true);
			global::UnityEngine.RectTransform component = triObj.GetComponent<global::UnityEngine.RectTransform>();
			global::UnityEngine.Vector2 anchoredPosition = component.anchoredPosition;
			anchoredPosition.x = -90f;
			component.anchoredPosition = anchoredPosition;
			global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetLoops(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOAnchorPosX(component, -85f, 1f), isIndependentUpdate: true), -1, global::DG.Tweening.LoopType.Restart), 0.5f), animCurve);
		}
		else
		{
			triObj.gameObject.SetActive(value: false);
			global::DG.Tweening.ShortcutExtensions.DOKill(triObj.GetComponent<global::UnityEngine.RectTransform>());
		}
	}
}
