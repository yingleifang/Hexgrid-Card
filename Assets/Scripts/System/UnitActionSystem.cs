using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
	public static UnitActionSystem Instance { get; private set; }

	HexCell currentCell;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There's more than one UnitActionSystem!");
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}
	public void DoPathfinding(HexUnit seletedUnit)
	{
		if (UpdateCurrentCell())
		{
			if (currentCell && seletedUnit.IsValidDestination(currentCell))
			{
				HexGrid.Instance.FindPath(seletedUnit.Location, currentCell, seletedUnit);
			}
			else
			{
				HexGrid.Instance.ClearCellColor(Color.blue);
			}
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
}
