public class KrakenMini : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Rigidbody rigidbody;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject splashPaticleObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject explosionParticleObj;

	private global::UnityEngine.Collider[] allColliders;

	private int damage;

	private bool isDie;

	public float acceleration;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		rigidbody = GetComponent<global::UnityEngine.Rigidbody>();
		GetComponent<Health>().AddDieAction(Die);
		allColliders = GetComponentsInChildren<global::UnityEngine.Collider>();
		int num = PlayerInfo.Instance.myData.mainShipLevel;
		num = global::UnityEngine.Mathf.Clamp(num - 1, 0, 999);
		acceleration = ShipDataManager.Instance.mainShipData.dataArray[num].Acceleration;
	}

	public void SetDamage(int dmg)
	{
		damage = dmg;
		GetComponentInChildren<KrakenMiniBomb>().damage = damage;
	}

	private void Die()
	{
		if (!isDie)
		{
			isDie = true;
			StopCoroutine("MoveToPlayer");
			rigidbody.velocity = global::UnityEngine.Vector3.zero;
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOShakePosition(modelTrans, 1.5f, 0.4f, 50), delegate
			{
				Explosion();
			});
		}
	}

	private void OnEnable()
	{
		modelTrans.localPosition = new global::UnityEngine.Vector3(0f, -5f, 0f);
		global::DG.Tweening.ShortcutExtensions.DOMoveY(modelTrans, 0f, 0.3f);
		StopCoroutine("MoveToPlayer");
		StartCoroutine("MoveToPlayer");
	}

	private global::System.Collections.IEnumerator MoveToPlayer()
	{
		yield return new global::UnityEngine.WaitForSeconds(1.5f);
		splashPaticleObj.SetActive(value: false);
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			if (GameManager.Instance.playerShip == null)
			{
				yield return null;
				continue;
			}
			rigidbody.AddForce(transform.forward * acceleration, global::UnityEngine.ForceMode.Acceleration);
			global::UnityEngine.Vector3 vector = GameManager.Instance.playerShip.transform.position - transform.position;
			transform.forward = global::UnityEngine.Vector3.Lerp(transform.forward, vector.normalized, global::UnityEngine.Time.fixedDeltaTime * 2f);
			yield return wait;
		}
	}

	public void Explosion()
	{
		explosionParticleObj.SetActive(value: true);
		global::UnityEngine.Collider[] array = allColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		modelTrans.gameObject.SetActive(value: false);
		global::UnityEngine.Object.Destroy(base.gameObject, 3f);
	}
}
