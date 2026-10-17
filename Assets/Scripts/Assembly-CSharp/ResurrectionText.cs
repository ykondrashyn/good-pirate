public class ResurrectionText : global::UnityEngine.MonoBehaviour
{
	private global::UnityEngine.UI.Text myText;

	private int prevCount;

	private long current = -1L;

	private void Awake()
	{
		myText = GetComponent<global::UnityEngine.UI.Text>();
		if (myText == null)
		{
			myText = GetComponentInChildren<global::UnityEngine.UI.Text>();
		}
	}

	private void Start()
	{
		prevCount = PlayerInfo.Instance.myData.resurrectionTicket;
		myText.text = Numbering.Convert(prevCount);
	}

	private void LateUpdate()
	{
		current = (int)PlayerInfo.Instance.myData.resurrectionTicket;
		if (current != -1 && current != prevCount)
		{
			myText.text = Numbering.Convert(current);
		}
		prevCount = PlayerInfo.Instance.myData.resurrectionTicket;
	}
}
