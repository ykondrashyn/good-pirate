public class ObjectPoolManager : global::UnityEngine.MonoBehaviour
{
	public global::System.Collections.Generic.Dictionary<string, global::EZObjectPools.EZObjectPool> objectPoolTable;

	private static ObjectPoolManager sInstance;

	public static ObjectPoolManager Instance
	{
		get
		{
			return sInstance;
		}
		private set
		{
		}
	}

	public static bool ContainsKeyTable(string name)
	{
		return Instance.objectPoolTable.ContainsKey(name);
	}

	public static bool TryGetNextObject(string name, global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, out global::UnityEngine.GameObject obj)
	{
		if (ContainsKeyTable(name))
		{
			return Instance.objectPoolTable[name].TryGetNextObject(pos, rot, out obj);
		}
		obj = null;
		return false;
	}

	public static void TryGetNextObject(string name, global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot)
	{
		if (ContainsKeyTable(name))
		{
			Instance.objectPoolTable[name].TryGetNextObject(pos, rot);
		}
	}

	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			objectPoolTable = new global::System.Collections.Generic.Dictionary<string, global::EZObjectPools.EZObjectPool>();
			global::EZObjectPools.EZObjectPool[] componentsInChildren = GetComponentsInChildren<global::EZObjectPools.EZObjectPool>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				objectPoolTable.Add(componentsInChildren[i].PoolName, componentsInChildren[i]);
			}
		}
		else
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void AllDisable()
	{
		foreach (global::System.Collections.Generic.KeyValuePair<string, global::EZObjectPools.EZObjectPool> item in objectPoolTable)
		{
			item.Value.AllInActive();
		}
	}
}
