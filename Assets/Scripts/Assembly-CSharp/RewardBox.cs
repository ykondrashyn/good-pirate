public class RewardBox : global::UnityEngine.MonoBehaviour
{
	public ERewardBox myType;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Collider collider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve easeAnim;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve heightAnimCurve;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject silver;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject gold;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject red;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject black;

	[global::UnityEngine.SerializeField]
	private RewardBoxStatus myBoxData;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform iconTransform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem ps;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Graphic[] changeColors;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image fillamountBGImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text nameText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] particles;

	private global::UnityEngine.Color bronzeColor = myColor.GetColor(195, 121, 62);

	private global::UnityEngine.Color silverColor = global::UnityEngine.Color.white;

	private global::UnityEngine.Color goldColor = myColor.GetColor(255, 197, 0);

	private global::UnityEngine.Color blackColor = myColor.GetColor(42, 0, 101);

	private global::UnityEngine.Color bgColorBlack = myColor.GetColor(0, 0, 0, 178);

	private global::UnityEngine.Color bgColorWhite = myColor.GetColor(255, 255, 255, 178);

	private global::UnityEngine.Coroutine repeatCheckCoroutine;

	public int level { get; private set; }

	private void Awake()
	{
		collider = GetComponent<global::UnityEngine.Collider>();
		StartCoroutine("DisappearCoroutine");
	}

	private global::System.Collections.IEnumerator DisappearCoroutine()
	{
		float totalTime = 0f;
		float duration = 181f;
		global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder
		{
			Capacity = 10
		};
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = duration - totalTime;
			sb.Length = 0;
			sb.Append((int)(num / 60f));
			sb.Append(":");
			sb.Append($"{num % 60f:00}");
			leftTimeText.text = sb.ToString();
			fillamountBGImage.fillAmount = global::UnityEngine.Mathf.Clamp01(totalTime / duration);
			if (num <= 0f)
			{
				break;
			}
			yield return null;
		}
		leftTimeText.text = "0:00";
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, 0f, 0.6f), global::DG.Tweening.Ease.InBack), delegate
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		});
	}

	private void OnEnable()
	{
		base.transform.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(base.transform, 1f, 0.8f), global::DG.Tweening.Ease.OutBack);
		iconTransform.localScale = global::UnityEngine.Vector3.zero;
		global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.ShortcutExtensions.DOScale(iconTransform, 1f, 0.8f), global::DG.Tweening.Ease.OutBack), 0.8f);
		SetRewardBox(myBoxData.boxType, myBoxData.level);
	}

	private void SelfDestroy()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetRewardBox(ERewardBox grade, int level)
	{
		myType = grade;
		silver.SetActive(value: false);
		gold.SetActive(value: false);
		red.SetActive(value: false);
		black.SetActive(value: false);
		switch (myType)
		{
		case ERewardBox.BRONZE:
		{
			silver.SetActive(value: true);
			fillamountBGImage.color = bgColorBlack;
			nameText.text = LanguageManager.Instance.translate("key_bronze");
			global::UnityEngine.UI.Graphic[] array = changeColors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = bronzeColor;
			}
			global::UnityEngine.ParticleSystem[] array2 = particles;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].startColor = bronzeColor;
			}
			break;
		}
		case ERewardBox.SILVER:
		{
			gold.SetActive(value: true);
			fillamountBGImage.color = bgColorBlack;
			nameText.text = LanguageManager.Instance.translate("key_silver");
			global::UnityEngine.UI.Graphic[] array = changeColors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = silverColor;
			}
			global::UnityEngine.ParticleSystem[] array2 = particles;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].startColor = silverColor;
			}
			break;
		}
		case ERewardBox.GOLD:
		{
			red.SetActive(value: true);
			fillamountBGImage.color = bgColorBlack;
			nameText.text = LanguageManager.Instance.translate("key_gold");
			global::UnityEngine.UI.Graphic[] array = changeColors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = goldColor;
			}
			global::UnityEngine.ParticleSystem[] array2 = particles;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].startColor = goldColor;
			}
			break;
		}
		case ERewardBox.BLACK:
		{
			black.SetActive(value: true);
			fillamountBGImage.color = bgColorWhite;
			nameText.text = LanguageManager.Instance.translate("key_black");
			global::UnityEngine.UI.Graphic[] array = changeColors;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = blackColor;
			}
			global::UnityEngine.ParticleSystem[] array2 = particles;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].startColor = blackColor;
			}
			break;
		}
		}
		level = global::UnityEngine.Mathf.Clamp(level, 1, 1000);
		this.level = level;
		myBoxData = new RewardBoxStatus();
		myBoxData.level = level;
		myBoxData.boxType = grade;
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		ShipController componentInParent = other.GetComponentInParent<ShipController>();
		if (componentInParent != null)
		{
			StopCoroutine("DisappearCoroutine");
			UI_ItemRewardBox emptyItemRewardBox = null;
			if (UI_Manager.Instance.ui_mainGame.ui_chestPanel.IsHaveEmptySlot(ref emptyItemRewardBox))
			{
				collider.enabled = false;
				StartCoroutine(GoMoveTarget(componentInParent.transform));
			}
			else if (repeatCheckCoroutine == null)
			{
				repeatCheckCoroutine = StartCoroutine(RepeatCheck());
			}
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		if (other.GetComponentInParent<ShipController>() != null)
		{
			StopCoroutine("DisappearCoroutine");
			StartCoroutine("DisappearCoroutine");
			if (repeatCheckCoroutine != null)
			{
				StopCoroutine(repeatCheckCoroutine);
			}
			repeatCheckCoroutine = null;
		}
	}

	private global::System.Collections.IEnumerator RepeatCheck()
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(2f);
		UI_ItemRewardBox emptyItemRewardBox = null;
		bool soundActive = true;
		while (!UI_Manager.Instance.ui_mainGame.ui_chestPanel.IsHaveEmptySlot(ref emptyItemRewardBox))
		{
			UI_Manager.Instance.ui_mainGame.ui_chestPanel.Alarm(soundActive);
			if (soundActive)
			{
				soundActive = false;
			}
			yield return wait;
		}
		StartCoroutine(GoMoveTarget(GameManager.Instance.playerShipController.transform));
		repeatCheckCoroutine = null;
	}

	private global::System.Collections.IEnumerator GoMoveTarget(global::UnityEngine.Transform trans)
	{
		global::DG.Tweening.ShortcutExtensions.DOScale(iconTransform, 0f, 0.3f);
		ps.Stop();
		float duration = 0.7f;
		float totalTime = 0f;
		float maxHeight = 5f;
		global::UnityEngine.Vector3 startPos = base.transform.position;
		global::DG.Tweening.ShortcutExtensions.DOKill(base.transform);
		global::DG.Tweening.ShortcutExtensions.DORotate(base.transform, new global::UnityEngine.Vector3(global::UnityEngine.Random.Range(-1000f, 1000f), global::UnityEngine.Random.Range(-1000f, 1000f), global::UnityEngine.Random.Range(-1000f, 1000f)), duration, global::DG.Tweening.RotateMode.FastBeyond360);
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
				base.transform.localScale = global::UnityEngine.Vector3.one * num2;
			}
			float y = global::UnityEngine.Mathf.Lerp(0f, maxHeight, t2);
			global::UnityEngine.Vector3 position = global::UnityEngine.Vector3.Lerp(startPos, trans.position, t);
			position.y = y;
			base.transform.position = position;
			if (num > 1f)
			{
				break;
			}
			yield return null;
		}
		UI_Manager.Instance.ui_mainGame.AddRewardBox(myBoxData);
		SelfDestroy();
	}
}
