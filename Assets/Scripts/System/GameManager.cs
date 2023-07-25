using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[SerializeField]
	string currentMap = "Alpha1";

	const int mapFileVersion = 5;

	//public event EventHandler OnSelectedUnitChanged;

	[SerializeField]
	public Player player1;

	public Player player2;

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
		currentPlayer = player1;
		TurnManager.Instance.OnTurnChanged += GameManager_OnTurnChanged;
	}

    // Start is called before the first frame update
    void Start()
	{
		Load();
	}
	private void GameManager_OnTurnChanged(object sender, EventArgs e)
	{
		if (currentPlayer == player1)
        {
			currentPlayer = player2;
        }
        else
        {
			currentPlayer = player1;
			Debug.Log(currentPlayer);
		}
	}
	private void Update()
	{
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
