using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    List<Card> deck;
    int draws = 3;
    int maxCards = 7;
    int maxUnit = 5;
    public int curMana = 10;
    public int maxMana = 10;
    [SerializeField]
    CardDatabase cardDatabase;
    [SerializeField]
    CardAreaManager cardArea;
    [SerializeField]
    ManaSystemUI manaSystemUI;
    public Feature selectedFeature;

    public List<HexUnit> playerUnit; 

    // Start is called before the first frame update
    void Start()
    {
        curMana = maxMana;
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
        curMana = maxMana;
        manaSystemUI.UpdateManaText();
    }

    // Update is called once per frame
    void Update()
    {

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
}
