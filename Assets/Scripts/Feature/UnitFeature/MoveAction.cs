using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
	public event EventHandler StartMoving;

	public event EventHandler StopMoving;


	List<HexCell> pathToTravel;


	/// <summary>
	/// Travel along a path.
	/// </summary>
	/// <param name="path">List of cells that describe a valid path.</param>
	void Travel(List<HexCell> path)
	{
		unit.location.Feature = null;
		unit.location = path[path.Count - 1];
		unit.location.Feature = unit;
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
	}

	IEnumerator TravelPath()
	{
		StartMoving?.Invoke(this, EventArgs.Empty);
		Vector3 a, b, c = pathToTravel[0].Position;
		yield return unit.GetLookAtAction().LookAt(pathToTravel[1].Position);

		if (!unit.currentTravelLocation)
		{
			unit.currentTravelLocation = pathToTravel[0];
		}
		int currentColumn = unit.currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * unit.travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++)
		{
			unit.currentTravelLocation = pathToTravel[i];
			a = c;
			b = pathToTravel[i - 1].Position;

			int nextColumn = unit.currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn)
			{
				HexGrid.Instance.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + unit.currentTravelLocation.Position) * 0.5f;

			for (; t < 1f; t += Time.deltaTime * unit.travelSpeed)
			{
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
			}
			t -= 1f;
		}
		unit.currentTravelLocation = null;

		a = c;
		b = unit.location.Position;
		c = b;
		for (; t < 1f; t += Time.deltaTime * unit.travelSpeed)
		{
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
		}

		transform.localPosition = unit.location.Position;
		unit.orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
		StopMoving?.Invoke(this, EventArgs.Empty);
	}

	public void DoMove()
	{
		if (HexGrid.Instance.HasPath)
		{
			Travel(HexGrid.Instance.GetPath());
			HexGrid.Instance.ClearCellColor(Color.blue);
			HexGrid.Instance.ClearCellColor(Color.white);
		}
	}
}
