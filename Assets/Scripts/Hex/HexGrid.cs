using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
using Unity.Netcode;

/// <summary>
/// Component that represents an entire hexagon map.
/// </summary>
public class HexGrid : MonoBehaviour
{
    [SerializeField]
    Material terrainMaterial;

    public static HexGrid Instance { get; private set; }

	public List<HexUnit> units = new List<HexUnit>();
	public List<Base> Bases = new List<Base>();
	public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
	
	public bool BlockActions = false;

    public string mapSuffix = ".map";
    public string featureSuffix = ".ftr";

    [SerializeField]
	public Base player1BasePrefab;
	[SerializeField]
	public Base player2BasePrefab;

	[SerializeField]
	public SpawnPoint player1SpawnPointPrefab;
	[SerializeField]
	public SpawnPoint player2SpawnPointPrefab;

	[SerializeField]
	HexCell cellPrefab;

	[SerializeField]
	Text cellLabelPrefab;

    [SerializeField]
	HexGridChunk chunkPrefab;

	[SerializeField]
	Texture2D noiseSource;

	[SerializeField]
	int seed;

	Transform[] columns;

	/// <summary>
	/// Amount of cells in the X dimension.
	/// </summary>
	public int CellCountX
	{ get; private set; }

	/// <summary>
	/// Amount of cells in the Z dimension.
	/// </summary>
	public int CellCountZ
	{ get; private set; }

	public HexCell currentPathFrom, currentPathTo;

	public List<HexCell> curPath;

	/// <summary>
	/// Whether there currently exists a path that should be displayed.
	/// </summary>
	public bool HasPath => CurrentPathExists;

	HexGridChunk[] chunks;
	HexCell[] cells;

	int chunkCountX, chunkCountZ;

	HexCellPriorityQueue searchFrontier;

	int searchFrontierPhase;

	public bool CurrentPathExists { get; private set; }

	HexCellShaderData cellShaderData;

	Dictionary<Color, List<HexCell>> coloredCells = new()
	{
		{ Color.white, new() },
		{ Color.red, new() },
		{ Color.blue, new() },

	};
	void Awake ()
	{
        terrainMaterial.DisableKeyword("_SHOW_GRID");
        Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CellCountX = 20;
		CellCountZ = 15;
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		cellShaderData = gameObject.AddComponent<HexCellShaderData>();
		CreateMap(CellCountX, CellCountZ);
	}
	public void AddFeature (Feature feature, HexCell location, float orientation)
	{
		if (feature is HexUnit unitFeature)
        {
			units.Add(unitFeature);
		}else if (feature is Base baseFeature)
        {
			Bases.Add(baseFeature);
        }
        else if (feature is SpawnPoint spawnPointFeature)
        {
			spawnPoints.Add(spawnPointFeature);
        }
		feature.Location = location;
        feature.Orientation = orientation;
        if (GameManagerClient.Instance && NetworkManager.Singleton.IsHost)
        {
            SyncSpawningToClient(feature, location);
        }
    }

    private static void SyncSpawningToClient(Feature feature, HexCell location)
    {
        NetworkObject featureNetworkObj = feature.GetComponent<NetworkObject>();
        featureNetworkObj.Spawn();
        ulong objId = feature.GetComponent<NetworkObject>().NetworkObjectId;
        Debug.Log(objId);
        GameManagerClient.Instance.AddFeatureToMapClientRpc(location.Coordinates.X, location.Coordinates.Z, objId, GameManagerClient.Instance.clientRpcParams);
    }

    public HexUnit AddUnit(HexCell location, float orientation, HexUnit unitPrefab)
    {
		HexUnit unit = Instantiate(unitPrefab);
		unit.Location = location;
		unit.Orientation = orientation;
		location.unitFeature = unit;
		units.Add(unit);
		GameManagerServer.Instance.currentPlayer.myUnits.Add(unit);
		unit.myPlayer = GameManagerServer.Instance.currentPlayer;
		return unit;
	}

