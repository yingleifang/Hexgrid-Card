using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CardAreaManager : MonoBehaviour
{

    public CardDisplay[] CardDisplays;
    int freeSlot = 0;

    [SerializeField] public Sprite[] cardBackGrounds;
    Player player;

    public void FillSlots(List<Card> cards)
    {
        foreach (var card in cards)
        {
            card.backGround = cardBackGrounds[(int)card.cardType];
            CardDisplays[freeSlot].SetCardInfo(card);
            freeSlot += 1;
        }
        StartCoroutine(RevealAll(Enumerable.Range(0, freeSlot).ToArray()));
    }

    IEnumerator RevealAll(int[] cardIndex)
    {
        foreach (int i in cardIndex)
        {
            StartCoroutine(CardDisplays[i].Reveal());
            yield return new WaitForSeconds(0.5f);
        }
    }
}
