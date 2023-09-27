using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

public class MagicWeaponEntity : WeaponBehavior
{

    public float RotationSpeed = 15;
    public float spellPositionScaler = 0.6f;
    public float radius = 10;

    Vector3 pivot; // The object the weapon will orbit around
    private void LateUpdate()
    {
        SetWeaponPostion();
        transform.RotateAround(pivot, new Vector3(0, 1, 0), RotationSpeed * Time.deltaTime);
    }
    public void SetWeaponPostion()
    {
        pivot = owner.transform.position + owner.meshHeight * owner.transform.localScale.y * Vector3.up / spellPositionScaler;
        transform.position = pivot + radius * Vector3.left;
    }
}