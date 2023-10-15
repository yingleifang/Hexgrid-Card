using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class SinglePlayer : MonoBehaviour
{
    [SerializeField]
    string currentMap = "Alpha1";
    const int mapFileVersion = 5;
    public static SinglePlayer Instance { get; private set; }
    [SerializeField]
    Player enemyAI;
    [SerializeField]
    GameManagerServer gameManagerServerPrefab;
    [SerializeField]
    GameManagerClient gameManagerClientPrefab;

    public Player player1;

    public Player player2;

    public Player currentPlayer;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        LoadMap();
        LoadFeature();
    }

    public void LoadFeature()
    {
        string path = Path.Combine(Application.persistentDataPath, currentMap + HexGrid.Instance.featureSuffix);
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        using BinaryReader reader = new(File.OpenRead(path));
        int header = reader.ReadInt32();
        HexGrid.Instance.LoadFeatureLocal(reader, header);
        HexMapCamera.ValidatePosition();
    }
    public void LoadMap()
    {
        string path = Path.Combine(Application.persistentDataPath, currentMap + HexGrid.Instance.mapSuffix);
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        using BinaryReader reader = new BinaryReader(File.OpenRead(path));
        int header = reader.ReadInt32();
        if (header <= mapFileVersion)
        {
            HexGrid.Instance.LoadMap(reader, header);
            HexMapCamera.ValidatePosition();
        }
        else
        {
            Debug.LogWarning("Unknown map format " + header);
        }
    }


}
