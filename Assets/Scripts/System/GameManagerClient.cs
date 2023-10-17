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

    public ClientRpcParams clientRpcParams;

    public ClientRpcParams hostRpcParams;

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
    public void StartGameAllClients()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        while (corresPlayer == null)
        {
            Debug.Log("Waiting");
            yield return new WaitForSeconds(0.01f);
        }
        if (IsHost)
        {
            clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { NetworkManager.Singleton.ConnectedClients[1].ClientId }
                }
            };
            hostRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { NetworkManager.Singleton.ConnectedClients[0].ClientId }
                }
            };
        }
        corresPlayer.InitiazeCardDisplay();
        LoadMap();
        GameUIHandler.Instance.disableCanvasGroupRayCast();
    }

    void TurnTransition(object sender, EventArgs e)
    {
        isMyTurn = !isMyTurn;
        GameUIHandler.Instance.disableCanvasGroupRayCast();
        //StartCoroutine(Wait1s());
        if (IsHost)
        {
            StartTurnClientRpc(clientRpcParams);
        }
        else
        {
            StartTurnClientRpc(hostRpcParams);
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

    public void AddFeatureToClientPlayer(Feature feature)
    {
        ulong objId = feature.GetComponent<NetworkObject>().NetworkObjectId;
        AddFeatureToPlayerClientRpc(objId, clientRpcParams);
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
    public void AddFeatureToPlayerClientRpc(ulong objId = 0, ClientRpcParams clientRpcParams = default)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<Feature>(out var myFeature))
        {
            Debug.LogError("Base is null");
        }
        if (myFeature is Base myBase)
        {
            corresPlayer.myBases.Add(myBase);
        }else if (myFeature is SpawnPoint mySpawnPoint)
        {
            corresPlayer.myspawnPoints.Add(mySpawnPoint);
        }
    }

    [ClientRpc]
    public void AddFeatureToMapClientRpc(int x, int z, ulong objId, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(TryAddFeature(x, z, objId));
    }

    public IEnumerator TryAddFeature(int x, int z, ulong objId)
    {
        while (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(objId))
        {
            Debug.Log(NetworkManager.Singleton.SpawnManager.SpawnedObjects.Count);
            yield return new WaitForSeconds(0.01f);
        }
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<Feature>(out var target))
        {
            Debug.LogError("Base is null");
        }
        HexCoordinates coordinates = new(x, z);
        HexGrid.Instance.AddFeatureBeforeGame(
            target, HexGrid.Instance.GetCell(coordinates), target.orientation
        );
    }

    [ClientRpc]
    public void SyncUnitInfoClientRpc(ulong objId, int x, int z, int cardId, ClientRpcParams clientRpcParams = default)
    {
        StartCoroutine(TryAdddUnit(objId, x, z, cardId));
    }
    public IEnumerator TryAdddUnit(ulong objId, int x, int z, int cardId)
    {
        while (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(objId))
        {
            Debug.Log(NetworkManager.Singleton.SpawnManager.SpawnedObjects.Count);
            yield return new WaitForSeconds(0.01f);
        }
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objId].TryGetComponent<HexUnit>(out var target))
        {
            Debug.LogError("Unit is null");
        }
        HexCell location = HexGrid.Instance.GetCell(new HexCoordinates(x, z));
        Debug.Log(corresPlayer);
        HexGrid.Instance.SetUnitAttributes(corresPlayer, CardDatabase.Instance.unitCardList[cardId], location, target);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestUnitInstantiationServerRpc(int x, int z, int cardId, ServerRpcParams serverRpcParams = default)
    {
        UnitCard card = CardDatabase.Instance.unitCardList[cardId];
        HexUnit unit = Instantiate(card.unitPrefab);
        NetworkObject unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.Spawn();
        HexCell location = HexGrid.Instance.GetCell(new HexCoordinates(x, z));
        Player player = serverRpcParams.Receive.SenderClientId == NetworkManager.Singleton.ConnectedClients[0].ClientId ? GameManagerServer.Instance.player1 : GameManagerServer.Instance.player2;
        HexGrid.Instance.SetUnitAttributes(player, card, location, unit);
        if (player == GameManagerServer.Instance.player1)
        {
            SyncUnitInfoClientRpc(unitNetworkObject.NetworkObjectId, x, z, cardId, hostRpcParams);
        }
        else
        {
            unitNetworkObject.ChangeOwnership(serverRpcParams.Receive.SenderClientId);
            SyncUnitInfoClientRpc(unitNetworkObject.NetworkObjectId, x, z, cardId, clientRpcParams);
        }   
    }

}
