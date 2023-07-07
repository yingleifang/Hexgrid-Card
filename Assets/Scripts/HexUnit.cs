using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Component representing a unit that occupies a cell of the hex map.
/// </summary>
public class HexUnit : MonoBehaviour {

	const float rotationSpeed = 180f;
	const float travelSpeed = 4f;

	public static HexUnit unitPrefab;

	public HexGrid Grid { get; set; }

	/// <summary>
	/// Cell that the unit occupies.
	/// </summary>
	public HexCell Location {
		get => location;
		set {
			if (location) {
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

	HexCell location, currentTravelLocation;

	/// <summary>
	/// Orientation that the unit is facing.
	/// </summary>
	public float Orientation {
		get => orientation;
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}

	/// <summary>
	/// Speed of the unit, in cells per turn.
	/// </summary>
	public int Speed => 24;

	float orientation;

	List<HexCell> pathToTravel;

	/// <summary>
	/// Validate the position of the unit.
	/// </summary>
	public void ValidateLocation () => transform.localPosition = location.Position;

	/// <summary>
	/// Checl whether a cell is a valid destination for the unit.
	/// </summary>
	/// <param name="cell">Cell to check.</param>
	/// <returns>Whether the unit could occupy the cell.</returns>
	public bool IsValidDestination (HexCell cell) => !cell.IsUnderwater && !cell.Unit;

	/// <summary>
	/// Travel along a path.
	/// </summary>
	/// <param name="path">List of cells that describe a valid path.</param>
	public void Travel (List<HexCell> path) {
		location.Unit = null;
		location = path[path.Count - 1];
		location.Unit = this;
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
	}

	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;
		yield return LookAt(pathToTravel[1].Position);

		if (!currentTravelLocation) {
			currentTravelLocation = pathToTravel[0];
		}
		int currentColumn = currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++) {
			currentTravelLocation = pathToTravel[i];
			a = c;
			b = pathToTravel[i - 1].Position;

			int nextColumn = currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn){
				Grid.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + currentTravelLocation.Position) * 0.5f;

			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
			}
			t -= 1f;
		}
		currentTravelLocation = null;

		a = c;
		b = location.Position;
		c = b;
		for (; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
		}

		transform.localPosition = location.Position;
		orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
	}

	IEnumerator LookAt (Vector3 point) {
		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			) {
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}

		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}

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
		int moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 1;
		}
		else if (fromCell.Walled != toCell.Walled) {
			return -1;
		}
		else {
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
			moveCost +=
				toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
	}

	/// <summary>
	/// Terminate the unit.
	/// </summary>
	public void Die () {
		location.Unit = null;
		Destroy(gameObject);
	}

	/// <summary>
	/// Save the unit data.
	/// </summary>
	/// <param name="writer"><see cref="BinaryWriter"/> to use.</param>
	public void Save (BinaryWriter writer) {
		location.Coordinates.Save(writer);
		writer.Write(orientation);
	}

	/// <summary>
	/// Load the unit data.
	/// </summary>
	/// <param name="reader"><see cref="BinaryReader"/> to use.</param>
	/// <param name="grid"><see cref="HexGrid"/> to add the unit to.</param>
	public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddUnit(
			Instantiate(unitPrefab), grid.GetCell(coordinates), orientation
		);
	}

	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
				currentTravelLocation = null;
			}
		}
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