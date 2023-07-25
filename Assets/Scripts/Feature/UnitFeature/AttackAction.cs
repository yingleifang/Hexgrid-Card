using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    UnitAnimation unitAnimation;

    HexCell target;
    private void Start()
    {
        unitAnimation = GetComponentInChildren<UnitAnimation>();
    }
    public void DoAttack()
    {
        target = unit.myPlayer.CurrentCell;
        StartCoroutine(Hitting());
    }

    IEnumerator Hitting()
    {
        yield return unit.GetLookAtAction().LookAt(target.Position);
        unitAnimation.UnitAnimation_Attack();
        yield return new WaitForSeconds(0.5f);
        target.unitFeature.TakeDamage(unit.attackDamage);
    }
}
