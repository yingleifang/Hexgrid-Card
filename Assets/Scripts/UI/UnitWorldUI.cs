using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    [SerializeField] UnitFeature myUnit;
    [SerializeField] TextMeshPro healthText;

    private void Start()
    {
        myUnit.OnDamaged += UpdateHealthBar;
        myUnit.OnDamaged += UpdateHealthText;
        healthText.text = $"{myUnit.UnitCurHealth}/{myUnit.UnitTotalHealth}";
    }

    private void UpdateHealthBar(object sender, EventArgs e)
    {
        healthBarImage.fillAmount = myUnit.GetHealthNormalized();
    }

    private void UpdateHealthText(object sender, EventArgs e)
    {
        healthText.text = $"{myUnit.UnitCurHealth}/{myUnit.UnitTotalHealth}";
    }
}
