using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    float HitDelay = 0.3f;
    float damagedDelay = 1f;
    float DeathDelay = 2f;

    UnitFeature target;
    public void DoAttack()
    {
        unit.canAttack = false;
        StartBlockingCoroutine(Hitting(unit.myPlayer.CurrentCell.unitFeature));
    }

    public IEnumerator Hitting(UnitFeature target)
    {
        this.target = target;
        yield return unit.LookAt(target.location.Position);
        unit.unitAnimation.UnitAnimation_Attack();
        yield return new WaitForSeconds(HitDelay);
        HitTarget();
        yield return new WaitForSeconds(damagedDelay);
    }

    public void HitTarget()
    {
        target.TakeDamage(unit.attackDamage);
    }

    public void PlayGetHitAnim()
    {
        unit.unitAnimation.UnitAnimation_GetHit();
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(HitDelay);
        unit.unitAnimation.UnitAnimation_Death();
        yield return new WaitForSeconds(DeathDelay);
        Destroy(gameObject);
    }
}
