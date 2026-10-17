public class UI_ItemBox : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image boxCloseImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image boxOpenImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image backLightImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image lightImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform generatePivotTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject blackObjectSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem freeBoxExplosionParticle;

	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> reserveDeleteList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	private global::UnityEngine.Animator anim;

	private global::System.Collections.Generic.List<ItemSlotValue> itemList;

	private global::UnityEngine.Events.UnityAction generateAction;

	[global::UnityEngine.Header("sprites")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite bronze_box_close;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite bronze_box_open;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite silver_box_close;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite silver_box_open;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite gold_box_close;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite gold_box_open;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite black_box_close;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite black_box_open;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite black_backlight;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite black_light;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite other_backlight;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite other_light;

	private ParticleUnscaledTime[] particles;

	[global::UnityEngine.SerializeField]
	private ParticleUnscaledTime bronzeParticle;

	[global::UnityEngine.SerializeField]
	private ParticleUnscaledTime silverParticle;

	[global::UnityEngine.SerializeField]
	private ParticleUnscaledTime goldParticle;

	[global::UnityEngine.SerializeField]
	private ParticleUnscaledTime blackParticle;

	private ParticleUnscaledTime selectedParticle;

	[global::UnityEngine.SerializeField]
	private ParticleUnscaledTime freeRewardBoxParticle;

	private void Awake()
	{
		anim = GetComponent<global::UnityEngine.Animator>();
	}

	public void SetBox(ERewardBox type)
	{
		switch (type)
		{
		case ERewardBox.BRONZE:
			boxCloseImage.sprite = bronze_box_close;
			boxOpenImage.sprite = bronze_box_open;
			selectedParticle = bronzeParticle;
			break;
		case ERewardBox.SILVER:
			boxCloseImage.sprite = silver_box_close;
			boxOpenImage.sprite = silver_box_open;
			selectedParticle = silverParticle;
			break;
		case ERewardBox.GOLD:
			boxCloseImage.sprite = gold_box_close;
			boxOpenImage.sprite = gold_box_open;
			selectedParticle = goldParticle;
			break;
		case ERewardBox.BLACK:
			boxCloseImage.sprite = black_box_close;
			boxOpenImage.sprite = black_box_open;
			selectedParticle = blackParticle;
			break;
		default:
			selectedParticle = null;
			break;
		}
		switch (type)
		{
		case ERewardBox.BRONZE:
		case ERewardBox.SILVER:
		case ERewardBox.GOLD:
			backLightImage.sprite = other_backlight;
			lightImage.sprite = other_light;
			break;
		case ERewardBox.BLACK:
			backLightImage.sprite = black_backlight;
			lightImage.sprite = black_light;
			break;
		}
	}

	public void SetItemList(global::System.Collections.Generic.List<ItemSlotValue> itemList, global::UnityEngine.Events.UnityAction generateAction)
	{
		this.itemList = itemList;
		anim.Play("Open", 0);
		this.generateAction = generateAction;
	}

	public void Boooom()
	{
		particles = GetComponentsInChildren<ParticleUnscaledTime>();
		ParticleUnscaledTime[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public void Skip()
	{
		anim.Play("Open", 0, 1f);
		foreach (global::UnityEngine.GameObject reserveDelete in reserveDeleteList)
		{
			global::UnityEngine.Object.Destroy(reserveDelete);
		}
		StopCoroutine("GenerateCoroutine");
	}

	public void Generate()
	{
		StartCoroutine("GenerateCoroutine");
	}

	private global::System.Collections.IEnumerator GenerateCoroutine()
	{
		bool isFree = GetComponentInParent<UI_ItemBoxPopup>().isFree;
		int number = itemList.Count;
		float intervalTime = 0.4f / (float)number;
		if (!isFree)
		{
			for (int i = 0; i < number; i++)
			{
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(blackObjectSrc, generatePivotTrans);
				global::UnityEngine.RectTransform component = gameObject.GetComponent<global::UnityEngine.RectTransform>();
				component.localScale = global::UnityEngine.Vector3.one * 0.6f;
				component.anchoredPosition = new global::UnityEngine.Vector2(global::UnityEngine.Random.Range(-80f, 70f), -65f);
				global::UnityEngine.Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.y = 1500f;
				global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.DOTweenModuleUI.DOAnchorPos(component, anchoredPosition, 0.4f), isIndependentUpdate: true), global::DG.Tweening.Ease.InCubic);
				global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.TweenSettingsExtensions.SetEase(global::DG.Tweening.TweenSettingsExtensions.SetUpdate(global::DG.Tweening.ShortcutExtensions.DOScale(component, 1.7f, 0.38f), isIndependentUpdate: true), global::DG.Tweening.Ease.InCubic), 0.07f);
				reserveDeleteList.Add(gameObject);
				yield return new global::UnityEngine.WaitForSecondsRealtime(intervalTime);
			}
			yield return new global::UnityEngine.WaitForSecondsRealtime(1.1f);
			foreach (global::UnityEngine.GameObject reserveDelete in reserveDeleteList)
			{
				global::UnityEngine.Object.Destroy(reserveDelete);
			}
		}
		if (generateAction != null)
		{
			generateAction();
		}
		reserveDeleteList.Clear();
	}

	public void SoundFreeBoxShake()
	{
		AudioManager.Instance.PlaySFX("ui_freebox_shake");
	}

	public void SoundFreeBoxOpen()
	{
		AudioManager.Instance.PlaySFX("ui_freebox_explo");
	}

	public void SoundRewardBoxShake()
	{
		AudioManager.Instance.PlaySFX("chestboxOpen_0");
	}

	public void SoundRewardBoxOpen()
	{
		AudioManager.Instance.PlaySFX("chestboxOpen_1");
	}

	public void BackgroundParticleStart()
	{
		if (selectedParticle != null)
		{
			selectedParticle.Play();
		}
	}

	public void FreeRewardBackgroundLightStart()
	{
		freeRewardBoxParticle.Play();
	}
}
