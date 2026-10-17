public class GoMain : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Animator openingAnim;

	private void Start()
	{
		StartCoroutine(SceneLoad());
	}

	private global::System.Collections.IEnumerator SceneLoad()
	{
		global::UnityEngine.AsyncOperation info = global::UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainUI");
		info.allowSceneActivation = false;
		while (!(openingAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			yield return null;
		}
		info.allowSceneActivation = true;
	}
}
