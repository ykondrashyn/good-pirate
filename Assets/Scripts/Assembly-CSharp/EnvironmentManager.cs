public class EnvironmentManager : global::UnityEngine.MonoBehaviour
{
	public static EnvironmentManager Instance;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject tornadoSrc;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<EnvironmentObject> typeBlueList = new global::System.Collections.Generic.List<EnvironmentObject>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<EnvironmentObject> typeRedList = new global::System.Collections.Generic.List<EnvironmentObject>();

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<EnvironmentObject> typeGreenList = new global::System.Collections.Generic.List<EnvironmentObject>();

	private const float BLUE_COOLTIME = 194f;

	private const float RED_COOLTIME = 86f;

	private const float GREEN_COOLTIME = 68f;

	private const string KEY_GHOSTSHIP = "GhostShipEvent";

	private void Awake()
	{
		Instance = this;
		if (typeBlueList.Count > 0)
		{
			StartCoroutine(EnvironmentCoolTimeRutine(typeBlueList, 194f));
		}
		if (typeRedList.Count > 0)
		{
			StartCoroutine(EnvironmentCoolTimeRutine(typeRedList, 86f));
		}
		if (typeGreenList.Count > 0)
		{
			StartCoroutine(EnvironmentCoolTimeRutine(typeGreenList, 68f));
		}
		StartCoroutine(EtcEnvironmentRutine());
	}

	private global::System.Collections.IEnumerator EtcEnvironmentRutine()
	{
		ShipBase playerShip = GameManager.Instance.playerShip;
		while (true)
		{
			if (GameManager.Instance.IsInIsland || GameManager.Instance.IsBossFighting || EnemySpawnManager.Instance.isSpawnPause)
			{
				yield return null;
				continue;
			}
			if ((int)PlayerInfo.Instance.myData.mainShipLevel >= 30)
			{
				PlayerInfo.Instance.myData.tornadoEventTotalTime += global::UnityEngine.Time.deltaTime;
			}
			PlayerInfo.Instance.myData.ghostEventTotalTime += global::UnityEngine.Time.deltaTime;
			if (PlayerInfo.Instance.myData.ghostEventTotalTime >= 1600f)
			{
				yield return new global::UnityEngine.WaitForSeconds(5f);
				EnemySpawnManager.Instance.SpawnGhost();
				PlayerInfo.Instance.myData.ghostEventTotalTime -= 1600f;
				PlayerInfo.Instance.SaveData();
				AudioManager.Instance.PlaySFX("ghost_spawn");
			}
			if (playerShip == null)
			{
				playerShip = GameManager.Instance.playerShip;
				yield return null;
				continue;
			}
			if (PlayerInfo.Instance.myData.tornadoEventTotalTime >= 420f)
			{
				yield return new global::UnityEngine.WaitForSeconds(5f);
				int num = global::UnityEngine.Random.Range(3, 8);
				for (int i = 0; i < num; i++)
				{
					global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(-50f, 50f), 0f) * playerShip.transform.forward;
					global::UnityEngine.Vector3 position = playerShip.transform.position + vector * global::UnityEngine.Random.Range(20f, 80f);
					global::UnityEngine.Object.Instantiate(tornadoSrc, position, global::UnityEngine.Quaternion.identity);
				}
				PlayerInfo.Instance.myData.tornadoEventTotalTime -= 420f;
				PlayerInfo.Instance.SaveData();
			}
			yield return null;
		}
	}

	private global::System.Collections.IEnumerator EnvironmentCoolTimeRutine(global::System.Collections.Generic.List<EnvironmentObject> list, float coolTime)
	{
		float totalTime = 0f;
		bool isPause = false;
		while (true)
		{
			if (GameManager.Instance.IsInIsland)
			{
				totalTime = 0f;
				yield return null;
				continue;
			}
			if (isPause)
			{
				yield return null;
				continue;
			}
			totalTime += global::UnityEngine.Time.deltaTime;
			if (totalTime > coolTime)
			{
				totalTime -= coolTime;
				int index = global::UnityEngine.Random.Range(0, list.Count);
				EnvironmentObject environment = list[index];
				global::UnityEngine.Vector3 vector = global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(0f, 360f), 0f) * global::UnityEngine.Vector3.forward;
				environment.gameObject.SetActive(value: true);
				environment.transform.position = base.transform.position + vector * 200f;
				environment.transform.forward = -(global::UnityEngine.Quaternion.Euler(0f, global::UnityEngine.Random.Range(3f, 15f) * ((global::UnityEngine.Random.value <= 0.5f) ? (-1f) : 1f), 0f) * vector);
				environment.transform.parent = null;
				environment.InactiveAction = delegate
				{
					environment.transform.parent = base.transform;
					isPause = false;
				};
				isPause = true;
			}
			yield return null;
		}
	}
}
