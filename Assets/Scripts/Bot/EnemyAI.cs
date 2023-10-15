using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : Player
{
    int aggressiveRange = 20;

    [SerializeField]
    int maxUnits = 5;
    enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    public override void TakeAction()
    {
        StartCoroutine(EnemyMovement());
        HexGrid.Instance.BlockActions = true;
    }

    IEnumerator EnemyMovement()
    {
        foreach (var curUnit in myUnits)
        {
            HexCell targetCell = FindNearestEnemyCell(curUnit.location.Coordinates);
            if (targetCell is null)
            {
                Debug.Log("No valid targetCell to move to");
            }
            else
            {
                UnitFeature tempCopy = targetCell.unitFeature;
                targetCell.unitFeature = null;
                HexGrid.Instance.FindPath(curUnit.Location, targetCell, curUnit, aggressiveRange);
                targetCell.unitFeature = tempCopy;
                if (HexGrid.Instance.HasPath)
                {
                    if (HexGrid.Instance.GetPath(curUnit.MovementRange))
                    {
                        targetCell.unitFeature = tempCopy;
                        if (HexGrid.Instance.curPath.Count == 1)
                        {
                            yield return StartCoroutine(curUnit.GetAttackAction().Hitting(targetCell.unitFeature));
                        }
                        else
                        {
                            curUnit.location.unitFeature = null;
                            curUnit.location = HexGrid.Instance.curPath[^1];
                            curUnit.location.unitFeature = curUnit;
                            yield return StartCoroutine(curUnit.GetMoveAction().TravelPath());
                        }
                    }
                }
            }

        }
        TurnManager.Instance.NextTurn();
    }

    //State state;
    //float timer;
    //private void Awake()
    //{
    //    state = State.WaitingForEnemyTurn;
    //}

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += EnemyAI_OnTurnChanged;
    }
    //private void Update()
    //{
    //    if (TurnManager.Instance.isPlayer1Turn)
    //    {
    //        return;
    //    }
    //    switch (state)
    //    {
    //        case State.WaitingForEnemyTurn:
    //            break;
    //        case State.TakingTurn:
    //            timer -= Time.deltaTime;
    //            if (timer <= 0f)
    //            {
    //                if (TryTakeEnemyAIAction(SetStateTakingTurn))
    //                {
    //                    state = State.Busy;
    //                }
    //                else
    //                {
    //                    TurnManager.Instance.NextTurn();
    //                }
    //            }
    //            break;
    //        case State.Busy:
    //            break;
    //    }
    //}

    //private void SetStateTakingTurn()
    //{
    //    timer = 0.5f;
    //    state = State.TakingTurn;
    //}
    private void EnemyAI_OnTurnChanged(object sender, EventArgs e)
    {
        if (myUnits.Count < maxUnits)
        {
            if (!TurnManager.Instance.isPlayer1Turn)
            {
                int baseNum = UnityEngine.Random.Range(0, myspawnPoints.Count);
                int attemps = 0;
                while (myspawnPoints[baseNum].location.unitFeature != null && attemps < myspawnPoints.Count)
                {
                    baseNum += 1;
                    baseNum %= myspawnPoints.Count;
                    attemps += 1;
                }
                if (attemps < myspawnPoints.Count)
                {
                    DoSelection(myspawnPoints[baseNum].location);
                    cardDatabase.GetRandomUnitCard().UseEffect(this);
                }
            }
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (HexUnit unit in GameManagerServer.Instance.player2.myUnits)
        {
            if (TakeEnemyAIAction(unit, onEnemyAIActionComplete))
            {
                return true;
            }
        }
        return false;
    }

    bool TakeEnemyAIAction(HexUnit unit, Action onEnemyAIActionComplete)
    {
        StartCoroutine(enemyTest());
        return true;
    }

    IEnumerator enemyTest()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
