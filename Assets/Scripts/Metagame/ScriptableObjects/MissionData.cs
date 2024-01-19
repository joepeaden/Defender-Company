using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(fileName = "MissionData", menuName = "Menu/MissionData")]
public class MissionData : ScriptableObject
{
    public enum VictoryCondition
    {
        EliminateAllEnemies
    }

    public enum DefeatCondition
    {
        PlayerDeath
    }

    public string missionTitle;
    public string missionDifficulty;
    public int completionReward;
    public int enemyCount;
    /// <summary>
    /// Percentage chance there will be an attack any given turn
    /// </summary>
    public float perTurnAttackChance;
    public int numberOfTurns;

    // Victory Condition might not be useful anymore.
    public VictoryCondition victoryCondition;

    public List<ControllerData> includedEnemyTypes;
}

