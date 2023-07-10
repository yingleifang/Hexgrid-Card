using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[SerializeField]
	string currentMap = "Alpha1";

	HexCell currentCell;

	public Feature selectedFeature;

	const int mapFileVersion = 5;

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
	}

	private void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (Input.GetMouseButtonDown(0))
			{
				DoSelection();
			}
		}
	}
	void DoSelection()
	{
		HexGrid.Instance.ClearPath();
		UpdateCurrentCell();
		if (currentCell)
		{
			selectedFeature = currentCell.Feature;
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
