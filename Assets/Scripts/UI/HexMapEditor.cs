using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Component that applies UI commands to the hex map.
/// Public methods are hooked up to the in-game UI.
/// </summary>
public class HexMapEditor : MonoBehaviour
{
	static int cellHighlightingId = Shader.PropertyToID("_CellHighlighting");

	//[SerializeField]
	//Material terrainMaterial;

	int activeElevation;
	int activeWaterLevel;

	int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

	int activeTerrainTypeIndex;

	int brushSize;

	bool applyElevation = true;
	bool applyWaterLevel = true;

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;

	public int activeFaction = 1;

	public Button factionButton;

	enum OptionalToggle
	{
		Ignore, Yes, No
	}

	OptionalToggle riverMode, roadMode, walledMode;

	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;

	public void SetTerrainTypeIndex (int index) => activeTerrainTypeIndex = index;

	public void SetApplyElevation (bool toggle) => applyElevation = toggle;

	public void SetElevation (float elevation) => activeElevation = (int)elevation;

	public void SetApplyWaterLevel (bool toggle) => applyWaterLevel = toggle;

	public void SetWaterLevel (float level) => activeWaterLevel = (int)level;

	public void SetApplyUrbanLevel (bool toggle) => applyUrbanLevel = toggle;

	public void SetUrbanLevel (float level) => activeUrbanLevel = (int)level;

	public void SetApplyFarmLevel (bool toggle) => applyFarmLevel = toggle;

	public void SetFarmLevel (float level) => activeFarmLevel = (int)level;

	public void SetApplyPlantLevel (bool toggle) => applyPlantLevel = toggle;

	public void SetPlantLevel (float level) => activePlantLevel = (int)level;

	public void SetApplySpecialIndex (bool toggle) => applySpecialIndex = toggle;

	public void SetSpecialIndex (float index) => activeSpecialIndex = (int)index;

	public void SetBrushSize (float size) => brushSize = (int)size;

	public void SetRiverMode (int mode) => riverMode = (OptionalToggle)mode;

	public void SetRoadMode (int mode) => roadMode = (OptionalToggle)mode;

	public void SetWalledMode (int mode) => walledMode = (OptionalToggle)mode;

	public void SetEditMode (bool toggle) => enabled = toggle;

	//public void ShowGrid (bool visible)
	//{
	//	if (visible)
	//	{
	//		terrainMaterial.EnableKeyword("_SHOW_GRID");
	//	}
	//	else
	//	{
	//		terrainMaterial.DisableKeyword("_SHOW_GRID");
	//	}
	//}

	//void Awake ()
	//{
 //       terrainMaterial.DisableKeyword("_SHOW_GRID");
 //       Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
 //       SetEditMode(true);
 //   }

	void Update ()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Input.GetMouseButton(0))
			{
				HandleInput();
				return;
			}
			else
			{
				// Potential optimization: only do this if camera or cursor has changed.
				UpdateCellHighlightData(GetCellUnderCursor());
			}
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				activeFaction = 0;
				factionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Faction: " + activeFaction;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1)){
                activeFaction = 1;
                factionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Faction: " + activeFaction;
			}else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
                activeFaction = 2;
                factionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Faction: " + activeFaction;
			}
            if (Input.GetKeyDown(KeyCode.M))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					DestroyUnit();
				}
				else
				{
					if (activeFaction == 1)
                        CreateUnitFeature(HexGrid.Instance.player1BasePrefab);
                    else if (activeFaction == 2)
						CreateUnitFeature(HexGrid.Instance.player2BasePrefab);
				}
				return;
			}
            if (Input.GetKeyDown(KeyCode.I))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					DestroyUnit();
				}
				else
				{
					if (activeFaction == 1)
                        CreateTerrainFeature(HexGrid.Instance.player1SpawnPointPrefab);
                    else if (activeFaction == 2)
                        CreateTerrainFeature(HexGrid.Instance.player2SpawnPointPrefab);
				}
				return;
			}
		}
		else
		{
			ClearCellHighlightData();
		}
		previousCell = null;
	}

	HexCell GetCellUnderCursor () =>
		HexGrid.Instance.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

	void CreateUnitFeature(UnitFeature unitFeature)
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.unitFeature)
		{
			Feature feature = Instantiate(unitFeature);
			HexGrid.Instance.AddFeatureBeforeGame(
                feature, cell, Random.Range(0f, 360f)
			);
		}
	}
	void CreateTerrainFeature(TerrainFeature terrainFeature)
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.terrainFeature)
		{
			HexGrid.Instance.AddFeatureBeforeGame(
				Instantiate(terrainFeature), cell, Random.Range(0f, 360f)
			);
		}
	}

	void DestroyUnit ()
	{
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.unitFeature)
		{
			HexGrid.Instance.RemoveUnit((HexUnit)cell.unitFeature);
		}
	}

	void HandleInput ()
	{
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell)
		{
			if (previousCell && previousCell != currentCell)
			{
				ValidateDrag(currentCell);
			}
			else
			{
				isDrag = false;
			}
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else
		{
			previousCell = null;
		}
		UpdateCellHighlightData(currentCell);
	}

	void UpdateCellHighlightData (HexCell cell)
	{
		if (cell == null)
		{
			ClearCellHighlightData();
			return;
		}

		// Works up to brush size 6.
		Shader.SetGlobalVector(
			cellHighlightingId,
			new Vector4(
				cell.Coordinates.HexX,
				cell.Coordinates.HexZ,
				brushSize * brushSize + 0.5f
			)
		);
	}

	void ClearCellHighlightData () =>
		Shader.SetGlobalVector(cellHighlightingId, new Vector4(0f, 0f, -1f, 0f));

	void ValidateDrag (HexCell currentCell)
	{
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		)
		{
			if (previousCell.GetNeighbor(dragDirection) == currentCell)
			{
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	void EditCells (HexCell center)
	{
		int centerX = center.Coordinates.X;
		int centerZ = center.Coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
		{
			for (int x = centerX - r; x <= centerX + brushSize; x++)
			{
				EditCell(HexGrid.Instance.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
		{
			for (int x = centerX - brushSize; x <= centerX + r; x++)
			{
				EditCell(HexGrid.Instance.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	void EditCell (HexCell cell)
	{
		if (cell)
		{
			if (activeTerrainTypeIndex >= 0)
			{
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
			if (applyElevation)
			{
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel)
			{
				cell.WaterLevel = activeWaterLevel;
			}
			if (applySpecialIndex)
			{
				cell.SpecialIndex = activeSpecialIndex;
			}
			if (applyUrbanLevel)
			{
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (applyFarmLevel)
			{
				cell.FarmLevel = activeFarmLevel;
			}
			if (applyPlantLevel)
			{
				cell.PlantLevel = activePlantLevel;
			}
			if (riverMode == OptionalToggle.No)
			{
				cell.RemoveRiver();
			}
			if (roadMode == OptionalToggle.No)
			{
				cell.RemoveRoads();
			}
			if (walledMode != OptionalToggle.Ignore)
			{
				cell.Walled = walledMode == OptionalToggle.Yes;
			}
			if (
				isDrag &&
				cell.TryGetNeighbor(dragDirection.Opposite(), out HexCell otherCell)
			)
			{
				if (riverMode == OptionalToggle.Yes)
				{
					otherCell.SetOutgoingRiver(dragDirection);
				}
				if (roadMode == OptionalToggle.Yes)
				{
					otherCell.AddRoad(dragDirection);
				}
			}
		}
	}
}
