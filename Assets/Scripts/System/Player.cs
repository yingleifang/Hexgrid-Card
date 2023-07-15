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
    [SerializeField]
    int energy = 10;
    [SerializeField]
    CardDatabase cardDatabase;
    [SerializeField]
    CardAreaManager cardArea;
    
    public Feature selectedFeature;

    public List<HexUnit> playerUnit; 

    // Start is called before the first frame update
    void Start()
    {
        foreach(var cardDisplay in cardArea.CardDisplays)
        {
            cardDisplay.CardUsed += Player_WarriorCardCheck;
        }
        initializeBase();
        deck.AddRange(cardDatabase.Draw());
        cardArea.FillSlots(deck);
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
        if (selectedFeature is not SpawnPoint)
        {
            return (false, null);
        }
        if (energy - cost >= 0)
        {
            energy -= cost;
            return (true, selectedFeature);
        }

        return (false, null);
    }
}
