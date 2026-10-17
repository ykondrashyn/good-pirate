public class ClaimTaxManager : global::UnityEngine.MonoBehaviour
{
	public static ClaimTaxManager Instance;

	private const float taxTime = 600f;

	private int goldValue;

	private bool isHaveTax;

	[global::UnityEngine.SerializeField]
	private IslandSheetData islandSheet;

	public int TaxValue => goldValue;

	private void Awake()
	{
		Instance = this;
	}

	public void RefreshTaxValue()
	{
		goldValue = 0;
		isHaveTax = false;
		for (int i = 0; i < ConstValue.islandCount; i++)
		{
			if (PlayerInfo.Instance.GetIsClearIslandClaim(i))
			{
				goldValue += (int)islandSheet.dataArray[i].Gold / 10;
				isHaveTax = true;
			}
		}
	}

	private void Update()
	{
		if (isHaveTax)
		{
			PlayerInfo.Instance.myData.taxTimeTotal += global::UnityEngine.Time.deltaTime;
			if (PlayerInfo.Instance.myData.taxTimeTotal >= 600f)
			{
				PlayerInfo.Instance.myData.taxTimeTotal -= 600f;
				PlayerInfo.Instance.SaveData();
				GameManager.Instance.TaxSummon();
			}
			if (UI_Manager.Instance.ui_mainGame.conquestHaveTaxTime.gameObject.activeSelf)
			{
				float num = 600f - PlayerInfo.Instance.myData.taxTimeTotal;
				int num2 = (int)(num / 60f);
				int num3 = (int)(num % 60f);
				UI_Manager.Instance.ui_mainGame.conquestHaveTaxTime.text = $"{num2}:{num3:00}";
			}
		}
	}
}
