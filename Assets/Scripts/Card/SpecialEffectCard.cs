using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectCard : Card
{
    public string cardDescription;

    void Reset()
    {
        cardType = CardType.SpecialEffect;
    }

}