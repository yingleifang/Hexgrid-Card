using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{

    [SerializeField] List<UnitCard> unitCardList = new();
    [SerializeField] List<WeaponCard> weaponCardList = new();
    [SerializeField] List<SpecialEffectCard> specialEffectCardList = new();

    public Card DrawRandomCard()
    {
        int cardType = Random.Range(0, 3);
        if (cardType == 0)
        {
            return GetRandomUnitCard();
        }else if (cardType == 1)
        {
            return GetRandomWeaponCard();
        }
        else
        {
            return GetRandomSpecialEffectCard();
        }
    }
    public Card GetRandomUnitCard()
    {
        return unitCardList[Random.Range(0, unitCardList.Count)];
    }

    public Card GetRandomWeaponCard()
    {
        return weaponCardList[Random.Range(0, weaponCardList.Count)];
    }

    public Card GetRandomSpecialEffectCard()
    {
        return specialEffectCardList[Random.Range(0, specialEffectCardList.Count)];
    }
}
