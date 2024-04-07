using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanySoldier
{
    public enum SoldierBackgrounds
    {
        Mercenary,
        Prisoner,
        Laborer
    }

    public string Name => name;
    private string name;
    public int HitPoints => hitPoints;
    private int hitPoints;
    public float MoveSpeed => moveSpeed;
    private float moveSpeed;
    public int AccuracyRating => accuracyRating;
    private int accuracyRating;
    public int HireCost => hireCost;
    private int hireCost;
    public WeaponData CurrentWeapon { get; set; }
    public int Level => level;
    private int level = 1;
    public int XP => xp;
    private int xp;
    public string ID => id;
    private string id;
    public SoldierBackgroundData BackgroundData => backgroundData;
    private SoldierBackgroundData backgroundData;
    public Sprite Face => face;
    private Sprite face;

    public bool isAlive = true;

    // mission specific
    public int MissionKills
    {
        get
        {
            xp += 1;
            return missionKills;
        }

        set
        {
            missionKills = value;
        }
    }
    private int missionKills;
    public int MissionHP { get; set; }

    public CompanySoldier(SoldierBackgroundData newData, Sprite newFace)
    {
        backgroundData = newData;

        hitPoints = UnityEngine.Random.Range(backgroundData.minHP, backgroundData.maxHP);
        moveSpeed = UnityEngine.Random.Range(backgroundData.minSpeed, backgroundData.maxSpeed);
        accuracyRating = UnityEngine.Random.Range(backgroundData.minAcc, backgroundData.maxAcc);
        CurrentWeapon = GameManager.Instance.GetDataStore().assaultRifle;
        hireCost = (int)(hitPoints + (moveSpeed * 60) + (accuracyRating * 100) + CurrentWeapon.cost);
        id = Guid.NewGuid().ToString();
        face = newFace;
        name = GetRandomName();
    }

    public void ResetMissionVariables()
    {
        // doesn't really need to reset hp cause it's set explicitly, but I think it's best to reset it. I'm just putting it to max value so it's very obvious it's at a reset value.
        MissionHP = int.MaxValue;
        MissionKills = 0;
    }

    public void LevelUp()
    {
        level++;

        hitPoints += UnityEngine.Random.Range(10, 30);
        moveSpeed += UnityEngine.Random.Range(.25f, 1);
        accuracyRating += UnityEngine.Random.Range(1, 2);
    }

    // this should be in the customizer. And I wonder if the customizer ref should be in here actually. perhaps a static reference.
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
        int randomIndex = UnityEngine.Random.Range(0, nameOptions.Length - 1);
        return nameOptions[randomIndex];
    }
}
