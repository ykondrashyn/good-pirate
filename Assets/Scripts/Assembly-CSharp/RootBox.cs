public class RootBox : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Collider collider;

	private global::System.Collections.Generic.List<Reward> getRewardList = new global::System.Collections.Generic.List<Reward>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve easeAnim;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve heightAnimCurve;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject particleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] particles;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.CanvasGroup tutorialMessageCanvasGroup;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject tutoParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject canvasObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image fillImage;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		collider = GetComponent<global::UnityEngine.Collider>();
		modelTrans = transform.Find("Model");
		StartCoroutine("SelfDestroy");
		if ((bool)modelTrans)
		{
			modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(global::UnityEngine.Random.Range(-5f, 5f), global::UnityEngine.Random.Range(0f, 360f), global::UnityEngine.Random.Range(-5f, 5f));
		}
	}

	public void DontSelfDestroy()
	{
		canvasObj.SetActive(value: false);
		StopCoroutine("SelfDestroy");
	}

	private global::System.Collections.IEnumerator SelfDestroy()
	{
		float totalTime = 0f;
		float duration = 61f;
		int prevInt = 0;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			int num = (int)(duration - totalTime);
			if (prevInt != num && (bool)leftTimeText)
			{
				leftTimeText.text = num.ToString();
			}
			if ((bool)fillImage)
			{
				fillImage.fillAmount = 1f - global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			}
			prevInt = num;
			if (totalTime >= duration)
			{
				break;
			}
			yield return null;
		}
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(transform, 0f, 0.6f), global::DG.Tweening.Ease.InBack), delegate
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	private void OnEnable()
	{
		getRewardList.Clear();
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		transform.localScale = global::UnityEngine.Vector3.one * 0.4f;
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(transform, 1f, 0.5f), global::DG.Tweening.Ease.OutBack), delegate
		{
			if ((bool)collider)
			{
				collider.enabled = true;
			}
			if (!TutorialManager.Instance.IsHadTutorial("goldbox") && (bool)tutoParticleObj)
			{
				TutorialManager.Instance.StartTutorial("goldbox");
				tutoParticleObj.SetActive(value: true);
				tutorialMessageCanvasGroup.gameObject.SetActive(value: true);
				tutorialMessageCanvasGroup.alpha = 0f;
				global::DG.Tweening.DOTweenModuleUI.DOFade(tutorialMessageCanvasGroup, 1f, 0.4f);
			}
		});
		if ((bool)modelTrans)
		{
			global::DG.Tweening.TweenSettingsExtensions.SetLoops(global::DG.Tweening.ShortcutExtensions.DORotate(transform, new global::UnityEngine.Vector3(0f, 360f, 0f), 30f, global::DG.Tweening.RotateMode.FastBeyond360), -1);
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		if (other.GetComponent<Eagle>() != null)
		{
			return;
		}
		ShipController componentInParent = other.GetComponentInParent<ShipController>();
		if (componentInParent != null)
		{
			if (IsInvoking("SelfDestroy"))
			{
				CancelInvoke("SelfDestroy");
			}
			collider.enabled = false;
			if (!TutorialManager.Instance.IsHadTutorial("goldbox") && TutorialManager.Instance.IsPlaying("goldbox"))
			{
				TutorialManager.Instance.Off();
			}
			StartCoroutine(GoMoveTarget(componentInParent.transform));
			AudioManager.Instance.PlaySFX("item_root");
		}
	}

	public void ForceRoot()
	{
		if ((bool)particleObj)
		{
			particleObj.SetActive(value: true);
			particleObj.transform.rotation = global::UnityEngine.Quaternion.Euler(90f, 0f, 0f);
		}
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Emit(60);
		}
		StartCoroutine(GoMoveTarget(GameManager.Instance.playerShip.transform));
	}

	private global::System.Collections.IEnumerator GoMoveTarget(global::UnityEngine.Transform trans, float delay = 0f)
	{
		if (delay != 0f)
		{
			yield return new global::UnityEngine.WaitForSeconds(delay);
		}
		float duration = 0.7f;
		float totalTime = 0f;
		float maxHeight = 5f;
		global::UnityEngine.Vector3 startPos = transform.position;
		global::DG.Tweening.ShortcutExtensions.DOKill(transform);
		global::DG.Tweening.ShortcutExtensions.DORotate(transform, new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-1000f, 1000f), global::UnityEngine.Random.Range(-1000f, 1000f), global::UnityEngine.Random.Range(-1000f, 1000f)), duration, global::DG.Tweening.RotateMode.FastBeyond360);
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			float t = easeAnim.Evaluate(num);
			float t2 = heightAnimCurve.Evaluate(num);
			if (num >= 0.5f)
			{
				float t3 = num * 2f - 1f;
				float num2 = global::UnityEngine.Mathf.Lerp(1f, 0f, t3);
				transform.localScale = global::UnityEngine.Vector3.one * num2;
			}
			float y = global::UnityEngine.Mathf.Lerp(0f, maxHeight, t2);
			global::UnityEngine.Vector3 position = global::UnityEngine.Vector3.Lerp(startPos, trans.position, t);
			position.y = y;
			transform.position = position;
			if (num > 1f)
			{
				break;
			}
			yield return null;
		}
		UI_Manager.Instance.AddReward(transform.position, getRewardList.ToArray());
		SelfDestroy();
		if (!TutorialManager.Instance.IsHadTutorial("firstbattle") && !TutorialManager.Instance.IsPlaying("firstbattle"))
		{
			Appsflyer.Instance.SendEvent("Fraud");
			TutorialManager.Instance.StartTutorial("firstbattle");
		}
	}

	public void AddReward(Reward reward)
	{
		getRewardList.Add(reward);
	}

	public void AddReward(params Reward[] reward)
	{
		foreach (Reward item in reward)
		{
			getRewardList.Add(item);
		}
	}
}
