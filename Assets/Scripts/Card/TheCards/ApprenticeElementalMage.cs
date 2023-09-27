using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Card;

[CreateAssetMenu(fileName = "ApprenticeElementalMage", menuName = "ApprenticeElementalMage")]

public class ApprenticeElementalMage : UnitCard
{
    void Reset()
    {
        cardType = CardType.ElementalMage;
    }

}
