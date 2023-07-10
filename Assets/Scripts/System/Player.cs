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
    CardDatabase cardDatabase;
    [SerializeField]
    CardAreaManager cardArea;

    // Start is called before the first frame update
    void Start()
    {
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
}
