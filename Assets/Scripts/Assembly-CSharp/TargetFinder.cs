public class TargetFinder : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Collider collider;

	private global::System.Collections.Generic.HashSet<Health> shipSet = new global::System.Collections.Generic.HashSet<Health>();

	public global::System.Collections.Generic.List<Health> HealthList => global::System.Linq.Enumerable.ToList(shipSet);

	public global::System.Collections.Generic.HashSet<Health> HealthHashSet => shipSet;

	private void Awake()
	{
		collider = GetComponent<global::UnityEngine.Collider>();
	}

	private void OnEnable()
	{
		StartCoroutine(ListCheck());
	}

	public void Clear()
	{
		StartCoroutine(ClearCoroutine());
	}

	private global::System.Collections.IEnumerator ClearCoroutine()
	{
		yield return new global::UnityEngine.WaitForFixedUpdate();
		shipSet.Clear();
		collider.enabled = false;
		yield return new global::UnityEngine.WaitForFixedUpdate();
		collider.enabled = true;
	}

	public void SetSize(float radius)
	{
		(collider as global::UnityEngine.SphereCollider).radius = radius;
	}

	private void OnTriggerEnter(global::UnityEngine.Collider col)
	{
		Health componentInParent = col.GetComponentInParent<Health>();
		if (componentInParent != null)
		{
			if (!shipSet.Contains(componentInParent))
			{
				shipSet.Add(componentInParent);
			}
		}
		else
		{
			global::UnityEngine.Debug.Log("ship is null");
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider col)
	{
		Health componentInParent = col.GetComponentInParent<Health>();
		if (componentInParent != null && shipSet.Contains(componentInParent))
		{
			shipSet.Remove(componentInParent);
		}
	}

	private global::System.Collections.IEnumerator ListCheck()
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.4f);
		while (true)
		{
			Check();
			yield return wait;
		}
	}

	public void Check()
	{
		global::System.Collections.Generic.List<Health> list = global::System.Linq.Enumerable.ToList(shipSet);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null && shipSet.Contains(list[i]))
			{
				shipSet.Remove(list[i]);
			}
		}
	}
}
