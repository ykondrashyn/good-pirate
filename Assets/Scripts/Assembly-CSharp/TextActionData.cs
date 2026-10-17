[global::System.Serializable]
public class TextActionData
{
	public string messageId;

	public bool isAutoNext;

	public bool isCanNext = true;

	public global::UnityEngine.Events.UnityEvent action;

	public global::UnityEngine.Events.UnityEvent endAction;
}
