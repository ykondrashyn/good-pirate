public class SharkMouse : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	private global::UnityEngine.Collider[] colliders;

	private bool isDie;

	private Hit myHit;

	private global::UnityEngine.Animator anim;

	public Health myHealth { get; private set; }

	private void Awake()
	{
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		transform = GetComponent<global::UnityEngine.Transform>();
		myHealth = GetComponent<Health>();
		anim = GetComponentInChildren<global::UnityEngine.Animator>();
		myHit = GetComponentInChildren<Hit>();
		colliders = GetComponentsInChildren<global::UnityEngine.Collider>();
		myHealth.AddDieAction(Die);
		Hit hit = myHit;
		hit.hitAction = (global::System.Action<Health>)global::System.Delegate.Combine(hit.hitAction, new global::System.Action<Health>(HitAction));
	}

	private void Die()
	{
		if (!isDie)
		{
			isDie = true;
			anim.enabled = false;
			global::UnityEngine.Collider[] array = colliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			rigidbody.velocity = global::UnityEngine.Vector3.zero;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOMoveY(transform, -10f, 2f), delegate
			{
				global::UnityEngine.Object.Destroy(base.gameObject, 2f);
			});
		}
	}

	private void HitAction(Health health)
	{
		Die();
	}

	private void Start()
	{
		StartCoroutine(FollowPlayer());
	}

	public void SetDamage(int dmg)
	{
		myHit.damage = dmg;
	}

	private global::System.Collections.IEnumerator FollowPlayer()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		float totalTime = 0f;
		float duration = 2f;
		while (true)
		{
			totalTime += global::UnityEngine.Time.fixedDeltaTime;
			rigidbody.AddForce(transform.forward * 4.7f, global::UnityEngine.ForceMode.Acceleration);
			if (totalTime > duration)
			{
				break;
			}
			yield return wait;
		}
		while (!isDie)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 vector = playerShip.transform.position - transform.position;
			transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.deltaTime);
			rigidbody.AddForce(transform.forward * global::UnityEngine.Random.Range(8f, 20f), global::UnityEngine.ForceMode.Acceleration);
			yield return wait;
		}
	}
}
