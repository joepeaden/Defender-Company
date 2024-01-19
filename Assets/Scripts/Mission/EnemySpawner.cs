using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Each instance of this handles spawning of enemies.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public static bool shouldSpawn;
    public static int totalEnemiesSpawned;

    public Transform enemiesParent;
    [SerializeField]
    private bool canSpawnOnScreen;
    [SerializeField] GameObject actorPrefab;

    /// <summary>
    /// The current list of enemy prefabs to be chosen from (based on current wave)
    /// </summary>
    private static List<ControllerData> spawnableEnemyTypes = new List<ControllerData>();

    public WaveData data;

    public void Awake()
    { 
        MissionManager.OnMissionEnd.AddListener(Reset);
        MissionManager.OnAttackStart.AddListener(StartSpawningCoroutine);

        // for reloading the scene
        spawnableEnemyTypes.Clear();

    }

    private void OnDestroy()
    {
        MissionManager.OnMissionEnd.RemoveListener(Reset);
        MissionManager.OnAttackStart.RemoveListener(StartSpawningCoroutine);
    }

    /// <summary>
    /// Reset at game over
    /// </summary>
    private void Reset(bool playerWon)
    {
        totalEnemiesSpawned = 0;
    }

    private void StartSpawningCoroutine()
    {
        totalEnemiesSpawned = 0;
        if (spawnableEnemyTypes.Count == 0)
        {
            foreach (ControllerData enemyPrefab in GameManager.Instance.CurrentMission.includedEnemyTypes)
            {
                spawnableEnemyTypes.Add(enemyPrefab);
            }
        }

        shouldSpawn = true;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies() 
    {
        while (true)
        {
            float waitTime = Random.Range(data.minSpawnTime, data.maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            int randomEnemyIndex = Random.Range(0, spawnableEnemyTypes.Count);

            // here because it's after the waitforseconds, so should be no accidental spawns after over the limit
            if (totalEnemiesSpawned >= MissionManager.CurrentMisison.enemyCount)
            {
                shouldSpawn = false;
                break;
            }

            // Don't spawn while on screen
            bool onScreen = false;
            do
            {
                // the +10f is just so they spawn OFF screen, not at the edge of the screen.
                onScreen = Camera.main.WorldToScreenPoint(transform.position).x < Screen.width + 10f &&
                Camera.main.WorldToScreenPoint(transform.position).x > 0 &&
                Camera.main.WorldToScreenPoint(transform.position).y < Screen.height + 10f &&
                Camera.main.WorldToScreenPoint(transform.position).y > 0;
                if (onScreen)
                {
                    yield return null;
                }
            } while (onScreen && !canSpawnOnScreen);

            // while waiting other spawners may have already spit out some and may want to not spawn now.
            if (shouldSpawn)
            {
                GameObject newEnemy = Instantiate(actorPrefab, transform.position, Quaternion.identity, enemiesParent);
                newEnemy.GetComponentInChildren<Actor>().GetComponentInChildren<ActorController>().SetControllerData(spawnableEnemyTypes[randomEnemyIndex]);
                newEnemy.name = "Enemy " + ++totalEnemiesSpawned; 

                if (totalEnemiesSpawned >= MissionManager.CurrentMisison.enemyCount)
                {
                    shouldSpawn = false;
                }

            }

            yield return new WaitForSeconds(1f);
        }
    }
}
