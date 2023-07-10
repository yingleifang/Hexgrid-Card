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
}
