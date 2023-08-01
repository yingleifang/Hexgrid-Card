using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFeature : Feature
{
	public int UnitCurHealth { get; protected set; } = 10;
	public int UnitTotalHealth { get; protected set; } = 10;

	public event EventHandler OnDamaged;

	public bool canMove { get; private set; } = true;
	public bool canAttack { get; private set; } = true;
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

	public virtual void TakeDamage(int damage)
    {
		UnitCurHealth -= damage;
		OnDamaged?.Invoke(this, EventArgs.Empty);
		GetHitVisual();
	}
    protected virtual void GetHitVisual()
    {
		Debug.Log("Show some damage Numbers");
    }
	public float GetHealthNormalized()
	{
		return (float)UnitCurHealth / UnitTotalHealth;
	}

	public void DisableUnit()
    {
		canMove = false;
		canAttack = false;
	}
}