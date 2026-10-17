public class SirenEmp : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.SphereCollider sphereCollider;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve animCurve;

	private float targetRadius = 60f;

	private global::System.Collections.Generic.HashSet<ShipBase> overLapCheckSet = new global::System.Collections.Generic.HashSet<ShipBase>();

	public int damage;

	private void Awake()
	{
		sphereCollider = GetComponent<global::UnityEngine.SphereCollider>();
	}

	private void OnEnable()
	{
		sphereCollider.enabled = true;
		sphereCollider.radius = 0f;
		overLapCheckSet.Clear();
		StartCoroutine(FixedUpdateRutine());
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
		yield return new global::UnityEngine.WaitForSeconds(1f);
		base.gameObject.SetActive(value: false);
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
