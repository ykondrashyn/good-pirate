public class MapDataPatrolObject : global::UnityEngine.MonoBehaviour
{
	public EShipType shipType;

	public PatrolStatus patrolStatus;

	public global::System.Collections.Generic.List<global::UnityEngine.Transform> coruseList = new global::System.Collections.Generic.List<global::UnityEngine.Transform>();

	[global::UnityEngine.ContextMenu("AddNewCourse")]
	public void AddCourse()
	{
		if (coruseList != null)
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Course " + coruseList.Count);
			if (coruseList.Count != 0)
			{
				gameObject.transform.position = coruseList[coruseList.Count - 1].position;
			}
			gameObject.transform.SetParent(base.transform);
			coruseList.Add(gameObject.transform);
		}
	}

	public void SetData(PatrolStatus status)
	{
		patrolStatus = status;
		shipType = patrolStatus.enemyType;
		coruseList.Clear();
		base.transform.position = status.startPosition;
		for (int i = 0; i < patrolStatus.patrolCourseList.Count; i++)
		{
			global::UnityEngine.GameObject gameObject = new global::UnityEngine.GameObject("Course " + i);
			gameObject.transform.position = patrolStatus.patrolCourseList[i];
			gameObject.transform.SetParent(base.transform);
			coruseList.Add(gameObject.transform);
		}
	}

	private void Awake()
	{
	}
}
