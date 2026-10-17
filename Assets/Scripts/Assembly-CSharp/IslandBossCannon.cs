public class IslandBossCannon : global::UnityEngine.MonoBehaviour
{
	private const float limitRange = 50f;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private global::UnityEngine.Transform myCannonTrans;

	private global::UnityEngine.Transform fireTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject projectileSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject warnObjSrc;

	private bool isFireReady;

	private float totalTime;

	private float coolTime = 2f;

	public int damage;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		myCannonTrans = transform.Find("boss_island_cannon");
		fireTrans = new global::UnityEngine.GameObject("fireTrans").transform;
		fireTrans.SetParent(myCannonTrans);
		fireTrans.localPosition = new global::UnityEngine.Vector3(0f, 3.54f, 2.81f);
		fireTrans.localRotation = global::UnityEngine.Quaternion.identity;
	}

	private void OnEnable()
	{
		StartCoroutine(ForwardFollow());
	}

	private void Update()
	{
		if (!isFireReady)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime >= coolTime)
			{
				totalTime -= coolTime;
				isFireReady = true;
			}
		}
	}

	private global::System.Collections.IEnumerator ForwardFollow()
	{
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		ShipBase playerShip = GameManager.Instance.playerShip;
		while (true)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 vector = playerShip.transform.position - transform.position;
			float sqrMagnitude = vector.sqrMagnitude;
			if (2500f > sqrMagnitude)
			{
				vector.y = 0f;
				vector.Normalize();
				if (global::UnityEngine.Vector3.Angle(transform.forward, vector) > 45f)
				{
					yield return null;
					continue;
				}
				myCannonTrans.transform.forward = global::UnityEngine.Vector3.Lerp(myCannonTrans.transform.forward, vector, global::UnityEngine.Time.deltaTime * 5f);
				if (isFireReady)
				{
					global::UnityEngine.Vector3 targetPos = playerShip.transform.position;
					if (global::UnityEngine.Random.value <= 0.5f)
					{
						targetPos += playerShip.rigidbody.velocity * 2.5f;
					}
					global::UnityEngine.GameObject warnObj = global::UnityEngine.Object.Instantiate(warnObjSrc, targetPos + new global::UnityEngine.Vector3(0f, 0.1f, 0f), global::UnityEngine.Quaternion.Euler(90f, 0f, 0f));
					yield return new global::UnityEngine.WaitForSeconds(2f);
					global::UnityEngine.Object.Destroy(warnObj);
					global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(projectileSrc, fireTrans.position, global::UnityEngine.Quaternion.identity);
					Barrel component = obj.GetComponent<Barrel>();
					obj.GetComponent<Hit>().damage = damage;
					component.isDestroy = true;
					float maxHeight = transform.position.y + 10f;
					component.SetMaxHeight(maxHeight);
					component.SetMaxHeightTime(0.5f);
					component.Fire(fireTrans.position, targetPos);
					isFireReady = false;
				}
			}
			yield return wait;
		}
	}
}