	/// <summary>
	/// Remove a unit from the map.
	/// </summary>
	/// <param name="unit">The unit to remove.</param>
	public void RemoveUnit (HexUnit unit)
	{
		units.Remove(unit);
		unit.Die();
	}

	/// <summary>
	/// Make a game object a child of a map column.
	/// </summary>
	/// <param name="child"><see cref="Transform"/> of the child game object.</param>
	/// <param name="columnIndex">Index of the parent column.</param>
	public void MakeChildOfColumn(Transform child, int columnIndex)
    {
		child.SetParent(columns[columnIndex], false);
	}

	/// <summary>
	/// Create a new map.
	/// </summary>
	/// <param name="x">X size of the map.</param>
	/// <param name="z">Z size of the map.</param>
	/// <param name="wrapping">Whether the map wraps east-west.</param>
	/// <returns>Whether the map was successfully created. It fails if the X or Z size
	/// is not a multiple of the respective chunk size.</returns>
	public bool CreateMap (int x, int z)
	{
		if (
			x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
			z <= 0 || z % HexMetrics.chunkSizeZ != 0
		)
		{
			Debug.LogError("Unsupported map size.");
			return false;
		}

		//ClearPath();
		ClearUnits();
		if (columns != null)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				Destroy(columns[i].gameObject);
			}
		}

		CellCountX = x;
		CellCountZ = z;
		chunkCountX = CellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = CellCountZ / HexMetrics.chunkSizeZ;
		cellShaderData.Initialize(CellCountX, CellCountZ);
		CreateChunks();
		CreateCells();
		return true;
	}

	void CreateChunks()
	{
		columns = new Transform[chunkCountX];
		for (int x = 0; x < chunkCountX; x++)
		{
			columns[x] = new GameObject("Column").transform;
			columns[x].SetParent(transform, false);
		}
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];
		for (int z = 0, i = 0; z < chunkCountZ; z++)
		{
			for (int x = 0; x < chunkCountX; x++)
			{
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(columns[x], false);
			}
		}
	}

	void CreateCells ()
	{
		cells = new HexCell[CellCountZ * CellCountX];

		for (int z = 0, i = 0; z < CellCountZ; z++)
		{
			for (int x = 0; x < CellCountX; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	void ClearUnits ()
	{
		for (int i = 0; i < units.Count; i++)
		{
			units[i].Die();
		}
		units.Clear();
	}

	void OnEnable ()
	{
		if (!HexMetrics.noiseSource)
		{
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
		}
	}

	/// <summary>
	/// Get a cell given a <see cref="Ray"/>.
	/// </summary>
	/// <param name="ray"><see cref="Ray"/> used to perform a raycast.</param>
	/// <returns>The hit cell, if any.</returns>
	public HexCell GetCell (Ray ray)
	{
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			return GetCell(hit.point);
		}
		return null;
	}

	/// <summary>
	/// Get the cell that contains a position.
	/// </summary>
	/// <param name="position">Position to check.</param>
	/// <returns>The cell containing the position, if it exists.</returns>
	public HexCell GetCell (Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		return GetCell(coordinates);
	}

	/// <summary>
	/// Get the cell with specific <see cref="HexCoordinates"/>.
	/// </summary>
	/// <param name="coordinates"><see cref="HexCoordinates"/> of the cell.</param>
	/// <returns>The cell with the given coordinates, if it exists.</returns>
	public HexCell GetCell (HexCoordinates coordinates)
	{
		int z = coordinates.Z;
		int x = coordinates.X + z / 2;
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			return null;
		}
		return cells[x + z * CellCountX];
	}

	/// <summary>
	/// Try to get the cell with specific <see cref="HexCoordinates"/>.
	/// </summary>
	/// <param name="coordinates"><see cref="HexCoordinates"/> of the cell.</param>
	/// <param name="cell">The cell, if it exists.</param>
	/// <returns>Whether the cell exists.</returns>
	public bool TryGetCell (HexCoordinates coordinates, out HexCell cell)
	{
		int z = coordinates.Z;
		int x = coordinates.X + z / 2;
		if (z < 0 || z >= CellCountZ || x < 0 || x >= CellCountX)
		{
			cell = null;
			return false;
		}
		cell = cells[x + z * CellCountX];
		return true;
	}

	/// <summary>
	/// Get the cell with specific offset coordinates.
	/// </summary>
	/// <param name="xOffset">X array offset coordinate.</param>
	/// <param name="zOffset">Z array offset coordinate.</param>
	/// <returns></returns>
	public HexCell GetCell (int xOffset, int zOffset) =>
		cells[xOffset + zOffset * CellCountX];

	/// <summary>
	/// Get the cell with a specific index.
	/// </summary>
	/// <param name="cellIndex">Cell index, which should be valid.</param>
	/// <returns>The indicated cell.</returns>
	public HexCell GetCell (int cellIndex) => cells[cellIndex];

	/// <summary>
	/// Control whether the map UI should be visible or hidden.
	/// </summary>
	/// <param name="visible">Whether the UI should be visibile.</param>
	public void ShowUI (bool visible)
	{
		for (int i = 0; i < chunks.Length; i++)
		{
			chunks[i].ShowUI(visible);
		}
	}

	void CreateCell (int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.Grid = this;
		cell.transform.localPosition = position;
		cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Index = i;
		cell.ColumnIndex = x / HexMetrics.chunkSizeX;
		cell.ShaderData = cellShaderData;

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		cell.UIRect = label.rectTransform;

		cell.Elevation = 0;

		AddCellToChunk(x, z, cell);
	}

	void AddCellToChunk (int x, int z, HexCell cell)
	{
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	/// <summary>
	/// Save the map.
	/// </summary>
	/// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
	public void SaveMap(BinaryWriter writer)
	{
		writer.Write(CellCountX);
		writer.Write(CellCountZ);

		for (int i = 0; i < cells.Length; i++)
		{
			cells[i].Save(writer);
		}
	}

    /// <summary>
    /// Load the map.
    /// </summary>
    /// <param name="reader"><see cref="BinaryReader"/> to use.</param>
    /// <param name="header">Header version.</param>
    public void LoadMap (BinaryReader reader, int header)
	{
		ClearUnits();
		int x = 20, z = 15;
		if (header >= 1)
		{
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		if (x != CellCountX || z != CellCountZ)
		{
			if (!CreateMap(x, z))
			{
				Debug.LogError("Cannot load map");
			}
		}
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader, header);
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }
    }
    public void SaveFeature(BinaryWriter writer)
    {
        writer.Write(Bases.Count);
        for (int i = 0; i < Bases.Count; i++)
        {
            writer.Write(Bases[i].playerID);
            Bases[i].Save(writer);
        }
        writer.Write(spawnPoints.Count);
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            writer.Write(spawnPoints[i].playerID);
            spawnPoints[i].Save(writer);
        }
    }
    public void LoadFeature(BinaryReader reader, int header)
	{
		if (header >= 2)
		{
			try
			{
                int baseCount = reader.ReadInt32();
				for (int i = 0; i < baseCount; i++)
				{
					int basePlayer = reader.ReadInt32();
					if (basePlayer == GameManagerClient.Instance.corresPlayer.playerID)
					{
                        GameManagerServer.Instance.player1.myBases.Add(Feature.Load(reader, this, player1BasePrefab) as Base);
					}
					else
					{
						Feature spawnedFeature = Feature.Load(reader, this, player2BasePrefab);
                        GameManagerServer.Instance.player2.myBases.Add(spawnedFeature as Base);
                        GameManagerClient.Instance.AddFeatureToClientPlayer(spawnedFeature);
					}
                }
                int spawnPointCount = reader.ReadInt32();
				for (int i = 0; i < spawnPointCount; i++)
				{
					int spawnPointPlayer = reader.ReadInt32();
                    if (spawnPointPlayer == GameManagerClient.Instance.corresPlayer.playerID)
                    {
                        GameManagerServer.Instance.player1.myspawnPoints.Add(Feature.Load(reader, this, player1SpawnPointPrefab) as SpawnPoint);
                    }
                    else
					{
                        Feature spawnedFeature = Feature.Load(reader, this, player2SpawnPointPrefab);
                        GameManagerServer.Instance.player2.myspawnPoints.Add(spawnedFeature as SpawnPoint);
                        GameManagerClient.Instance.AddFeatureToClientPlayer(spawnedFeature);
                    }
                }
			}
            catch (Exception e) { Debug.LogException(e); }
        }
    }

    public void LoadFeatureLocal(BinaryReader reader, int header)
    {
        if (header >= 2)
        {
            try
            {
                Feature spawnedFeature = null;
                int baseCount = reader.ReadInt32();
                for (int i = 0; i < baseCount; i++)
                {
                    int basePlayer = reader.ReadInt32();
                    Debug.Log(basePlayer);
                    Base basePrefab = (basePlayer == 1) ? player1BasePrefab : player2BasePrefab;
                    spawnedFeature = Feature.Load(reader, this, basePrefab);
					if (basePlayer == SinglePlayer.Instance.player1.playerID)
					{
                        SinglePlayer.Instance.player1.myBases.Add(spawnedFeature as Base);
					}
					else
					{
                        SinglePlayer.Instance.player2.myBases.Add(spawnedFeature as Base);
                    }
                }
                int spawnPointCount = reader.ReadInt32();
                for (int i = 0; i < spawnPointCount; i++)
                {
                    int spawnPointPlayer = reader.ReadInt32();
                    Debug.Log(spawnPointPlayer);
                    SpawnPoint spawnPointPrefab = (spawnPointPlayer == 1) ? player1SpawnPointPrefab : player2SpawnPointPrefab;
                    spawnedFeature = Feature.Load(reader, this, spawnPointPrefab);
                    if (spawnPointPlayer == SinglePlayer.Instance.player1.playerID)
                    {
                        SinglePlayer.Instance.player1.myspawnPoints.Add(spawnedFeature as SpawnPoint);
					}
					else
					{
                        SinglePlayer.Instance.player2.myspawnPoints.Add(spawnedFeature as SpawnPoint);
                    }
                }
            }
            catch (Exception e) { Debug.LogException(e); }
        }
    }

    /// <summary>
    /// Get a list of cells representing the currently visible path.
    /// </summary>
    /// <returns>The current path list, if a visible path exists.</returns>
    public bool GetPath (int pathLimit = 100)
	{
		if (!CurrentPathExists)
		{
			return false;
		}
		curPath = ListPool<HexCell>.Get();
		int loopTimes = 0;
		for (HexCell c = currentPathTo; c != currentPathFrom && loopTimes < 200; c = c.PathFrom)
		{
			curPath.Add(c);
			loopTimes += 1;
		}
		if (loopTimes > 190)
        {
			Debug.LogError("GetPath didn't find path");
        }
		curPath.Add(currentPathFrom);
		if (curPath.Count > pathLimit)
        {
			curPath = curPath.GetRange(curPath.Count - pathLimit - 1, pathLimit + 1);
		}
		curPath.Reverse();
		if (curPath[^1].unitFeature != null)
		{
			curPath.RemoveAt(curPath.Count - 1);
		}
		return true;
	}

	void ShowPath()
	{
		if (CurrentPathExists)
		{
			HexCell current = currentPathTo;
			while (current != currentPathFrom)
			{
				//int turn = (current.Distance - 1) / speed;
				//current.SetLabel(turn.ToString());
				ColorCells(Color.blue, current);
				current = current.PathFrom;
			}
		}
		ColorCells(Color.blue, currentPathFrom);
		ColorCells(Color.blue, currentPathTo);
	}

	/// <summary>
	/// Try to find a path.
	/// </summary>
	/// <param name="fromCell">Cell to start the search from.</param>
	/// <param name="toCell">Cell to find a path towards.</param>
	/// <param name="unit">Unit for which the path is.</param>
	public void FindPath (HexCell fromCell, HexCell toCell, HexUnit unit, int movementRange)
	{
		ClearCellColor(Color.blue); ;
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		CurrentPathExists = Search(fromCell, toCell, unit, movementRange);
		if (GameManagerServer.Instance.currentPlayer == GameManagerClient.Instance.corresPlayer)
		{
			ShowPath();
		}
	}

	bool Search (HexCell fromCell, HexCell toCell, HexUnit unit, int movementRange)
	{
		searchFrontierPhase += 2;
		if (searchFrontier == null)
		{
			searchFrontier = new HexCellPriorityQueue();
		}
		else
		{
			searchFrontier.Clear();
		}

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0)
		{
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			if (current == toCell)
			{
				return true;
			}

			int currentTurn = (current.Distance - 1) / movementRange;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (
					!current.TryGetNeighbor(d, out HexCell neighbor) ||
					neighbor.SearchPhase > searchFrontierPhase
				)
				{
					continue;
				}
				if (!unit.IsValidDestination(neighbor))
				{
					continue;
				}
				int moveCost = unit.GetMoveCost(current, neighbor, d);
				if (moveCost < 0)
				{
					continue;
				}

				int distance = current.Distance + moveCost;
				int turn = (distance - 1) / movementRange;
				if (turn > currentTurn)
				{
					distance = turn * movementRange + moveCost;
				}

				if (neighbor.SearchPhase < searchFrontierPhase && turn <= currentTurn)
				{
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.Coordinates.DistanceTo(toCell.Coordinates);
					searchFrontier.Enqueue(neighbor);
				}
				//else if (distance < neighbor.Distance)
				//{
				//	int oldPriority = neighbor.SearchPriority;
				//	neighbor.Distance = distance;
				//	neighbor.PathFrom = current;
				//	searchFrontier.Change(neighbor, oldPriority);
				//}
			}
		}
		return false;
	}
	public void ClearCellColor(Color color)
	{
		foreach (HexCell current in coloredCells[color])
		{

			current.DisableHighlight();
			foreach (var item in coloredCells)
			{
				if (item.Key != color && item.Value.Contains(current))
				{
					current.EnableHighlight(item.Key);
					break;
				}
			}
		}
		coloredCells[color] = new();
	}
	public void showMoveRange(HexCell fromCell, HexUnit unit)
	{
		ClearCellColor(Color.white);
		int speed = unit.MovementRange;
		searchFrontierPhase += 2;
		if (searchFrontier == null)
		{
			searchFrontier = new HexCellPriorityQueue();
		}
		else
		{
			searchFrontier.Clear();
		}

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0)
		{
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			int currentTurn = (current.Distance - 1) / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				if (
					!current.TryGetNeighbor(d, out HexCell neighbor) ||
					neighbor.SearchPhase > searchFrontierPhase
				)
				{
					continue;
				}
				if (!unit.IsValidDestination(neighbor))
				{
					continue;
				}
				int moveCost = unit.GetMoveCost(current, neighbor, d);
				if (moveCost < 0)
				{
					continue;
				}

				int distance = current.Distance + moveCost;
				int turn = (distance - 1) / speed;

				if (neighbor.SearchPhase < searchFrontierPhase && turn <= currentTurn)
				{
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Enqueue(neighbor);
				}
				//else if (distance < neighbor.Distance)
				//{
				//	int oldPriority = neighbor.SearchPriority;
				//	neighbor.Distance = distance;
				//	neighbor.PathFrom = current;
				//	searchFrontier.Change(neighbor, oldPriority);
				//}
				ColorCells(Color.white, current);
			}
		}
	}
	public void ColorCells(Color color, HexCell current)
	{
		coloredCells[color].Add(current);
		current.EnableHighlight(color);
	}
}
