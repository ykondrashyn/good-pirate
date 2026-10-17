public class RewardCanvas : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject rewardSrc;

	private const float delay = 0.8f;

	private const float duration = 0.5f;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		if (global::UnityEngine.Camera.main != null)
		{
			transform.forward = global::UnityEngine.Camera.main.transform.forward;
		}
	}

	private void SelfDestroy()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public void AddReward(params Reward[] reward)
	{
		Invoke("SelfDestroy", (float)reward.Length * 0.8f + 3f);
		for (int i = 0; i < reward.Length; i++)
		{
			PlayerInfo.Instance.AddMoney(reward[i].moneyType, reward[i].amount, isSave: true, (float)i * 0.8f);
		}
		StartCoroutine(RewardRoutine(reward));
	}

	private global::System.Collections.IEnumerator RewardRoutine(Reward[] reward)
	{
		global::UnityEngine.WaitForSeconds wait = new global::UnityEngine.WaitForSeconds(0.8f);
		for (int i = 0; i < reward.Length; i++)
		{
			RewardSlot component = global::UnityEngine.Object.Instantiate(rewardSrc, transform.position, global::UnityEngine.Quaternion.identity, transform).GetComponent<RewardSlot>();
			global::UnityEngine.Vector3 localPosition = component.transform.localPosition;
			localPosition.z = -80f;
			component.transform.localPosition = localPosition;
			component.transform.localRotation = global::UnityEngine.Quaternion.identity;
			component.SetReward(reward[i]);
			yield return wait;
		}
	}
}
