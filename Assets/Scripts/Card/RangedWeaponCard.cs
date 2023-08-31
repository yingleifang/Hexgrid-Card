using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponCard : WeaponCard
{
    void Reset()
    {
        cardType = CardType.RangedWeapon;
    }

    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is HexUnit temp && temp.unitType == CardType.RangedSoldier)
        {
            return true;
        }

        return false;
    }

    public override void UseEffect(Player player)
    {
        base.UseEffect(player);
        if ((player.selectedFeature is HexUnit temp))
        {
            temp.projectile = weaponPrefab.GetComponentInChildren<Projectile>();
        }
    }
}
