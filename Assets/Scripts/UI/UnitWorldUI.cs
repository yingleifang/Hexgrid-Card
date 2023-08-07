using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] UnitFeature myUnit;

    private void Awake()
    {
        myUnit.OnDamaged += UpdateHealthBar;
    }

    private void UpdateHealthBar(object sender, EventArgs e)
    {
        healthBarImage.fillAmount = myUnit.GetHealthNormalized();
    }
}
