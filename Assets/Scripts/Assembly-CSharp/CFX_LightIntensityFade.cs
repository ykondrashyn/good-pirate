[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.Light))]
public class CFX_LightIntensityFade : global::UnityEngine.MonoBehaviour
{
	public float duration = 1f;

	public float delay;

	public float finalIntensity;

	private float baseIntensity;

	public bool autodestruct;

	private float p_lifetime;

	private float p_delay;

	private void Start()
	{
		baseIntensity = GetComponent<global::UnityEngine.Light>().intensity;
	}

	private void OnEnable()
	{
		p_lifetime = 0f;
		p_delay = delay;
		if (delay > 0f)
		{
			GetComponent<global::UnityEngine.Light>().enabled = false;
		}
	}

	private void Update()
	{
		if (p_delay > 0f)
		{
			p_delay -= global::UnityEngine.Time.deltaTime;
			if (p_delay <= 0f)
			{
				GetComponent<global::UnityEngine.Light>().enabled = true;
			}
		}
		else if (p_lifetime / duration < 1f)
		{
			GetComponent<global::UnityEngine.Light>().intensity = global::UnityEngine.Mathf.Lerp(baseIntensity, finalIntensity, p_lifetime / duration);
			p_lifetime += global::UnityEngine.Time.deltaTime;
		}
		else if (autodestruct)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
