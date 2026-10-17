public class TestCaller : global::UnityEngine.MonoBehaviour
{
	private void Start()
	{
		global::Percent.CrossPromotion.showAds();
	}

	public void showGDPRWindow()
	{
		global::Percent.CrossPromotion.showPrivacyWindow();
	}
}
