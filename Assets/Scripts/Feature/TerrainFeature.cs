using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainFeature : Feature
{
	public override HexCell Location
	{
		get => location;
		set
		{
			if (location)
			{
				location.terrainFeature = null;
			}
			location = value;
			value.terrainFeature = this;
			transform.localPosition = value.Position;
			//HexGrid.Instance.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}
}
