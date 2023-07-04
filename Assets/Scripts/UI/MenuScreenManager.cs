using UnityEngine;

public class MenuScreenManager : MonoBehaviour
{
    private static MenuScreenManager _instance;
    public static MenuScreenManager Instance { get { return _instance; } }

    [SerializeField] private GameObject startMenuGO;
    [SerializeField] private GameObject mainMenuGO;
    [SerializeField] private GameObject missionSelectGO;
    [SerializeField] private GameObject marketGO;
    [SerializeField] private GameObject aarGo;

    private GameObject currentlyLoadedScreen;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Gameplay UI, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        currentlyLoadedScreen = startMenuGO;
    }

    /// <summary>
    /// Display After Action Report screen
    /// </summary>
    public void DisplayAAR()
    {
        LoadScreen(aarGo);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayMainMenu()
    {
        LoadScreen(mainMenuGO);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayStartMenu()
    {
        LoadScreen(startMenuGO);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayMissionSelect()
    {
        LoadScreen(missionSelectGO);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
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
}
