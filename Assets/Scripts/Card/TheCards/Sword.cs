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

    public override void UseEffect(UseEffectArgs useEffectArgs)
    {
        //if (useEffectArgs.feature is SpawnPoint temp)
        //{
        //    HexGrid.Instance.AddUnit(temp.Location, temp.Orientation).AttackRange = attackRange;
        //}
        //else
        //{
        //    Debug.LogError("Trying to spawn unit outside of spawnpoint");
        //}
    }
}
