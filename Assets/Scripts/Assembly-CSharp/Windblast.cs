public class Windblast : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.SphereCollider sphereCollider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject shieldParticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	private float targetRadius = 70f;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform shieldTrans;

	public int damage;

	private global::System.Collections.Generic.HashSet<ShipBase> overLapCheckSet = new global::System.Collections.Generic.HashSet<ShipBase>();

	private void Awake()
	{
		sphereCollider = GetComponent<global::UnityEngine.SphereCollider>();
		StartCoroutine(FixedUpdateRutine());
		CameraShakeManager.Instance.CameraShake(0.6f, 1.5f);
	}

	private void OnEnable()
	{
		GameManager.Instance.playerShip.myHealth.AddHitAction(ShieldCheck);
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerShip != null)
		{
			GameManager.Instance.playerShip.myHealth.RemoveHitAction(ShieldCheck);
		}
	}

	private void ShieldCheck()
	{
		if (GameManager.Instance.playerShip.myHealth.SHIELD_ENERGY <= 0)
		{
			shieldParticleObj.SetActive(value: false);
		}
	}

	public void SetShieldScale(float scale)
	{
		shieldTrans.localScale = global::UnityEngine.Vector3.one * scale;
	}

	private global::System.Collections.IEnumerator FixedUpdateRutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		yield return new global::UnityEngine.WaitForSeconds(0.05f);
		float totalTime = 0f;
		float duration = 3f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.fixedDeltaTime;
			float time = totalTime / duration;
			time = animCurve.Evaluate(time);
			sphereCollider.radius = global::UnityEngine.Mathf.Lerp(0f, targetRadius, time);
			if (totalTime >= duration)
			{
				break;
			}
			yield return wait;
		}
		sphereCollider.enabled = false;
	}

	private void OnCollisionEnter(global::UnityEngine.Collision col)
	{
		ShipBase componentInParent = col.gameObject.GetComponentInParent<ShipBase>();
		if (componentInParent != null && !overLapCheckSet.Contains(componentInParent))
		{
			overLapCheckSet.Add(componentInParent);
			componentInParent.myHealth.Hit(damage);
		}
	}
}
