using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the player in metagame terms - i.e. what does the player's company own, who are their troops, etc.
/// </summary>
public class PlayerCompany
{
    public int PlayerCash => playerCash;
    private int playerCash;

    private Dictionary<GearData.GearType, GearData> ownedGear = new Dictionary<GearData.GearType, GearData>();

    // eventually remove the dataStore param when we add addressables. Or whatever. Just clean this up at some point.
    public PlayerCompany(InventoryItemDataStorage dataStore)
    {
        ownedGear.Add(GearData.GearType.Pistol, dataStore.pistol);
    }

    public void AddCash(int amount)
    {
        playerCash += amount;
    }

    public void AddOwnedGear(GearData gear)
    {
        ownedGear.Add(gear.gearType, gear);
    }

    public Dictionary<GearData.GearType, GearData> GetOwnedGear()
    {
        return ownedGear;
    }
}
