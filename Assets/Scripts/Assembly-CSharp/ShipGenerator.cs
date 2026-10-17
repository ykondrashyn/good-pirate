public class ShipGenerator : global::UnityEngine.MonoBehaviour
{
	public static ShipGenerator Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject playerShip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject enemyShip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject allyShip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject subShip;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject particleSrc;

	private void Awake()
	{
		Instance = this;
	}

	public void SpawnMain(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, out ShipBase ship, out ShipController shipController)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(playerShip, pos, rot);
		ship = gameObject.GetComponent<ShipBase>();
		shipController = gameObject.GetComponent<ShipController>();
		global::UnityEngine.Object.Instantiate(particleSrc, pos, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f)).transform.localScale = global::UnityEngine.Vector3.one * 5f;
	}

	public void SpawnEnemy(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, out ShipBase ship)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(enemyShip, pos, rot);
		ship = gameObject.GetComponent<ShipBase>();
		if (pos != global::UnityEngine.Vector3.zero)
		{
			global::UnityEngine.Object.Instantiate(particleSrc, pos, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f)).transform.localScale = global::UnityEngine.Vector3.one * 5f;
		}
	}

	public void SpawnAlly(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, out ShipBase ship)
	{
		global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(allyShip, pos, rot);
		ship = gameObject.GetComponent<ShipBase>();
		global::UnityEngine.Object.Instantiate(particleSrc, pos, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f)).transform.localScale = global::UnityEngine.Vector3.one * 5f;
	}

	public void JustSpawnParticle(global::UnityEngine.Vector3 pos)
	{
		global::UnityEngine.Object.Instantiate(particleSrc, pos, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f)).transform.localScale = global::UnityEngine.Vector3.one * 5f;
	}

	public ShipSubController SpawnSubShip(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, ShipBase followTarget, global::UnityEngine.Vector3 offsetPos)
	{
		ShipSubController component = global::UnityEngine.Object.Instantiate(subShip, pos, rot).GetComponent<ShipSubController>();
		component.SetTarget(followTarget, offsetPos);
		global::UnityEngine.Object.Instantiate(particleSrc, pos, global::UnityEngine.Quaternion.Euler(-90f, 0f, 0f)).transform.localScale = global::UnityEngine.Vector3.one * 5f;
		return component;
	}
}
