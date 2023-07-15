using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[SerializeField]
	string currentMap = "Alpha1";

	HexCell currentCell;

	const int mapFileVersion = 5;

	public event EventHandler OnSelectedUnitChanged;

	[SerializeField]
	Player player1;

	public Player currentPlayer;

	private void Awake()
    {
		if (Instance != null)
		{
			Debug.LogError("There's more than one TurnSystem!");
			Destroy(gameObject);
			return;
		}
		Instance = this;
		Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
	}

    // Start is called before the first frame update
    void Start()
	{
		Load();
		currentPlayer = player1;
	}

	private void Update()
	{
		if (HexGrid.Instance.unitIsBusy)
		{
			return;
		}
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Input.GetMouseButtonDown(0))
			{
				DoSelection();
			}
			else if (player1.selectedFeature is HexUnit temp)
			{
				if (Input.GetMouseButtonDown(1))
				{
					temp.GetMoveAction().DoMove();
				}
				else
				{
					UnitActionSystem.Instance.DoPathfinding((HexUnit)player1.selectedFeature);
				}
			}
		}
	}
	void DoSelection()
	{
		HexGrid.Instance.ClearCellColor(Color.blue);
		HexGrid.Instance.ClearCellColor(Color.white);
		UpdateCurrentCell();
		if (currentCell)
		{
			if (player1.selectedFeature)
            {
				player1.selectedFeature.RaiseFeatureDeSelectedEvent();
			}
			player1.selectedFeature = currentCell.Feature;
			if (player1.selectedFeature)
			{
				player1.selectedFeature.RaiseFeatureSelectedEvent();
			}
		}
		if (player1.selectedFeature is HexUnit temp)
		{
			HexGrid.Instance.showMoveRange(temp.Location, temp);
		}
	}
	bool UpdateCurrentCell()
	{
		HexCell cell =
			HexGrid.Instance.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell)
		{
			currentCell = cell;
			return true;
		}
		return false;
	}

	void Load()
	{
		string path = Path.Combine(Application.persistentDataPath, currentMap + ".map");
		if (!File.Exists(path))
		{
			Debug.LogError("File does not exist " + path);
			return;
		}
        using BinaryReader reader = new BinaryReader(File.OpenRead(path));
        int header = reader.ReadInt32();
        if (header <= mapFileVersion)
        {
            HexGrid.Instance.Load(reader, header);
            HexMapCamera.ValidatePosition();
        }
        else
        {
            Debug.LogWarning("Unknown map format " + header);
        }
    }
}
