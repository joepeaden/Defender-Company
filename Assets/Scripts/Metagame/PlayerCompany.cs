using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the player in metagame terms - i.e. what does the player's company own, who are their troops, etc.
/// </summary>
public class PlayerCompany
{
    public int PlayerCash => playerCash;
    private int playerCash = 30000;

    private Dictionary<GearData.GearType, GearData> ownedGear = new Dictionary<GearData.GearType, GearData>();

    public Dictionary<string, CompanySoldier> Soldiers => soldiers;
    private Dictionary<string, CompanySoldier> soldiers = new Dictionary<string, CompanySoldier>();
    public HashSet<string> DeployedSoldiers = new HashSet<string>();

    // eventually remove the dataStore param when we add addressables. Or whatever. Just clean this up at some point.
    public PlayerCompany(InventoryItemDataStorage dataStore)
    {
        ownedGear.Add(GearData.GearType.Pistol, dataStore.pistol);

        AssignStartTroops();
    }

    /// <summary>
    /// Generate start troops
    /// </summary>
    /// <returns></returns>
    private void AssignStartTroops()
    {
        for (int i = 0; i < 4; i++)
        {
            int hitPoints = Random.Range(50, 150);
            float speed = Random.Range(2, 7);
            int accuracyRating = Random.Range(0, 5);
            WeaponData weapon = GameManager.Instance.GetDataStore().pistol;
            int cost = (int)(hitPoints + (speed * 60) + (accuracyRating * 100) + weapon.cost);

            CompanySoldier newSoldier = new CompanySoldier(GetRandomName(), hitPoints, speed, accuracyRating, weapon, cost);
            soldiers.Add(newSoldier.ID, newSoldier);
        }
    }

    private string GetRandomName()
    {
        string[] nameOptions =
        {
            "Rourke",
            "Niels",
            "Smith",
            "Danson",
            "Peters",
            "Wang",
            "O'Malley",
            "Bauer",
            "Rochefort",
            "Dumas",
            "Garcia",
            "Vargas",
            "Anderson",
            "Thomas",
            "Brown",
            "Grey",
            "Benson",
            "Al-Hilli",
            "Cohen",
            "Rosenberg",
            "Goldstein"
        };
        int randomIndex = Random.Range(0, nameOptions.Length - 1);
        return nameOptions[randomIndex];
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

    public void AddRecruit(CompanySoldier recruit)
    {
        soldiers.Add(recruit.ID, recruit);
    }

    /// <summary>
    /// Update XP and death state after a mission
    /// </summary>
    public void UpdateDeployedSoldiersStats()
    {
        foreach (string id in DeployedSoldiers)
        {
            CompanySoldier soldier = soldiers[id];

            if (soldier.MissionHP <= 0)
            {
                soldier.isAlive = false;
                continue;
            }

            if (soldier.XP >= soldier.Level * 10)
            {
                soldier.LevelUp();
            }
        }
    }

    public List<CompanySoldier> GetLivingSoldiersAsList()
    {
        List<CompanySoldier> soldierList = new List<CompanySoldier>();
        foreach (KeyValuePair<string, CompanySoldier> kvp in soldiers)
        {
            if (kvp.Value.isAlive)
            {
                soldierList.Add(kvp.Value);
            }
        }

        return soldierList;
    }

    public List<CompanySoldier> GetDeployedSoldiersAsList()
    {
        List<CompanySoldier> soldierList = new List<CompanySoldier>();
        foreach (string id in DeployedSoldiers)
        {
            if (soldiers.ContainsKey(id))
            {
                soldierList.Add(soldiers[id]);
            }
        }

        return soldierList;
    }

    public void RemoveDeadSoldiers()
    {
        // can't remove items in a foreach and idk how else to iterate this type of collection so whatever.
        List<string> soldiersToRemove = new List<string>();
        foreach (KeyValuePair<string, CompanySoldier> kvp in soldiers)
        {
            CompanySoldier soldier = kvp.Value;
            if (!soldier.isAlive)
            {
                soldiersToRemove.Add(kvp.Key);
            }
        }

        for (int i = 0; i < soldiersToRemove.Count; i++)
        {
            soldiers.Remove(soldiersToRemove[i]);
            DeployedSoldiers.Remove(soldiersToRemove[i]);
        }
    }
}
