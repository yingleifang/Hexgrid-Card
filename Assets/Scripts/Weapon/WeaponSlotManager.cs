using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot;
    WeaponHolderSlot rightHandSlot;

    private void Awake()
    {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach(WeaponHolderSlot weaponSlot in weaponHolderSlots)
        {
            if (weaponSlot.isLeftHandSlot)
            {
                leftHandSlot = weaponSlot;
            }else if (weaponSlot.isRightHandSlot)
            {
                rightHandSlot = weaponSlot;
            }
        }
    }

    public WeaponBehavior LoadWeaponOnSlot(WeaponBehavior weaponModel, bool isLeft)
    {
        if (isLeft)
        {
             return leftHandSlot.LoadWeaponModel(weaponModel);
        }
        else
        {
            return rightHandSlot.LoadWeaponModel(weaponModel);
        }
    }
}
