namespace EZObjectPools
{
	[global::UnityEngine.AddComponentMenu("EZ Object Pool/Object Pool")]
	public class EZObjectPool : global::UnityEngine.MonoBehaviour
	{
		private static global::System.Collections.Generic.Dictionary<string, global::EZObjectPools.EZObjectPool> SharedPools = new global::System.Collections.Generic.Dictionary<string, global::EZObjectPools.EZObjectPool>();

		private static global::UnityEngine.GameObject Marker;

		public global::UnityEngine.GameObject Template;

		public string PoolName;

		public global::System.Collections.Generic.List<global::UnityEngine.GameObject> ObjectList;

		public bool AutoResize;

		public int PoolSize = 100;

		public bool InstantiateOnAwake;

		public bool Shared;

		private global::System.Collections.Generic.List<global::UnityEngine.GameObject> AvailableObjects;

		public static global::EZObjectPools.EZObjectPool CreateObjectPool(global::UnityEngine.GameObject template, string name, int size, bool autoResize, bool instantiateImmediate, bool shared)
		{
			if (Marker == null)
			{
				Marker = new global::UnityEngine.GameObject("EZ Object Pools Container");
				SharedPools.Clear();
			}
			if (shared)
			{
				if (SharedPools.ContainsKey(name))
				{
					return SharedPools[name];
				}
				global::UnityEngine.GameObject obj = new global::UnityEngine.GameObject(name);
				global::EZObjectPools.EZObjectPool eZObjectPool = obj.AddComponent<global::EZObjectPools.EZObjectPool>();
				eZObjectPool.InstantiateOnAwake = false;
				eZObjectPool.SetProperties(template, size, name, autoResize);
				SharedPools.Add(name, eZObjectPool);
				if (instantiateImmediate)
				{
					eZObjectPool.InstantiatePool();
				}
				obj.transform.parent = Marker.transform;
				return eZObjectPool;
			}
			global::UnityEngine.GameObject obj2 = new global::UnityEngine.GameObject(name);
			global::EZObjectPools.EZObjectPool eZObjectPool2 = obj2.AddComponent<global::EZObjectPools.EZObjectPool>();
			eZObjectPool2.InstantiateOnAwake = false;
			eZObjectPool2.SetProperties(template, size, name, autoResize);
			if (instantiateImmediate)
			{
				eZObjectPool2.InstantiatePool();
			}
			obj2.transform.parent = Marker.transform;
			return eZObjectPool2;
		}

		private void Awake()
		{
			if (Marker == null)
			{
				Marker = new global::UnityEngine.GameObject("EZ Object Pools Container");
				SharedPools.Clear();
			}
			if (InstantiateOnAwake)
			{
				ObjectList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>(PoolSize);
				AvailableObjects = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>(PoolSize);
				InstantiatePool();
			}
			if (Shared)
			{
				SharedPools.Add(PoolName, this);
			}
		}

		public void SetProperties(global::UnityEngine.GameObject objectTemplate, int size, string name, bool autoResize)
		{
			Template = objectTemplate;
			PoolSize = size;
			ObjectList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>(size);
			AvailableObjects = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>(size);
			PoolName = name;
			AutoResize = autoResize;
		}

		public void InstantiatePool()
		{
			if (Template == null)
			{
				global::UnityEngine.Debug.LogError("EZ Object Pool: " + base.name + ": Template GameObject is null! Make sure you assigned a template either in the inspector or in your scripts.");
				return;
			}
			ClearPool();
			for (int i = 0; i < PoolSize; i++)
			{
				global::UnityEngine.GameObject gameObject = NewActiveObject();
				gameObject.SetActive(value: false);
				ObjectList.Add(gameObject);
			}
		}

