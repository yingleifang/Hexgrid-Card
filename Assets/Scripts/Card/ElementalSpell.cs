using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementalSpell : WeaponCard
{
    void Reset()
    {
        cardType = CardType.ElementalSpell;
    }
    public override WeaponBehavior EquipWeapon(HexUnit temp)
    {
        temp.weaponCard = this;
        if (weaponPrefab is MagicWeaponEntity magicWeaponEntity)
        {
            MagicWeaponEntity weaponInstance = Instantiate(magicWeaponEntity, temp.transform);
            temp.unitAnimation.unitAnimator.runtimeAnimatorController = overrideController;
            attackActionDelay = attackAnimationLength * 2 / 3;
            weaponInstance.setOwner(temp);
            return weaponInstance;
        }
        return null;
    }
    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is HexUnit temp && (temp.unitType == CardType.ElementalMage || temp.unitType == CardType.SpiritualMage))
        {
            return true;
        }

        return false;
    }
}
