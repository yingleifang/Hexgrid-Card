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

    public WeaponBehavior LoadWeaponModel(WeaponBehavior weaponItem)
    {
        UnloadWeaponAndDestroy();
        if (weaponItem == null)
        {
            UnLoadWeaopn();
            return null;
        }
        GameObject model = Instantiate(weaponItem.transform.gameObject);
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
        return model.GetComponent<WeaponBehavior>();
    }
}
