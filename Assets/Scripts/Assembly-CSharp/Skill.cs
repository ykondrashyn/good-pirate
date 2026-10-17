public abstract class Skill : global::UnityEngine.MonoBehaviour
{
	protected delegate void SkillActionDelegate();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myOutlineImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myTriIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myTriOutlineImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Button myButton;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject lockObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject coolTimeObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject skillLiveObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject skillLiveTriObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image coolTimeFillImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text coolTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image durationImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text durationText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image canSkillActiveImage;

	[global::UnityEngine.SerializeField]
	private UI_SkillStatus skillStatus;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<global::UnityEngine.UI.Image> whiteAlphaList = new global::System.Collections.Generic.List<global::UnityEngine.UI.Image>();

	protected Skill.SkillActionDelegate actionDelegate;

	protected Skill.SkillActionDelegate skillOverDelegate;

	protected float coolTime = 300f;

	protected float durationTime = 10f;

	protected string myCoolTimeKey = "";

	public bool IsSkillLive { get; private set; }

	[global::UnityEngine.ContextMenu("Try GetComponent")]
	private void AutoInspector()
	{
		myImage = GetComponent<global::UnityEngine.UI.Image>();
		myButton = GetComponent<global::UnityEngine.UI.Button>();
		myIcon = base.transform.Find("skill icon").GetComponent<global::UnityEngine.UI.Image>();
		myOutlineImage = base.transform.Find("outline").GetComponent<global::UnityEngine.UI.Image>();
		myTriIcon = base.transform.Find("triangle bg").Find("tri icon").GetComponent<global::UnityEngine.UI.Image>();
		myTriOutlineImage = base.transform.Find("triangle bg").Find("tri outline").GetComponent<global::UnityEngine.UI.Image>();
		skillLiveTriObj = base.transform.Find("triangle bg").Find("tri duration").gameObject;
		lockObj = base.transform.Find("LockObj").gameObject;
		coolTimeObj = base.transform.Find("CoolTime Fill").gameObject;
		skillLiveObj = base.transform.Find("Skill duration").gameObject;
		coolTimeFillImage = coolTimeObj.GetComponent<global::UnityEngine.UI.Image>();
		durationImage = skillLiveObj.GetComponent<global::UnityEngine.UI.Image>();
		coolTimeText = coolTimeObj.GetComponentInChildren<global::UnityEngine.UI.Text>();
		durationText = skillLiveObj.GetComponentInChildren<global::UnityEngine.UI.Text>();
		canSkillActiveImage = base.transform.Find("CanSkillActive").GetComponent<global::UnityEngine.UI.Image>();
		whiteAlphaList.Clear();
		whiteAlphaList.Add(myIcon);
		whiteAlphaList.Add(myOutlineImage);
		whiteAlphaList.Add(myTriIcon);
		whiteAlphaList.Add(myTriOutlineImage);
	}

	protected virtual void Awake()
	{
		SetMyCoolTimeKey();
	}

	private void OnEnable()
	{
		Check();
	}

	protected abstract void SetMyCoolTimeKey();

	protected abstract bool IsHaveSkill();

	public void Active()
	{
		if (!GameManager.Instance.IsInIsland && skillStatus.IsCanUse())
		{
			if (actionDelegate != null)
			{
				actionDelegate();
			}
			coolTime = 15f;
			CoolTimeManager.AddCooltimeSecondFromNow(myCoolTimeKey, (int)(coolTime + durationTime));
			skillLiveObj.SetActive(value: true);
			skillLiveTriObj.SetActive(value: true);
			myButton.enabled = false;
			StartCoroutine("SkillDuring");
			if (!IsInvoking("SkillOver"))
			{
				Invoke("SkillOver", durationTime);
			}
			skillStatus.Used();
		}
	}

	public void SkillOver()
	{
		if (skillOverDelegate != null)
		{
			skillOverDelegate();
		}
		if (base.gameObject.activeSelf)
		{
			StartCoroutine("SkillCoolTime");
		}
	}

	public void Check()
	{
		if (CoolTimeManager.HaveKey(myCoolTimeKey))
		{
			if (CoolTimeManager.GetLeftCoolTime(myCoolTimeKey).TotalSeconds > (double)coolTime)
			{
				StopCoroutine("SkillDuring");
				StartCoroutine("SkillDuring");
			}
			else
			{
				StartCoroutine("SkillCoolTime");
			}
			return;
		}
		coolTimeObj.SetActive(value: false);
		skillLiveObj.SetActive(value: false);
		skillLiveTriObj.SetActive(value: false);
		myIcon.rectTransform.localScale = global::UnityEngine.Vector3.one;
		myIcon.rectTransform.anchoredPosition = global::UnityEngine.Vector3.zero;
		if (IsHaveSkill())
		{
			CheckCanSkillActiveObj();
			if (skillStatus.IsCanUse())
			{
				GetComponent<global::UnityEngine.CanvasGroup>().alpha = 1f;
			}
			else
			{
				GetComponent<global::UnityEngine.CanvasGroup>().alpha = 0.5f;
			}
			lockObj.SetActive(value: false);
			myButton.enabled = true;
		}
		else
		{
			lockObj.SetActive(value: true);
			myButton.enabled = false;
		}
	}

	private void CheckCanSkillActiveObj()
	{
		if (!canSkillActiveImage.gameObject.activeSelf)
		{
			canSkillActiveImage.gameObject.SetActive(value: true);
			canSkillActiveImage.color = global::UnityEngine.Color.white;
			canSkillActiveImage.transform.localScale = global::UnityEngine.Vector3.one * 1.08f;
			global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOScale(canSkillActiveImage.transform, 1f, 0.5f), 0.1f);
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOColor(canSkillActiveImage, new global::UnityEngine.Color(1f, 1f, 1f, 0f), 1f), 0.2f), delegate
			{
				canSkillActiveImage.gameObject.SetActive(value: false);
			});
		}
	}

	private global::System.Collections.IEnumerator SkillDuring()
	{
		myIcon.rectTransform.localScale = global::UnityEngine.Vector3.one * 0.8f;
		myIcon.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 18f);
		global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(myCoolTimeKey);
		float totaltime = durationTime - ((float)leftCoolTime.TotalSeconds - coolTime);
		while (true)
		{
			totaltime += global::UnityEngine.Time.deltaTime;
			float num = durationTime - totaltime;
			durationText.text = $"{(int)(num / 60f):0}:{(int)(num % 60f):00}";
			durationImage.fillAmount = 1f - global::UnityEngine.Mathf.Clamp01(totaltime / durationTime);
			if (!(totaltime >= durationTime))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	private global::System.Collections.IEnumerator SkillCoolTime()
	{
		myIcon.rectTransform.localScale = global::UnityEngine.Vector3.one * 0.8f;
		myIcon.rectTransform.anchoredPosition = new global::UnityEngine.Vector2(0f, 18f);
		coolTimeObj.SetActive(value: true);
		skillLiveObj.SetActive(value: false);
		skillLiveTriObj.SetActive(value: false);
		lockObj.SetActive(value: false);
		foreach (global::UnityEngine.UI.Image whiteAlpha in whiteAlphaList)
		{
			whiteAlpha.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.5f);
		}
		while (true)
		{
			global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(myCoolTimeKey);
			double totalSeconds = leftCoolTime.TotalSeconds;
			int num = (int)(totalSeconds / 60.0);
			int num2 = (int)(totalSeconds % 60.0);
			coolTimeText.text = $"{num:0}:{num2:00}";
			coolTimeFillImage.fillAmount = (float)totalSeconds / coolTime;
			if (leftCoolTime.TotalSeconds <= 0.0)
			{
				break;
			}
			yield return null;
		}
		foreach (global::UnityEngine.UI.Image whiteAlpha2 in whiteAlphaList)
		{
			whiteAlpha2.color = global::UnityEngine.Color.white;
		}
		myIcon.rectTransform.localScale = global::UnityEngine.Vector3.one;
		myIcon.rectTransform.anchoredPosition = global::UnityEngine.Vector3.zero;
		myButton.enabled = true;
		coolTimeObj.SetActive(value: false);
		CoolTimeManager.DeleteCoolTime(myCoolTimeKey);
		CheckCanSkillActiveObj();
	}

	protected void AddActiveCallBack(Skill.SkillActionDelegate ac)
	{
		actionDelegate = (Skill.SkillActionDelegate)global::System.Delegate.Combine(actionDelegate, ac);
	}

	protected void RemoveActiveCallBack(Skill.SkillActionDelegate ac)
	{
		actionDelegate = (Skill.SkillActionDelegate)global::System.Delegate.Remove(actionDelegate, ac);
	}

	protected void AddSkillOverCallBack(Skill.SkillActionDelegate ac)
	{
		skillOverDelegate = (Skill.SkillActionDelegate)global::System.Delegate.Combine(skillOverDelegate, ac);
	}

	protected void RemoveSkillOverCallBack(Skill.SkillActionDelegate ac)
	{
		skillOverDelegate = (Skill.SkillActionDelegate)global::System.Delegate.Remove(skillOverDelegate, ac);
	}
}
