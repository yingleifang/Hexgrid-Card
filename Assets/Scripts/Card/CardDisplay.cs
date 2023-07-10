using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Image portraitImage;
    public Image backGroundImage;

    public bool Occupied;

    public TextMeshProUGUI Description;

    public delegate void UseEffect();

    public UseEffect useEffect;
    public void SetCardInfo(Card card)
    {
        cardNameText.text = card.name;
        costText.text = card.cost.ToString();
        backGroundImage.sprite = card.backGround;
        portraitImage.sprite = card.portrait;
        useEffect = card.UseEffect;
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

}
