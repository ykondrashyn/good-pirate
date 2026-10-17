public class LoadingScene : global::UnityEngine.MonoBehaviour
{
	private void Start()
	{
		global::UnityEngine.Debug.Log("GP113_STARTUP loading-scene-start");
		string sceneName = "Intro";
		try
		{
			PlayerInfo playerInfo = PlayerInfo.Instance;
			if (playerInfo != null && playerInfo.myData != null && playerInfo.myData.isShowIntro)
			{
				sceneName = "MainGame";
			}
		}
		catch (global::System.Exception exception)
		{
			global::UnityEngine.Debug.LogException(exception, this);
			global::UnityEngine.Debug.LogWarning("GP113_STARTUP player-state-failed defaulting-to-intro", this);
		}

		if (!global::UnityEngine.Application.CanStreamedLevelBeLoaded(sceneName))
		{
			global::UnityEngine.Debug.LogError("GP113_STARTUP missing-scene " + sceneName, this);
			return;
		}

		global::UnityEngine.Debug.Log("GP113_STARTUP loading-scene-route " + sceneName, this);
		global::GoodPirate113.Reconstruction.Bootstrap.GoodPirateRuntimeBootstrap runtime = global::GoodPirate113.Reconstruction.Bootstrap.GoodPirateRuntimeBootstrap.Instance;
		if (runtime != null)
		{
			runtime.MarkStartupRouteRequested(sceneName);
		}
		global::UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
	}
}
