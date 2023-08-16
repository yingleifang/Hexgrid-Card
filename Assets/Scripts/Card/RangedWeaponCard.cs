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
}
