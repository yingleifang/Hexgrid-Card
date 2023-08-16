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

    //public struct UseEffectArgs
    //{
    //    public Feature feature;

    //    public UseEffectArgs(Feature feature = null)
    //    {
    //        this.feature = feature;
    //    }
    //}
    //public struct CardCheckstArgs
    //{
    //    public Feature feature;

    //    public CardCheckstArgs(Feature feature = null)
    //    {
    //        this.feature = feature;
    //    }
    //}
    public virtual void UseEffect(Player player)
    {
        Debug.Log("Card Used");
    }

    public virtual bool CardSpecificChecks(Player player)
    {
        return true;
    }

    //public virtual UseEffectArgs SetupUseEffectArgs(Player player)
    //{
    //    return new UseEffectArgs();
    //}

    //public virtual CardCheckstArgs SetupCardChecksArgs(Player player)
    //{
    //    return new CardCheckstArgs();
    //}
}
