public class CardGotchaAnim : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] auraParticles;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] bottomParticles;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] shiningParticles;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject skipBtn;

	public global::UnityEngine.ParticleSystem ptcl_card_aura;

	public global::UnityEngine.ParticleSystem ptcl_Legendary_bottom;

	public global::UnityEngine.ParticleSystem ptcl_Legendary_shining;

	private ParticleUnscaledTime unscaled_aura;

	private ParticleUnscaledTime unscaled_bottom;

	private ParticleUnscaledTime unscaled_shining;

	[global::UnityEngine.SerializeField]
	private UI_ItemSlot cardSlot;

	[global::UnityEngine.SerializeField]
	private UI_ItemSlot moneyTypeSlot;

	private global::UnityEngine.Animator anim;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image bgImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color sBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color aBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color bBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color cBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image cardImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite cPack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite bPack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite aPack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite sPack;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color lrBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color lyBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color lpBgColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color lgBgColor;

	public bool isCrew = true;

	private void Awake()
	{
		anim = GetComponent<global::UnityEngine.Animator>();
	}

	private void OnEnable()
	{
		if ((bool)cardSlot)
		{
			cardSlot.gameObject.SetActive(value: false);
		}
		if ((bool)moneyTypeSlot)
		{
			moneyTypeSlot.gameObject.SetActive(value: false);
		}
		if ((bool)skipBtn)
		{
			skipBtn.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		isCrew = true;
	}

	public void SetCardData(MySailorInventory sailorData)
	{
		if (moneyTypeSlot != null)
		{
			moneyTypeSlot.gameObject.SetActive(value: false);
		}
		cardSlot.SetData(sailorData, 0);
	}

	public void SetMoneyType(EMonenyType moneyType, int amount)
	{
		if (cardSlot != null)
		{
			cardSlot.gameObject.SetActive(value: false);
		}
		moneyTypeSlot.SetData(moneyType, amount);
	}

	public void CardOn()
	{
		if (isCrew)
		{
			if (cardSlot != null)
			{
				cardSlot.gameObject.SetActive(value: true);
				cardSlot.FreeBoxOpen();
			}
		}
		else if (moneyTypeSlot != null)
		{
			moneyTypeSlot.gameObject.SetActive(value: true);
			moneyTypeSlot.FreeBoxOpen();
		}
	}

	public void Skip()
	{
		if (isCrew)
		{
			if (cardSlot != null)
			{
				cardSlot.gameObject.SetActive(value: true);
				cardSlot.FreeBoxOpen();
			}
		}
		else if (moneyTypeSlot != null)
		{
			moneyTypeSlot.gameObject.SetActive(value: true);
			moneyTypeSlot.FreeBoxOpen();
		}
		anim.Play("Open", 0, 1f);
		if ((bool)ptcl_card_aura)
		{
			ptcl_card_aura.Stop();
		}
		if ((bool)ptcl_Legendary_bottom)
		{
			ptcl_Legendary_bottom.Stop();
		}
		if ((bool)ptcl_Legendary_shining)
		{
			ptcl_Legendary_shining.Stop();
		}
		skipBtn.SetActive(value: false);
	}

	public void SkipBtnOff()
	{
		if ((bool)skipBtn)
		{
			skipBtn.SetActive(value: false);
		}
	}

	public void SetPurple()
	{
		ptcl_card_aura = auraParticles[2];
		ptcl_Legendary_bottom = bottomParticles[2];
		ptcl_Legendary_shining = shiningParticles[2];
		bgImage.color = lpBgColor;
	}

	public void SetYellow()
	{
		ptcl_card_aura = auraParticles[1];
		ptcl_Legendary_bottom = bottomParticles[1];
		ptcl_Legendary_shining = shiningParticles[1];
		bgImage.color = lyBgColor;
	}

	public void SetGreen()
	{
		ptcl_card_aura = auraParticles[3];
		ptcl_Legendary_bottom = bottomParticles[3];
		ptcl_Legendary_shining = shiningParticles[3];
		bgImage.color = lgBgColor;
	}

	public void SetRed()
	{
		ptcl_card_aura = auraParticles[0];
		ptcl_Legendary_bottom = bottomParticles[0];
		ptcl_Legendary_shining = shiningParticles[0];
		bgImage.color = lrBgColor;
	}

	public void SetGrade(EGrade grade)
	{
		switch (grade)
		{
		case EGrade.cClass:
			bgImage.color = cBgColor;
			ptcl_card_aura = auraParticles[0];
			cardImage.sprite = cPack;
			break;
		case EGrade.bClass:
			bgImage.color = bBgColor;
			ptcl_card_aura = auraParticles[1];
			cardImage.sprite = bPack;
			break;
		case EGrade.aClass:
			bgImage.color = aBgColor;
			ptcl_card_aura = auraParticles[2];
			cardImage.sprite = aPack;
			break;
		case EGrade.sClass:
			bgImage.color = sBgColor;
			ptcl_card_aura = auraParticles[3];
			cardImage.sprite = sPack;
			break;
		}
	}

	public void LegendaryStartParticlePlay()
	{
		unscaled_bottom = ptcl_Legendary_bottom.gameObject.GetComponent<ParticleUnscaledTime>();
		unscaled_bottom.Play();
	}

	public void LegendaryShiningParticlePlay()
	{
		unscaled_shining = ptcl_Legendary_shining.gameObject.GetComponent<ParticleUnscaledTime>();
		unscaled_shining.Play();
	}

	public void CardParticlePlay()
	{
		unscaled_aura = ptcl_card_aura.gameObject.GetComponent<ParticleUnscaledTime>();
		unscaled_aura.Play();
	}
}
