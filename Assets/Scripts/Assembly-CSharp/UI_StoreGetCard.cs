public class UI_StoreGetCard : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image cardIcon;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text cardCountText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite ScardSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite AcardSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite BcardSprite;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite CcardSprite;

	private EGrade selectGrade;

	private int prevHaveCount;

	private global::UnityEngine.Coroutine increaseCoroutine;

	private void OnEnable()
	{
		prevHaveCount = PlayerInfo.Instance.myData.storeCardList.Count;
		Refresh();
		if (cardIcon == null)
		{
			cardIcon = GetComponentInChildren<global::UnityEngine.UI.Image>();
		}
		if (cardCountText == null)
		{
			cardCountText = GetComponentInChildren<global::UnityEngine.UI.Text>();
		}
	}

	public void Refresh()
	{
		if (PlayerInfo.Instance.myData.storeCardList.Count > 0)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			SetData(PlayerInfo.Instance.myData.storeCardList[0]);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		if (prevHaveCount != PlayerInfo.Instance.myData.storeCardList.Count && prevHaveCount < PlayerInfo.Instance.myData.storeCardList.Count && base.gameObject.activeSelf)
		{
			if (increaseCoroutine != null)
			{
				StopCoroutine(increaseCoroutine);
				increaseCoroutine = null;
			}
			increaseCoroutine = StartCoroutine(IncreaseCount(prevHaveCount, PlayerInfo.Instance.myData.storeCardList.Count));
		}
		prevHaveCount = PlayerInfo.Instance.myData.storeCardList.Count;
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
				cardCountText.text = num2.ToString();
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

	private void SetData(EGrade grade)
	{
		selectGrade = grade;
		switch (selectGrade)
		{
		case EGrade.sClass:
			cardIcon.sprite = ScardSprite;
			break;
		case EGrade.aClass:
			cardIcon.sprite = AcardSprite;
			break;
		case EGrade.bClass:
			cardIcon.sprite = BcardSprite;
			break;
		case EGrade.cClass:
			cardIcon.sprite = CcardSprite;
			break;
		}
		if (PlayerInfo.Instance.myData.storeCardList.Count == 0)
		{
			cardCountText.text = "!";
		}
		else
		{
			cardCountText.text = PlayerInfo.Instance.myData.storeCardList.Count.ToString();
		}
		if (increaseCoroutine != null)
		{
			StopCoroutine(increaseCoroutine);
			increaseCoroutine = null;
		}
	}

	public void Open()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		switch (selectGrade)
		{
		case EGrade.cClass:
			num = 23f;
			num2 = 10f;
			num3 = 3f;
			num4 = 0.5f;
			break;
		case EGrade.bClass:
			num = 73f;
			num2 = 20f;
			num3 = 6f;
			num4 = 1f;
			break;
		case EGrade.aClass:
			num = 0f;
			num2 = 86f;
			num3 = 12f;
			num4 = 2f;
			break;
		case EGrade.sClass:
			num = 0f;
			num2 = 0f;
			num3 = 96f;
			num4 = 4f;
			break;
		}
		int num5 = 0;
		int num6 = 400;
		ESailor eSailor = ESailor.END;
		while (true)
		{
			float num7 = global::UnityEngine.Random.Range(0f, 100f);
			global::System.Collections.Generic.List<ESailor> list = ((num7 < num4) ? SailorDataManager.Instance.lRankList : ((num7 <= num4 + num3) ? SailorDataManager.Instance.sRankList : ((num7 <= num4 + num3 + num2) ? SailorDataManager.Instance.aRankList : ((!(num7 <= num4 + num3 + num2 + num)) ? SailorDataManager.Instance.cRankList : SailorDataManager.Instance.bRankList))));
			ESailor eSailor2 = ESailor.END;
			if (list.Count > 0)
			{
				int index = global::UnityEngine.Random.Range(0, list.Count);
				eSailor2 = list[index];
			}
			if (SailorDataManager.Instance.IsCanGiveSailor(eSailor2))
			{
				eSailor = eSailor2;
				break;
			}
			num5++;
			if (num5 >= num6)
			{
				global::UnityEngine.Debug.Log("1");
				break;
			}
		}
		UI_Manager.Instance.cardOpenPopup.gameObject.SetActive(value: true);
		UI_Manager.Instance.cardOpenPopup.SetGrade(selectGrade);
		if (eSailor != ESailor.END)
		{
			UI_Manager.Instance.cardOpenPopup.isCrew = true;
			MyInventoryDataManager.Instance.AddSailor(eSailor, 1);
			UI_Manager.Instance.cardOpenPopup.SetCardData(MyInventoryDataManager.Instance.GetMySailorData(eSailor));
		}
		else
		{
			UI_Manager.Instance.cardOpenPopup.isCrew = false;
			PlayerInfo.Instance.AddMoney(EMonenyType.diamond, 5L);
			UI_Manager.Instance.cardOpenPopup.SetMoneyType(EMonenyType.diamond, 5);
		}
		PlayerInfo.Instance.myData.storeCardList.RemoveAt(0);
		PlayerInfo.Instance.SaveData();
		if (PlayerInfo.Instance.myData.storeCardList.Count > 0)
		{
			SetData(PlayerInfo.Instance.myData.storeCardList[0]);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
