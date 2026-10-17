public class Rock : global::UnityEngine.MonoBehaviour
{
	public RockStatus myStatus;

	public void SetStatus(RockStatus status)
	{
		base.transform.position = status.rockPosition;
		base.transform.rotation = status.rockRotaion;
		base.transform.localScale = status.rockLocalScale;
		myStatus.isNotDrawWorldMap = status.isNotDrawWorldMap;
	}

	public RockStatus GetStatus()
	{
		return new RockStatus
		{
			rockPosition = base.transform.position,
			rockRotaion = base.transform.rotation,
			rockLocalScale = base.transform.localScale,
			rockType = myStatus.rockType,
			isNotDrawWorldMap = myStatus.isNotDrawWorldMap
		};
	}
}
