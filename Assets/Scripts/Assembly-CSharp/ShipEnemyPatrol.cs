public class ShipEnemyPatrol : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private ShipBase myShip;

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
	}
}
