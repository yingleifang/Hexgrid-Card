using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    [SerializeField] Animator unitAnimator;
    private void Awake()
    {
        unitAnimator = FindObjectOfType<Animator>();
        unitAnimator.fireEvents = false;
        HexUnit myUnit = GetComponent<HexUnit>();
        myUnit.GetMoveAction().StartMoving += UnitAnimation_StartMoving;
        myUnit.GetMoveAction().StopMoving += UnitAnimation_StopMoving;
    }
    public void UnitAnimation_Attack()
    {
        unitAnimator.SetTrigger("attack");
    }

    public void UnitAnimation_GetHit()
    {
        unitAnimator.SetTrigger("getHit");
    }

    public void UnitAnimation_Death()
    {
        unitAnimator.SetTrigger("Die");
    }

    void UnitAnimation_StartMoving(object sender, EventArgs empty)
    {
        HexGrid.Instance.unitIsBusy = true;
        unitAnimator.SetBool("isRunning", true);
    }
    void UnitAnimation_StopMoving(object sender, EventArgs empty)
    {
        unitAnimator.SetBool("isRunning", false);
        HexGrid.Instance.unitIsBusy = false;
    }
}
