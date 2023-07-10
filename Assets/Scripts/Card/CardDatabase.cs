using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{

    [SerializeField] List<UnitCard> unitCardList = new();
    [SerializeField] List<WeaponCard> weaponCardList = new();
    [SerializeField] List<SpecialEffectCard> specialEffectCardList = new();

    public Card[] Draw()
    {
        return new Card[] { GetRandomUnitCard(), GetRandomWeaponCard(), GetRandomSpecialEffectCard() };
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
