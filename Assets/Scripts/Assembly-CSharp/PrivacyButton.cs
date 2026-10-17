public class PrivacyButton : global::UnityEngine.MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(!global::Percent.CrossPromotion.isNOTEURegion());
	}

	public void showPrivacy()
	{
		global::Percent.CrossPromotion.showPrivacyWindow();
	}
}
