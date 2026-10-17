public class Fog : global::UnityEngine.MonoBehaviour
{
	private void OnTriggerEnter(global::UnityEngine.Collider col)
	{
		ShipController shipController = col.GetComponent<ShipController>();
		if (shipController == null)
		{
			shipController = col.GetComponentInParent<ShipController>();
		}
		if (shipController == null)
		{
			shipController = col.GetComponentInChildren<ShipController>();
		}
		if (!(shipController == null))
		{
			shipController.EnterFog();
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider col)
	{
		ShipController shipController = col.GetComponent<ShipController>();
		if (shipController == null)
		{
			shipController = col.GetComponentInParent<ShipController>();
		}
		if (shipController == null)
		{
			shipController = col.GetComponentInChildren<ShipController>();
		}
		if (!(shipController == null))
		{
			shipController.ExitFog();
		}
	}
}
