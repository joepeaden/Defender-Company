using UnityEngine;

/// <summary>
/// Just ease of access for getting SOs. Could probably look more into addressables and fix this
/// </summary>
public class InventoryItemDataStorage : MonoBehaviour
{
    public MedkitData medkit;
    public ExplosiveData plasticExplosive;
    public WeaponData assaultRifle;
    public WeaponData subMachineGun;
    public WeaponData semiAutoRifle;
    public WeaponData shotgun;
    public WeaponData pistol;
    public WeaponData psaSabre;

    public SoldierBackgroundData mercenary;
    public SoldierBackgroundData laborer;
    public SoldierBackgroundData prisoner;
}
