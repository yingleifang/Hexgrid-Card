using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCard : Card
{
    public int attack;
    public int attackRange;
    public string Description;
    public WeaponBehavior weaponPrefab;
    public AnimatorOverrideController overrideController;
    public float attackAnimationLength;
    public float attackActionDelay;
    public bool isLeftHand = false;
    public enum AttackType
    {   
        projectTile,
        Melee,
    }

    public override void UseEffect(Player player)
    {
        if ((player.selectedFeature is HexUnit temp))
        {
            temp.weaponInstance = EquipWeapon(temp);
        }
        else
        {
            Debug.LogError("Weapon cannot be used on none Hexunit feature");
        }
    }

    public virtual WeaponBehavior EquipWeapon(HexUnit temp)
    {
        temp.weaponCard = this;
        temp.unitAnimation.unitAnimator.runtimeAnimatorController = overrideController;
        attackActionDelay = attackAnimationLength * 2 / 3;
        return temp.myWeaponSlotManager.LoadWeaponOnSlot(weaponPrefab, isLeftHand);
    }

    public virtual void WeaponVisualEffect(Vector3 position)
    {

    }
}
