public class SocialPlatformManager : global::UnityEngine.MonoBehaviour
{
	private static SocialPlatformManager sInstance;

	public const string BEST_SCORE_LEADER_BOARD = "";

	public const string ACHIEVEMENT_ = "CgkIzLDnmcgCEAIQAQ";

	private global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();

	public static SocialPlatformManager Instance
	{
		get
		{
			return sInstance;
		}
		private set
		{
		}
	}

	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("social", "google-play-initialize");
	}

	private void HandleAuthenticated(bool success)
	{
		if (success)
		{
			global::UnityEngine.Debug.Log("**** Handle Authenticated Success" + success);
		}
		else
		{
			global::UnityEngine.Debug.Log("**** Handle Authenticated FAILED" + success);
		}
	}

	public void ScoreReport(int score)
	{
		LeaderBoardReport(score);
	}

	public void LeaderBoardReport(float score, int demical = 0)
	{
		int num = 1;
		for (int i = 0; i < demical; i++)
		{
			num *= 10;
		}
		global::UnityEngine.Debug.Log("LeaderBoard Report Score: " + score);
		ReportLeaderBoardScore("", (long)(score * (float)num));
	}

	public void LeaderBoardReport(int score, int demical = 0)
	{
		int num = 1;
		for (int i = 0; i < demical; i++)
		{
			num *= 10;
		}
		global::UnityEngine.Debug.Log("LeaderBoard Report Score: " + score);
		ReportLeaderBoardScore("", score * num);
	}

	public void ReportLeaderBoardScore(string leaderboardID, long score)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("social", "report-leaderboard-score");
	}

	public void ReportAchievementProgress(string achievementId, double progress)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.NoOp("social", "report-achievement-progress");
	}

	private void HandleAchieVEmentReported(bool success)
	{
		if (success)
		{
			global::UnityEngine.Debug.Log("**** HandleAchieVEmentReported Success" + success);
		}
		else
		{
			global::UnityEngine.Debug.Log("**** HandleAchieVEmentReported FAILED" + success);
		}
	}

	private void HandleProgressReported(bool success)
	{
	}

	public void ShowAchievements()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("social", "show-achievements");
	}

	public void ShowLeaderBoard()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("social", "show-leaderboard");
	}
}
