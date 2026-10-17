public class Cannon : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform modelTrans;

	public int damage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform fireTrans;

	private string poolName = "Projectile";

	private void Awake()
	{
		if (fireTrans == null)
		{
			fireTrans = base.transform;
		}
	}

	public void SetPoolName(string name)
	{
		poolName = name;
	}

	public void Fire(float offsetAngle, ELayerType myType, float forcePower = 40f)
	{
		global::UnityEngine.GameObject obj = null;
		if (ObjectPoolManager.TryGetNextObject(poolName, fireTrans.position, global::UnityEngine.Quaternion.identity, out obj))
		{
			Projectile component = obj.GetComponent<Projectile>();
			component.SetProejctile(myType);
			component.damage = damage;
			float num = ((base.transform.localRotation.y < 0f) ? (0f - offsetAngle) : offsetAngle);
			fireTrans.localRotation = global::UnityEngine.Quaternion.Euler(-13f + num, 0f, 0f);
			component.Fire(fireTrans.forward, forcePower);
		}
	}

	public void SetAngle(float angle)
	{
		if ((bool)modelTrans)
		{
			modelTrans.localRotation = global::UnityEngine.Quaternion.Euler(0f, angle, 0f);
		}
	}
}
