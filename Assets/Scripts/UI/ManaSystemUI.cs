using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaNumberText;
    public void UpdateManaText(int curMana)
    {
        manaNumberText.text = "Mana " + curMana;
    }
}
