public class AutoInActive : global::UnityEngine.MonoBehaviour
{
	public float duration = 0.6f;

	private void OnEnable()
	{
		Invoke("Inactive", duration);
	}

	private void OnDisable()
	{
		if (IsInvoking("Inactive"))
		{
			CancelInvoke("Inactive");
		}
	}

	private void Inactive()
	{
		base.gameObject.SetActive(value: false);
	}
}
