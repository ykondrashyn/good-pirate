public class UI_EmojiSlot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image coolTimeImage;

	private global::UnityEngine.UI.Button myBtn;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image myIcon;

	private eEmoji myType;

	private float totalTime;

	private float coolTime = 3.5f;

	private bool isHaveCoolTime = true;

	private void Awake()
	{
		myBtn = GetComponent<global::UnityEngine.UI.Button>();
		coolTimeImage.gameObject.SetActive(value: false);
	}

	public void SetEmoji(eEmoji eType, global::UnityEngine.Sprite sprite)
	{
		myType = eType;
		myIcon.sprite = sprite;
		coolTimeImage.sprite = sprite;
	}

	public void ActiveEmoji()
	{
		if (GameManager.Instance.playerShip != null)
		{
			EmojiManager.Instance.MakeEmoji(myType, GameManager.Instance.playerShip.transform);
			base.transform.parent.parent.SendMessage("ActiveCooltime");
		}
	}

	public void ActiveCoolTime()
	{
		coolTimeImage.gameObject.SetActive(value: true);
		coolTimeImage.fillAmount = 1f;
		myBtn.enabled = false;
		isHaveCoolTime = true;
		totalTime = 0f;
	}

	private void LateUpdate()
	{
		if (isHaveCoolTime)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / coolTime;
			coolTimeImage.fillAmount = 1f - num;
			if (num >= 1f)
			{
				isHaveCoolTime = false;
				myBtn.enabled = true;
				coolTimeImage.gameObject.SetActive(value: false);
			}
		}
	}
}
