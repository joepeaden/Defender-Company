using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instance of loot for actor to pick up.
/// </summary>
public class Loot : Interactable, ISetActive
{
    [SerializeField] private InventoryItemDataStorage dataStorage;

    public InventoryItem item;
    // temporary, eventually will do a dropdown to select the loot type.
    public GearData.GearType gearType;

    private MeshRenderer rend;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        switch (gearType)
        {
            case GearData.GearType.PlasticExplosive:
                item = new ExplosiveEquipment(dataStorage.plasticExplosive);
                break;
            case GearData.GearType.Medkit:
                item = new MedkitEquipment(dataStorage.medkit);
                break;
            case GearData.GearType.AssaultRifle:
                item = new InventoryWeapon(dataStorage.assaultRifle);
                break;
            case GearData.GearType.SubMachineGun:
                item = new InventoryWeapon(dataStorage.subMachineGun);
                break;
            case GearData.GearType.SemiAutoRifle:
                item = new InventoryWeapon(dataStorage.semiAutoRifle);
                break;
            case GearData.GearType.Shotgun:
                item = new InventoryWeapon(dataStorage.shotgun);
                break;
            case GearData.GearType.Pistol:
                item = new InventoryWeapon(dataStorage.pistol);
                break;
            case GearData.GearType.Sabre:
                item = new InventoryWeapon(dataStorage.psaSabre);
                break;
        }

        item.gearType = gearType;

        MissionUI.Instance.AddObjectiveMarker(this.gameObject, "SUPPORT");
    }

    public override void Interact(Actor a)
    {
        base.Interact(a);

        a.PickupLoot(this);

        Destroy(gameObject);
    }

    void ISetActive.Activate()
    {
        rend.enabled = true;   
    }

    void ISetActive.DeActivate()
    {
        rend.enabled = false;
    }
}
