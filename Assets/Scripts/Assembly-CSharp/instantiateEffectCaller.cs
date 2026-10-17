public class instantiateEffectCaller : global::UnityEngine.MonoBehaviour
{
	[global::System.Serializable]
	public class chainEffect
	{
		[global::System.NonSerialized]
		public bool isPlayed;

		public float activateTimer;

		public global::UnityEngine.GameObject Effect;

		public global::UnityEngine.Transform effectLocator;
	}

	[global::System.NonSerialized]
	public bool fired;

	private float timer;

	public float timeLimit;

	public instantiateEffectCaller.chainEffect[] chainEffectList;

	private void Start()
	{
	}

	private void Update()
	{
		timer += global::UnityEngine.Time.deltaTime;
		CheckTimer();
	}

	private void CheckTimer()
	{
		for (int i = 0; i < chainEffectList.Length; i++)
		{
			if (timer >= chainEffectList[i].activateTimer && !chainEffectList[i].isPlayed)
			{
				global::UnityEngine.Object.Instantiate(chainEffectList[i].Effect, chainEffectList[i].effectLocator.transform.position, chainEffectList[i].effectLocator.transform.rotation);
				chainEffectList[i].isPlayed = true;
			}
		}
		if (timer >= timeLimit)
		{
			fired = false;
			ResetTimers();
		}
	}

	public void ResetTimers()
	{
		for (int i = 0; i < chainEffectList.Length; i++)
		{
			chainEffectList[i].isPlayed = false;
		}
		timer = 0f;
	}
}
