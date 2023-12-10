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
    public int completionReward;
    public int enemyCount;
    public VictoryCondition victoryCondition;
    public List<ControllerData> includedEnemyTypes;
}

