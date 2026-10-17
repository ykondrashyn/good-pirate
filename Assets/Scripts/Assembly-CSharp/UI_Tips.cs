public class UI_Tips : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Text myText;

	private void Awake()
	{
		myText = GetComponent<global::UnityEngine.UI.Text>();
		myText.font = LanguageManager.Instance.getLanguageFont();
	}

	private void OnEnable()
	{
		int num = global::UnityEngine.Random.Range(0, 17);
		myText.text = LanguageManager.Instance.translate("key_tips") + ": " + LanguageManager.Instance.translate($"tip_{num:D2}");
	}
}
