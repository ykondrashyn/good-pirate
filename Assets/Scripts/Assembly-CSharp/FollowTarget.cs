public class FollowTarget : global::UnityEngine.MonoBehaviour
{
	public enum FollowType
	{
		LateUpdate = 0,
		FixedUpdate = 1
	}

	public static FollowTarget Instance;

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	public global::UnityEngine.Transform cameraPivot;

	[global::UnityEngine.SerializeField]
	private FollowTarget.FollowType followType;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform targetTrans;

	public bool isSmooth;

	public bool isPause;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Vector3 offset = global::UnityEngine.Vector3.zero;

	public global::UnityEngine.Transform TargetTrans => targetTrans;

	public global::UnityEngine.Vector3 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
		}
	}

	private void Awake()
	{
		Instance = this;
		transform = GetComponent<global::UnityEngine.Transform>();
	}

	private void OnEnable()
	{
		StartCoroutine(Follow());
	}

	public void SetTarget(global::UnityEngine.Transform target)
	{
		targetTrans = target;
	}

	private global::System.Collections.IEnumerator Follow()
	{
		global::UnityEngine.WaitForFixedUpdate waitFixed = new global::UnityEngine.WaitForFixedUpdate();
		global::UnityEngine.WaitForEndOfFrame waitEndFrame = new global::UnityEngine.WaitForEndOfFrame();
		yield return new global::UnityEngine.WaitUntil(() => targetTrans != null);
		while (true)
		{
			if (targetTrans == null)
			{
				if (followType == FollowTarget.FollowType.FixedUpdate)
				{
					yield return waitFixed;
				}
				else
				{
					yield return waitEndFrame;
				}
				continue;
			}
			if (isPause)
			{
				if (followType == FollowTarget.FollowType.FixedUpdate)
				{
					yield return waitFixed;
				}
				else
				{
					yield return waitEndFrame;
				}
				continue;
			}
			if (isSmooth)
			{
				transform.position = global::UnityEngine.Vector3.Lerp(transform.position, targetTrans.position + offset, ((followType == FollowTarget.FollowType.FixedUpdate) ? global::UnityEngine.Time.fixedDeltaTime : global::UnityEngine.Time.deltaTime) * 3f);
			}
			else
			{
				transform.position = targetTrans.position + offset;
			}
			if (followType == FollowTarget.FollowType.FixedUpdate)
			{
				yield return waitFixed;
			}
			else
			{
				yield return waitEndFrame;
			}
		}
	}

	public void IslandCameraSetting(global::UnityEngine.Transform dockingTrans)
	{
		isPause = true;
		global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(-31.3f, -2f, -46.9f);
		transform.position = dockingTrans.position + dockingTrans.TransformVector(vector);
		transform.rotation = global::UnityEngine.Quaternion.Euler(0f, 45f + dockingTrans.rotation.eulerAngles.y, 0f);
	}

	public void FormationCameraSetting(bool active)
	{
	}

	public void CameraPivotReset()
	{
		cameraPivot.localPosition = new global::UnityEngine.Vector3(0f, 384f, -661f);
		cameraPivot.localRotation = global::UnityEngine.Quaternion.Euler(30f, 0f, 0f);
		global::UnityEngine.Camera.main.orthographicSize = 45f;
	}
}
