using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    float timer;

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += EnemyAI_OnTurnChanged;
    }
    private void Update()
    {
        if (TurnManager.Instance.isPlayer1Turn)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TurnManager.Instance.NextTurn();
        }
    }

    private void EnemyAI_OnTurnChanged(object sender, EventArgs e)
    {
        timer = 2f;
    }
}
