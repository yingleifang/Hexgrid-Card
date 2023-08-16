using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
	public static UnitActionSystem Instance { get; private set; }

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
	public void DoPathfinding(HexUnit seletedUnit, HexCell toCell)
	{
		if (toCell && seletedUnit.IsValidDestination(toCell))
		{
			HexGrid.Instance.FindPath(seletedUnit.Location, toCell, seletedUnit, seletedUnit.MovementRange);
		}
		else
		{
			HexGrid.Instance.ClearCellColor(Color.blue);
		}
	}
	public bool CanAttack(HexUnit seletedUnit, HexCell toCell)
	{
		if (seletedUnit.Location.Coordinates.DistanceTo(toCell.Coordinates) <= seletedUnit.GetUnitAttackRange())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
