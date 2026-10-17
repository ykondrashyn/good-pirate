[global::UnityEngine.RequireComponent(typeof(ShipBase))]
public class CarrierController : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public ShipBase myShip;

	private global::System.Collections.Generic.List<global::UnityEngine.Vector3> destinationList;

	private const float defaultGuideLinePosY = 0.2f;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.LineRenderer guideLineRenderer;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.LineRenderer miniMapGuideLineRenderer;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Material guideMaterial;

	private void Awake()
	{
		myShip = GetComponent<ShipBase>();
		StartCoroutine(GuideArrowMove());
	}

	public void SetDestination(global::System.Collections.Generic.List<global::UnityEngine.Vector3> list)
	{
		destinationList = list;
		global::UnityEngine.Vector3 forward = destinationList[1] - destinationList[0];
		forward.y = 0f;
		forward.Normalize();
		base.transform.position = destinationList[0];
		base.transform.forward = forward;
		global::UnityEngine.GameObject original = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/GoalCircle");
		global::UnityEngine.GameObject original2 = global::UnityEngine.Resources.Load<global::UnityEngine.GameObject>("Prefabs/GoalArrow");
		guideLineRenderer.positionCount = destinationList.Count;
		miniMapGuideLineRenderer.positionCount = destinationList.Count;
		for (int i = 0; i < destinationList.Count; i++)
		{
			global::UnityEngine.Vector3 vector = destinationList[i];
			vector.y = 0.2f;
			if (i == destinationList.Count - 1)
			{
				global::UnityEngine.Vector3 vector2 = destinationList[i - 1] - destinationList[i];
				vector2.Normalize();
				global::UnityEngine.Object.Instantiate(original, vector, global::UnityEngine.Quaternion.Euler(90f, 0f, 0f));
				global::UnityEngine.GameObject obj = global::UnityEngine.Object.Instantiate(original2, vector + vector2 * 38f, global::UnityEngine.Quaternion.identity);
				obj.transform.LookAt(destinationList[i], global::UnityEngine.Vector3.up);
				obj.transform.Rotate(90f, 0f, 0f, global::UnityEngine.Space.Self);
				guideLineRenderer.SetPosition(i, vector + vector2 * 12f);
				miniMapGuideLineRenderer.SetPosition(i, vector + vector2 * 50f);
			}
			else
			{
				guideLineRenderer.SetPosition(i, vector);
				miniMapGuideLineRenderer.SetPosition(i, vector);
			}
		}
	}

	public void MoveStart()
	{
		StopCoroutine(CarryStart());
		StartCoroutine(CarryStart());
	}

	private global::System.Collections.IEnumerator CarryStart()
	{
		global::UnityEngine.WaitForFixedUpdate wait = new global::UnityEngine.WaitForFixedUpdate();
		int nowGoalCount = 1;
		while (true)
		{
			global::UnityEngine.Vector3 dir = base.transform.position - destinationList[nowGoalCount];
			if (dir.sqrMagnitude <= 10f)
			{
				float totalTime = 0f;
				while (true)
				{
					dir.y = 0f;
					dir.Normalize();
					float angleForward = myMathf.GetAngleForward(-dir);
					myShip.SetTargetAngle(angleForward);
					totalTime += global::UnityEngine.Time.fixedDeltaTime;
					if (totalTime > 4f)
					{
						break;
					}
					yield return wait;
				}
				nowGoalCount++;
				if (nowGoalCount == destinationList.Count)
				{
					break;
				}
			}
			dir.y = 0f;
			dir.Normalize();
			float angleForward2 = myMathf.GetAngleForward(-dir);
			myShip.SetTargetAngle(angleForward2);
			myShip.Acceleration(1f);
			yield return wait;
		}
	}

	private global::System.Collections.IEnumerator GuideArrowMove()
	{
		while (true)
		{
			guideMaterial.mainTextureOffset += new global::UnityEngine.Vector2(-1f, 0f) * global::UnityEngine.Time.deltaTime;
			if (guideMaterial.mainTextureOffset.x < -1f)
			{
				guideMaterial.mainTextureOffset += new global::UnityEngine.Vector2(1f, 0f);
			}
			yield return null;
		}
	}
}
