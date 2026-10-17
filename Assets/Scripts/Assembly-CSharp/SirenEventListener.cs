public class SirenEventListener : global::UnityEngine.MonoBehaviour
{
	private Siren mySiren;

	private void Awake()
	{
		mySiren = GetComponentInParent<Siren>();
	}

	public void EmpWarnOn()
	{
		mySiren.empWarnObj.SetActive(value: true);
	}

	public void EmpOn()
	{
		mySiren.empWarnObj.SetActive(value: false);
		mySiren.sirenEmp.gameObject.SetActive(value: true);
	}

	public void ThrowHarpWarnOn()
	{
		mySiren.harpWarnObj.SetActive(value: true);
	}

	public void ThrowHarpSound()
	{
		AudioManager.Instance.PlaySFX("harp_throw");
	}

	public void HarpHitOn()
	{
		mySiren.circleWarnObj.SetActive(value: false);
		mySiren.harpWarnObj.SetActive(value: false);
		mySiren.harpHit.gameObject.SetActive(value: true);
	}

	public void HarpHitOff()
	{
		mySiren.harpHit.gameObject.SetActive(value: false);
	}

	public void CircleWarnOn()
	{
		mySiren.circleWarnObj.SetActive(value: true);
	}
}
