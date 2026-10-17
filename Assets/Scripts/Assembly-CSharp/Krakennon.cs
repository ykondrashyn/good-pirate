public class Krakennon : global::UnityEngine.MonoBehaviour
{
	public int damage;

	private Cannon[] myCannons;

	private void Awake()
	{
		myCannons = GetComponentsInChildren<Cannon>();
		Cannon[] array = myCannons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetPoolName("KrakenProjectile");
		}
	}

	public void Fire()
	{
		Cannon[] array = myCannons;
		foreach (Cannon obj in array)
		{
			obj.damage = damage;
			obj.Fire(0f, ELayerType.Enemy, 20f);
		}
	}
}
