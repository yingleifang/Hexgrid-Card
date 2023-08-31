using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCard : Card
{
    public int attack;
    public int attackRange;
    public string Description;
    public GameObject weaponPrefab;
    public AnimatorOverrideController overrideController;
    public float attackAnimationLength;

    public override void UseEffect(Player player)
    {
        if ((player.selectedFeature is HexUnit temp))
        {
            temp.weaponCard = this;
            temp.myWeaponSlotManager.LoadWeaponOnSlot(weaponPrefab, false);
            temp.unitAnimation.unitAnimator.runtimeAnimatorController = overrideController;
            SetattackAnimationLength();
        }
        else
        {
            Debug.LogError("Weapon cannot be used on none Hexunit feature");
        }
    }

    public void SetattackAnimationLength()
    {
        attackAnimationLength = overrideController.animationClips[0].length;
    }

    public virtual void WeaponVisualEffect(Vector3 position)
    {

    }
}
