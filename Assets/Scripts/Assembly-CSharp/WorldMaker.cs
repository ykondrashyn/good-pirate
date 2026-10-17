[global::UnityEngine.ExecuteInEditMode]
public class WorldMaker : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Transform distATrans;

	[global::UnityEngine.HideInInspector]
	public global::UnityEngine.Transform distBTrans;

	[global::UnityEngine.HideInInspector]
	public float distBetween;

	[global::UnityEngine.HideInInspector]
	public string distTime;

	[global::UnityEngine.HideInInspector]
	public int cubeNum = 1;

	[global::UnityEngine.HideInInspector]
	public int worldNum = 1;

	[global::UnityEngine.HideInInspector]
	public bool isShowOnlyTwoIsland;

	public WorldPivot startPosition;

	private const string gameDataProjectFilePathLoad = "MapData/Cube_{0}_{1}";

	private const string gameDataProjectFilePathSave = "/Resources/MapData/";

	private const string FileName = "Cube_{0}_{1}.json";

	public int worldSizeX = 10000;

	public int worldSizeZ = 10000;

	public bool isHaveGate = true;

	public bool removeWall;

	public int gridSize = 11;

	public int islandCount = 20;

	public int islandStartId;

	public int[] islandLevel;

	public int[] islandLevelCount;

	private global::System.Collections.Generic.List<global::UnityEngine.Transform> makeIslandTransList = new global::System.Collections.Generic.List<global::UnityEngine.Transform>();

	[global::UnityEngine.Header("바다/빛 색")]
	public global::UnityEngine.Color cameraColor = myColor.GetColor(44, 141, 245);

	public global::UnityEngine.Color lightColor = global::UnityEngine.Color.white;

	[global::UnityEngine.HideInInspector]
	public int generateRockCount;

	public void ClipBoard()
	{
		global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
		global::UnityEngine.Object[] array = global::UnityEngine.Object.FindObjectsOfType(typeof(Island));
		global::System.Collections.Generic.List<global::UnityEngine.Object> list = global::System.Linq.Enumerable.ToList(array);
		list.Sort(CompareIsland);
		if (array.Length == 0)
		{
			return;
		}
		foreach (global::UnityEngine.Object item in list)
		{
			Island island = item as Island;
			if (island.status.islandId != -1)
			{
				stringBuilder.Append($"{island.transform.position.x}\t{island.transform.position.z}\n");
			}
		}
	}

	public void MakeIsland()
	{
	}

	public void MakeRocks()
	{
	}

	private bool IsCanIslandSpawnPosition(global::UnityEngine.Vector3 pos, float minInterval)
	{
		bool result = true;
		for (int i = 0; i < makeIslandTransList.Count; i++)
		{
			global::UnityEngine.Vector3 position = makeIslandTransList[i].transform.position;
			if ((pos - position).magnitude < minInterval)
			{
				result = false;
				break;
			}
		}
		float num = -worldSizeX / 2;
		float num2 = worldSizeX / 2;
		float num3 = -worldSizeZ / 2;
		float num4 = worldSizeZ / 2;
		if (pos.x < num || pos.x > num2 || pos.z < num3 || pos.z > num4)
		{
			global::UnityEngine.Debug.Log("1");
			result = false;
		}
		return result;
	}

	private int PatrolSort(object a, object b)
	{
		MapDataPatrolObject mapDataPatrolObject = a as MapDataPatrolObject;
		MapDataPatrolObject mapDataPatrolObject2 = b as MapDataPatrolObject;
		if (mapDataPatrolObject.patrolStatus.patrolKey != "" && mapDataPatrolObject2.patrolStatus.patrolKey != "")
		{
			return 0;
		}
		if (mapDataPatrolObject.patrolStatus.patrolKey == "")
		{
			return 1;
		}
		if (mapDataPatrolObject2.patrolStatus.patrolKey == "")
		{
			return -1;
		}
		return 0;
	}

	private int BossSort(object a, object b)
	{
		Boss boss = a as Boss;
		Boss boss2 = b as Boss;
		if (boss.status.bossKey != "" && boss2.status.bossKey != "")
		{
			global::UnityEngine.Debug.Log("0");
			return 0;
		}
		if (boss.status.bossKey == "")
		{
			global::UnityEngine.Debug.Log("1");
			return 1;
		}
		if (boss2.status.bossKey == "")
		{
			global::UnityEngine.Debug.Log("-1");
			return -1;
		}
		return 0;
	}

	public void Save()
	{
		global::UnityEngine.Object[] array = global::UnityEngine.Object.FindObjectsOfType(typeof(Island));
		global::UnityEngine.Object[] array2 = global::UnityEngine.Object.FindObjectsOfType(typeof(Rock));
		global::UnityEngine.Object[] array3 = global::UnityEngine.Object.FindObjectsOfType(typeof(Gate));
		global::UnityEngine.Object[] array4 = global::UnityEngine.Object.FindObjectsOfType(typeof(Boss));
		global::UnityEngine.Object[] source = global::UnityEngine.Object.FindObjectsOfType(typeof(MapDataPatrolObject));
		global::UnityEngine.Object[] array5 = global::UnityEngine.Object.FindObjectsOfType(typeof(MapEnvironmentObject));
		WorldData worldData = new WorldData
		{
			cameraColor = cameraColor,
			lightColor = lightColor,
			cubeNum = cubeNum,
			worldNum = worldNum,
			haveGate = isHaveGate,
			removeWall = removeWall
		};
		global::UnityEngine.Object[] array6 = array;
		for (int i = 0; i < array6.Length; i++)
		{
			Island island = array6[i] as Island;
			IslandStatus status = island.status;
			status.islandPosition = island.transform.position;
			status.islandRotation = island.transform.rotation;
			worldData.islandStatuslist.Add(status);
		}
		array6 = array2;
		for (int i = 0; i < array6.Length; i++)
		{
			RockStatus status2 = (array6[i] as Rock).GetStatus();
			worldData.rockStatuslist.Add(status2);
		}
		int num = 0;
		global::System.Collections.Generic.List<global::UnityEngine.Object> list = global::System.Linq.Enumerable.ToList(source);
		list.Sort(PatrolSort);
		foreach (global::UnityEngine.Object item in list)
		{
			MapDataPatrolObject mapDataPatrolObject = item as MapDataPatrolObject;
			PatrolStatus patrolStatus = mapDataPatrolObject.patrolStatus;
			if (patrolStatus.patrolKey == "")
			{
				patrolStatus.patrolKey = $"Patrol_{cubeNum}_{worldNum}_{num}";
			}
			patrolStatus.startPosition = mapDataPatrolObject.transform.position;
			patrolStatus.startRotation = mapDataPatrolObject.transform.rotation;
			patrolStatus.patrolCourseList = new global::System.Collections.Generic.List<global::UnityEngine.Vector3>();
			foreach (global::UnityEngine.Transform coruse in mapDataPatrolObject.coruseList)
			{
				patrolStatus.patrolCourseList.Add(coruse.position);
			}
			patrolStatus.enemyType = mapDataPatrolObject.shipType;
			worldData.patrolStatusList.Add(patrolStatus);
			num++;
		}
		int num2 = 0;
		global::System.Linq.Enumerable.ToList(array4).Sort(BossSort);
		array6 = array4;
		for (int i = 0; i < array6.Length; i++)
		{
			Boss obj = array6[i] as Boss;
			BossStatus status3 = obj.status;
			if (status3.bossKey == "")
			{
				status3.bossKey = $"Boss_{cubeNum}_{worldNum}_{num2}";
			}
			global::UnityEngine.Transform component = obj.GetComponent<global::UnityEngine.Transform>();
			status3.position = component.position;
			status3.rotation = component.rotation;
			worldData.bossStatusList.Add(status3);
			num2++;
		}
		worldData.worldSizeX = worldSizeX;
		worldData.worldSizeZ = worldSizeZ;
		for (int j = 0; j < array3.Length; j++)
		{
			for (int k = 0; k < array3.Length; k++)
			{
				Gate gate = array3[k] as Gate;
				if (j == gate.Direction)
				{
					worldData.gateList.Add(gate.gateStatus);
					break;
				}
			}
		}
		array6 = array5;
		for (int i = 0; i < array6.Length; i++)
		{
			MapEnvironmentObject mapEnvironmentObject = array6[i] as MapEnvironmentObject;
			worldData.environmentList.Add(mapEnvironmentObject.GetStatus());
		}
		string contents = global::UnityEngine.JsonUtility.ToJson(worldData);
		if (!global::System.IO.File.Exists(global::UnityEngine.Application.dataPath + "/Resources/MapData/"))
		{
			global::System.IO.Directory.CreateDirectory(global::UnityEngine.Application.dataPath + "/Resources/MapData/");
		}
		global::System.IO.File.WriteAllText(global::UnityEngine.Application.dataPath + "/Resources/MapData/" + $"Cube_{cubeNum}_{worldNum}.json", contents);
	}

	public void Load()
	{
	}

	private void MakeWallAngGate(WorldData worldData = null)
	{
	}

	public WorldData GetWorldData(int cube, int world)
	{
		global::UnityEngine.TextAsset textAsset = global::UnityEngine.Resources.Load<global::UnityEngine.TextAsset>($"MapData/Cube_{cube}_{world}");
		if (textAsset == null)
		{
			global::UnityEngine.Debug.Log("TextAsset is NULL");
			return null;
		}
		WorldData worldData = global::UnityEngine.JsonUtility.FromJson<WorldData>(textAsset.text);
		if (worldData.worldSizeX == 0)
		{
			worldData.worldSizeX = 10000;
			worldData.worldSizeZ = 10000;
		}
		return worldData;
	}

	private void Update()
	{
		if (distATrans != null && distBTrans != null)
		{
			distBetween = (distATrans.position - distBTrans.position).magnitude;
		}
		else
		{
			distBetween = 0f;
		}
	}

	private int CompareIsland(object a, object b)
	{
		Island island = a as Island;
		Island island2 = b as Island;
		if (island.status.islandId > island2.status.islandId)
		{
			return 1;
		}
		if (island.status.islandId < island2.status.islandId)
		{
			return -1;
		}
		return 0;
	}
}
