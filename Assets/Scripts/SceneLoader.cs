using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static UnityEvent OnSceneLoadComplete = new UnityEvent();

    public static SceneLoader Instance { get { return _instance; } }
    private static SceneLoader _instance;

    public enum SceneName
    {
        MainMenu,
        Mission,
        TutorialMission,
        Map
    }

    private void Awake()
    {
        // singleton stuff
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one SceneLoader, deleting one.");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;

        ChangeScene(SceneName.Map, null, true);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Load or unload scene; additive or not.
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="sceneToUnload">Pass null if you don't wanna unload nuthin</param>
    /// <param name="additive"></param>
    public void ChangeScene(SceneName? sceneToLoad, SceneName? sceneToUnload, bool additive)
    {
        if (sceneToUnload.HasValue)
        {
            SceneManager.UnloadSceneAsync(sceneToUnload.Value.ToString());
        }
        else if (sceneToLoad.HasValue)
        {
            SceneManager.LoadScene(sceneToLoad.Value.ToString(), additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("No scene to load or unload");
        }
    }

    /// <summary>
    /// Just sets the active scene.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
        OnSceneLoadComplete.Invoke();
    }
}
