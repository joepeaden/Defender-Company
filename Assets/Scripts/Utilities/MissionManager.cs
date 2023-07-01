using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// "Manages" stuff related specifically to the Mission gameplay.
/// </summary>
public class MissionManager : MonoBehaviour
{
    /// <summary>
    /// Raised when mission end. Bool param false if player lost, true if player won.
    /// </summary>
    public static UnityEvent<bool> OnMissionEnd = new UnityEvent<bool>();
    public static UnityEvent OnMissionStart = new UnityEvent();

    private static MissionManager _instance;
    public static MissionManager Instance { get { return _instance; } }
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
            return;
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
        player.OnPlayerDeath.AddListener(HandlePlayerDeath);

        PlayerInput.InitializeControls();

        // notify anyone who cares - mission is a go!
        OnMissionStart.Invoke();

        // What is this here for again? Should have left a comment. I don't think it's necessary. Test some time.
        //InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true);
    }

    public void OnDestroy()
    {
        player.OnPlayerDeath.RemoveListener(HandlePlayerDeath);
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

    private void HandlePlayerDeath()
    {
        OnMissionEnd.Invoke(false);
    }
}
