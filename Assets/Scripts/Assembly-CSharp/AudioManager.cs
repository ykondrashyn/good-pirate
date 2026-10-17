public class AudioManager : global::UnityEngine.MonoBehaviour
{
	public bool sfxOff;

	private static AudioManager sInstance;

	public global::UnityEngine.AudioSource sfxSource;

	public global::UnityEngine.AudioSource sfxSource2;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Audio.AudioMixer audioMixer;

	private global::System.Collections.Generic.Dictionary<string, Audio> sfxMap;

	public static AudioManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("_AudioMananger");
				global::UnityEngine.GameObject obj = new global::UnityEngine.GameObject("_SFXSource");
				global::UnityEngine.GameObject gameObject2 = new global::UnityEngine.GameObject("_SFXSource2");
				obj.transform.SetParent(gameObject.transform);
				gameObject2.transform.SetParent(gameObject.transform);
				global::UnityEngine.AudioSource audioSource = obj.AddComponent<global::UnityEngine.AudioSource>();
				global::UnityEngine.AudioSource audioSource2 = gameObject2.AddComponent<global::UnityEngine.AudioSource>();
				audioSource.playOnAwake = false;
				audioSource2.playOnAwake = false;
				gameObject.AddComponent<AudioManager>();
			}
			return sInstance;
		}
	}

	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			global::UnityEngine.Transform transform = base.transform.Find("_SFXSource");
			global::UnityEngine.Transform transform2 = base.transform.Find("_SFXSource2");
			if (transform == null)
			{
				global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("_SFXSource");
				gameObject.transform.SetParent(base.transform);
				sfxSource = gameObject.AddComponent<global::UnityEngine.AudioSource>();
				sfxSource.playOnAwake = false;
			}
			else
			{
				sfxSource = transform.GetComponent<global::UnityEngine.AudioSource>();
				sfxSource.playOnAwake = false;
			}
			if (transform2 == null)
			{
				global::UnityEngine.GameObject gameObject2 = new global::UnityEngine.GameObject("_SFXSource2");
				gameObject2.transform.SetParent(base.transform);
				sfxSource2 = gameObject2.AddComponent<global::UnityEngine.AudioSource>();
				sfxSource2.playOnAwake = false;
			}
			else
			{
				sfxSource2 = transform2.GetComponent<global::UnityEngine.AudioSource>();
				sfxSource2.playOnAwake = false;
			}
			sfxSource2.priority = 128;
			sfxSource.priority = 10;
			sfxMap = new global::System.Collections.Generic.Dictionary<string, Audio>();
			LoadSound();
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public bool ReadySFX(string name, float volume = 1f, int priority = 2)
	{
		if (!sfxMap.ContainsKey(name))
		{
			global::UnityEngine.AudioClip audioClip = global::UnityEngine.Resources.Load<global::UnityEngine.AudioClip>("Sound/SFX/" + name);
			if (audioClip != null)
			{
				Audio value = new Audio
				{
					clip = audioClip,
					volume = volume,
					priority = priority
				};
				sfxMap.Add(name, value);
				return true;
			}
		}
		return false;
	}

	public void PlaySFX(string name)
	{
		if (sfxOff)
		{
			return;
		}
		if (!sfxMap.ContainsKey(name))
		{
			if (ReadySFX(name))
			{
				switch (sfxMap[name].priority)
				{
				case 1:
					sfxSource.PlayOneShot(sfxMap[name].clip, sfxMap[name].volume);
					break;
				case 2:
					sfxSource2.PlayOneShot(sfxMap[name].clip, sfxMap[name].volume);
					break;
				}
			}
		}
		else
		{
			switch (sfxMap[name].priority)
			{
			case 1:
				sfxSource.PlayOneShot(sfxMap[name].clip, sfxMap[name].volume);
				break;
			case 2:
				sfxSource2.PlayOneShot(sfxMap[name].clip, sfxMap[name].volume);
				break;
			}
		}
	}

	public void SFXStop()
	{
		audioMixer.SetFloat("sfxVolume", -80f);
		if (audioMixer.GetFloat("audioMixer", out var value))
		{
			global::UnityEngine.Debug.Log("sfx volume" + value);
		}
		sfxSource.Stop();
		sfxOff = true;
	}

	public void SFXStart()
	{
		audioMixer.SetFloat("sfxVolume", 0f);
		sfxOff = false;
	}

	private void LoadSound()
	{
		ReadySFX("EXPLOSION_Short_Smooth_Deep_stereo");
		ReadySFX("error");
		ReadySFX("island_win");
		ReadySFX("item_root");
		ReadySFX("player_die");
		ReadySFX("ui_complete_rewardbox");
		ReadySFX("ui_freebox_explo");
		ReadySFX("ui_freebox_shake");
		ReadySFX("ui_get_rewardbox");
		ReadySFX("ui_success");
		ReadySFX("ui_upgrade");
		ReadySFX("use_diamond");
		ReadySFX("chestboxOpen_0");
		ReadySFX("chestboxOpen_1");
		ReadySFX("ship_spawn");
		ReadySFX("ship_spawn_request");
		ReadySFX("journey", 0.9f, 1);
		ReadySFX("spark_hit");
	}

	public static void PlayButtonClick()
	{
		Instance.PlaySFX("ui_click");
	}
}
