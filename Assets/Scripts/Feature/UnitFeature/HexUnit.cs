using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.Rendering;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;

/// <summary>
/// Component representing a unit that occupies a cell of the hex map.
/// </summary>
public class HexUnit : UnitFeature
{
	float DeathDelay = 2f;

	MoveAction moveAction;
	AttackAction attackAction;

	public float rotationSpeed = 180f;
	public float travelSpeed = 4f;
	[SerializeField]
	int attackDamage = 3;
	[SerializeField]
	int AttackRange = 0;

	public static HexUnit unitPrefab;

	public HexCell currentTravelLocation;

	/// <summary>
	/// Speed of the unit, in cells per turn.
	/// </summary>
	public int MovementRange => 3;
	public bool canMove = true;
	public bool canAttack = true;

	public Card.CardType unitType;

	public WeaponCard weaponCard;

	public UnitAnimation unitAnimation;

	[SerializeField]
	public WeaponSlotManager myWeaponSlotManager;

	public WeaponBehavior weaponInstance;
	protected override void Awake()
	{
		base.Awake();
		moveAction = GetComponent<MoveAction>();
		attackAction = GetComponent<AttackAction>();
    }

    public float getHitDelay()
    {
		return weaponCard.attackActionDelay;
    }
	public void reFillActions()
    {
		canMove = true;
		canAttack = true;
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

	public AttackAction GetAttackAction()
	{
		return attackAction;
	}
	public override void TakeDamage(int damage)
	{
		base.TakeDamage(damage);
			if (UnitCurHealth <= 0)
			{
				StartCoroutine(Death());
				DisableUnit();
				myPlayer.myUnits.Remove(this);
			}
			else
			{
				unitAnimation.UnitAnimation_GetHit();
			}
	}
	public IEnumerator Death()
	{
		unitAnimation.UnitAnimation_Death();
		yield return new WaitForSeconds(DeathDelay);
		Destroy(gameObject);
	}

	public IEnumerator TurnTo(Vector3 point)
	{
		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f)
		{
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			)
			{
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}

		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}
	public void DisableUnit()
	{
		canMove = false;
		canAttack = false;
	}
	
	public int GetUnitDamage()
    {
		int totalDamage = attackDamage;
		if (weaponCard)
        {
			totalDamage += weaponCard.attack;
		}
		return totalDamage;
    }

	public int GetUnitAttackRange()
	{
		int totalAttackRange = AttackRange;
		if (weaponCard)
		{
			totalAttackRange += weaponCard.attackRange;
		}
		return totalAttackRange;
	}

	public void Shoot()
    {
		Debug.Log("????????????????????");
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