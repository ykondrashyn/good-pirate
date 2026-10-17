public class FormationPos : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject obj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem ps;

	public int index;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject selectObj;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject justSelectObj;

	[global::UnityEngine.HideInInspector]
	public bool isCanSelect;

	public ShipSubController shipController { get; private set; }

	public void SetStatus(ShipSubController ship)
	{
		shipController = ship;
	}

	public void Active()
	{
		global::UnityEngine.ParticleSystem.MainModule main = ps.main;
		global::UnityEngine.Color startColor = ps.startColor;
		startColor.a = 0.8f;
		main.startColor = startColor;
		obj.SetActive(value: true);
	}

	public void InActive()
	{
		global::UnityEngine.ParticleSystem.MainModule main = ps.main;
		global::UnityEngine.Color startColor = ps.startColor;
		startColor.a = 0.152f;
		main.startColor = startColor;
		obj.SetActive(value: false);
	}

	public void SetLayer()
	{
	}

	public void SetSelect(bool active)
	{
		selectObj.SetActive(active);
	}

	public void SetJustSelect(bool active)
	{
		justSelectObj.SetActive(active);
	}

	public void MoveMode(bool active)
	{
		if (active)
		{
			if (!(shipController != null))
			{
				obj.SetActive(value: true);
				isCanSelect = true;
			}
		}
		else
		{
			obj.SetActive(value: false);
			isCanSelect = false;
		}
	}
}
