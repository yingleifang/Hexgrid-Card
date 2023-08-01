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
        if (useEffectArgs.feature is SpawnPoint temp)
        {
            HexGrid.Instance.AddUnit(temp.Location, temp.Orientation).AttackRange = attackRange;
        }
        else
        {
            Debug.LogError("Trying to spawn unit outside of spawnpoint");
        }
    }
}
