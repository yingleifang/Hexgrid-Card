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

    public override void UseEffect()
    {
        Feature selected = GameManager.Instance.selectedFeature;
        if (selected is SpawnPoint)
        {
            HexGrid.Instance.AddUnit(selected.Location, selected.Orientation);
        }
    }
}
