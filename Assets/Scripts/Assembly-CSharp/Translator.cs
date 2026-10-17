public class Translator : global::UnityEngine.MonoBehaviour
{
	public string wordId;

	private void Awake()
	{
		global::UnityEngine.UI.Text component = GetComponent<global::UnityEngine.UI.Text>();
		component.text = LanguageManager.Instance.translate(wordId);
		component.font = LanguageManager.Instance.getLanguageFont();
	}
}
