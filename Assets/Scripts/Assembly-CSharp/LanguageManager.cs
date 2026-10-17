public class LanguageManager : global::UnityEngine.MonoBehaviour
{
	[global::System.Serializable]
	public struct Fonts
	{
		public global::UnityEngine.SystemLanguage lang;

		public global::UnityEngine.Font font;
	}

	[global::UnityEngine.SerializeField]
	private Localization localizationSheetData;

	public LanguageMode mode;

	public Word[] words;

	public global::UnityEngine.SystemLanguage language;

	public global::UnityEngine.SystemLanguage defaultLanguage;

	public LanguageManager.Fonts[] fonts;

	private global::UnityEngine.Font currentFont;

	public static LanguageManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			if (PlayerInfo.Instance.myData.isChangeLanguage)
			{
				language = PlayerInfo.Instance.myData.selectedLanguage;
				mode = LanguageMode.MANUAL;
			}
			if (mode.Equals(LanguageMode.AUTO))
			{
				language = global::UnityEngine.Application.systemLanguage;
			}
			setCurrentFont();
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	[global::UnityEngine.ContextMenu("시트아이템 긁어오기")]
	private void GetSheetData()
	{
		if (localizationSheetData == null)
		{
			localizationSheetData = global::UnityEngine.Resources.Load<Localization>("Data/Localization");
		}
		words = new Word[localizationSheetData.dataArray.Length];
		for (int i = 0; i < localizationSheetData.dataArray.Length; i++)
		{
			LocalizationData localizationData = localizationSheetData.dataArray[i];
			words[i].id = localizationData.Key;
			words[i].str = new Translated[10];
			words[i].str[0].lang = global::UnityEngine.SystemLanguage.English;
			words[i].str[0].str = localizationData.En;
			words[i].str[1].lang = global::UnityEngine.SystemLanguage.Korean;
			words[i].str[1].str = localizationData.Ko;
			words[i].str[2].lang = global::UnityEngine.SystemLanguage.ChineseSimplified;
			words[i].str[2].str = localizationData.Hans;
			words[i].str[3].lang = global::UnityEngine.SystemLanguage.ChineseTraditional;
			words[i].str[3].str = localizationData.Hant;
			words[i].str[4].lang = global::UnityEngine.SystemLanguage.Japanese;
			words[i].str[4].str = localizationData.Ja;
			words[i].str[5].lang = global::UnityEngine.SystemLanguage.German;
			words[i].str[5].str = localizationData.De;
			words[i].str[6].lang = global::UnityEngine.SystemLanguage.Russian;
			words[i].str[6].str = localizationData.Ru;
			words[i].str[7].lang = global::UnityEngine.SystemLanguage.French;
			words[i].str[7].str = localizationData.Fr;
			words[i].str[8].lang = global::UnityEngine.SystemLanguage.Spanish;
			words[i].str[8].str = localizationData.Es;
			words[i].str[9].lang = global::UnityEngine.SystemLanguage.Portuguese;
			words[i].str[9].str = localizationData.Pt;
		}
	}

	public void setCurrentFont()
	{
		for (int i = 0; i < fonts.Length; i++)
		{
			if (fonts[i].lang.Equals(language))
			{
				currentFont = fonts[i].font;
				return;
			}
		}
		for (int j = 0; j < fonts.Length; j++)
		{
			if (fonts[j].lang.Equals(defaultLanguage))
			{
				currentFont = fonts[j].font;
				global::UnityEngine.Debug.Log("Set current font FAIL, Set Default font");
				break;
			}
		}
	}

	public void ReloadFromPlayerInfo()
	{
		if (PlayerInfo.Instance.myData.isChangeLanguage)
		{
			language = PlayerInfo.Instance.myData.selectedLanguage;
			mode = LanguageMode.MANUAL;
		}
		else if (mode.Equals(LanguageMode.AUTO))
		{
			language = global::UnityEngine.Application.systemLanguage;
		}
		setCurrentFont();
	}

	public string translate(string id)
	{
		int index = indexOf(id);
		if (index.Equals(-1))
		{
			global::UnityEngine.Debug.Log("Invalid word ID");
			return "Invalid word ID";
		}
		return getTranslated(index);
	}

	private int indexOf(string id)
	{
		for (int i = 0; i < words.Length; i++)
		{
			if (words[i].id.Equals(id))
			{
				return i;
			}
		}
		return -1;
	}

	private string getTranslated(int index)
	{
		Word word = words[index];
		for (int i = 0; i < word.str.Length; i++)
		{
			if (word.str[i].lang.Equals(language))
			{
				return word.str[i].str;
			}
		}
		for (int j = 0; j < word.str.Length; j++)
		{
			if (word.str[j].lang.Equals(defaultLanguage))
			{
				return word.str[j].str;
			}
		}
		global::UnityEngine.Debug.Log("There is no translated default words");
		return "";
	}

	public global::UnityEngine.Font getLanguageFont()
	{
		return currentFont;
	}

	public global::UnityEngine.Font getLanguageFont(global::UnityEngine.SystemLanguage lan)
	{
		for (int i = 0; i < fonts.Length; i++)
		{
			if (fonts[i].lang.Equals(lan))
			{
				return fonts[i].font;
			}
		}
		for (int j = 0; j < fonts.Length; j++)
		{
			if (fonts[j].lang.Equals(defaultLanguage))
			{
				global::UnityEngine.Debug.Log("Set current font FAIL, Set Default font");
				return fonts[j].font;
			}
		}
		return null;
	}

	public void logAllText(global::UnityEngine.SystemLanguage lang)
	{
		global::UnityEngine.Debug.Log(string.Concat("All ", lang, " Text : ", getAllText(lang)));
	}

	public void logAllTextUnique(global::UnityEngine.SystemLanguage lang)
	{
		getAllText(lang);
	}

	private string getAllText(global::UnityEngine.SystemLanguage lang)
	{
		string text = "";
		for (int i = 0; i < words.Length; i++)
		{
			for (int j = 0; j < words[i].str.Length; j++)
			{
				if (words[i].str[j].lang.Equals(lang))
				{
					text += words[i].str[j].str;
				}
			}
		}
		return text;
	}
}
