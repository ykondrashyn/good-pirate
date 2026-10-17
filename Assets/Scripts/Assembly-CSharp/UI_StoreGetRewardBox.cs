public class UI_StoreGetRewardBox : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image boxIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text boxCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite goldBoxSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite blackBoxSprite;

	private RewardBoxStatus myData;

	private int prevHaveCount;

	private global::UnityEngine.Coroutine increaseCoroutine;

	private void OnEnable()
	{
		prevHaveCount = PlayerInfo.Instance.myData.storeGetRewardBoxList.Count;
		Refresh();
	}

	public void Refresh()
	{
		if (PlayerInfo.Instance.myData.storeGetRewardBoxList.Count > 0)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			SetData(PlayerInfo.Instance.myData.storeGetRewardBoxList[0]);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		if (prevHaveCount != PlayerInfo.Instance.myData.storeGetRewardBoxList.Count && prevHaveCount < PlayerInfo.Instance.myData.storeGetRewardBoxList.Count && base.gameObject.activeSelf)
		{
			if (increaseCoroutine != null)
			{
				StopCoroutine(increaseCoroutine);
				increaseCoroutine = null;
			}
			increaseCoroutine = StartCoroutine(IncreaseCount(prevHaveCount, PlayerInfo.Instance.myData.storeGetRewardBoxList.Count));
		}
		prevHaveCount = PlayerInfo.Instance.myData.storeGetRewardBoxList.Count;
	}

	private global::System.Collections.IEnumerator IncreaseCount(int start, int end)
	{
		float totalTime = 0f;
		float duration = 0.9f;
		int prev = start;
		while (true)
		{
			totalTime += global::UnityEngine.Time.deltaTime;
			float num = totalTime / duration;
			int num2 = (int)global::UnityEngine.Mathf.Lerp(start, end, num);
			if (num2 != prev)
			{
				global::UnityEngine.Debug.Log("사운드너코~");
				boxCountText.text = num2.ToString();
				prev = num2;
			}
			if (num >= 1f)
			{
				break;
			}
			yield return null;
		}
		increaseCoroutine = null;
	}

	private void SetData(RewardBoxStatus boxStatus)
	{
		myData = boxStatus;
		switch (myData.boxType)
		{
		case ERewardBox.GOLD:
			boxIcon.sprite = goldBoxSprite;
			break;
		case ERewardBox.BLACK:
			boxIcon.sprite = blackBoxSprite;
			break;
		}
		if (PlayerInfo.Instance.myData.storeGetRewardBoxList.Count == 0)
		{
			boxCountText.text = "!";
		}
		else
		{
			boxCountText.text = PlayerInfo.Instance.myData.storeGetRewardBoxList.Count.ToString();
		}
		if (increaseCoroutine != null)
		{
			StopCoroutine(increaseCoroutine);
			increaseCoroutine = null;
		}
	}

	public void Open()
	{
		UI_Manager.Instance.ui_itemBoxPopup.gameObject.SetActive(value: true);
		UI_Manager.Instance.ui_itemBoxPopup.SetIsFreeBox(isTrue: false);
		UI_Manager.Instance.ui_itemBoxPopup.SetItem(myData);
		UI_Manager.Instance.ui_itemBoxPopup.Open(myData.boxType);
		PlayerInfo.Instance.myData.storeGetRewardBoxList.RemoveAt(0);
		PlayerInfo.Instance.SaveData();
		if (PlayerInfo.Instance.myData.storeGetRewardBoxList.Count > 0)
		{
			SetData(PlayerInfo.Instance.myData.storeGetRewardBoxList[0]);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
