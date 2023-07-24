using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Warrior", menuName = "Warrior")]
public class Warrior : UnitCard
{
    void Reset()
    {
        cardType = CardType.MeleeSoldier;
    }

    public override void UseEffect(UseEffectArgs useEffectArgs)
    {
        HexGrid.Instance.AddUnit(useEffectArgs.feature.Location, useEffectArgs.feature.Orientation);
    }
}
