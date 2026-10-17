public class PreciseLocaleText : global::UnityEngine.MonoBehaviour
{
	private void Start()
	{
		GetComponent<global::UnityEngine.UI.Text>().text = $"LANGUAGE ID: {PreciseLocale.GetLanguageID()} \nLANGUAGE: {PreciseLocale.GetLanguage()} \n REGION: {PreciseLocale.GetRegion()} \n CURRENCY CODE: {PreciseLocale.GetCurrencyCode()} \n CURRENCY SYMBOL: {PreciseLocale.GetCurrencySymbol()}";
	}
}
