using System;
using System.Collections;
using System.Collections.Generic;
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

	List<HexCoordinates> baseCoords;
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
		TurnManager.Instance.OnTurnChanged += TurnTransition;
		TurnManager.Instance.OnTurnChanged += GameManager_OnTurnChanged;
	}

    // Start is called before the first frame update
    void Start()
	{
		HexGrid.Instance.localPlayer = player1;
		Load();
		AssignBase();
		AssignSpawnPoints();
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
		}
	}
	private void Update()
	{
		if (HexGrid.Instance.BlockActions) { return; }
		currentPlayer.TakeAction();
	}

	void TurnTransition(object sender, EventArgs e)
    {
		StartCoroutine(Wait1s());
    }

	IEnumerator Wait1s()
    {
		yield return new WaitForSeconds(1f);
		HexGrid.Instance.BlockActions = false;
		GameUIHandler.Instance.enableCanvasGroupRayCast();
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
			baseCoords = HexGrid.Instance.Load(reader, header);
            HexMapCamera.ValidatePosition();
        }
        else
        {
            Debug.LogWarning("Unknown map format " + header);
        }
    }

	void AssignBase()
    {
		if (baseCoords.Count != 4)
        {
			Debug.LogError("There aren't 4 bases");
        }
		int dist1 = baseCoords[0].DistanceTo(baseCoords[1]);
		int dist2 = baseCoords[0].DistanceTo(baseCoords[2]);
		if (dist2 > dist1)
        {
			AddBaseToPlayer(player1, baseCoords[0]);
			AddBaseToPlayer(player1, baseCoords[1]);
			AddBaseToPlayer(player2, baseCoords[2]);
			AddBaseToPlayer(player2, baseCoords[3]);
		}
        else
        {
			AddBaseToPlayer(player1, baseCoords[0]);
			AddBaseToPlayer(player1, baseCoords[2]);
			AddBaseToPlayer(player2, baseCoords[1]);
			AddBaseToPlayer(player2, baseCoords[3]);
		}
	}
	void AddBaseToPlayer(Player player, HexCoordinates targetCoord)
    {
		Base temp;
		if (player == player1)
        {
			temp = Instantiate(HexGrid.Instance.player1BasePrefab);
			HexGrid.Instance.AddFeature(temp, HexGrid.Instance.GetCell(targetCoord), 0);
        }
        else
        {
			temp = Instantiate(HexGrid.Instance.player2BasePrefab);
			HexGrid.Instance.AddFeature(temp, HexGrid.Instance.GetCell(targetCoord), 0);
		}
		player.myBases.Add(temp);
		temp.myPlayer = player;
	}
	void AddSpawnPoitnsToPlayer(Player player, SpawnPoint target)
	{
		player.myspawnPoints.Add(target);
		target.myPlayer = player;
	}
	void AssignSpawnPoints()
	{
		if (HexGrid.Instance.spawnPoints.Count != 8)
		{
			Debug.LogError("There isn't 4 bases");
		}
		Base player1Base = player1.myBases[0];
		Base player2Base = player2.myBases[0];
		foreach (var curSpawnpoint in HexGrid.Instance.spawnPoints)
        {
			int dist1 = player1Base.location.Coordinates.DistanceTo(curSpawnpoint.location.Coordinates);
			int dist2 = player2Base.location.Coordinates.DistanceTo(curSpawnpoint.location.Coordinates);
			if (dist2 > dist1)
            {
				AddSpawnPoitnsToPlayer(player1, curSpawnpoint);
            }
            else
            {
				AddSpawnPoitnsToPlayer(player2, curSpawnpoint);
			}
		}
	}
}
