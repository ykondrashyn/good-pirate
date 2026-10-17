public class Flyingfish : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.Animator anim;

	private void Awake()
	{
		anim = GetComponent<global::UnityEngine.Animator>();
	}

	private void OnEnable()
	{
		StartCoroutine(JumpRutine());
	}

	private global::System.Collections.IEnumerator JumpRutine()
	{
		float seconds = global::UnityEngine.Random.Range(4f, 5f);
		float coolTime = 5f;
		float totalTime = 0f;
		yield return new global::UnityEngine.WaitForSeconds(seconds);
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime > coolTime)
			{
				totalTime -= coolTime;
				anim.SetTrigger("Jump");
			}
			yield return null;
		}
	}
}
