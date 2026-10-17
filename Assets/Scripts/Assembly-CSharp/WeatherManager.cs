public class WeatherManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Camera mainCamera;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Light directionalLight;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color originalCameraColor;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Color originalLightColor;

	public global::System.Collections.Generic.List<Weather> weatherList;

	[global::UnityEngine.SerializeField]
	private EWeatherType nowWeather;

	[global::UnityEngine.Header("오디오소스")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioSource audioSource;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioSource effectAudioSource;

	[global::UnityEngine.Header("클립")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip raindropClip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AudioClip thunderClip;

	[global::UnityEngine.Header("파티클")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem rainFallParticle;

	private global::System.Collections.Generic.Dictionary<EWeatherType, Weather> weatherDic = new global::System.Collections.Generic.Dictionary<EWeatherType, Weather>();

	public bool isPause;

	private void Awake()
	{
		nowWeather = EWeatherType.NORMAL;
		foreach (Weather weather in weatherList)
		{
			weatherDic.Add(weather.weatherType, weather);
		}
	}

	private void Start()
	{
		StartCoroutine(WeatherRutine());
	}

	public void OriginalColorChange(global::UnityEngine.Color camColor, global::UnityEngine.Color lightColor)
	{
		Weather value = weatherDic[EWeatherType.NORMAL];
		value.lightColor = lightColor;
		value.cameraColor = camColor;
		weatherDic[EWeatherType.NORMAL] = value;
	}

	public void ChangeWeather(EWeatherType weather, float crossTime)
	{
		if (nowWeather != EWeatherType.NORMAL)
		{
			weatherDic[nowWeather].endCallBack.Invoke();
		}
		nowWeather = weather;
		StartCoroutine(WeatherChange(crossTime));
	}

	private global::System.Collections.IEnumerator WeatherChange(float duration)
	{
		float totalTime = 0f;
		global::UnityEngine.Color startCamColor = mainCamera.backgroundColor;
		global::UnityEngine.Color startLightColor = directionalLight.color;
		Weather weather = weatherDic[nowWeather];
		weather.startCallBack.Invoke();
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			mainCamera.backgroundColor = global::UnityEngine.Color.Lerp(startCamColor, weather.cameraColor, num);
			directionalLight.color = global::UnityEngine.Color.Lerp(startLightColor, weather.lightColor, num);
			if (!(num > 1f))
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public void RainStart()
	{
		if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
		audioSource.clip = raindropClip;
		audioSource.volume = 0f;
		global::DG.Tweening.DOTweenModuleAudio.DOFade(audioSource, 0.3f, 0.8f);
		audioSource.loop = true;
		audioSource.Play();
		rainFallParticle.gameObject.SetActive(value: true);
		rainFallParticle.Play();
		StopCoroutine("Thunder");
		StartCoroutine("Thunder");
	}

	private global::System.Collections.IEnumerator Thunder()
	{
		float coolTime = 10f;
		float totalTime = 0f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime / coolTime > 1f)
			{
				totalTime -= coolTime;
				coolTime = global::UnityEngine.Random.Range(10f, 20f);
				UI_Manager.Instance.Thunder();
				yield return new global::UnityEngine.WaitForSeconds(global::UnityEngine.Random.Range(0f, 1.5f));
				effectAudioSource.PlayOneShot(thunderClip);
			}
			yield return null;
		}
	}

	public void RainOver()
	{
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.DOTweenModuleAudio.DOFade(audioSource, 0f, 0.4f), delegate
		{
			audioSource.Stop();
		});
		rainFallParticle.Stop();
		StopCoroutine("Thunder");
	}

	private void RainParticleOff()
	{
		rainFallParticle.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.T))
		{
			ChangeWeather(EWeatherType.RAIN, 1f);
		}
		if (global::UnityEngine.Input.GetKeyDown(global::UnityEngine.KeyCode.Y))
		{
			ChangeWeather(EWeatherType.NORMAL, 1f);
		}
	}

	private global::System.Collections.IEnumerator WeatherRutine()
	{
		while (true)
		{
			float coolTime = global::UnityEngine.Random.Range(180f, 200f);
			float totaltTime = 0f;
			while (true)
			{
				if (isPause)
				{
					yield return null;
					continue;
				}
				totaltTime += global::UnityEngine.Time.deltaTime;
				if (totaltTime >= coolTime)
				{
					break;
				}
				yield return null;
			}
			ChangeWeather(EWeatherType.RAIN, 1f);
			coolTime = global::UnityEngine.Random.Range(30f, 60f);
			totaltTime = 0f;
			while (true)
			{
				if (isPause)
				{
					yield return null;
					continue;
				}
				totaltTime += global::UnityEngine.Time.deltaTime;
				if (totaltTime >= coolTime)
				{
					break;
				}
				yield return null;
			}
			ChangeWeather(EWeatherType.NORMAL, 1f);
		}
	}
}
