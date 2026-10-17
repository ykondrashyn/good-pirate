public class DestinationMinimap : global::UnityEngine.MonoBehaviour
{
	private int layer;

	private void Awake()
	{
		layer = global::UnityEngine.LayerMask.NameToLayer("MiniMap");
	}

	private void OnEnable()
	{
		base.gameObject.layer = layer;
	}

	private global::System.Collections.IEnumerator Start()
	{
		global::UnityEngine.WaitForEndOfFrame wait = new global::UnityEngine.WaitForEndOfFrame();
		while (true)
		{
			yield return wait;
			if (base.gameObject.layer != layer)
			{
				base.gameObject.layer = layer;
			}
		}
	}
}
