using UnityEngine;

public class Card : ScriptableObject
{
    public enum CardType
    {
        ElementalMage,
        MeleeSoldier,
        RangedSoldier,
        SpiritualMage,
        ElementalSpell,
        MeleeWeapon,
        RangedWeapon,
        SpiritualSpell,
        SpecialEffect
    }

    public int id;
    public string cardName;
    public int cost;
    public CardType cardType;
    public Sprite backGround;
    public Sprite portrait;
    public virtual void UseEffect()
    {
        Debug.Log("Card Used");
    }
}
