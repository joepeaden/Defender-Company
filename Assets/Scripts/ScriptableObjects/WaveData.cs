using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "MyScriptables/WaveData")]
public class WaveData : ScriptableObject
{
    public int enemyPistolIntroWave;
    public int enemyRifleIntroWave;
    public int enemySMGIntroWave;
    public int enemyBreacherIntroWave;
    public int enemyMarksmanIntroWave;

    // revise to only have enemyPrefab, then use specific scriptable objects to customize to type
    // yeah yeah, whatever.
    public GameObject enemyPistolPrefab;
    public GameObject enemyRiflePrefab;
    public GameObject enemySMGPrefab;
    public GameObject enemyBreacherPrefab;
    public GameObject enemyMarksmanPrefab;

    public int baseEnemyCount;
    public int maxEnemyCount;

    public float wavePopulationMultiplier;

    public float minSpawnTime;
    public float maxSpawnTime;
    public float waveDelay;
    public float firstWaveDelay;

}

