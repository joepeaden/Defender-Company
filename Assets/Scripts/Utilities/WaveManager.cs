using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    // ya know. I still haven't really thought through when a Singleton is the right move vs. a static class. Or other methods.
    // I'll need to give that some thought. Obviously there are a lot of design inconsistencies in this project. \_(o_o)_/ Deal with it.
    //                                                                                                                ^ that's my attempt at a shrug
    public static WaveManager Instance { get { return _instance; } }
    private static WaveManager _instance;
    public static UnityEvent OnWaveEnd = new UnityEvent();
    public static UnityEvent OnPrepForNextWave = new UnityEvent();
    public static int totalEnemiesAlive;

    public int initialWaveNum;
    [SerializeField]
    private WaveData waveData;
    private bool shouldStartNextWave;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("More than one WaveManager instance! Deleting this one.");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        totalEnemiesAlive = 0;
        GameplayUI.OnRewardsPicked.AddListener(HandleRewardsPicked);

        EnemySpawner.waveNumber = initialWaveNum;

        // prep first wave
        StartCoroutine(BeginFirstWave());
    }

    /// <summary>
    /// Handle rewards picked event.
    /// </summary>
    /// <param name="rewardKey">Thank you, I don't use this param, goodbye.</param>
    private void HandleRewardsPicked(ShopItem item)
    {
        shouldStartNextWave = true;
    }

    private IEnumerator BeginFirstWave()
    {
        yield return new WaitForSeconds(waveData.firstWaveDelay);
        shouldStartNextWave = true;
        StartCoroutine(WaitForWaveDelay());
    }

    private IEnumerator WaitForWaveDelay()
    {
        while (true)
        {
            yield return new WaitUntil(() => shouldStartNextWave);
            shouldStartNextWave = false;

            // launch prep events
            OnPrepForNextWave.Invoke();

            // start next wave
            EnemySpawner.NextWave();

            // wait until something is spawned
            yield return new WaitUntil(() => totalEnemiesAlive > 0);

            // wait until all enemies die before continuing loop
            while (totalEnemiesAlive > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            OnWaveEnd.Invoke();
        }
    }

    public WaveData GetWaveData()
    {
        return waveData;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        GameplayUI.OnRewardsPicked.RemoveListener(HandleRewardsPicked);
    }
}
