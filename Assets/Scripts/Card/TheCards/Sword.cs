using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Sword")]
public class Sword : WeaponCard
{
    void Reset()
    {
        cardType = CardType.MeleeWeapon;
    }


    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is HexUnit temp && temp.unitType == CardType.MeleeSoldier)
        {
            return true;
        }

        return false;
    }
}
