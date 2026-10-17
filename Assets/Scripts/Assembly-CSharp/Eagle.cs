public class Eagle : global::UnityEngine.MonoBehaviour
{
	private global::System.Collections.Generic.List<RootBox> rootBoxList = new global::System.Collections.Generic.List<RootBox>();

	public global::UnityEngine.Transform modelTrans;

	private void Awake()
	{
		if (modelTrans == null)
		{
			modelTrans = base.transform.Find("Model");
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		RootBox component = other.transform.GetComponent<RootBox>();
		if (component != null && !rootBoxList.Contains(component))
		{
			rootBoxList.Add(component);
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		RootBox component = other.transform.GetComponent<RootBox>();
		if (component != null && rootBoxList.Contains(component))
		{
			rootBoxList.Remove(component);
		}
	}

	private global::System.Collections.IEnumerator Start()
	{
		float seconds = 3f;
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(seconds);
		while (true)
		{
			if (rootBoxList.Count > 0)
			{
				foreach (RootBox rootBox in rootBoxList)
				{
					if (rootBox != null && rootBox.collider.enabled)
					{
						rootBox.ForceRoot();
					}
				}
				AudioManager.Instance.PlaySFX("item_root");
				rootBoxList.Clear();
			}
			yield return wait;
			yield return new global::UnityEngine.WaitForFixedUpdate();
		}
	}
}
