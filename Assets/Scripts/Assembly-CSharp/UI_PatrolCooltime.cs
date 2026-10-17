public class UI_PatrolCooltime : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text myText;

	public void SetText(string str)
	{
		myText.text = str;
	}
}
