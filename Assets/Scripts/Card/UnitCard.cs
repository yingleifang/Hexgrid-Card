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

    public override void UseEffect(Player player)
    {
        if (player.selectedFeature is SpawnPoint temp)
        {
            HexUnit spawnedUnit = HexGrid.Instance.AddUnit(temp.Location, temp.Orientation, unitPrefab);
            baseWeapon.EquipWeapon(spawnedUnit);
            spawnedUnit.weaponInstance = baseWeapon.weaponPrefab;
            spawnedUnit.unitType = cardType;
        }
        else
        {
            Debug.LogError("Trying to spawn unit outside of spawnpoint");
        }
    }

    public override bool CardSpecificChecks(Player player)
    {
        if (player.selectedFeature is not SpawnPoint || player.selectedFeature.location.unitFeature != null)
        {
            return false;
        }
        if (player.selectedFeature.myPlayer != GameManager.Instance.currentPlayer)
        {
            return false;
        }
        return true;
    }
}