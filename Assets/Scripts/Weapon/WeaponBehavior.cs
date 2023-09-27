using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    protected UnitFeature owner;
    public virtual IEnumerator AttackBehavior(UnitFeature target = default, UnitFeature attaker = default)
    {
        return null;
    }

    public virtual void setOwner(UnitFeature temp)
    {
        owner = temp;
    }
}
