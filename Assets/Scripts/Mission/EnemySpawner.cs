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

    /// <summary>
    /// The current list of enemy prefabs to be chosen from (based on current wave)
    /// </summary>
    private static List<GameObject> spawnableEnemyPrefabs = new List<GameObject>();

    public WaveData data;

    public void Awake()
    {
        MissionManager.OnMissionEnd.AddListener(Reset);
        MissionManager.OnMissionStart.AddListener(StartSpawningCoroutine);

        // for reloading the scene
        spawnableEnemyPrefabs.Clear();
        spawnableEnemyPrefabs.Add(data.enemyPistolPrefab);
    }

    private void OnDestroy()
    {
        MissionManager.OnMissionEnd.RemoveListener(Reset);
        MissionManager.OnMissionStart.RemoveListener(StartSpawningCoroutine);
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
        shouldSpawn = true;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies() 
    {
        while (true)
        {
            float waitTime = Random.Range(data.minSpawnTime, data.maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            int randomEnemyIndex = Random.Range(0, spawnableEnemyPrefabs.Count);

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
                Instantiate(spawnableEnemyPrefabs[randomEnemyIndex], transform.position, Quaternion.identity, enemiesParent);

                totalEnemiesSpawned++;
                if (totalEnemiesSpawned >= MissionManager.CurrentMisison.enemyCount)
                {
                    shouldSpawn = false;
                }

            }

            yield return new WaitForSeconds(1f);
        }
    }
}
