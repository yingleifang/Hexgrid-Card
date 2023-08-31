using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField]
    List<Card> deck;
    int draws = 3;
    int maxCards = 7;
    int maxUnit = 5;
    public int startingMana = 10;
    public int curMana = 10;
    public int manaRegen = 10;
    public int maxMana = 20;
    public int deleteCardCost = 1;
    [SerializeField]
    CardAreaManager cardArea;
    [SerializeField]
    ManaSystemUI manaSystemUI;
    public HexCell CurrentCell { get; protected set; }
    public Feature selectedFeature;

    public List<HexUnit> myUnits;

    public List<Base> myBases;

    public List<SpawnPoint> myspawnPoints;

    bool CurrentCellChanged;

    void Start()
    {
        curMana = startingMana;
        foreach(var cardDisplay in cardArea.CardDisplays)
        {
            cardDisplay.UseCardChecks += Player_CardCheck;
            cardDisplay.OnCardUsed += ConsumeMana;
        }
        cardArea.FillSlots();
        TurnManager.Instance.OnTurnChanged += Player_OnTurnChanged;
    }

    private void Player_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnManager.Instance.isPlayer1Turn && this == GameManager.Instance.player1)
        {
            cardArea.HideAllCards();
            return;
        }else if (TurnManager.Instance.isPlayer1Turn && this == GameManager.Instance.player2)
        {
            cardArea.HideAllCards();
            return;
        }
        curMana += manaRegen;
        curMana = Math.Min(curMana, maxMana);
        manaSystemUI.UpdateManaText();
        cardArea.FillSlots();
    }
    public virtual void TakeAction()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection(HexGrid.Instance.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition)));
            }
            else if(selectedFeature is HexUnit temp)
            {
                UpdateCurrentCell(HexGrid.Instance.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition)));
                if (CurrentCell && selectedFeature.myPlayer == this)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (temp.canAttack && CurrentCell.unitFeature != null && CurrentCell.unitFeature.myPlayer != this
                            && UnitActionSystem.Instance.CanAttack(temp, CurrentCell))
                        {
                            temp.GetAttackAction().DoAttack();
                        }
                        else if (temp.canMove && HexGrid.Instance.CurrentPathExists && CurrentCell.unitFeature == null)
                        {
                            temp.GetMoveAction().DoMove();
                        }
                    }
                    else if (CurrentCellChanged && temp.canMove && CurrentCell.unitFeature == null)
                    {
                        UnitActionSystem.Instance.DoPathfinding(temp, CurrentCell);
                    }
                }
            }
        }
    }

    protected void DoSelection(HexCell cell)
    {
        HexGrid.Instance.ClearCellColor(Color.blue);
        HexGrid.Instance.ClearCellColor(Color.white);
        UpdateCurrentCell(cell);
        if (CurrentCell)
        {
            if (selectedFeature)
            {
                selectedFeature.RaiseFeatureDeSelectedEvent();
            }
            if (CurrentCell.unitFeature != null)
            {
                selectedFeature = CurrentCell.unitFeature;
            }
            else
            {
                selectedFeature = CurrentCell.terrainFeature;
            }
            if (selectedFeature)
            {
                selectedFeature.RaiseFeatureSelectedEvent();
            }
        }
        if (selectedFeature is HexUnit temp && temp.canMove == true)
        {
            HexGrid.Instance.showMoveRange(temp.Location, temp);
        }
    }
    void UpdateCurrentCell(HexCell cell)
    {
        if (cell != CurrentCell)
        {
            CurrentCell = cell;
            CurrentCellChanged = true;
        }
        else
        {
            CurrentCellChanged = false;
        }
    }
    (bool, Player) Player_CardCheck(int cost)
    {
        if (curMana - cost >= 0)
        {
            return (true, this);
        }
        return (false, this);
    }
    void ConsumeMana(int cost)
    {
        curMana -= cost;
        manaSystemUI.UpdateManaText();
    }
    protected HexCell FindNearestEnemyCell(HexCoordinates curUnitCoord)
    {
        List<HexUnit> enemyUnits;
        List<Base> enemyBases;
        HexCell res = null;
        if (GameManager.Instance.currentPlayer == GameManager.Instance.player1)
        {
            enemyUnits = GameManager.Instance.player2.myUnits;
            enemyBases = GameManager.Instance.player2.myBases;
        }
        else
        {
            enemyUnits = GameManager.Instance.player1.myUnits;
            enemyBases = GameManager.Instance.player1.myBases;
        }
        int minDistance = Int32.MaxValue;
        foreach(var enemyUnit in enemyUnits)
        {
            int curDistance = enemyUnit.location.Coordinates.DistanceTo(curUnitCoord);
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                res = enemyUnit.location;
            }
        }
        foreach (var enemyBase in enemyBases)
        {
            int curDistance = enemyBase.location.Coordinates.DistanceTo(curUnitCoord);
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                res = enemyBase.location;
            }
        }
        return res;
    }
}
