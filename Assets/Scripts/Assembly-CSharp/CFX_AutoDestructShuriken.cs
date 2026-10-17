[global::UnityEngine.RequireComponent(typeof(global::UnityEngine.ParticleSystem))]
public class CFX_AutoDestructShuriken : global::UnityEngine.MonoBehaviour
{
	public bool OnlyDeactivate;

	private void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}

	private global::System.Collections.IEnumerator CheckIfAlive()
	{
		global::UnityEngine.ParticleSystem ps = GetComponent<global::UnityEngine.ParticleSystem>();
		while (ps != null)
		{
			yield return new global::UnityEngine.WaitForSeconds(0.5f);
			if (!ps.IsAlive(withChildren: true))
			{
				if (OnlyDeactivate)
				{
					base.gameObject.SetActive(value: false);
				}
				else
				{
					global::UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			}
		}
	}
}
