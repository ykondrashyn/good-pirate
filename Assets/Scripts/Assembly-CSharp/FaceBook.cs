public class FaceBook : global::UnityEngine.MonoBehaviour
{
	private void Awake()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("social", "facebook-initialize");
	}

	private void InitCallback()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("social", "facebook-init-callback");
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			global::UnityEngine.Time.timeScale = 0f;
		}
		else
		{
			global::UnityEngine.Time.timeScale = 1f;
		}
	}
}
