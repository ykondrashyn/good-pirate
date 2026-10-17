namespace Percent.Event
{
	public abstract class ButtonEventBinder : global::UnityEngine.MonoBehaviour
	{
		private void Start()
		{
			GetComponent<global::UnityEngine.UI.Button>().onClick.AddListener(onFindMethod());
		}

		protected abstract global::UnityEngine.Events.UnityAction onFindMethod();
	}
}
