[global::System.Serializable]
public class ExploreTimeData
{
	public int sailorId;

	public long startTicks;

	public long endTicks;

	public int durationTime;

	public global::UnityEngine.Events.UnityAction unityAction;

	public int LeftTotalSeconds => (int)global::System.TimeSpan.FromTicks(endTicks - global::System.DateTime.Now.Ticks).TotalSeconds;

	public float LeftTimeRatio => global::UnityEngine.Mathf.Clamp01((float)(global::System.TimeSpan.FromTicks(endTicks - global::System.DateTime.Now.Ticks).TotalSeconds / (double)durationTime));
}
