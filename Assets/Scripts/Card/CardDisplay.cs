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

    public TextMeshProUGUI Description;

    public Card card;

    public event Func<int, (bool, Player)> UseCardChecks;

    public event Action<int> OnCardUsed;

    public bool CardUsed = true;

    [SerializeField] CanvasGroup CardInfo;

    float dissolveAmount = 1;

    [SerializeField] Material material;

    Material myMaterial;

    private void Awake()
    {
        myMaterial = new Material(material);
        portraitImage.material = myMaterial;
        backGroundImage.material = myMaterial;
        CardUsed = true;
    }

    public void SetCardInfo(Card card)
    {
        RevertDissolving();
        CardUsed = false;
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

    public IEnumerator Hide()
    {
        while (transform.localPosition.y >= -205)
        {
            transform.localPosition -= moveSpeed * Time.deltaTime * new Vector3(0, 1, 0);
            yield return null;
        }
    }

    public void UseCard()
    {
        if (CardUsed)
        {
            return;
        }

            var result = UseCardChecks?.Invoke(card.cost);
            if (result.HasValue)
            {
                if (result.Value.Item1) //Card can be Used
                {
                if (!card.CardSpecificChecks(result.Value.Item2))
                {
                    return;
                }
                    CardUsed = true;
                    OnCardUsed?.Invoke(card.cost);
                    StartCoroutine(StartDissolving());
                    StartCoroutine(StartFading());
                    card.UseEffect(result.Value.Item2);
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

    IEnumerator StartDissolving()
    {
        while (dissolveAmount > 0)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
            portraitImage.material.SetFloat("_DissolveAmount", dissolveAmount);
            backGroundImage.material.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }
        yield return null;
    }

    public void RevertDissolving()
    {
        dissolveAmount = 1;
        portraitImage.material.SetFloat("_DissolveAmount", dissolveAmount);
        backGroundImage.material.SetFloat("_DissolveAmount", dissolveAmount);
        CardInfo.alpha = 1;
    }

    IEnumerator StartFading()
    {
        while (CardInfo.alpha > 0)
        {
            CardInfo.alpha = Mathf.Clamp01(CardInfo.alpha - Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
