public class KrakenMiniBomb : global::UnityEngine.MonoBehaviour
{
	public int damage;

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
		if (!(shipBase == null))
		{
			shipBase.myHealth.Hit(damage);
			if (!UI_Manager.Instance.inkObj.activeSelf)
			{
				UI_Manager.Instance.inkObj.SetActive(value: true);
			}
			GetComponentInParent<KrakenMini>().Explosion();
		}
	}
}
