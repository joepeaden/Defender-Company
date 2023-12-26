using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanySoldier
{
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

    public CompanySoldier(string newName, int newHitPoints, float newMoveSpeed, int newAccuracyRating, WeaponData newCurrentWeapon, int newHireCost)
    {
        name = newName;
        hitPoints = newHitPoints;
        moveSpeed = newMoveSpeed;
        accuracyRating = newAccuracyRating;
        hireCost = newHireCost;
        CurrentWeapon = newCurrentWeapon;
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

        hitPoints += Random.Range(10, 30);
        moveSpeed += Random.Range(.25f, 1);
        accuracyRating += Random.Range(1, 2);
    }
}
