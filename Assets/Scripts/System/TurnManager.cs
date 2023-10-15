using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public static TurnManager Instance { get; private set; }
	public event EventHandler OnTurnChanged;
	public bool isPlayer1Turn = true;

	private int turnNumber = 1;

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
	public void NextTurn()
	{
		HexGrid.Instance.BlockActions = true;
		GameUIHandler.Instance.disableCanvasGroupRayCast();
		turnNumber++;
		isPlayer1Turn = !isPlayer1Turn;
		OnTurnChanged?.Invoke(this, EventArgs.Empty);
		foreach (var unit in GameManagerServer.Instance.currentPlayer.myUnits)
        {
			unit.reFillActions();
        }
	}

	public int GetTurnNumber()
	{
		return turnNumber;
	}
}