		public bool TryGetNextObject(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot, out global::UnityEngine.GameObject obj)
		{
			if (ObjectList.Count == 0)
			{
				global::UnityEngine.Debug.LogError("EZ Object Pool " + PoolName + ", the pool has not been instantiated but you are trying to retrieve an object!");
			}
			int index = AvailableObjects.Count - 1;
			if (AvailableObjects.Count > 0)
			{
				if (AvailableObjects[index] == null)
				{
					global::UnityEngine.Debug.LogError("EZObjectPool " + PoolName + " has missing objects in its pool! Are you accidentally destroying any GameObjects retrieved from the pool?");
					obj = null;
					return false;
				}
				AvailableObjects[index].transform.position = pos;
				AvailableObjects[index].transform.rotation = rot;
				AvailableObjects[index].SetActive(value: true);
				obj = AvailableObjects[index];
				AvailableObjects.RemoveAt(index);
				return true;
			}
			if (AutoResize)
			{
				global::UnityEngine.GameObject gameObject = NewActiveObject();
				gameObject.transform.position = pos;
				gameObject.transform.rotation = rot;
				ObjectList.Add(gameObject);
				obj = gameObject;
				return true;
			}
			obj = null;
			return false;
		}

		public void TryGetNextObject(global::UnityEngine.Vector3 pos, global::UnityEngine.Quaternion rot)
		{
			if (ObjectList.Count == 0)
			{
				global::UnityEngine.Debug.LogError("EZ Object Pool " + PoolName + ", the pool has not been instantiated but you are trying to retrieve an object!");
			}
			int index = AvailableObjects.Count - 1;
			if (AvailableObjects.Count > 0)
			{
				if (AvailableObjects[index] == null)
				{
					global::UnityEngine.Debug.LogError("EZObjectPool " + PoolName + " has missing objects in its pool! Are you accidentally destroying any GameObjects retrieved from the pool?");
					return;
				}
				AvailableObjects[index].transform.position = pos;
				AvailableObjects[index].transform.rotation = rot;
				AvailableObjects[index].SetActive(value: true);
				AvailableObjects.RemoveAt(index);
			}
			else if (AutoResize)
			{
				global::UnityEngine.GameObject gameObject = NewActiveObject();
				gameObject.transform.position = pos;
				gameObject.transform.rotation = rot;
				ObjectList.Add(gameObject);
			}
		}

		private global::UnityEngine.GameObject NewActiveObject()
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(Template);
			gameObject.transform.parent = base.transform;
			global::EZObjectPools.PooledObject component = gameObject.GetComponent<global::EZObjectPools.PooledObject>();
			if ((bool)component)
			{
				component.ParentPool = this;
			}
			else
			{
				gameObject.AddComponent<global::EZObjectPools.PooledObject>().ParentPool = this;
			}
			return gameObject;
		}

		public void ClearPool()
		{
			for (int i = 0; i < ObjectList.Count; i++)
			{
				global::UnityEngine.Object.Destroy(ObjectList[i]);
			}
			ObjectList.Clear();
			AvailableObjects.Clear();
		}

		public void DeletePool(bool deleteActiveObjects)
		{
			for (int i = 0; i < ObjectList.Count; i++)
			{
				if (!ObjectList[i].activeInHierarchy || (ObjectList[i].activeInHierarchy && deleteActiveObjects))
				{
					global::UnityEngine.Object.Destroy(ObjectList[i]);
				}
			}
		}

		public void AddToAvailableObjects(global::UnityEngine.GameObject obj)
		{
			AvailableObjects.Add(obj);
		}

		public int ActiveObjectCount()
		{
			return ObjectList.Count - AvailableObjects.Count;
		}

		public int AvailableObjectCount()
		{
			return AvailableObjects.Count;
		}

		public void AllInActive()
		{
			AvailableObjects.Clear();
			for (int i = 0; i < ObjectList.Count; i++)
			{
				AvailableObjects.Add(ObjectList[i]);
				if (ObjectList[i] != null && ObjectList[i].activeSelf)
				{
					ObjectList[i].SetActive(value: false);
				}
			}
		}
	}
}
