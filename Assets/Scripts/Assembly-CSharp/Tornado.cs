public class Tornado : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private global::UnityEngine.Vector3 myMoveDir;

	private float moveSpeed = 1f;

	private global::System.Collections.Generic.Dictionary<ShipBase, float> shipDic = new global::System.Collections.Generic.Dictionary<ShipBase, float>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem[] myParticles;

	private const float intervalTime = 0.8f;

	private global::UnityEngine.Collider myCollider;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		myCollider = GetComponent<global::UnityEngine.Collider>();
	}

	private void OnEnable()
	{
		myMoveDir = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward;
		moveSpeed = global::UnityEngine.Random.Range(0.2f, 2f);
		global::UnityEngine.ParticleSystem[] array = myParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		myCollider.enabled = false;
		StartCoroutine(StartRandomDelay());
	}

	public void SetDirection(global::UnityEngine.Vector3 dir)
	{
		myMoveDir = dir;
	}

	private global::System.Collections.IEnumerator StartRandomDelay()
	{
		yield return new global::UnityEngine.WaitForSeconds(global::UnityEngine.Random.Range(0f, 5f));
		myCollider.enabled = true;
		global::UnityEngine.ParticleSystem[] array = myParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		Invoke("InActive", 15f);
	}

	private void InActive()
	{
		shipDic.Clear();
		global::UnityEngine.ParticleSystem[] array = myParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
		myCollider.enabled = false;
		global::UnityEngine.Object.Destroy(base.gameObject, 5f);
	}

	private void FixedUpdate()
	{
		foreach (ShipBase item in global::System.Linq.Enumerable.ToList(shipDic.Keys))
		{
			if (!(item != null))
			{
				continue;
			}
			float num = shipDic[item];
			num += global::UnityEngine.Time.fixedDeltaTime;
			if (num >= 0.8f)
			{
				global::UnityEngine.Vector3 velocity = item.rigidbody.velocity;
				velocity *= 0.15f;
				item.rigidbody.velocity = velocity;
				num -= 0.8f;
				int damage = global::UnityEngine.Mathf.Max(1, item.myHealth.HP_MAX / 100);
				item.myHealth.Hit(damage);
				if (item == GameManager.Instance.playerShip)
				{
					CameraShakeManager.Instance.CameraShake(0.25f, 0.8f);
				}
			}
			shipDic[item] = num;
		}
	}

	private void Update()
	{
		transform.Translate(myMoveDir * global::UnityEngine.Time.deltaTime * moveSpeed, global::UnityEngine.Space.World);
	}

	private void OnTriggerEnter(global::UnityEngine.Collider col)
	{
		ShipBase shipBase = col.GetComponent<ShipBase>();
		if (shipBase == null)
		{
			shipBase = col.GetComponentInParent<ShipBase>();
		}
		if (shipBase == null)
		{
			shipBase = col.GetComponentInChildren<ShipBase>();
		}
		if (!(shipBase == null) && !shipDic.ContainsKey(shipBase))
		{
			shipDic.Add(shipBase, 0f);
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider col)
	{
		ShipBase shipBase = col.GetComponent<ShipBase>();
		if (shipBase == null)
		{
			shipBase = col.GetComponentInParent<ShipBase>();
		}
		if (shipBase == null)
		{
			shipBase = col.GetComponentInChildren<ShipBase>();
		}
		if (!(shipBase == null) && shipDic.ContainsKey(shipBase))
		{
			shipDic.Remove(shipBase);
		}
	}
}
