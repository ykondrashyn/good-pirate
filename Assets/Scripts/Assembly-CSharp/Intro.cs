public class Intro : global::UnityEngine.MonoBehaviour
{
	public global::UnityEngine.ParticleSystem ptcl_rain;

	public global::UnityEngine.ParticleSystem ptcl_raindrop;

	public KrakenLeg krakenLegOne;

	public KrakenLeg krakenLegTwo;

	public TurtleLeaf[] turtleLeaf;

	public global::UnityEngine.ParticleSystem[] ptcl_pirate_fire_R;

	public global::UnityEngine.ParticleSystem[] ptcl_pirate_fire_L;

	public global::UnityEngine.ParticleSystem[] ptcl_navy_fire_R;

	public global::UnityEngine.ParticleSystem[] ptcl_navy_fire_L;

	public global::UnityEngine.ParticleSystem[] ptcl_player0_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_player1_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_player2_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_player3_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_navy0_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_navy1_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_navy2_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_navy3_burn;

	public global::UnityEngine.ParticleSystem[] ptcl_burn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioSource BGMAudioSource;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip BGM;

	[global::UnityEngine.Header("thunders")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image thunderImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Light directionalLight;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioSource effectAudioSource;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip thunderClip;

	public global::UnityEngine.ParticleSystem ptclShining;

	public global::UnityEngine.ParticleSystem ptclGoldFadeout;

	private global::System.Collections.IEnumerator Start()
	{
		global::UnityEngine.Animator anim = GetComponent<global::UnityEngine.Animator>();
		yield return null;
		while (!(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		PlayerInfo.Instance.myData.isShowIntro = true;
		PlayerInfo.Instance.SaveData();
		Appsflyer.Instance.SendEvent("tutorial_Start");
		global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
	}

	public void BGMStart()
	{
		BGMAudioSource.PlayOneShot(BGM);
	}

	public void RainStart()
	{
		ptcl_rain.gameObject.SetActive(value: true);
		ptcl_raindrop.gameObject.SetActive(value: true);
	}

	public void RatinStop()
	{
		ptcl_rain.gameObject.SetActive(value: false);
		ptcl_raindrop.gameObject.SetActive(value: false);
	}

	public void KrakenAttackOne()
	{
		krakenLegOne.AttackTrigger(1);
	}

	public void KrakenAttackTwo()
	{
		krakenLegTwo.AttackTrigger(2);
	}

	public void TurtleAttack()
	{
		StartCoroutine(TurtleAttackCor());
	}

	private global::System.Collections.IEnumerator TurtleAttackCor()
	{
		while (true)
		{
			for (int i = 0; i < turtleLeaf.Length; i++)
			{
				turtleLeaf[i].Attack();
				yield return new global::UnityEngine.WaitForSeconds(0.1f);
				if (i == turtleLeaf.Length - 1)
				{
					yield break;
				}
			}
			yield return null;
		}
	}

	public void PirateFireRight(int index)
	{
		ptcl_pirate_fire_R[index].Play();
		CannonSound();
	}

	public void PirateFireLeft(int index)
	{
		ptcl_pirate_fire_L[index].Play();
		CannonSound();
	}

	public void NavFireRight(int index)
	{
		ptcl_navy_fire_R[index].Play();
		CannonSound();
	}

	public void NavFireLeft(int index)
	{
		ptcl_navy_fire_L[index].Play();
		CannonSound();
	}

	public void Player0_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_player0_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Player0_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_player0_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Player1_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_player1_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Player1_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_player1_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Player2_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_player2_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Player2_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_player2_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Player3_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_player3_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Player3_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_player3_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void CannonSound()
	{
		AudioManager.Instance.PlaySFX("EXPLOSION_Recorded_03_stereo");
	}

	public void Navy0_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_navy0_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Navy0_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_navy0_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Navy1_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_navy1_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Navy1_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_navy1_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Navy2_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_navy2_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Navy2_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_navy2_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Navy3_burn(int level)
	{
		for (int i = 0; i <= level; i++)
		{
			ptcl_navy3_burn[i].gameObject.SetActive(value: true);
		}
	}

	public void Navy3_burn_off()
	{
		for (int i = 0; i < ptcl_player0_burn.Length; i++)
		{
			ptcl_navy3_burn[i].gameObject.SetActive(value: false);
		}
	}

	public void Thunder()
	{
		StartCoroutine(ThunderCoroutine());
		Invoke("ThunderSound", 0.1f);
	}

	private void ThunderSound()
	{
		effectAudioSource.PlayOneShot(thunderClip);
	}

	private global::System.Collections.IEnumerator ThunderCoroutine()
	{
		thunderImage.gameObject.SetActive(value: true);
		thunderImage.color = new global::UnityEngine.Color(1f, 1f, 1f, 0.745f);
		global::UnityEngine.Color prevColor = directionalLight.color;
		directionalLight.intensity = 60f;
		directionalLight.color = global::UnityEngine.Color.white;
		if (global::UnityEngine.Random.value <= 0.5f)
		{
			yield return new global::UnityEngine.WaitForSeconds(0.07f);
			directionalLight.intensity = 1f;
			directionalLight.color = prevColor;
			thunderImage.gameObject.SetActive(value: false);
			yield return new global::UnityEngine.WaitForSeconds(0.07f);
			thunderImage.gameObject.SetActive(value: true);
			directionalLight.intensity = 60f;
			directionalLight.color = global::UnityEngine.Color.white;
		}
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.DOTweenModuleUI.DOColor(thunderImage, new global::UnityEngine.Color(1f, 1f, 1f, 0f), 2f), 0.1f), delegate
		{
			thunderImage.gameObject.SetActive(value: false);
		});
		global::DG.Tweening.TweenSettingsExtensions.SetDelay(global::DG.Tweening.ShortcutExtensions.DOColor(directionalLight, prevColor, 2f), 0.1f);
		yield return new global::UnityEngine.WaitForSeconds(0.1f);
		float totalTime = 0f;
		float duration = 2f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			directionalLight.intensity = global::UnityEngine.Mathf.Lerp(60f, 1.2f, num);
			if (!(num > 1f))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public void PtclBrunETC(int index)
	{
		StartCoroutine(PtclBurnETCCor(index));
	}

	private global::System.Collections.IEnumerator PtclBurnETCCor(int index)
	{
		float total = 0f;
		float duration = 4f;
		ptcl_burn[index].gameObject.SetActive(value: true);
		while (true)
		{
			total += global::UnityEngine.Time.deltaTime;
			if (total >= duration)
			{
				break;
			}
			yield return null;
		}
		ptcl_burn[index].gameObject.SetActive(value: false);
	}

	public void ParticleShiningPlay()
	{
		if (!ptclShining.gameObject.activeSelf)
		{
			ptclShining.gameObject.SetActive(value: true);
		}
		ptclShining.Play();
	}

	public void ParticleGoldFadeOut()
	{
		if (!ptclGoldFadeout.gameObject.activeSelf)
		{
			ptclGoldFadeout.gameObject.SetActive(value: true);
		}
		ptclGoldFadeout.Play();
	}
}
