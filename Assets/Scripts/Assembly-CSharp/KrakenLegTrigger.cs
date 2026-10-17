public class KrakenLegTrigger : global::UnityEngine.MonoBehaviour
{
	private KrakenLeg myLeg;

	private void Awake()
	{
		myLeg = GetComponentInParent<KrakenLeg>();
	}

	public void ColliderOn()
	{
		myLeg.ColliderOn();
		AudioManager.Instance.PlaySFX("boss_swipe");
	}

	public void ColliderOff()
	{
		myLeg.AttackOff();
	}
}
