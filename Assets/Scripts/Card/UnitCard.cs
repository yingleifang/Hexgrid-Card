using UnityEngine;
public class UnitCard : Card
{
    public int attack;
    public int attackRange;
    public int health;
    public int Speed;
    public WeaponCard baseWeapon;
    public HexUnit unitPrefab;

    public override void UseEffect(Player player)
    {
        HexGrid.Instance.AddUnit(player, id, this);
    }

    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is not SpawnPoint || player.selectedFeature.location.unitFeature != null)
        {
            return false;
        }
        if (player.selectedFeature.myPlayer == GameManagerClient.Instance.corresPlayer)
        {
            return false;
        }
        return true;
    }
}