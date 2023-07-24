using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI manaNumberText;

    private void Start()
    {
        UpdateManaText();
    }
    public void UpdateManaText()
    {
        manaNumberText.text = "Mana " + GameManager.Instance.currentPlayer.curMana;
    }
}
