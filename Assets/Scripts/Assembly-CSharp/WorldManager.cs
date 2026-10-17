public class WorldManager : global::UnityEngine.MonoBehaviour
{
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject gateSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject wallSrc;

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<Island> islandList = new global::System.Collections.Generic.List<Island>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<Rock> rockList = new global::System.Collections.Generic.List<Rock>();

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<global::UnityEngine.GameObject> environmentList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	private global::System.Collections.Generic.List<global::System.Collections.Generic.List<Grid>> gridList;

	public int minX;

	public int maxX;

	public int minZ;

	public int maxZ;

	private float intervalX;

	private float intervalZ;

	private const int gridCount = 15;

	private global::System.Collections.Generic.Dictionary<int, Island> islandDic = new global::System.Collections.Generic.Dictionary<int, Island>();

	private global::System.Collections.Generic.Dictionary<int, UI_WorldMapIsland> worldmapIslandDic = new global::System.Collections.Generic.Dictionary<int, UI_WorldMapIsland>();

	[global::UnityEngine.HideInInspector]
	public Gate[] gates = new Gate[4];

	[global::UnityEngine.HideInInspector]
	public global::System.Collections.Generic.List<global::UnityEngine.GameObject> wallObjList = new global::System.Collections.Generic.List<global::UnityEngine.GameObject>();

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Camera mainCamera;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.Light directionalLight;

	private global::System.Collections.Generic.Dictionary<PatrolStatus, UI_PatrolCooltime> patrolDic = new global::System.Collections.Generic.Dictionary<PatrolStatus, UI_PatrolCooltime>(new PatrolStatusComparer());

	private global::UnityEngine.GameObject destinationIconObj;

	private bool isGenerate;

	private global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder();

	private global::System.Collections.Generic.List<ShipEnemyController> patrolEnemySpawnList = new global::System.Collections.Generic.List<ShipEnemyController>();

	private global::System.Collections.Generic.List<Boss> bossList = new global::System.Collections.Generic.List<Boss>();

	[global::UnityEngine.Header("BossPrefabs")]
	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject krakenSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject treeSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject turtleSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject sharkSrc;

	[global::UnityEngine.SerializeField]
	private global::UnityEngine.GameObject sirenSrc;

	public WorldData selectedWorldData { get; private set; }

	public Gate TopGate => gates[0];

	public Gate BottomGate => gates[1];

	public Gate LeftGate => gates[2];

	public Gate RightGate => gates[3];

	private void Awake()
	{
		gridList = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<Grid>>();
		for (int i = 0; i < 15; i++)
		{
			gridList.Insert(i, new global::System.Collections.Generic.List<Grid>());
			for (int j = 0; j < 15; j++)
			{
				gridList[i].Insert(j, new Grid());
			}
		}
	}

	private void Update()
	{
		if (!UI_Manager.Instance.ui_worldMap.gameObject.activeSelf)
		{
			return;
		}
		foreach (global::System.Collections.Generic.KeyValuePair<PatrolStatus, UI_PatrolCooltime> item in global::System.Linq.Enumerable.ToList(patrolDic))
		{
			sb.Length = 0;
			global::System.TimeSpan leftCoolTime = CoolTimeManager.GetLeftCoolTime(item.Key.patrolKey);
			sb.Append((int)(leftCoolTime.TotalSeconds / 60.0));
			sb.Append(":");
			sb.Append($"{leftCoolTime.TotalSeconds % 60.0:00}");
			item.Value.SetText(sb.ToString());
			if (leftCoolTime.TotalSeconds <= 0.0)
			{
				SpawnPatrolEnemy(item.Key);
				global::UnityEngine.Object.Destroy(item.Value.gameObject);
				patrolDic.Remove(item.Key);
			}
		}
	}

	public void ClearWorld()
	{
		worldmapIslandDic.Clear();
		foreach (Island island in islandList)
		{
			if (island != null)
			{
				global::UnityEngine.Object.Destroy(island.gameObject);
			}
		}
		islandList.Clear();
		foreach (Rock rock in rockList)
		{
			if (rock != null)
			{
				global::UnityEngine.Object.Destroy(rock.gameObject);
			}
		}
		rockList.Clear();
		foreach (global::UnityEngine.GameObject environment in environmentList)
		{
			if (environment != null)
			{
				global::UnityEngine.Object.Destroy(environment);
			}
		}
		environmentList.Clear();
		EnemyAllDestroy();
		for (int i = 0; i < gridList.Count; i++)
		{
			foreach (Grid item in gridList[i])
			{
				item.haveObjList.Clear();
			}
		}
	}

	public void GenerateWorld(int cube, int world)
	{
		islandDic.Clear();
		isGenerate = true;
		WorldData worldData = null;
		worldData = GameManager.Instance.worldMaker.GetWorldData(cube, world);
		selectedWorldData = worldData;
		mainCamera.backgroundColor = selectedWorldData.cameraColor;
		directionalLight.color = selectedWorldData.lightColor;
		GameManager.Instance.weatherManager.OriginalColorChange(selectedWorldData.cameraColor, selectedWorldData.lightColor);
		UI_Manager.Instance.ui_worldMap.SetSize(selectedWorldData.worldSizeX, selectedWorldData.worldSizeZ);
		maxX = selectedWorldData.worldSizeX / 2;
		minX = -maxX;
		maxZ = selectedWorldData.worldSizeZ / 2;
		minZ = -maxZ;
		intervalX = (float)selectedWorldData.worldSizeX / 15f;
		intervalZ = (float)selectedWorldData.worldSizeX / 15f;
		if (selectedWorldData != null)
		{
			foreach (IslandStatus item in selectedWorldData.islandStatuslist)
			{
				IslandData islandData = IslandDataManager.Instance.GetIslandData(item.islandType);
				global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(islandData.source);
				Island component = gameObject.GetComponent<Island>();
				component.SetIslandData(item);
				AddObject(gameObject);
				islandList.Add(component);
				if (item.islandType == EIsland.Island_BLANK)
				{
					continue;
				}
				global::UnityEngine.Transform transform = UI_Manager.Instance.ui_worldMap.GenerateObject(item.islandPosition, item.islandRotation, islandData.minimapIcon);
				if (component.myDock != null)
				{
					UI_Manager.Instance.ui_worldMap.GenerateDock(component.myDock.transform.position, transform);
					if (PlayerInfo.Instance.myData.destinationIslandId == item.islandId)
					{
						destinationIconObj = UI_Manager.Instance.ui_worldMap.GenerateDestinationIcon(component.myDock.transform.position, transform);
					}
				}
				if (item.islandId != -1)
				{
					if (!islandDic.ContainsKey(item.islandId))
					{
						islandDic.Add(item.islandId, component);
					}
					UI_WorldMapIsland component2 = transform.gameObject.GetComponent<UI_WorldMapIsland>();
					if (!worldmapIslandDic.ContainsKey(item.islandId))
					{
						worldmapIslandDic.Add(item.islandId, component2);
					}
					component2.targetScalePivot = UI_Manager.Instance.ui_worldMap.PivotTransform;
					component2.SetData(component);
				}
			}
			foreach (RockStatus item2 in selectedWorldData.rockStatuslist)
			{
				RockData rockData = IslandDataManager.Instance.GetRockData(item2.rockType);
				global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(rockData.source);
				Rock component3 = gameObject2.GetComponent<Rock>();
				component3.SetStatus(item2);
				AddObject(gameObject2);
				rockList.Add(component3);
				if (rockData.minimapIcon != null && !item2.isNotDrawWorldMap)
				{
					UI_Manager.Instance.ui_worldMap.GenerateObject(item2.rockPosition, item2.rockRotaion, rockData.minimapIcon, item2.rockLocalScale.x);
				}
			}
			foreach (PatrolStatus patrolStatus in selectedWorldData.patrolStatusList)
			{
				bool flag = true;
				if (CoolTimeManager.HaveKey(patrolStatus.patrolKey))
				{
					if (CoolTimeManager.GetLeftCoolTime(patrolStatus.patrolKey).TotalSeconds <= 0.0)
					{
						CoolTimeManager.DeleteCoolTime(patrolStatus.patrolKey);
					}
					else
					{
						AddPatrolCoolTimeList(patrolStatus);
						flag = false;
					}
				}
				if (flag)
				{
					SpawnPatrolEnemy(patrolStatus);
				}
			}
			foreach (EnvironmentStatus environment in selectedWorldData.environmentList)
			{
				global::UnityEngine.GameObject gameObject3 = global::UnityEngine.Object.Instantiate(EnvironmentDataManager.Instance.GetEnvironmentData(environment.myType).environmentSrc);
				gameObject3.GetComponent<MapEnvironmentObject>().SetStatus(environment);
				AddObject(gameObject3);
				environmentList.Add(gameObject3);
			}
			foreach (BossStatus bossStatus in selectedWorldData.bossStatusList)
			{
				if (PlayerInfo.Instance.GetIsClearBoss(bossStatus.bossKey))
				{
					global::UnityEngine.GameObject obj = UI_Manager.Instance.ui_worldMap.GenerateBossClear();
					global::UnityEngine.Vector3 position = bossStatus.position;
					obj.GetComponent<global::UnityEngine.RectTransform>().anchoredPosition = new global::UnityEngine.Vector2(position.x, position.z);
					obj.GetComponent<global::UnityEngine.UI.Image>().sprite = BossDataManager.Instance.GetBossData(bossStatus.bossType).bossIcon;
					continue;
				}
				global::UnityEngine.GameObject gameObject4 = null;
				gameObject4 = BossDataManager.Instance.GetBossData(bossStatus.bossType).bossSrc;
				if (gameObject4 != null)
				{
					Boss component4 = global::UnityEngine.Object.Instantiate(gameObject4, bossStatus.position, bossStatus.rotation).GetComponent<Boss>();
					component4.status = bossStatus;
					bossList.Add(component4);
				}
			}
		}
		MakeWallAngGate();
		if (selectedWorldData.haveGate)
		{
			UI_Manager.Instance.ui_worldMap.SetGateWorldText(gates);
		}
	}

	private void SpawnPatrolEnemy(PatrolStatus data)
	{
		int level = global::UnityEngine.Mathf.Max(data.minLevel, PlayerInfo.Instance.myData.mainShipLevel);
		ShipEnemyController component = EnemySpawnManager.Instance.Spawn(data.enemyType, level).GetComponent<ShipEnemyController>();
		component.SetPatrolData(data);
		patrolEnemySpawnList.Add(component);
	}

	public void PatrolEnemyReset()
	{
		for (int i = 0; i < patrolEnemySpawnList.Count; i++)
		{
			ShipEnemyController shipEnemyController = patrolEnemySpawnList[i];
			if (shipEnemyController != null)
			{
				shipEnemyController.PatrolReset();
				continue;
			}
			patrolEnemySpawnList.RemoveAt(i);
			i--;
		}
	}

	public void EnemyAllDestroy()
	{
		for (int i = 0; i < patrolEnemySpawnList.Count; i++)
		{
			ShipEnemyController shipEnemyController = patrolEnemySpawnList[i];
			if (shipEnemyController != null)
			{
				shipEnemyController.PatrolIconDestroy();
				shipEnemyController.RemovePatrolSubShip();
				global::UnityEngine.Object.Destroy(shipEnemyController.gameObject);
			}
		}
		patrolEnemySpawnList.Clear();
		for (int j = 0; j < bossList.Count; j++)
		{
			Boss boss = bossList[j];
			if (boss != null)
			{
				boss.IconDestroy();
				global::UnityEngine.Object.Destroy(boss.gameObject);
			}
		}
		bossList.Clear();
	}

	public void AddPatrolCoolTimeList(PatrolStatus data)
	{
		UI_WorldMapSameScale uI_WorldMapSameScale = UI_Manager.Instance.ui_worldMap.GeneratePatrolCoolTime();
		uI_WorldMapSameScale.SetPosition(data.startPosition);
		UI_PatrolCooltime component = uI_WorldMapSameScale.GetComponent<UI_PatrolCooltime>();
		if (!patrolDic.ContainsKey(data))
		{
			patrolDic.Add(data, component);
		}
	}

	private void MakeWallAngGate()
	{
		if (gates.Length != 0)
		{
			Gate[] array = gates;
			foreach (Gate gate in array)
			{
				if (gate != null)
				{
					global::UnityEngine.Object.Destroy(gate.gameObject);
				}
			}
		}
		if (wallObjList.Count > 0)
		{
			foreach (global::UnityEngine.GameObject wallObj in wallObjList)
			{
				if (wallObj != null)
				{
					global::UnityEngine.Object.Destroy(wallObj);
				}
			}
		}
		gates = new Gate[4];
		int worldSizeX = selectedWorldData.worldSizeX;
		int num = worldSizeX / 2;
		int worldSizeZ = selectedWorldData.worldSizeZ;
		int num2 = worldSizeZ / 2;
		global::UnityEngine.Quaternion[] array2 = new global::UnityEngine.Quaternion[4];
		global::UnityEngine.Vector3[] array3 = new global::UnityEngine.Vector3[4];
		int num3 = 0;
		array3[num3++] = new global::UnityEngine.Vector3(0f, 0f, num2);
		array3[num3++] = new global::UnityEngine.Vector3(0f, 0f, -num2);
		array3[num3++] = new global::UnityEngine.Vector3(-num, 0f, 0f);
		array3[num3++] = new global::UnityEngine.Vector3(num, 0f, 0f);
		num3 = 0;
		array2[num3++] = global::UnityEngine.Quaternion.Euler(0f, 0f, 0f);
		array2[num3++] = global::UnityEngine.Quaternion.Euler(0f, 180f, 0f);
		array2[num3++] = global::UnityEngine.Quaternion.Euler(0f, 270f, 0f);
		array2[num3++] = global::UnityEngine.Quaternion.Euler(0f, 90f, 0f);
		if (selectedWorldData.removeWall)
		{
			return;
		}
		for (int j = 0; j < array3.Length; j++)
		{
			global::UnityEngine.GameObject gameObject = global::UnityEngine.Object.Instantiate(wallSrc, array3[j], global::UnityEngine.Quaternion.identity);
			global::UnityEngine.GameObject gameObject2 = global::UnityEngine.Object.Instantiate(wallSrc, array3[j], global::UnityEngine.Quaternion.identity);
			Wall component = gameObject.GetComponent<Wall>();
			Wall component2 = gameObject2.GetComponent<Wall>();
			global::UnityEngine.Quaternion rotation;
			global::UnityEngine.Quaternion rotation2;
			if (j < 2)
			{
				rotation = array2[0];
				rotation2 = array2[1];
				component.SetSize(worldSizeX, selectedWorldData.haveGate);
				component2.SetSize(worldSizeX, selectedWorldData.haveGate);
			}
			else
			{
				rotation = array2[2];
				rotation2 = array2[3];
				component.SetSize(worldSizeZ, selectedWorldData.haveGate);
				component2.SetSize(worldSizeZ, selectedWorldData.haveGate);
			}
			gameObject.transform.rotation = rotation;
			gameObject2.transform.rotation = rotation2;
			wallObjList.Add(gameObject);
			wallObjList.Add(gameObject2);
			if (selectedWorldData.haveGate)
			{
				Gate component3 = global::UnityEngine.Object.Instantiate(gateSrc, array3[j], array2[j]).GetComponent<Gate>();
				component3.SetDirection(j);
				component3.gateStatus = selectedWorldData.gateList[j];
				component3.SetDoor(PlayerInfo.Instance.GetIsClearDoor(selectedWorldData.cubeNum, selectedWorldData.worldNum, j));
				gates[j] = component3;
			}
		}
	}

	public void DestinationIconLastSilblingCheck()
	{
		NextFrame();
	}

	private global::System.Collections.IEnumerator NextFrame()
	{
		yield return null;
		yield return null;
		yield return null;
		if (destinationIconObj != null)
		{
			destinationIconObj.transform.SetParent(destinationIconObj.transform.parent.parent);
			destinationIconObj.transform.SetAsLastSibling();
		}
	}

	public void RefreshDestination(Island island)
	{
		if (destinationIconObj != null)
		{
			global::UnityEngine.Object.Destroy(destinationIconObj);
		}
		if (!(island == null) && PlayerInfo.Instance.myData.destinationIslandId == island.status.islandId && worldmapIslandDic.ContainsKey(island.status.islandId))
		{
			global::UnityEngine.Transform trans = worldmapIslandDic[island.status.islandId].transform;
			destinationIconObj = UI_Manager.Instance.ui_worldMap.GenerateDestinationIcon(island.myDock.transform.position, trans);
			destinationIconObj.transform.SetParent(destinationIconObj.transform.parent.parent);
			destinationIconObj.transform.SetAsLastSibling();
		}
	}

	public Island GetNearIsland(global::UnityEngine.Vector3 pos)
	{
		Island result = null;
		float num = float.PositiveInfinity;
		for (int i = 0; i < islandList.Count; i++)
		{
			if (islandList[i].status.islandId >= 0)
			{
				float sqrMagnitude = (islandList[i].status.islandPosition - pos).sqrMagnitude;
				if (num > sqrMagnitude)
				{
					num = sqrMagnitude;
					result = islandList[i];
				}
			}
		}
		return result;
	}

	public Island GetIslandData(int level)
	{
		if (level <= -1)
		{
			return null;
		}
		if (islandDic.ContainsKey(level))
		{
			return islandDic[level];
		}
		return null;
	}

	public void AddObject(global::UnityEngine.GameObject obj)
	{
		global::UnityEngine.Vector3 position = obj.transform.position;
		GetIndex(position, out var indexX, out var indexZ);
		gridList[indexX][indexZ].AddObject(obj);
		obj.SetActive(value: false);
	}

	public void AllActive(bool active)
	{
		for (int i = 0; i < gridList.Count; i++)
		{
			for (int j = 0; j < gridList[i].Count; j++)
			{
				gridList[i][j].AllActive(active);
			}
		}
	}

	public void GetIndex(global::UnityEngine.Vector3 pos, out int indexX, out int indexZ)
	{
		indexX = global::UnityEngine.Mathf.Clamp((int)((pos.x - (float)minX) / intervalX), 0, 14);
		indexZ = global::UnityEngine.Mathf.Clamp((int)((pos.z - (float)minZ) / intervalZ), 0, 14);
	}

	public void Check(int indexX, int indexZ)
	{
		int num = global::UnityEngine.Mathf.Clamp(indexX - 1, 0, 14);
		int num2 = global::UnityEngine.Mathf.Clamp(indexX + 1, 0, 14);
		int num3 = global::UnityEngine.Mathf.Clamp(indexZ - 1, 0, 14);
		int num4 = global::UnityEngine.Mathf.Clamp(indexZ + 1, 0, 14);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				gridList[i][j].AllActive(active: true);
			}
		}
	}

	public void ObjSetActive(global::UnityEngine.Vector3 pos, bool active)
	{
		GetIndex(pos, out var indexX, out var indexZ);
		ObjSetActive(indexX, indexZ, active);
	}

	public void ObjSetActive(int indexX, int indexZ, bool active)
	{
		int num = global::UnityEngine.Mathf.Clamp(indexX - 1, 0, 14);
		int num2 = global::UnityEngine.Mathf.Clamp(indexX + 1, 0, 14);
		int num3 = global::UnityEngine.Mathf.Clamp(indexZ - 1, 0, 14);
		int num4 = global::UnityEngine.Mathf.Clamp(indexZ + 1, 0, 14);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				gridList[i][j].AllActive(active);
			}
		}
	}

	public void Check(int prevX, int prevZ, int indexX, int indexZ)
	{
		if (prevX != indexX)
		{
			int value = ((indexX <= prevX) ? (prevX + 1) : (prevX - 1));
			value = global::UnityEngine.Mathf.Clamp(value, 0, 14);
			int num = global::UnityEngine.Mathf.Clamp(indexZ - 1, 0, 14);
			int num2 = global::UnityEngine.Mathf.Clamp(indexZ + 1, 0, 14);
			for (int i = num; i <= num2; i++)
			{
				gridList[value][i].AllActive(active: false);
			}
		}
		if (prevZ != indexZ)
		{
			int value2 = ((indexZ <= prevZ) ? (prevZ + 1) : (prevZ - 1));
			value2 = global::UnityEngine.Mathf.Clamp(value2, 0, 14);
			int num3 = global::UnityEngine.Mathf.Clamp(indexX - 1, 0, 14);
			int num4 = global::UnityEngine.Mathf.Clamp(indexX + 1, 0, 14);
			for (int j = num3; j <= num4; j++)
			{
				gridList[j][value2].AllActive(active: false);
			}
		}
		Check(indexX, indexZ);
	}
}
