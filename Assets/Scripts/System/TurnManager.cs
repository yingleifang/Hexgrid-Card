using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	public static TurnManager Instance { get; private set; }

	public event EventHandler OnTurnChanged;

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
		turnNumber++;
		OnTurnChanged?.Invoke(this, EventArgs.Empty);
	}

	public int GetTurnNumber()
	{
		return turnNumber;
	}
}
