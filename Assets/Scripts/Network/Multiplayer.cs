using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviour
{
    public static Multiplayer Instance { get; private set; }
    [SerializeField]
    GameManagerServer gameManagerServerPrefab;
    [SerializeField]
    GameManagerClient gameManagerClientPrefab;

    [SerializeField]
    Button startHostButton;
    [SerializeField]
    Button startClientButton;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TurnSystem!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameUIHandler.Instance.enableCanvasGroupRayCast();
    }
    public void StartGameHost()
    {
        NetworkManager.Singleton.StartHost();
        GameManagerClient gameManagerClient = Instantiate(gameManagerClientPrefab);
        gameManagerClient.GetComponent<NetworkObject>().Spawn();
        Instantiate(gameManagerServerPrefab).SetHostPlayer(gameManagerClient);
        startHostButton.gameObject.SetActive(false);
        startClientButton.gameObject.SetActive(false);
    }
    public void StartGameClient()
    {
        NetworkManager.Singleton.StartClient();
        startClientButton.gameObject.SetActive(false);
        startHostButton.gameObject.SetActive(false);
    }
}
