using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnBtn;
    [SerializeField] private TextMeshProUGUI turnNumberText;

    private void Start()
    {
        endTurnBtn.onClick.AddListener(() => TurnManager.Instance.NextTurn());
        TurnManager.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
    }
    private void UpdateTurnText()
    {
        turnNumberText.text = "TURN " + TurnManager.Instance.GetTurnNumber();
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnBtn.gameObject.SetActive(TurnManager.Instance.isPlayer1Turn);
    }
}
