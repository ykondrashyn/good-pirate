public class TestEventCaller : global::UnityEngine.MonoBehaviour
{
	public global::Percent.Event.PercentTracker tracker;

	private void Start()
	{
		tracker.trigger();
	}
}
