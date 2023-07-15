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

    public void UseEffect(Feature selected)
    {
        HexGrid.Instance.AddUnit(selected.Location, selected.Orientation);
    }
}
