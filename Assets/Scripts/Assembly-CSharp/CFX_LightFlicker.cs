[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.Light))]
public class CFX_LightFlicker : global::UnityEngine.MonoBehaviour
{
	public bool loop;

	public float smoothFactor = 1f;

	public float addIntensity = 1f;

	private float minIntensity;

	private float maxIntensity;

	private float baseIntensity;

	private void Awake()
	{
		baseIntensity = GetComponent<global::UnityEngine.Light>().intensity;
	}

	private void OnEnable()
	{
		minIntensity = baseIntensity;
		maxIntensity = minIntensity + addIntensity;
	}

	private void Update()
	{
		GetComponent<global::UnityEngine.Light>().intensity = global::UnityEngine.Mathf.Lerp(minIntensity, maxIntensity, global::UnityEngine.Mathf.PerlinNoise(global::UnityEngine.Time.time * smoothFactor, 0f));
	}
}
