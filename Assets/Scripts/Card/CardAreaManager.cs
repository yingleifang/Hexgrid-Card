using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CardAreaManager : MonoBehaviour
{
    [SerializeField]
    CardDatabase cardDatabase;

    public CardDisplay[] CardDisplays;

    [SerializeField] public Sprite[] cardBackGrounds;

    public void FillSlots()
    {
        foreach (var cardDisplay in CardDisplays)
        {
            if (cardDisplay.CardUsed)
            {
                var curCard = cardDatabase.DrawRandomCard();
                cardDisplay.SetCardInfo(curCard);
                curCard.backGround = cardBackGrounds[(int)curCard.cardType]; //Set card background based on type
            }
        }
        StartCoroutine(RevealAll(Enumerable.Range(0, 5).ToArray()));
    }

    IEnumerator RevealAll(int[] cardIndex)
    {
        GameUIHandler.Instance.disableCanvasGroupRayCast();
        foreach (int i in cardIndex)
        {
            StartCoroutine(CardDisplays[i].Reveal());
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.3f);
        GameUIHandler.Instance.enableCanvasGroupRayCast();
    }

    public void HideAllCards()
    {
        StartCoroutine(HideAll(Enumerable.Range(0, 5).ToArray()));
    }
    IEnumerator HideAll(int[] cardIndex)
    {
        GameUIHandler.Instance.disableCanvasGroupRayCast();
        foreach (int i in cardIndex)
        {
            StartCoroutine(CardDisplays[i].Hide());
        }
        yield return null;
    }
}
