using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponCard : WeaponCard
{
    void Reset()
    {
        cardType = CardType.MeleeWeapon;
    }

    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is HexUnit temp && (temp.unitType == CardType.MeleeSoldier || temp.unitType == CardType.RangedSoldier))
        {
            return true;
        }

        return false;
    }
}
