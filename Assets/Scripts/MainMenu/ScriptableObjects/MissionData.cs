using UnityEngine;

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
}

