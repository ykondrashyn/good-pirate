public class Spark : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform electricSpherePivotTrans;

	private global::UnityEngine.GameObject sparkParticleSrc;

	private const float intervalTime = 0.8f;

	[global::UnityEngine.HideInInspector]
	public int damage;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Collider collider;

	private global::System.Collections.Generic.Dictionary<Health, float> enemyDic = new global::System.Collections.Generic.Dictionary<Health, float>();

	private void Awake()
	{
		sparkParticleSrc = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/skill/SparkHit");
	}

	private void Start()
	{
		collider = GetComponent<global::UnityEngine.Collider>();
		(collider as global::UnityEngine.SphereCollider).radius = ConstValue.CannonRange * (1f / base.transform.localScale.x);
	}

	private void FixedUpdate()
	{
		foreach (Health item in new global::System.Collections.Generic.List<Health>(enemyDic.Keys))
		{
			if (item != null)
			{
				if (!(item.myShip != null) || !item.myShip.IsDie)
				{
					float num = enemyDic[item];
					num += global::UnityEngine.Time.fixedDeltaTime;
					if (num >= 0.8f)
					{
						num -= 0.8f;
						item.Hit(damage);
						global::UnityEngine.Vector3 vector = item.transform.position - electricSpherePivotTrans.position;
						global::UnityEngine.Transform obj = global::UnityEngine.Object.Instantiate(sparkParticleSrc).transform;
						obj.position = item.transform.position;
						obj.forward = vector.normalized;
						float num2 = vector.magnitude / 8f;
						obj.localScale = global::UnityEngine.Vector3.one * num2;
						CameraShakeManager.Instance.CameraShake(0.2f, 1.5f);
						AudioManager.Instance.PlaySFX("spark_hit");
					}
					enemyDic[item] = num;
				}
			}
			else
			{
				enemyDic.Remove(item);
			}
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		Health componentInParent = other.GetComponentInParent<Health>();
		if (componentInParent != null && !enemyDic.ContainsKey(componentInParent))
		{
			enemyDic.Add(componentInParent, 0f);
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		Health componentInParent = other.GetComponentInParent<Health>();
		if (componentInParent != null && enemyDic.ContainsKey(componentInParent))
		{
			enemyDic.Remove(componentInParent);
		}
	}
}
