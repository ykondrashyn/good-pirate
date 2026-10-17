public class destroyMe : global::UnityEngine.MonoBehaviour
{
	private float timer;

	public float deathtimer = 10f;

	private void Start()
	{
	}

	private void Update()
	{
		timer += global::UnityEngine.Time.deltaTime;
		if (timer >= deathtimer)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
