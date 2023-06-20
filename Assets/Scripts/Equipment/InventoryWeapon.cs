using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logical rep. of weapons for inventory.
/// </summary>
public class InventoryWeapon : InventoryItem
{
    public WeaponData data;
    /// <summary>
    /// Ammo count loaded in weapon.
    /// </summary>
    public int amountLoaded;

    public InventoryWeapon(WeaponData data)
    {
        this.data = data;

        amount = data.totalAmount;
        // start weapon off with full mag
        amountLoaded = data.ammoCapacity;
        amount -= amountLoaded;
    }
}
