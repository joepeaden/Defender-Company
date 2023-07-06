using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission
{
    public enum VictoryCondition
    {
        EliminateAllEnemies
    }

    public enum DefeatCondition
    {
        PlayerDeath
    }

    // should make Scriptable Objects.. or maybe a VictoryCondition class?
    public VictoryCondition victoryCondition;
    public DefeatCondition defeatCondition;

    // enemies in mission ?
    public float enemyCount;
}
