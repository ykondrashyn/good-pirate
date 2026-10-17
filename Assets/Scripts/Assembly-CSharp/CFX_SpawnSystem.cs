public class CFX_SpawnSystem : global::UnityEngine.MonoBehaviour
{
	private static CFX_SpawnSystem instance;

	public global::UnityEngine.GameObject[] objectsToPreload = new global::UnityEngine.GameObject[0];

	public int[] objectsToPreloadTimes = new int[0];

	public bool hideObjectsInHierarchy;

	public bool spawnAsChildren = true;

	public bool onlyGetInactiveObjects;

	public bool instantiateIfNeeded;

	private bool allObjectsLoaded;

	private global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::UnityEngine.GameObject>> instantiatedObjects = new global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::UnityEngine.GameObject>>();

	private global::System.Collections.Generic.Dictionary<int, int> poolCursors = new global::System.Collections.Generic.Dictionary<int, int>();

	public static bool AllObjectsLoaded => instance.allObjectsLoaded;

	public static global::UnityEngine.GameObject GetNextObject(global::UnityEngine.GameObject sourceObj, bool activateObject = true)
	{
		int instanceID = sourceObj.GetInstanceID();
		if (!instance.poolCursors.ContainsKey(instanceID))
		{
			global::UnityEngine.Debug.LogError("[CFX_SpawnSystem.GetNextObject()] Object hasn't been preloaded: " + sourceObj.name + " (ID:" + instanceID + ")\n", instance);
			return null;
		}
		int num = instance.poolCursors[instanceID];
		global::UnityEngine.GameObject gameObject = null;
		if (instance.onlyGetInactiveObjects)
		{
			int num2 = num;
			while (true)
			{
				gameObject = instance.instantiatedObjects[instanceID][num];
				instance.increasePoolCursor(instanceID);
				num = instance.poolCursors[instanceID];
				if (gameObject != null && !gameObject.activeSelf)
				{
					break;
				}
				if (num == num2)
				{
					if (instance.instantiateIfNeeded)
					{
						global::UnityEngine.Debug.Log("[CFX_SpawnSystem.GetNextObject()] A new instance has been created for \"" + sourceObj.name + "\" because no active instance were found in the pool.\n", instance);
						PreloadObject(sourceObj);
						global::System.Collections.Generic.List<global::UnityEngine.GameObject> list = instance.instantiatedObjects[instanceID];
						gameObject = list[list.Count - 1];
						break;
					}
					global::UnityEngine.Debug.LogWarning("[CFX_SpawnSystem.GetNextObject()] There are no active instances available in the pool for \"" + sourceObj.name + "\"\nYou may need to increase the preloaded object count for this prefab?", instance);
					return null;
				}
			}
		}
		else
		{
			gameObject = instance.instantiatedObjects[instanceID][num];
			instance.increasePoolCursor(instanceID);
		}
		if (activateObject && gameObject != null)
		{
			gameObject.SetActive(value: true);
		}
		return gameObject;
	}

	public static void PreloadObject(global::UnityEngine.GameObject sourceObj, int poolSize = 1)
	{
		instance.addObjectToPool(sourceObj, poolSize);
	}

	public static void UnloadObjects(global::UnityEngine.GameObject sourceObj)
	{
		instance.removeObjectsFromPool(sourceObj);
	}

	private void addObjectToPool(global::UnityEngine.GameObject sourceObject, int number)
	{
		int instanceID = sourceObject.GetInstanceID();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			instantiatedObjects.Add(instanceID, new global::System.Collections.Generic.List<global::UnityEngine.GameObject>());
			poolCursors.Add(instanceID, 0);
		}
		for (int i = 0; i < number; i++)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(sourceObject);
			gameObject.SetActive(value: false);
			CFX_AutoDestructShuriken[] componentsInChildren = gameObject.GetComponentsInChildren<CFX_AutoDestructShuriken>(includeInactive: true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].OnlyDeactivate = true;
			}
			CFX_LightIntensityFade[] componentsInChildren2 = gameObject.GetComponentsInChildren<CFX_LightIntensityFade>(includeInactive: true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].autodestruct = false;
			}
			instantiatedObjects[instanceID].Add(gameObject);
			if (hideObjectsInHierarchy)
			{
				gameObject.hideFlags = global::UnityEngine.HideFlags.HideInHierarchy;
			}
			if (spawnAsChildren)
			{
				gameObject.transform.parent = base.transform;
			}
		}
	}

	private void removeObjectsFromPool(global::UnityEngine.GameObject sourceObject)
	{
		int instanceID = sourceObject.GetInstanceID();
		if (!instantiatedObjects.ContainsKey(instanceID))
		{
			global::UnityEngine.Debug.LogWarning("[CFX_SpawnSystem.removeObjectsFromPool()] There aren't any preloaded object for: " + sourceObject.name + " (ID:" + instanceID + ")\n", base.gameObject);
			return;
		}
		for (int num = instantiatedObjects[instanceID].Count - 1; num >= 0; num--)
		{
			global::UnityEngine.GameObject obj = instantiatedObjects[instanceID][num];
			instantiatedObjects[instanceID].RemoveAt(num);
			global::UnityEngine.Object.Destroy(obj);
		}
		instantiatedObjects.Remove(instanceID);
		poolCursors.Remove(instanceID);
	}

	private void increasePoolCursor(int uniqueId)
	{
		instance.poolCursors[uniqueId]++;
		if (instance.poolCursors[uniqueId] >= instance.instantiatedObjects[uniqueId].Count)
		{
			instance.poolCursors[uniqueId] = 0;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			global::UnityEngine.Debug.LogWarning("CFX_SpawnSystem: There should only be one instance of CFX_SpawnSystem per Scene!\n", base.gameObject);
		}
		instance = this;
	}

	private void Start()
	{
		allObjectsLoaded = false;
		for (int i = 0; i < objectsToPreload.Length; i++)
		{
			PreloadObject(objectsToPreload[i], objectsToPreloadTimes[i]);
		}
		allObjectsLoaded = true;
	}
}
