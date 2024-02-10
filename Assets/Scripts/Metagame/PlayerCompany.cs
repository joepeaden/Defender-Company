using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the player in metagame terms - i.e. what does the player's company own, who are their troops, etc.
/// </summary>
public class PlayerCompany
{
    public int PlayerCash => playerCash;
    private int playerCash = 300;

    private Dictionary<GearData.GearType, GearData> ownedGear = new Dictionary<GearData.GearType, GearData>();

    public Dictionary<string, CompanySoldier> Soldiers => soldiers;
    private Dictionary<string, CompanySoldier> soldiers = new Dictionary<string, CompanySoldier>();
    public HashSet<string> DeployedSoldiers = new HashSet<string>();

    private CharacterCustomization customizer = new CharacterCustomization();

    // eventually remove the dataStore param when we add addressables. Or whatever. Just clean this up at some point.
    public PlayerCompany(InventoryItemDataStorage dataStore)
    {
        ownedGear.Add(GearData.GearType.Pistol, dataStore.pistol);
        customizer.OnCustomizationInitialized.AddListener(AssignStartTroops);
    }

    /// <summary>
    /// Generate start troops
    /// </summary>
    /// <returns></returns>
    private void AssignStartTroops()
    {
        List<CompanySoldier> recruits = GetNewRecruits(3);

        for (int i = 0; i < recruits.Count; i++)
        {
            soldiers.Add(recruits[i].ID, recruits[i]);
        }
    }

    public List<CompanySoldier> GetNewRecruits(int num, List<CompanySoldier.SoldierBackgrounds> recruitBackgrounds = null)
    {
        List<SoldierBackgroundData> allowedBackgrounds = new List<SoldierBackgroundData>();
        InventoryItemDataStorage dataStore = GameManager.Instance.GetDataStore();
        if (recruitBackgrounds == null)
        {
            allowedBackgrounds.Add(dataStore.mercenary);
            allowedBackgrounds.Add(dataStore.laborer);
            allowedBackgrounds.Add(dataStore.prisoner);
        }
        else
        {
            foreach (CompanySoldier.SoldierBackgrounds background in recruitBackgrounds)
            {
                switch (background)
                {
                    case CompanySoldier.SoldierBackgrounds.Laborer:
                        allowedBackgrounds.Add(GameManager.Instance.GetDataStore().laborer);
                        break;
                    case CompanySoldier.SoldierBackgrounds.Mercenary:
                        allowedBackgrounds.Add(GameManager.Instance.GetDataStore().mercenary);
                        break;
                    case CompanySoldier.SoldierBackgrounds.Prisoner:
                        allowedBackgrounds.Add(GameManager.Instance.GetDataStore().prisoner);
                        break;
                }
            }
        }

        List<CompanySoldier> recruits = new List<CompanySoldier>();
        for (int i = 0; i < num; i++)
        {
            SoldierBackgroundData data = allowedBackgrounds[Random.Range(0, allowedBackgrounds.Count)];
            Sprite face = customizer.GetRandomFace();
            CompanySoldier recruit = new CompanySoldier(data, face);
            recruits.Add(recruit);
        }

        return recruits;
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
