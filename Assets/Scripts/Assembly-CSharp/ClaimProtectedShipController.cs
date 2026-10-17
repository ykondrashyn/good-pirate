[global::UnityEngine.RequireComponent(typeof(ShipBase))]
public class ClaimProtectedShipController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private ShipBase myShip;

	private global::System.Collections.Generic.List<global::UnityEngine.Vector3> moveCourseList = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();

	public ShipBase MyShip => myShip;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		myShip = GetComponent<ShipBase>();
		myShip.accelerationFactor = 0.4f;
	}

	private void OnEnable()
	{
		StartCoroutine(MoveRutine());
	}

	private void SetMoveCourse()
	{
		moveCourseList.Clear();
		global::UnityEngine.Vector3 zero = global::UnityEngine.Vector3.zero;
		foreach (ShipBase allyShip in ClaimManager.Instance.allyShipList)
		{
			zero += allyShip.transform.position;
		}
		if (GameManager.Instance.playerShip == null)
		{
			if (ClaimManager.Instance.allyShipList.Count != 0)
			{
				zero /= (float)ClaimManager.Instance.allyShipList.Count;
			}
		}
		else
		{
			zero += GameManager.Instance.playerShip.transform.position;
			zero /= (float)(ClaimManager.Instance.allyShipList.Count + 1);
		}
		for (int i = 0; i < 2; i++)
		{
			global::UnityEngine.Vector3 item = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward * global::UnityEngine.Random.Range(0f, 30f) + zero;
			moveCourseList.Add(item);
		}
	}

	private global::System.Collections.IEnumerator MoveRutine()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		while (true)
		{
			if (moveCourseList.Count <= 0)
			{
				SetMoveCourse();
				yield return wait;
			}
			global::UnityEngine.Vector3 vector = moveCourseList[0] - transform.position;
			if (vector.magnitude <= 10f)
			{
				moveCourseList.RemoveAt(0);
			}
			vector.y = 0f;
			float angleForward = myMathf.GetAngleForward(vector.normalized);
			myShip.SetTargetAngle(angleForward);
			myShip.Acceleration(myShip.acceleration);
			yield return wait;
		}
	}
}
