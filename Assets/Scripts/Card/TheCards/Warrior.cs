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

    public override void UseEffect(Player player)
    {
        if (player.selectedFeature is SpawnPoint temp)
        {
            HexUnit spawnedUnit = HexGrid.Instance.AddUnit(temp.Location, temp.Orientation);
            spawnedUnit.AttackRange = attackRange;
            spawnedUnit.unitType = cardType;
        }
        else
        {
            Debug.LogError("Trying to spawn unit outside of spawnpoint");
        }
    }

    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is not SpawnPoint || player.selectedFeature.location.unitFeature != null)
        {
            return false;
        }
        if (player.selectedFeature.myPlayer != GameManager.Instance.currentPlayer)
        {
            return false;
        }
        return true;
    }
}
