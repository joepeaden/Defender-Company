using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Has references to the player.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static UnityEvent OnGameOver = new UnityEvent();

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public static bool isSlowMotion;
    // should be in SO probably
    public static float slowMotionSpeed = .5f;

    [SerializeField] private GameObject playerGO;
    private Player player;

    private AudioSource genericSoundPlayer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Game Manager, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        if (!playerGO)
        {
            Debug.LogWarning("Player not assigned, finding by tag.");
            playerGO = GameObject.FindGameObjectWithTag("Player");

            if (!playerGO)
            {
                Debug.LogWarning("Player not found by tag.");
            }
        }

        player = playerGO.GetComponent<Player>();
        player.OnPlayerDeath.AddListener(GameOver);

        InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true);
    }

    public void OnDestroy()
    {
        player.OnPlayerDeath.RemoveListener(GameOver);
    }

    /// <summary>
    /// Creates a new object with an audio source component to play the sound. One use of this is classes that aren't monobehaviours needing to play audio. 
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip)
    {
        if (genericSoundPlayer == null)
        {
            GameObject soundPlayerGO = Instantiate(new GameObject());
            soundPlayerGO.name = "GM Sound Player";
            genericSoundPlayer = soundPlayerGO.AddComponent<AudioSource>();
            genericSoundPlayer.volume = 1f;
        }
        
        genericSoundPlayer.clip = clip;
        genericSoundPlayer.Play();
        
    }

    public Player GetPlayerScript()
    {
        return player;
    }

    public GameObject GetPlayerGO()
    {
        return playerGO;
    }

    public void StartSlowMotion(float secondsToWait)
    {
        StartCoroutine(StartSlowMotionRoutine(secondsToWait));
    }

    private IEnumerator StartSlowMotionRoutine(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);

        isSlowMotion = true;
        Time.timeScale = slowMotionSpeed;

        yield return new WaitForSeconds(2.75f);

        isSlowMotion = false;
        Time.timeScale = 1f;
    }

    private void GameOver()
    {
        OnGameOver.Invoke();
        SceneLoader.Instance.LoadScene(SceneLoader.SceneList.FailMenu, true);
    }
}
