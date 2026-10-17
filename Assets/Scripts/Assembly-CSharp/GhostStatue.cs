public class GhostStatue : global::UnityEngine.MonoBehaviour
{
	private Health myHealth;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject ghostEnemySrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform handTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Transform heartTrans;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem heartExplosionPS;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.ParticleSystem heartIdlePs;

	[global::UnityEngine.SerializeField]
	private global::System.Collections.Generic.List<ShipBase> myShipList;

	private bool isDie;

	private HpBar hpBar;

	private void Awake()
	{
		myHealth = GetComponent<Health>();
		myShipList = new global::System.Collections.Generic.List<ShipBase>();
		EnemySpawnManager.Instance.AddOnlyHealth(myHealth);
		myHealth.AddDieAction(Die);
		myHealth.AddHealAction(HpBarRefresh);
		myHealth.AddHitAction(HpBarRefresh);
		myHealth.AddHpSetAction(HpBarRefresh);
		int healthPoint = (int)((float)ShipDataManager.Instance.mainShipData.dataArray[(int)PlayerInfo.Instance.myData.mainShipLevel - 1].Health * 0.8f);
		myHealth.SetHealthPoint(healthPoint);
	}

	private void Start()
	{
		if (hpBar == null)
		{
			hpBar = HpBarManager.Instance.Generate(ELayerType.Enemy, isBig: true);
		}
		if (hpBar != null)
		{
			hpBar.SetGauge(myHealth.HpRatio);
		}
		if (GameManager.Instance.IsInIsland)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
		float num = 10f;
		global::UnityEngine.Vector3 position = base.transform.position;
		global::UnityEngine.Vector3 pos = position + new global::UnityEngine.Vector3(0f - num, 0f, num);
		global::UnityEngine.Vector3 pos2 = position + new global::UnityEngine.Vector3(num, 0f, num);
		global::UnityEngine.Vector3 pos3 = position + new global::UnityEngine.Vector3(0f - num, 0f, 0f - num);
		global::UnityEngine.Vector3 pos4 = position + new global::UnityEngine.Vector3(num, 0f, 0f - num);
		global::UnityEngine.Quaternion rot = global::UnityEngine.Quaternion.Euler(0f, -180f, 0f);
		SpawnEnemy(pos, rot);
		SpawnEnemy(pos2, rot);
		SpawnEnemy(pos3, rot);
		SpawnEnemy(pos4, rot);
	}

	private void HpBarRefresh()
	{
		if ((bool)hpBar)
		{
			hpBar.SetGauge(myHealth.HpRatio);
		}
	}

	private void LateUpdate()
	{
		if ((bool)hpBar)
		{
			hpBar.SetPosition(base.transform.position + new global::UnityEngine.Vector3(0f, -1f, 0f));
		}
	}

	private void OnEnable()
	{
		if ((bool)hpBar)
		{
			hpBar.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if ((bool)hpBar)
		{
			hpBar.gameObject.SetActive(value: false);
		}
	}

	private void SpawnEnemy(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot)
	{
		ShipGenerator.Instance.SpawnEnemy(global::UnityEngine.Vector3.zero, global::UnityEngine.Quaternion.identity, out var ship);
		ship.SpawnShipModel(ghostEnemySrc, ELayerType.Enemy);
		ship.SetBaseData(ShipDataManager.Instance.GetShipBaseData(EShipType.Ghost));
		ship.SetLevel(PlayerInfo.Instance.myData.mainShipLevel);
		EnemySpawnManager.Instance.AddOnlyHealth(ship.myHealth);
		ship.transform.position = pos;
		ship.transform.rotation = rot;
		myShipList.Add(ship);
		GameManager.Instance.AddObject(ship.gameObject);
	}

	private void Die()
	{
		if (isDie)
		{
			return;
		}
		isDie = true;
		if ((bool)hpBar)
		{
			global::UnityEngine.Object.Destroy(hpBar.gameObject);
		}
		foreach (ShipBase myShip in myShipList)
		{
			myShip.Die();
		}
		heartTrans.GetComponent<global::UnityEngine.Animator>().enabled = false;
		heartIdlePs.gameObject.SetActive(value: false);
		StartCoroutine(DieRutine());
	}

	private global::System.Collections.IEnumerator DieRutine()
	{
		global::DG.Tweening.ShortcutExtensions.DOShakePosition(heartTrans, 2f, 0.8f, 80, 360f);
		global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOScale(heartTrans, 4f, 2f), delegate
		{
			global::DG.Tweening.TweenSettingsExtensions.OnComplete(global::DG.Tweening.ShortcutExtensions.DOLocalMoveY(handTrans, -7f, 2f), delegate
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			});
		});
		yield return new global::UnityEngine.WaitForSeconds(2.5f);
		heartTrans.gameObject.SetActive(value: false);
		heartExplosionPS.Emit(40);
	}
}
