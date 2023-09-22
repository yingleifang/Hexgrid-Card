using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public virtual IEnumerator AttackBehavior(UnitFeature target = default, UnitFeature attaker = default)
    {
        return null;
    }
}
