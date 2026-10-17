public class AutoSyncDestroy : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Animator anim;

	private void Awake()
	{
		anim = GetComponentInChildren<global::UnityEngine.Animator>();
	}

	private void OnEnable()
	{
		StartCoroutine(AutoCorou());
	}

	private global::System.Collections.IEnumerator AutoCorou()
	{
		yield return null;
		while (!(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}
}
