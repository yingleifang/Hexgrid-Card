using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// Component representing a unit that occupies a cell of the hex map.
/// </summary>
public class HexUnit : UnitFeature
{

	MoveAction moveAction;
	LookAtAction lookAtAction;
	AttackAction attackAction;

	public float rotationSpeed = 180f;
	public float travelSpeed = 4f;
	public int attackDamage = 3;

	public static HexUnit unitPrefab;

	public HexCell currentTravelLocation;

	public event EventHandler OnUnitDamaged;

	/// <summary>
	/// Speed of the unit, in cells per turn.
	/// </summary>
	public int MovementRange => 3;

	public int AttackRange = 0;

	void Awake()
	{
		moveAction = GetComponent<MoveAction>();
		lookAtAction = GetComponent<LookAtAction>();
		attackAction = GetComponent<AttackAction>();
	}

	/// <summary>
	/// Checl whether a cell is a valid destination for the unit.
	/// </summary>
	/// <param name="cell">Cell to check.</param>
	/// <returns>Whether the unit could occupy the cell.</returns>
	public bool IsValidDestination (HexCell cell) => !cell.IsUnderwater && !cell.unitFeature;

	/// <summary>
	/// Get the movement cost of moving from one cell to another.
	/// </summary>
	/// <param name="fromCell">Cell to move from.</param>
	/// <param name="toCell">Cell to move to.</param>
	/// <param name="direction">Movement direction.</param>
	/// <returns></returns>
	public int GetMoveCost (
		HexCell fromCell, HexCell toCell, HexDirection direction)
	{
		if (!IsValidDestination(toCell)) {
			return -1;
		}
		HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == HexEdgeType.Cliff) {
			return -1;
		}
		//int moveCost;
		//if (fromCell.HasRoadThroughEdge(direction)) {
		//	moveCost = 1;
		//}
		if (fromCell.Walled != toCell.Walled) {
			return -1;
		}
		return 1;
		//else {
		//	moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
		//	moveCost +=
		//		toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		//}
		//return moveCost;
	}

	/// <summary>
	/// Terminate the unit.
	/// </summary>
	public void Die () {
		location.unitFeature = null;
		Destroy(gameObject);
	}

	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
				currentTravelLocation = null;
			}
		}
	}
	public MoveAction GetMoveAction()
	{
		return moveAction;
	}
	public LookAtAction GetLookAtAction()
	{
		return lookAtAction;
	}

	public AttackAction GetAttackAction()
	{
		return attackAction;
	}
	public override void TakeDamage(int damage)
	{
		base.TakeDamage(damage);
			if (UnitCurHealth <= 0)
			{
				StartCoroutine(GetAttackAction().Death());
				DisableUnit();
				myPlayer.myUnits.Remove(this);
			}
			else
			{
				GetAttackAction().PlayGetHitAnim();
			}
		GetHitVisual();
	}

	//	void OnDrawGizmos () {
	//		if (pathToTravel == null || pathToTravel.Count == 0) {
	//			return;
	//		}
	//
	//		Vector3 a, b, c = pathToTravel[0].Position;
	//
	//		for (int i = 1; i < pathToTravel.Count; i++) {
	//			a = c;
	//			b = pathToTravel[i - 1].Position;
	//			c = (b + pathToTravel[i].Position) * 0.5f;
	//			for (float t = 0f; t < 1f; t += 0.1f) {
	//				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
	//			}
	//		}
	//
	//		a = c;
	//		b = pathToTravel[pathToTravel.Count - 1].Position;
	//		c = b;
	//		for (float t = 0f; t < 1f; t += 0.1f) {
	//			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
	//		}
	//	}
}