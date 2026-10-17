public class MapEnvironmentObject : global::UnityEngine.MonoBehaviour
{
	public EnvironmentStatus status;

	public EnvironmentStatus GetStatus()
	{
		status.position = base.transform.position;
		status.rotation = base.transform.rotation;
		status.scale = base.transform.localScale;
		return status;
	}

	public void SetStatus(EnvironmentStatus status)
	{
		this.status = status;
		base.transform.position = status.position;
		base.transform.localScale = status.scale;
		base.transform.rotation = status.rotation;
	}
}
