using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalSpell : WeaponCard
{
    void Reset()
    {
        cardType = CardType.ElementalSpell;
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
