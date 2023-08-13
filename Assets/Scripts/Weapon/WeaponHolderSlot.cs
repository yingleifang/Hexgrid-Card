using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    public Transform parentOverride;
    public bool isLeftHandSlot;
    public bool isRightHandSlot;

    public GameObject curerntWeaponModel;

    public void UnLoadWeaopn()
    {
        if (curerntWeaponModel != null)
        {
            curerntWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy()
    {
        if (curerntWeaponModel != null)
        {
            Destroy(curerntWeaponModel);
        }
    }

    public void LoadWeaponModel(GameObject weaponItem)
    {
        UnloadWeaponAndDestroy();
        if (weaponItem == null)
        {
            UnLoadWeaopn();
            return;
        }
        GameObject model = Instantiate(weaponItem);
        if (model != null)
        {
            if(parentOverride != null)
            {
                model.transform.parent = parentOverride;
            }
            else
            {
                model.transform.parent = transform;
            }
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }
        curerntWeaponModel = model;
    }
}
