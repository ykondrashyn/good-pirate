public class BarrelGas : global::UnityEngine.MonoBehaviour
{
	private global::System.Collections.Generic.Dictionary<Health, float> shipDic = new global::System.Collections.Generic.Dictionary<Health, float>();

	private int enemyProjectileLayer;

	private int playerProjectileLayer;

	public int damage;

	private const float intervalDamage = 1f;

	private void Awake()
	{
		enemyProjectileLayer = global::UnityEngine.LayerMask.NameToLayer("EnemyProjectile");
		playerProjectileLayer = global::UnityEngine.LayerMask.NameToLayer("PlayerProjectile");
		Invoke("SelfDestroy", 12f);
	}

	private void SelfDestroy()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetLayer(ELayerType layerType)
	{
		switch (layerType)
		{
		case ELayerType.Enemy:
			base.gameObject.layer = enemyProjectileLayer;
			break;
		case ELayerType.Player:
			base.gameObject.layer = playerProjectileLayer;
			break;
		}
	}

	public void SetLayer(int layer)
	{
		base.gameObject.layer = layer;
	}

	private void Update()
	{
		foreach (Health item in global::System.Linq.Enumerable.ToList(shipDic.Keys))
		{
			if (item != null)
			{
				shipDic[item] += global::UnityEngine.Time.deltaTime;
				if (shipDic[item] >= 1f)
				{
					shipDic[item] -= 1f;
					item.Hit(damage);
					UI_Manager.Instance.DamageUIGenerate(EDamageType.NORMAL, item.transform.position, damage);
				}
			}
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		Health componentInParent = other.GetComponentInParent<Health>();
		if (componentInParent != null && !shipDic.ContainsKey(componentInParent))
		{
			shipDic.Add(componentInParent, 0f);
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		Health componentInParent = other.GetComponentInParent<Health>();
		if (componentInParent != null && shipDic.ContainsKey(componentInParent))
		{
			shipDic.Remove(componentInParent);
		}
	}
}
