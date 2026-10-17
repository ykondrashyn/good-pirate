public class FormationHelper : global::UnityEngine.MonoBehaviour
{
	public UI_Formation uiFormation;

	private global::System.Collections.Generic.HashSet<FormationPos> formationSet = new global::System.Collections.Generic.HashSet<FormationPos>();

	public FormationPos nearFormation { get; private set; }

	private void OnTriggerEnter(global::UnityEngine.Collider other)
	{
		FormationPos component = other.GetComponent<FormationPos>();
		if (component != null && !formationSet.Contains(component))
		{
			formationSet.Add(component);
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider other)
	{
		FormationPos component = other.GetComponent<FormationPos>();
		if (component != null && formationSet.Contains(component))
		{
			component.InActive();
			formationSet.Remove(component);
		}
	}

	private void LateUpdate()
	{
		if (formationSet.Count <= 0)
		{
			nearFormation = null;
			return;
		}
		float num = float.PositiveInfinity;
		foreach (FormationPos item in formationSet)
		{
			float sqrMagnitude = (base.transform.position - item.transform.position).sqrMagnitude;
			if (num > sqrMagnitude)
			{
				num = sqrMagnitude;
				nearFormation = item;
				nearFormation.Active();
			}
		}
	}

	public void Clear()
	{
		formationSet.Clear();
		nearFormation = null;
	}
}
