public class CFX_InspectorHelp : global::UnityEngine.MonoBehaviour
{
	public bool Locked;

	public string Title;

	public string HelpText;

	public int MsgType;

	[global::UnityEngine.ContextMenu("Unlock editing")]
	private void Unlock()
	{
		Locked = false;
	}
}
