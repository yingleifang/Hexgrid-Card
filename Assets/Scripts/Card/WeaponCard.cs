using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCard : Card
{
    public int attack;
    public int attackRange;
    public string Description;
    public GameObject weaponPrefab;

    public override void UseEffect(Player player)
    {
        if ((player.selectedFeature is HexUnit temp))
        {
            string prevWeapon = "Base";
            if (temp.weaponCard)
            {
                prevWeapon = temp.weaponCard.name;
            }
            temp.unitAnimation.setActiveLayer(prevWeapon, 0);
            temp.weaponCard = this;
            temp.myWeaponSlotManager.LoadWeaponOnSlot(weaponPrefab, false);
            temp.unitAnimation.setActiveLayer(temp.weaponCard.name, 1);
        }
        else
        {
            Debug.LogError("Weapon cannot be used on none Hexunit feature");
        }
    }
}
