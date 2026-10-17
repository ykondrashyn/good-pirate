public class UI_InIslandShipHealth : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text currentHpText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Text leftTimeText;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image currentHpTop;

	private void LateUpdate()
	{
		CurrentHpRefresh();
	}

	private void CurrentHpRefresh()
	{
		_ = PlayerInfo.Instance.myData.mainShipLevel;
		int hP = GameManager.Instance.playerShip.myHealth.HP;
		int hP_MAX = GameManager.Instance.playerShip.myHealth.HP_MAX;
		float hpRatio = GameManager.Instance.playerShip.myHealth.HpRatio;
		currentHpTop.fillAmount = hpRatio;
		currentHpText.text = $"{Numbering.Convert(hP)} / {Numbering.Convert(hP_MAX)}";
		float num = 0f;
		float num2 = ConstValue.HealthRegenerationTime;
		num = num2 * hpRatio;
		float num3 = num2 - num;
		leftTimeText.text = $"{(int)num3 / 60:00} : {(int)num3 % 60:00}";
	}
}
