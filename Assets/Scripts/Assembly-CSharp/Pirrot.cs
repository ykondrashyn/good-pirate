public class Pirrot : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private float intervalTime = 1f;

	[global::UnityEngine.Range(0.01f, 1f)]
	[global::UnityEngine.SerializeField]
	private float healPercent = 0.01f;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem myParticle;

	public global::UnityEngine.Transform modelTrans;

	private void Awake()
	{
		if (modelTrans == null)
		{
			modelTrans = base.transform.Find("Model");
		}
	}

	private global::System.Collections.IEnumerator Start()
	{
		float totalTime = 0f;
		while (true)
		{
			if (GameManager.Instance != null && GameManager.Instance.IsInIsland)
			{
				totalTime = 0f;
				yield return null;
				continue;
			}
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime >= intervalTime)
			{
				totalTime -= intervalTime;
				bool flag = false;
				if (GameManager.Instance.playerShip.myHealth.HpRatio < 1f)
				{
					int heal = (int)((float)GameManager.Instance.playerShip.myHealth.HP_MAX * healPercent);
					GameManager.Instance.playerShip.myHealth.Heal(heal);
					GameManager.Instance.GenerateHealParticle(GameManager.Instance.playerShip.transform);
					flag = true;
				}
				foreach (ShipSubController subShip in GameManager.Instance.playerShipController.subShipList)
				{
					if (subShip.myShip.myHealth.HpRatio < 1f)
					{
						flag = true;
						int heal = (int)((float)subShip.myShip.myHealth.HP_MAX * healPercent);
						subShip.myShip.myHealth.Heal(heal);
						GameManager.Instance.GenerateHealParticle(subShip.transform);
					}
				}
				if (flag)
				{
					myParticle.Play();
				}
			}
			yield return null;
		}
	}
}
