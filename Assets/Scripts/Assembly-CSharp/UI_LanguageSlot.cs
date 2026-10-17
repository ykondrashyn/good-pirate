public class UI_LanguageSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	private global::UnityEngine.SystemLanguage myLanguage;

	private void Awake()
	{
		myText = GetComponentInChildren<global::UnityEngine.UI.Text>();
	}

	public void SetSystemLanguage(global::UnityEngine.SystemLanguage lan)
	{
		myLanguage = lan;
		myText.font = LanguageManager.Instance.getLanguageFont(lan);
		switch (lan)
		{
		case global::UnityEngine.SystemLanguage.Korean:
			myText.text = "한국어";
			break;
		case global::UnityEngine.SystemLanguage.English:
			myText.text = "English";
			break;
		case global::UnityEngine.SystemLanguage.Japanese:
			myText.text = "日本語";
			break;
		case global::UnityEngine.SystemLanguage.ChineseSimplified:
			myText.text = "中國語-簡體";
			break;
		case global::UnityEngine.SystemLanguage.ChineseTraditional:
			myText.text = "中國語-繁體";
			break;
		case global::UnityEngine.SystemLanguage.French:
			myText.text = "le français";
			break;
		case global::UnityEngine.SystemLanguage.Spanish:
			myText.text = "el español";
			break;
		case global::UnityEngine.SystemLanguage.Russian:
			myText.text = "Русский";
			break;
		case global::UnityEngine.SystemLanguage.German:
			myText.text = "Deutsch";
			break;
		case global::UnityEngine.SystemLanguage.Portuguese:
			myText.text = "português";
			break;
		}
	}

	public void SelectThisLanguage()
	{
		if (myLanguage != LanguageManager.Instance.language)
		{
			LanguageManager.Instance.language = myLanguage;
			LanguageManager.Instance.mode = LanguageMode.MANUAL;
			PlayerInfo.Instance.myData.isChangeLanguage = true;
			PlayerInfo.Instance.myData.selectedLanguage = myLanguage;
			LanguageManager.Instance.setCurrentFont();
			UI_Manager.Instance.BlackScreenOn(0.4f, delegate
			{
				global::UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
			});
		}
	}
}
