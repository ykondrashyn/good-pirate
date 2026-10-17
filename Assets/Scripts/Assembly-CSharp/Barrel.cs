public class Barrel : Projectile
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject gasSmokeParticleSrc;

	private global::UnityEngine.Vector3 startPos;

	private global::UnityEngine.Vector3 destPos;

	private float vz;

	private float vx;

	private float vy;

	private float g;

	private float dat;

	private float mh;

	private float dh;

	private float t;

	private float mht = 0.9f;

	private float MAX_Y = 13f;

	private bool isTriggerOn;

	[global::UnityEngine.HideInInspector]
	public bool isDestroy;

	private void PreCalculate()
	{
		dh = destPos.y - startPos.y;
		mh = MAX_Y - startPos.y;
		g = 2f * mh / (mht * mht);
		vy = global::UnityEngine.Mathf.Sqrt(2f * g * mh);
		float num = g;
		float num2 = -2f * vy;
		float num3 = 2f * dh;
		dat = (0f - num2 + global::UnityEngine.Mathf.Sqrt(num2 * num2 - 4f * num * num3)) / (2f * num);
		vx = (0f - (startPos.x - destPos.x)) / dat;
		vz = (0f - (startPos.z - destPos.z)) / dat;
	}

	private global::UnityEngine.Vector3 Move()
	{
		t += global::UnityEngine.Time.deltaTime;
		if (t > dat)
		{
			return global::UnityEngine.Vector3.zero;
		}
		float x = startPos.x + vx * t;
		float z = startPos.z + vz * t;
		float y = startPos.y + vy * t - 0.5f * g * t * t;
		return new global::UnityEngine.Vector3(x, y, z);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		isTriggerOn = false;
		t = 0f;
		rigidbody.isKinematic = true;
	}

	private void GenerateGas()
	{
		if (gasSmokeParticleSrc != null)
		{
			global::UnityEngine.Vector3 position = transform.position;
			position.y = 0.5f;
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(gasSmokeParticleSrc, position, global::UnityEngine.Quaternion.identity);
			BarrelGas component = gameObject.GetComponent<BarrelGas>();
			if (component != null)
			{
				component.SetLayer(base.gameObject.layer);
				component.damage = damage;
			}
			else
			{
				gameObject.GetComponent<HitContinuous>().damage = damage;
			}
		}
	}

	public void SetMaxHeightTime(float time = 0.9f)
	{
		mht = time;
	}

	public void SetMaxHeight(float height)
	{
		MAX_Y = height;
	}

	public override void Fire(global::UnityEngine.Vector3 startPos, global::UnityEngine.Vector3 endPos)
	{
		t = 0f;
		this.startPos = startPos;
		destPos = endPos;
		PreCalculate();
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(Parabola());
			CheckFloor();
		}
	}

	protected override void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		isTriggerOn = true;
	}

	private global::System.Collections.IEnumerator Parabola()
	{
		while (true)
		{
			global::UnityEngine.Vector3 vector = Move();
			if (!(vector != global::UnityEngine.Vector3.zero))
			{
				break;
			}
			transform.position = vector;
			transform.Rotate(200f * global::UnityEngine.Time.deltaTime, -400f * global::UnityEngine.Time.deltaTime, 100f * global::UnityEngine.Time.deltaTime, global::UnityEngine.Space.World);
			yield return null;
		}
		global::UnityEngine.GameObject obj = null;
		ObjectPoolManager.TryGetNextObject("WaterSplash03", transform.position, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f), out obj);
		obj.transform.localScale = global::UnityEngine.Vector3.one * 1.5f;
		modelTrans.gameObject.SetActive(value: false);
		GenerateGas();
		yield return new global::UnityEngine.WaitForSeconds(2f);
		if (isDestroy)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
