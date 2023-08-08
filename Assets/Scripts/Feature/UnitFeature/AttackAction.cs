using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    UnitAnimation unitAnimation;

    float HitDealy = 0.3f;
    float DeathDelay = 2f;

    UnitFeature target;
    private void Start()
    {
        unitAnimation = GetComponentInChildren<UnitAnimation>();
    }
    public void DoAttack()
    {
        unit.canAttack = false;
        StartBlockingCoroutine(Hitting(unit.myPlayer.CurrentCell.unitFeature));
    }

    public IEnumerator Hitting(UnitFeature target)
    {
        this.target = target;
        yield return unit.LookAt(target.location.Position);
        unitAnimation.UnitAnimation_Attack();
        yield return new WaitForSeconds(HitDealy);
        HitTarget();
    }

    public void HitTarget()
    {
        target.TakeDamage(unit.attackDamage);
    }

    public void PlayGetHitAnim()
    {
        unitAnimation.UnitAnimation_GetHit();
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(HitDealy);
        unitAnimation.UnitAnimation_Death();
        yield return new WaitForSeconds(DeathDelay);
        Destroy(gameObject);
    }
}
