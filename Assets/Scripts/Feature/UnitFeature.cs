using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitFeature : Feature
{
	public int UnitCurHealth { get; private set; } = 10;
	public int UnitTotalHealth { get; private set; } = 10;

	public event EventHandler OnDamaged;
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
		UnitCurHealth -= damage;
		OnDamaged?.Invoke(this, EventArgs.Empty);
		if (this is HexUnit temp)
        {
			if (temp.UnitCurHealth <= 0){
                StartCoroutine(temp.GetAttackAction().Death());
            }
            else
            {
				temp.GetAttackAction().PlayGetHitAnim();
			}
		}

	}
    protected virtual void GetHitVisual()
    {
		Debug.Log("Show some damage Numbers");
    }
	public float GetHealthNormalized()
	{
		return (float)UnitCurHealth / UnitTotalHealth;
	}
}