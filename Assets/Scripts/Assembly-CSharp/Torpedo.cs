public class Torpedo : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.AnimationCurve veloAnimCurve;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject waterSplashObj;

	[global::UnityEngine.HideInInspector]
	public Hit myHit;

	private global::UnityEngine.Collider myCollider;

	private bool isCollision;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		myCollider = GetComponent<global::UnityEngine.Collider>();
		myHit = GetComponent<Hit>();
		waterSplashObj.SetActive(value: false);
		Hit hit = myHit;
		hit.hittingAction = (global::System.Action)global::System.Delegate.Combine(hit.hittingAction, new global::System.Action(Collision));
	}

	private void OnEnable()
	{
		StartCoroutine(Acceleration());
	}

	private void Collision()
	{
		modelTrans.gameObject.SetActive(value: false);
		myCollider.enabled = false;
		rigidbody.velocity = global::UnityEngine.Vector3.zero;
		waterSplashObj.SetActive(value: true);
		CameraShakeManager.Instance.CameraShake(0.3f, 1f);
		global::UnityEngine.Object.Destroy(base.gameObject, 2f);
		StopCoroutine(Acceleration());
		isCollision = true;
	}

	private global::System.Collections.IEnumerator Acceleration()
	{
		float totalTime = 0f;
		float duration = 0.8f;
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			if (isCollision)
			{
				yield break;
			}
			totalTime += global::UnityEngine.Time.fixedDeltaTime;
			float time = totalTime / duration;
			time = veloAnimCurve.Evaluate(time);
			float num = global::UnityEngine.Mathf.Lerp(0f, 150f, time);
			rigidbody.velocity = transform.forward * num;
			if (time >= 1f)
			{
				break;
			}
			yield return wait;
		}
		yield return new global::UnityEngine.WaitForSeconds(3f);
		modelTrans.gameObject.SetActive(value: false);
		myCollider.enabled = false;
		rigidbody.velocity = global::UnityEngine.Vector3.zero;
		yield return new global::UnityEngine.WaitForSeconds(2f);
		global::UnityEngine.Object.Destroy(base.gameObject);
	}
}
