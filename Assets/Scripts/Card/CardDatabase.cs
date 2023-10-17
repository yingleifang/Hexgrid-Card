using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }

    public List<UnitCard> unitCardList = new();
    public List<WeaponCard> weaponCardList = new();
    public List<SpecialEffectCard> specialEffectCardList = new();
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CardDatabase!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < unitCardList.Count; i++)
        {
            unitCardList[i].id = i;
        }
    }
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
