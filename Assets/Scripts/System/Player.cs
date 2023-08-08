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
    [SerializeField]
    CardDatabase cardDatabase;
    [SerializeField]
    CardAreaManager cardArea;
    [SerializeField]
    ManaSystemUI manaSystemUI;
    public HexCell CurrentCell { get; private set; }
    public Feature selectedFeature;

    public List<HexUnit> myUnits;

    public List<Base> myBases;

    public List<SpawnPoint> myspawnPoints;

    bool CurrentCellChanged;
    // Start is called before the first frame update
    void Start()
    {
        curMana = startingMana;
        foreach(var cardDisplay in cardArea.CardDisplays)
        {
            cardDisplay.UseCardChecks += Player_WarriorCardCheck;
            cardDisplay.OnCardUsed += ConsumeMana;
        }
        initializeBase();
        deck.AddRange(cardDatabase.Draw());
        cardArea.FillSlots(deck);
        TurnManager.Instance.OnTurnChanged += Player_OnTurnChanged;
    }

    private void Player_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnManager.Instance.isPlayer1Turn && this == GameManager.Instance.player1)
        {
            return;
        }else if (TurnManager.Instance.isPlayer1Turn && this == GameManager.Instance.player2)
        {
            return;
        }
        curMana += manaRegen;
        curMana = Math.Min(curMana, maxMana);
        manaSystemUI.UpdateManaText();
    }
    public virtual void TakeAction()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if(selectedFeature is HexUnit temp)
            {
                UpdateCurrentCell();
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

    void DoSelection()
    {
        HexGrid.Instance.ClearCellColor(Color.blue);
        HexGrid.Instance.ClearCellColor(Color.white);
        UpdateCurrentCell();
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
    void UpdateCurrentCell()
    {
        HexCell cell =
            HexGrid.Instance.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
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
    void showCards()
    {
        foreach (var card in deck)
        {

        }
    }
    void initializeBase()
    {

    }

    (bool, Feature) Player_WarriorCardCheck(int cost)
    {
        if (selectedFeature is not SpawnPoint || selectedFeature.location.unitFeature != null)
        {
            return (false, null);
        }
        if (selectedFeature.myPlayer != this)
        {
            return (false, null);
        }
        if (curMana - cost >= 0)
        {
            return (true, selectedFeature);
        }

        return (false, null);
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
