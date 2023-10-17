using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManagerServer : MonoBehaviour
{
	public static GameManagerServer Instance { get; private set; }
	[SerializeField]
	string currentMap = "Alpha1";

	//const int mapFileVersion = 5;

	public bool deleteMode = false; 

	//public event EventHandler OnSelectedUnitChanged;

	public Player player1;

	public Player player2;

	//public Player currentPlayer;

	public Vector3 curAttackingPosition;

	bool gameStarted = false;

	GameManagerClient localGameManagerClient;
	private void Awake()
    {
		if (Instance != null)
		{
			Debug.LogError("There's more than one GameManagerServer!");
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public void SetHostPlayer(GameManagerClient clientGameManager)
	{
		localGameManagerClient = clientGameManager;
        player1 = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<Player>();
        //currentPlayer = player1;
    }
	//private void GameManager_OnTurnChanged(object sender, EventArgs e)
	//{
	//	if (currentPlayer == player1)
 //       {
	//		currentPlayer = player2;
 //       }
 //       else
 //       {
	//		currentPlayer = player1;
	//	}
	//}
	private void Update()
	{
		if (!gameStarted) 
		{ 
			if (NetworkManager.Singleton.ConnectedClients.Count == 2)
			{
                Debug.Log("Start Game Server");
                player2 = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId + 1].PlayerObject.GetComponent<Player>();
                localGameManagerClient.StartGameAllClients();
				gameStarted = true;
			}
			return;
		}
	}

    void TurnTransition(object sender, EventArgs e)
    {
		GameUIHandler.Instance.disableCanvasGroupRayCast();
		StartCoroutine(Wait1s());
    }

	IEnumerator Wait1s()
    {
		yield return new WaitForSeconds(1f);
		HexGrid.Instance.BlockActions = false;
	}

}
