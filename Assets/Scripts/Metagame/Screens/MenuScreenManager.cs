using UnityEngine;

public class MenuScreenManager : MonoBehaviour
{
    private static MenuScreenManager _instance;
    public static MenuScreenManager Instance { get { return _instance; } }

    [SerializeField] private GameObject startMenuGO;
    [SerializeField] private GameObject mainMenuGO;
    [SerializeField] private GameObject missionSelectGO;
    [SerializeField] private GameObject marketGO;
    [SerializeField] private GameObject recruitmentGO;
    [SerializeField] private GameObject troopsGO;
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
        ToggleScreen(aarGo);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayMainMenu()
    {
        ToggleScreen(mainMenuGO);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayStartMenu()
    {
        ToggleScreen(startMenuGO);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayMissionSelect()
    {
        ToggleScreen(missionSelectGO);
    }

    /// <remarks>
    /// This is set on some buttons in the inspector!
    /// </remarks>
    public void DisplayMarket()
    {
        ToggleScreen(marketGO);
    }

    /// <summary>
    /// Set in the inspector
    /// </summary>
    public void DisplayRecruitment()
    {
        ToggleScreen(recruitmentGO);
    }

    /// <summary>
    /// Set in the inspector
    /// </summary>
    public void DisplayTroops()
    {
        ToggleScreen(troopsGO);
    }

    /// <summary>
    /// Enable the screenToOpen, disable currentlyLoadedScreen, and set new currentlyLoadedScreen
    /// </summary>
    /// <remarks>
    /// Called through the editor at least in the main menu
    /// </remarks>
    /// <param name="screenToOpen"></param>
    public void ToggleScreen(GameObject screenToOpen)
    {
        screenToOpen.SetActive(true);
        currentlyLoadedScreen.SetActive(false);
        currentlyLoadedScreen = screenToOpen;
    }
}
