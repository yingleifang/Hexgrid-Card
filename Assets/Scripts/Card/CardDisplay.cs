using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Image portraitImage;
    public Image backGroundImage;

    public bool Occupied;

    public TextMeshProUGUI Description;

    public Card card;

    public event Func<int, (bool, Feature)> CardUsed;
    public void SetCardInfo(Card card)
    {
        this.card = card;
        cardNameText.text = card.name;
        costText.text = card.cost.ToString();
        backGroundImage.sprite = card.backGround;
        portraitImage.sprite = card.portrait;
        if (card is UnitCard temp)
        {
            Description.text = $"<sprite=\"sword\" index=0>{temp.attack} <sprite=\"arrow\" index=0>{temp.attackRange}" +
                $"\n <sprite=\"heart\" index=0>{temp.health} <sprite=\"Fast Boot\" index=0>{temp.Speed}";
        }
    }

    int moveSpeed = 300;
    public IEnumerator Reveal()
    {
        while (transform.localPosition.y <= -1)
        {
            transform.localPosition += moveSpeed * Time.deltaTime * new Vector3(0, 1, 0);
            yield return null;
        }
    }

    public void UseCard()
    {
        if (card is Warrior warrior)
        {
            var result = CardUsed?.Invoke(card.cost);
            if (result.HasValue)
            {
                if (result.Value.Item1)
                {
                    warrior.UseEffect(result.Value.Item2);
                }
                else
                {
                    Debug.Log("Card cannot be used");
                }
            }
            else
            {
                Debug.LogError("CardUsed Invoke returned Null");
            }
        }
    }
}
