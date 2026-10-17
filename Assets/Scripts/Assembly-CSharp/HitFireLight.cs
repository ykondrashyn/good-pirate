public class HitFireLight : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Light myLight;

	private void Awake()
	{
		myLight = GetComponent<global::UnityEngine.Light>();
	}

	private void OnEnable()
	{
		global::DG.Tweening.ShortcutExtensions.DOKill(myLight);
		myLight.intensity = 40f;
		global::DG.Tweening.ShortcutExtensions.DOIntensity(myLight, 0f, 1.5f);
	}
}
