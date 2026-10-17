public class UI_PatrolCombatStatus : global::UnityEngine.MonoBehaviour
{
	private Health selectedEnemy;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image hpTopImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.UI.Image IconImage;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Sprite patrolSprite;

	public void SetEnemy(Health health)
	{
		selectedEnemy = health;
		selectedEnemy.AddHitAction(HpRefresh);
		selectedEnemy.AddDieAction(HpRefresh);
		HpRefresh();
	}

	public void SetImage(global::UnityEngine.Sprite sprite)
	{
		IconImage.sprite = sprite;
	}

	public void SetPatrol()
	{
		SetImage(patrolSprite);
	}

	private void OnDisable()
	{
	}

	private void OnEnable()
	{
		if (UI_Manager.Instance.ui_mainGame.ui_chestPanel.IsOpen)
		{
			UI_Manager.Instance.ui_mainGame.ui_chestPanel.Close();
		}
	}

	private void HpRefresh()
	{
		if (selectedEnemy != null)
		{
			hpTopImage.fillAmount = selectedEnemy.HpRatio;
		}
	}
}
