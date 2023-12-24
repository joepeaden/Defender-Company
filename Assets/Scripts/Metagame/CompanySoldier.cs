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

    public CompanySoldier(string newName, int newHitPoints, float newMoveSpeed, int newAccuracyRating, int newHireCost)
    {
        name = newName;
        hitPoints = newHitPoints;
        moveSpeed = newMoveSpeed;
        accuracyRating = newAccuracyRating;
        hireCost = newHireCost;
    }
}
