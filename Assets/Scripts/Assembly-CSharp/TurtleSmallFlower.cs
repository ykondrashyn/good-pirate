public class TurtleSmallFlower : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Animator anim;

	public int damage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject projectileSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform warningObj;

	private global::UnityEngine.Vector3 endPos;

	private bool isFireReady;

	private float totalTime;

	private float coolTime = 4f;

	private const float limitRange = 50f;

	private void Awake()
	{
		anim = GetComponent<global::UnityEngine.Animator>();
		warningObj.gameObject.SetActive(value: false);
	}

	public void FireTrigger()
	{
	}

	public void SeedFire()
	{
		Barrel component = global::UnityEngine.Object.Instantiate(projectileSrc, base.transform.position, global::UnityEngine.Quaternion.identity).GetComponent<Barrel>();
		warningObj.parent = base.transform;
		warningObj.gameObject.SetActive(value: false);
		component.isDestroy = true;
		component.SetMaxHeightTime(0.4f);
		component.SetMaxHeight(12f);
		component.damage = damage;
		component.Fire(base.transform.position, endPos);
		isFireReady = false;
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

	private global::System.Collections.IEnumerator Start()
	{
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		yield return new global::UnityEngine.WaitUntil(() => GameManager.Instance != null);
		ShipBase playerShip = GameManager.Instance.playerShip;
		while (true)
		{
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			global::UnityEngine.Vector3 to = playerShip.transform.position - base.transform.position;
			float sqrMagnitude = to.sqrMagnitude;
			if (2500f > sqrMagnitude)
			{
				to.y = 0f;
				to.Normalize();
				global::UnityEngine.Vector3 up = base.transform.up;
				up.y = 0f;
				if (global::UnityEngine.Vector3.Angle(up.normalized, to) > 25f)
				{
					yield return null;
					continue;
				}
				if (isFireReady)
				{
					anim.SetTrigger("Attack");
					global::UnityEngine.Vector3 position = playerShip.transform.position;
					if (global::UnityEngine.Random.value <= 0.5f)
					{
						position += playerShip.rigidbody.velocity * 2.5f;
					}
					endPos = position;
					warningObj.gameObject.SetActive(value: true);
					warningObj.parent = null;
					warningObj.position = endPos + new global::UnityEngine.Vector3(0f, 0.05f, 0f);
					warningObj.transform.position = position + new global::UnityEngine.Vector3(0f, 0.1f, 0f);
					warningObj.rotation = global::UnityEngine.Quaternion.Euler(90f, 0f, 0f);
					yield return new global::UnityEngine.WaitUntil(() => !isFireReady);
				}
			}
			yield return wait;
		}
	}
}
