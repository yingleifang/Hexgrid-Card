using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire", menuName = "Fire")]
public class Fire : ElementalSpell
{

    public override WeaponBehavior EquipWeapon(HexUnit temp)
    {
        temp.weaponCard = this;
        WeaponBehavior weaponInstance = temp.myWeaponSlotManager.LoadWeaponOnSlot(weaponPrefab, isLeftHand);
        temp.unitAnimation.unitAnimator.runtimeAnimatorController = overrideController;
        attackActionDelay = attackAnimationLength * 2 / 3;
        MagicWeaponEntity magicWeaponEntity = weaponPrefab.GetComponent<MagicWeaponEntity>();
        magicWeaponEntity.leftHandParticple.Play();
        magicWeaponEntity.rightHandParticple.Play();
        Debug.Log(magicWeaponEntity.leftHandParticple.isPlaying);
        Debug.Log(magicWeaponEntity.rightHandParticple.isPlaying);
        return weaponInstance;

    }

}