public class GoogleCloudManager : global::UnityEngine.MonoBehaviour
{
	public static GoogleCloudManager Instance;

	private GoogleCloudData cloudData = new GoogleCloudData();

	public global::System.Action saveAction;

	public global::System.Action loadAction;

	private const string GOOGLE_CLOUD_FILENAME_KEY = "GoogleCloud";

	private const string FREE_DIAMOND = "KEY_FREEDIA";

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (PercentCloud.Instance != null)
		{
			PercentCloud.Instance.networkingError = ShowCloudUnavailable;
		}
	}

	private void ShowCloudUnavailable()
	{
		if (UI_Manager.Instance != null)
		{
			UI_Manager.Instance.BlackScreenOff(0.4f);
			UI_Manager.Instance.CloudFailPopup();
		}
	}

	public void GoogleCloudSave()
	{
		if (saveAction != null)
		{
			saveAction();
		}
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "google-cloud-save");
		ShowCloudUnavailable();
	}

	public void GoogleCloudLoad()
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "google-cloud-load");
		ShowCloudUnavailable();
	}

	private void UploadSuccess(global::Boomlagoon.JSON.JSONObject json)
	{
		UI_Manager.Instance.BlackScreenOff(0.4f);
		UI_Manager.Instance.CloudSuccessPopup();
	}

	private void DownloadData(global::Boomlagoon.JSON.JSONObject json)
	{
		string s = json.GetString("content");
		global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
		global::System.IO.MemoryStream serializationStream = new global::System.IO.MemoryStream(global::System.Convert.FromBase64String(s));
		cloudData = (GoogleCloudData)binaryFormatter.Deserialize(serializationStream);
		global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame", global::UnityEngine.SceneManagement.LoadSceneMode.Single);
		if (loadAction != null)
		{
			loadAction();
		}
		CoolTimeManager.AddCooltimeSecondFromNow("KEY_FREEDIA", 601);
	}

	private void OpenSavedGame(string filename)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "open-google-saved-game");
	}

	private void OnSavedGameOpened(global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus status, global::GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata game)
	{
		if (status == global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
		{
			cloudData.Clear();
			if (saveAction != null)
			{
				saveAction();
			}
			global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream();
			binaryFormatter.Serialize(memoryStream, cloudData);
			byte[] buffer = memoryStream.GetBuffer();
			SaveGame(game, buffer);
			global::UnityEngine.Debug.Log("===============구글드라이브 오픈 성공===============");
		}
		else
		{
			UI_Manager.Instance.BlackScreenOff(0.4f, null, isHaveDelay: false);
			global::UnityEngine.Debug.Log("===============구글드라이브 오픈 실패===============");
		}
	}

	private void OpenSavedGameLoad(string filename)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "open-google-saved-game-load");
	}

	private void OnLoadGameOpened(global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus status, global::GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata game)
	{
		UI_Manager.Instance.BlackScreenOff(0.4f, null, isHaveDelay: false);
		if (status == global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
		{
			global::UnityEngine.Debug.Log("===============구글드라이브 오픈 성공===============");
			LoadGameData(game);
		}
		else
		{
			global::UnityEngine.Debug.Log("===============구글드라이브 오픈 실패===============");
		}
	}

	private void SaveGame(global::GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata game, byte[] savedData)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "commit-google-saved-game");
	}

	private void OnSavedGameWritten(global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus status, global::GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata game)
	{
		UI_Manager.Instance.BlackScreenOff(0.4f, null, isHaveDelay: false);
		if (status == global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
		{
			global::UnityEngine.Debug.Log("===============구글드라이브 저장 성공===============");
		}
		else
		{
			global::UnityEngine.Debug.Log("===============구글드라이브 저장 실패===============");
		}
	}

	private void LoadGameData(global::GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata game)
	{
		global::GoodPirate113.Reconstruction.Offline.OfflineServicePolicy.Unavailable("cloud", "read-google-saved-game");
	}

	private void OnSavedGameDataRead(global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus status, byte[] data)
	{
		if (status == global::GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
		{
			global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new global::System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			global::System.IO.MemoryStream serializationStream = new global::System.IO.MemoryStream(data);
			cloudData = (GoogleCloudData)binaryFormatter.Deserialize(serializationStream);
			UI_Manager.Instance.BlackScreenOff(0.4f, delegate
			{
				global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame", global::UnityEngine.SceneManagement.LoadSceneMode.Single);
				if (loadAction != null)
				{
					loadAction();
				}
			}, isHaveDelay: false);
			global::UnityEngine.Debug.Log("===============구글드라이브 로드 성공===============");
		}
		else
		{
			global::UnityEngine.Debug.Log("===============구글드라이브 로드 실패===============");
			UI_Manager.Instance.BlackScreenOff(0.4f, null, isHaveDelay: false);
		}
	}

	public void SetString(string key, string value)
	{
		if (cloudData.stringDic.ContainsKey(key))
		{
			cloudData.stringDic[key] = value;
		}
		else
		{
			cloudData.stringDic.Add(key, value);
		}
	}

	public void SetBool(string key, bool value)
	{
		if (cloudData.boolDic.ContainsKey(key))
		{
			cloudData.boolDic[key] = value;
		}
		else
		{
			cloudData.boolDic.Add(key, value);
		}
	}

	public void SetInt(string key, int value)
	{
		if (cloudData.intDic.ContainsKey(key))
		{
			cloudData.intDic[key] = value;
		}
		else
		{
			cloudData.intDic.Add(key, value);
		}
	}

	public string GetString(string key, string defaultValue = "")
	{
		if (cloudData.stringDic.ContainsKey(key))
		{
			return cloudData.stringDic[key];
		}
		return defaultValue;
	}

	public bool GetBool(string key, bool defaultValue = false)
	{
		if (cloudData.boolDic.ContainsKey(key))
		{
			return cloudData.boolDic[key];
		}
		return defaultValue;
	}

	public int GetInt(string key, int defaultValue = 0)
	{
		if (cloudData.intDic.ContainsKey(key))
		{
			return cloudData.intDic[key];
		}
		return defaultValue;
	}
}
