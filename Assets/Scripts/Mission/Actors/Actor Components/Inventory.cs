using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    public static UnityEvent OnNewItemAddedPlayer = new UnityEvent();

    [SerializeField] InventoryItemDataStorage dataStorage;
    [SerializeField] WeaponInstance weaponInstance;
    // temporary. eventually replace with dropdown for StartingWeapon
    //[SerializeField] private bool hasRifle;
    //[SerializeField] private bool hasSMG;
    [SerializeField] private int startingAmmo;

    //[Header("Debug Options")]
    //[SerializeField] private bool hasAllWeapons;

    // wierd?
    public int weaponCount
    {
        get
        {
            return weapons != null ? weapons.Count() : 0;
        }
    }

    private Equipment equipment;
    private List<InventoryWeapon> weapons = new List<InventoryWeapon>();
    private int currentWeaponIndex;
    private Actor actor;

    private void Awake()
    {
        actor = GetComponent<Actor>();

        //if (hasAllWeapons)
        //{
        //    // This is the second place where these reward key strings are. I need to make these into an enumeration if there comes a third place or the list gets out of hand.
        //    // then I can just iterate a for loop of all the enum names as strings or maybe keep them in a hashmap with their associated data...
        //    InventoryWeapon inventoryWeapon;
        //    inventoryWeapon = new InventoryWeapon(dataStorage.assaultRifle);
        //    inventoryWeapon.rewardKey = "RIFLE";
        //    inventoryWeapon.amount = startingAmmo;
        //    AttemptAddItem(inventoryWeapon);
        //    inventoryWeapon = new InventoryWeapon(dataStorage.semiAutoRifle);
        //    inventoryWeapon.rewardKey = "SAR";
        //    inventoryWeapon.amount = startingAmmo;
        //    AttemptAddItem(inventoryWeapon);
        //    inventoryWeapon = new InventoryWeapon(dataStorage.shotgun);
        //    inventoryWeapon.rewardKey = "SHOTGUN";
        //    inventoryWeapon.amount = startingAmmo;
        //    AttemptAddItem(inventoryWeapon);
        //    inventoryWeapon = new InventoryWeapon(dataStorage.subMachineGun);
        //    inventoryWeapon.rewardKey = "SMG";
        //    inventoryWeapon.amount = startingAmmo;
        //    AttemptAddItem(inventoryWeapon);
        //    inventoryWeapon = new InventoryWeapon(dataStorage.pistol);
        //    inventoryWeapon.rewardKey = "PISTOL";
        //    inventoryWeapon.amount = startingAmmo;
        //    AttemptAddItem(inventoryWeapon);
        //}
        //else
        //{
    }

    public void SetWeaponFromData(WeaponData weaponData = null)
    {
        weaponData = weaponData != null ? weaponData : dataStorage.pistol;

        InventoryWeapon inventoryWeapon;

        inventoryWeapon = new InventoryWeapon(weaponData);
        inventoryWeapon.rewardKey = weaponData.rewardKey;

        //if (hasRifle)
        //{
        //    inventoryWeapon = new InventoryWeapon(dataStorage.assaultRifle);
        //    inventoryWeapon.rewardKey = "RIFLE";
        //}
        //if (hasSMG)
        //{
        //    inventoryWeapon = new InventoryWeapon(dataStorage.subMachineGun);
        //    inventoryWeapon.rewardKey = "SMG";
        //}
        //else
        //{
        //    inventoryWeapon = new InventoryWeapon(dataStorage.pistol);
        //    inventoryWeapon.rewardKey = "PISTOL";
        //}

        if (startingAmmo != 0)
        {
            inventoryWeapon.amount = startingAmmo;
        }
        AttemptAddItem(inventoryWeapon);
    }

    public bool AttemptAddItem(InventoryItem item)
    {
        bool result = true;
        try
        {
            // check if actor already has this weapon (if it even is a weapon)
            InventoryWeapon existingInventoryWeapon;
            existingInventoryWeapon = weapons.Where(x => x.data.rewardKey.Equals(item.rewardKey)).FirstOrDefault<InventoryWeapon>();

            InventoryItem itemToAddAmountTo = item;
            if (item as InventoryWeapon != null)
            {
                if (existingInventoryWeapon != null)
                {
                    itemToAddAmountTo = existingInventoryWeapon;
                }
                else
                {
                    weapons.Add((InventoryWeapon)item);
                    AttemptSwitchWeapons();

                }
            }
            else if (item as ExplosiveEquipment != null)
            {
                Equipment eqItem = (Equipment)item;
                equipment = eqItem;
                equipment.owningActor = actor;
            }
            else if (item as MedkitEquipment != null)
            {
                Equipment eqItem = (Equipment)item;
                equipment = eqItem;
                equipment.owningActor = actor;
            }

            // add charges to the item
            itemToAddAmountTo.AddAmount(item.amount);

            // process item based on type
            //switch (item.rewardKey)
            //{
            //    case "BOMB":
            //        if (equipment == null)
            //        {
            //            Equipment eqItem = (Equipment)item;
            //            equipment = eqItem;
            //            equipment.owningActor = actor;
            //        }
            //        break;
            //case "RIFLE":
            //    if (existingInventoryWeapon != null)
            //    {

            //    }
            //    else
            //    {
            //        weapons.Add((InventoryWeapon)item);
            //        AttemptSwitchWeapons();
            //    }
            //    break;
            //case "PISTOL":
            //    weapons.Add((InventoryWeapon)item);
            //    AttemptSwitchWeapons();
            //    break;
            //}

            if (actor.IsPlayer)
            {
                OnNewItemAddedPlayer.Invoke();
            }
        }
        catch
        {
            result = false;
        }

        return result;
    }

    public bool AttemptUseEquipment()
    {
        if (equipment != null)
        {
            return equipment.Use();
        }

        return false;
    }

    public bool AttemptUseWeapon(bool triggerPull)
    {
        if (weaponInstance != null)
        {
            return weaponInstance.InitiateAttack(actor.data.recoilControl, triggerPull);
        }

        return false;
    }

    public bool AttemptStartReload()
    {
        // non-players for now have no backup ammo limit.
        if (weaponInstance != null && weaponInstance.inventoryWeapon.amount > 0)
        {
            weaponInstance.StartReload();
        }

        return true;
    }

    public bool AttemptSwitchWeapons()
    {
        if (weaponInstance != null && weapons.Count >= 1)
        {
            currentWeaponIndex += 1;
            if (currentWeaponIndex >= weapons.Count)
            {
                currentWeaponIndex = 0;
            }

            weaponInstance.UpdateWeapon(weapons[currentWeaponIndex]);

            return true;
        }

        return false;
    }

    public int GetEquippedWeaponAmmo()
    {
        if (weaponInstance != null)
        {
            return weaponInstance.GetAmmo();
        }

        return 0;
    }

    public InventoryWeapon GetEquippedWeapon()
    {
        if (weaponInstance != null)
        {
            return weaponInstance.inventoryWeapon;
        }

        return null;
    }

    /// <summary>
    /// Here, equipment refers to the bombs, tools etc. Maybe Tools is a better description for this kind of stuff. Idk.
    /// </summary>
    /// <returns></returns>
    public Equipment GetEquipment()
    {
        if (equipment != null)
        {
            return equipment;
        }

        return null;
    }
}
