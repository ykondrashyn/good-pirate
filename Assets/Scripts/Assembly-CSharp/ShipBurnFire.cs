public class ShipBurnFire : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<global::UnityEngine.GameObject> fireParticleObjs = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	private Health myHealth;

	private global::UnityEngine.ParticleSystem[] particles;

	private int defaultLayer;

	private void Awake()
	{
		defaultLayer = global::UnityEngine.LayerMask.NameToLayer("Default");
		myHealth = GetComponentInParent<Health>();
		if (myHealth != null)
		{
			myHealth.AddHitAction(HpCheck);
			myHealth.AddHealAction(HpCheck);
			myHealth.AddHpSetAction(HpCheck);
		}
		particles = GetComponentsInChildren<global::UnityEngine.ParticleSystem>();
	}

	private void Start()
	{
		foreach (global::UnityEngine.GameObject fireParticleObj in fireParticleObjs)
		{
			fireParticleObj.SetActive(value: false);
		}
		StartCoroutine(NextFrame());
	}

	private global::System.Collections.IEnumerator NextFrame()
	{
		yield return null;
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = defaultLayer;
		}
	}

	private void HpCheck()
	{
		float num = 1f / (float)(fireParticleObjs.Count + 1);
		int num2 = (int)(myHealth.HpRatio * 150f) / (int)(num * 100f);
		int num3 = fireParticleObjs.Count - num2;
		for (int i = 0; i < fireParticleObjs.Count; i++)
		{
			if (i < num3)
			{
				if (!fireParticleObjs[i].activeSelf)
				{
					fireParticleObjs[i].SetActive(value: true);
				}
			}
			else if (fireParticleObjs[i].activeSelf)
			{
				fireParticleObjs[i].SetActive(value: false);
			}
		}
		global::UnityEngine.ParticleSystem[] array = particles;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].gameObject.layer = defaultLayer;
		}
	}
}
