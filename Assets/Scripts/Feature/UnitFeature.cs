using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitFeature : Feature
{
	[SerializeField]
	int UnitFeatureHealth;
	public override HexCell Location
	{
		get => location;
		set
		{
			if (location)
			{
				location.unitFeature = null;
			}
			location = value;
			value.unitFeature = this;
			transform.localPosition = value.Position;
			HexGrid.Instance.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

	public void TakeDamage(int damage)
    {
		Debug.Log("Hurt!!!!");
		UnitFeatureHealth -= damage;

	}

}