public class CFX_AutodestructWhenNoChildren : global::UnityEngine.MonoBehaviour
{
	private void Update()
	{
		if (base.transform.childCount == 0)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
