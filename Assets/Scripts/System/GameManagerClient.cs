using System;
using System.Collections;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class GameManagerClient : NetworkBehaviour
{
    public static GameManagerClient Instance { get; private set; }
    [SerializeField]
    string currentMap = "Alpha1";

    const int mapFileVersion = 5;

    public bool deleteMode = false;
    public Player corresPlayer { get; private set; }

    public bool isMyTurn { get; private set; } = false;
    public void SetCorresPlayer(Player player)
    {
        corresPlayer = player;
    }

    ClientRpcParams clientRpcParams = new ClientRpcParams
    {
        Send = new ClientRpcSendParams
        {
            TargetClientIds = new ulong[] {}
        }
    };

    public Vector3 curAttackingPosition;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GameManagerClient!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += TurnTransition;
        if (!IsHost)
        {
            LoadFeatureServerRpc();
        }
    }

    private void Update()
    {
        if (isMyTurn)
        {
            corresPlayer.TakeAction();
        }
        else
        {
            //Debug.Log("Not my turn");
        }
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    Shader.EnableKeyword("_HEX_MAP_EDIT_MODE");
    //    TurnManager.Instance.OnTurnChanged += TurnTransition;
    //    TurnManager.Instance.OnTurnChanged += GameManager_OnTurnChanged;
    //}
    public void StartGameAllClients()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        StartCoroutine(StartGame());
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void PlayerInitializedServerRpc()
    //{
    //    GameManagerServer.Instance.LoadFeature();
    //}
    IEnumerator StartGame()
    {
        while (corresPlayer == null)
        {
            Debug.Log("Waiting");
            yield return new WaitForSeconds(0.01f);
        }
        corresPlayer.InitiazeCardDisplay();
        LoadMap();
        GameUIHandler.Instance.disableCanvasGroupRayCast();
    }
    public void ClientStartTurn(ulong clientId)
    {
        clientRpcParams.Send.TargetClientIds = new ulong[] { clientId };
        StartTurnClientRpc(clientRpcParams);
    }

    void TurnTransition(object sender, EventArgs e)
    {
        isMyTurn = !isMyTurn;
        GameUIHandler.Instance.disableCanvasGroupRayCast();
        //StartCoroutine(Wait1s());
        if (IsHost)
        {
            ClientStartTurn(NetworkManager.Singleton.ConnectedClients[1].ClientId);
        }
        else
        {
            ClientStartTurn(NetworkManager.Singleton.ConnectedClients[0].ClientId);
        }
    }

    IEnumerator Wait1s()
    {
        yield return new WaitForSeconds(1f);
        HexGrid.Instance.BlockActions = false;
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
        HexGrid.Instance.LoadFeature(reader, header);
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
    public void UnsetDeleteMode()
    {
        var cursorHotspot = new Vector2(GameAssets.Instance.mainCursor.width / 2, GameAssets.Instance.mainCursor.height / 2);
        deleteMode = false;
        Cursor.SetCursor(GameAssets.Instance.mainCursor, cursorHotspot, CursorMode.Auto);
    }

    public void AddToFeatureToClient(Feature feature)
    {
        clientRpcParams.Send.TargetClientIds = new ulong[] { NetworkManager.Singleton.ConnectedClients[1].ClientId};
        ulong objId = feature.GetComponent<NetworkObject>().NetworkObjectId;
        if (feature is HexUnit)
        {
            AddUnitClientRpc(objId, clientRpcParams);
            GameManagerServer.Instance.player2.myUnits.Add(feature as HexUnit);
        }
        else if (feature is Base)
        {
            AddBaseClientRpc(objId, clientRpcParams);
            GameManagerServer.Instance.player2.myBases.Add(feature as Base);
        }
        else if (feature is SpawnPoint)
        {
            AddSpawnPointClientRpc(objId, clientRpcParams);
            GameManagerServer.Instance.player2.myspawnPoints.Add(feature as SpawnPoint);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void LoadFeatureServerRpc()
    {
        Debug.Log("Loading Feature");
        LoadFeature();
        isMyTurn = true;
    }

    [ClientRpc]
    public void StartTurnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Start turn");
        isMyTurn = true;
        GameUIHandler.Instance.enableCanvasGroupRayCast();
    }

    [ClientRpc]
    public void AddBaseClientRpc(ulong objId = 0, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Add base");
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<Base>(out var myBase))
        {
            Debug.LogError("Base is null");
        }
        corresPlayer.myBases.Add(myBase);
    }

    [ClientRpc]
    public void AddSpawnPointClientRpc(ulong objId = 0, ClientRpcParams clientRpcParams = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<SpawnPoint>(out var mySpawnPoint))
        {
            Debug.LogError("SpawnPoint is null");
        }
        corresPlayer.myspawnPoints.Add(mySpawnPoint);
    }

    [ClientRpc]
    public void AddUnitClientRpc(ulong objId = 0, ClientRpcParams clientRpcParams = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<HexUnit>(out var myUnit))
        {
            Debug.LogError("Unit is null");
        }
        corresPlayer.myUnits.Add(myUnit);
    }
}
