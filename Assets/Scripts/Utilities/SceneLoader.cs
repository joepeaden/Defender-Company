using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get { return _instance; } }
    private static SceneLoader _instance;

    public enum SceneName
    {
        MainMenu,
        Survival
    }

    [SerializeField] private GameObject startMenuGO;
    [SerializeField] private GameObject mainMenuGO;
    [SerializeField] private GameObject missionSelectGO;
    [SerializeField] private GameObject marketGO;

    private GameObject currentlyLoadedScreen;

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

        currentlyLoadedScreen = startMenuGO;
    }

    public void DisplayMainMenu()
    {
        LoadScreen(mainMenuGO);
    }

    public void DisplayStartMenu()
    {
        LoadScreen(startMenuGO);
    }

    public void DisplayMissionSelect()
    {
        LoadScreen(missionSelectGO);
    }

    public void DisplayMarket()
    {
        LoadScreen(marketGO);
    }

    /// <summary>
    /// Enable the screenToOpen, disable currentlyLoadedScreen, and set new currentlyLoadedScreen
    /// </summary>
    /// <remarks>
    /// Called through the editor at least in the main menu
    /// </remarks>
    /// <param name="screenToOpen"></param>
    public void LoadScreen(GameObject screenToOpen)
    {
        screenToOpen.SetActive(true);
        currentlyLoadedScreen.SetActive(false);
        currentlyLoadedScreen = screenToOpen;
    }

    /// <summary>
    /// Load scene; additive or not.
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <param name="additive"></param>
    public void LoadScene(SceneName sceneToLoad, bool additive)
    {
        SceneManager.LoadScene(sceneToLoad.ToString(), additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
}
