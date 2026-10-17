public class Sunken : global::UnityEngine.MonoBehaviour
{
	private struct Overlap
	{
		public int count;

		public float time;
	}

	[global::UnityEngine.HideInInspector]
	public new global::UnityEngine.Transform transform;

	private global::System.Collections.Generic.Dictionary<ShipBase, Sunken.Overlap> shipDic = new global::System.Collections.Generic.Dictionary<ShipBase, Sunken.Overlap>();

	private const float intervalTime = 1f;

	private global::UnityEngine.Collider myCollider;

	private bool isPassiveActive;

	private void Awake()
	{
		transform = GetComponent<global::UnityEngine.Transform>();
		myCollider = GetComponent<global::UnityEngine.Collider>();
	}

	private void FixedUpdate()
	{
		if (isPassiveActive)
		{
			return;
		}
		foreach (ShipBase item in global::System.Linq.Enumerable.ToList(shipDic.Keys))
		{
			if (!(item != null))
			{
				continue;
			}
			Sunken.Overlap value = shipDic[item];
			value.time += global::UnityEngine.Time.fixedDeltaTime;
			if (value.time >= 1f)
			{
				value.time -= 1f;
				if (item == GameManager.Instance.playerShip)
				{
					if (isPassiveActive)
					{
						continue;
					}
					CameraShakeManager.Instance.CameraShake(0.25f, 0.8f);
				}
				global::UnityEngine.Vector3 velocity = item.rigidbody.velocity;
				velocity *= 0.15f;
				item.rigidbody.velocity = velocity;
				int damage = global::UnityEngine.Mathf.Max(1, item.myHealth.HP_MAX / 100);
				item.myHealth.Hit(damage);
			}
			shipDic[item] = value;
		}
	}

	private void OnTriggerEnter(global::UnityEngine.Collider col)
	{
		ShipBase shipBase = col.GetComponent<ShipBase>();
		if (shipBase == null)
		{
			shipBase = col.GetComponentInParent<ShipBase>();
		}
		if (shipBase == null)
		{
			shipBase = col.GetComponentInChildren<ShipBase>();
		}
		if (shipBase == null)
		{
			return;
		}
		global::System.Collections.Generic.List<ESailor> skill_avoidEnvironment = SailorDataManager.Instance.skill_avoidEnvironment;
		if (skill_avoidEnvironment.Count > 0)
		{
			MySailorInventory mySailorData = MyInventoryDataManager.Instance.GetMySailorData(skill_avoidEnvironment[0]);
			if (mySailorData.isHave && mySailorData.level >= 2)
			{
				isPassiveActive = true;
			}
		}
		if (!shipDic.ContainsKey(shipBase))
		{
			Sunken.Overlap value = new Sunken.Overlap
			{
				count = 1,
				time = 0f
			};
			shipDic.Add(shipBase, value);
			ShipController component = shipBase.GetComponent<ShipController>();
			if (component != null)
			{
				component.EnterSunken();
			}
		}
		else
		{
			Sunken.Overlap value2 = shipDic[shipBase];
			value2.count++;
			shipDic[shipBase] = value2;
		}
	}

	private void OnTriggerExit(global::UnityEngine.Collider col)
	{
		ShipBase shipBase = col.GetComponent<ShipBase>();
		if (shipBase == null)
		{
			shipBase = col.GetComponentInParent<ShipBase>();
		}
		if (shipBase == null)
		{
			shipBase = col.GetComponentInChildren<ShipBase>();
		}
		if (shipBase == null || !shipDic.ContainsKey(shipBase))
		{
			return;
		}
		Sunken.Overlap value = shipDic[shipBase];
		if (--value.count <= 0)
		{
			shipDic.Remove(shipBase);
			ShipController component = shipBase.GetComponent<ShipController>();
			if (component != null)
			{
				component.ExitSunken();
			}
		}
		else
		{
			shipDic[shipBase] = value;
		}
	}
}
