public class BGMManager : global::UnityEngine.MonoBehaviour
{
	private static BGMManager sInstance;

	private global::System.Collections.Generic.Dictionary<string, global::UnityEngine.AudioClip> bgmTable;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioSource audio0;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioSource audio1;

	private float crossFadeTime = 5f;

	[global::UnityEngine.Range(0f, 1f)]
	public float masterVolume = 1f;

	private float volume0;

	private float volume1;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Audio.AudioMixer audioMixer;

	private global::System.Collections.Generic.Dictionary<string, int> temp = new global::System.Collections.Generic.Dictionary<string, int>();

	public static BGMManager Instance
	{
		get
		{
			if (sInstance == null)
			{
				sInstance = new global::UnityEngine.GameObject("_BGMManager").AddComponent<BGMManager>();
			}
			return sInstance;
		}
	}

	public global::UnityEngine.AudioSource AUDIOSOURCE => audio0;

	private void Awake()
	{
		if (sInstance != null)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		sInstance = this;
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		bgmTable = new global::System.Collections.Generic.Dictionary<string, global::UnityEngine.AudioClip>();
		if (audio0 == null)
		{
			audio0 = base.gameObject.AddComponent<global::UnityEngine.AudioSource>();
		}
		if (audio1 == null)
		{
			audio1 = base.gameObject.AddComponent<global::UnityEngine.AudioSource>();
		}
		audio0.loop = true;
		audio1.loop = true;
		audio0.volume = 0f;
		audio1.volume = 0f;
		Load();
	}

	private void Load()
	{
		LoadBGM("wave_loop_freesound");
		LoadBGM("bgm_Adventure_Theme");
	}

	private void LoadBGM(string name)
	{
		if (!bgmTable.ContainsKey(name))
		{
			global::UnityEngine.AudioClip audioClip = global::UnityEngine.Resources.Load("Sound/BGM/" + name) as global::UnityEngine.AudioClip;
			if (!(audioClip == null))
			{
				bgmTable.Add(name, audioClip);
			}
		}
	}

	private void Update()
	{
		audio0.volume = volume0 * masterVolume;
		audio1.volume = volume1 * masterVolume;
	}

	public void StopBGM()
	{
		if (audioMixer != null)
		{
			audioMixer.SetFloat("bgmVolume", -80f);
		}
		if (audio0.isPlaying)
		{
			audio0.Stop();
			audio0.clip = null;
		}
		if (audio1.isPlaying)
		{
			audio1.Stop();
			audio1.clip = null;
		}
	}

	public void PauseBGM()
	{
		audioMixer.SetFloat("bgmVolume", -80f);
		if (audio0.isPlaying)
		{
			audio0.Pause();
		}
		if (audio1.isPlaying)
		{
			audio1.Pause();
		}
	}

	public bool UnPauseBGM()
	{
		bool result = false;
		audioMixer.SetFloat("bgmVolume", 0f);
		if (audio0.clip != null)
		{
			audio0.UnPause();
			result = true;
		}
		if (audio1.clip != null)
		{
			audio1.UnPause();
		}
		return result;
	}

	public void PlayBGM(string bgmName)
	{
		audioMixer.SetFloat("bgmVolume", 0f);
		if (!bgmTable.ContainsKey(bgmName))
		{
			global::UnityEngine.AudioClip audioClip = global::UnityEngine.Resources.Load("BGM/" + bgmName) as global::UnityEngine.AudioClip;
			if (audioClip == null)
			{
				return;
			}
			bgmTable.Add(bgmName, audioClip);
		}
		if (!(audio0.clip != null) || !(audio0.clip == bgmTable[bgmName]))
		{
			audio0.clip = bgmTable[bgmName];
			audio0.Play();
			volume0 = 1f;
			volume1 = 0f;
		}
	}

	public void PlayBGM(string bgmName, string bgmName2)
	{
		audioMixer.SetFloat("bgmVolume", 1f);
		if (!bgmTable.ContainsKey(bgmName))
		{
			global::UnityEngine.AudioClip audioClip = global::UnityEngine.Resources.Load("BGM/" + bgmName) as global::UnityEngine.AudioClip;
			if (audioClip == null)
			{
				return;
			}
			bgmTable.Add(bgmName, audioClip);
		}
		if (!bgmTable.ContainsKey(bgmName2))
		{
			global::UnityEngine.AudioClip audioClip2 = global::UnityEngine.Resources.Load("BGM/" + bgmName2) as global::UnityEngine.AudioClip;
			if (audioClip2 == null)
			{
				return;
			}
			bgmTable.Add(bgmName2, audioClip2);
		}
		if ((!(audio0.clip != null) || !(audio0.clip == bgmTable[bgmName])) && (!(audio1.clip != null) || !(audio1.clip == bgmTable[bgmName2])))
		{
			audio0.clip = bgmTable[bgmName];
			audio0.Play();
			audio1.clip = bgmTable[bgmName2];
			audio1.Play();
			volume0 = 1f;
			volume1 = 1f;
		}
	}

	public void CrossFadeBGM(string bgmName, float crossFadeTime = 1f)
	{
		if (!bgmTable.ContainsKey(bgmName))
		{
			global::UnityEngine.AudioClip audioClip = global::UnityEngine.Resources.Load("BGM/" + bgmName) as global::UnityEngine.AudioClip;
			if (audioClip == null)
			{
				return;
			}
			bgmTable.Add(bgmName, audioClip);
		}
		if (!(audio0.clip != null) || !(audio0.clip == bgmTable[bgmName]))
		{
			this.crossFadeTime = crossFadeTime;
			global::UnityEngine.AudioSource audioSource = audio0;
			audio0 = audio1;
			audio1 = audioSource;
			float num = volume0;
			volume0 = volume1;
			volume1 = num;
			audio0.clip = bgmTable[bgmName];
			audio0.Play();
		}
	}
}
