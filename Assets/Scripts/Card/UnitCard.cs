using UnityEngine;

public class UnitCard : Card
{
    public int attack;
    public int attackRange;
    public int health;
    public int Speed;
    public WeaponCard baseWeapon;
    [SerializeField]
    protected HexUnit unitPrefab;
}