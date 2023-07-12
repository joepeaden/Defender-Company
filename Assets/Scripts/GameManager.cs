using UnityEngine;

/// <summary>
/// Handles transitions between Mission and Menu.
/// </summary>
/// <remarks>
/// Try to keep this only with information or functionality which is always necessary to have on hand.
/// </remarks>
public class GameManager : MonoBehaviour
{
    // data store for market items
    [SerializeField] private InventoryItemDataStorage dataStore;

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public MissionData CurrentMission => currentMission;
    private MissionData currentMission;

    public bool PlayerWonLastMission => playerWonLastMission;
    private bool playerWonLastMission;

    public PlayerCompany Company => company;
    private PlayerCompany company;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Game Manager, deleting one.");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        company = new PlayerCompany(dataStore);

        MissionManager.OnMissionEnd.AddListener(HandleMissionEnd);
    }

    public void SetCurrentMission(MissionData mission)
    {
        currentMission = mission;
    }

    #region Transitional Stuff

    private void HandleMissionEnd(bool playerWon)
    {
        SceneLoader.OnSceneLoadComplete.AddListener(LoadAARScreen);
        SceneLoader.Instance.LoadScene(SceneLoader.SceneName.MainMenu, SceneLoader.SceneName.Mission, true);
        playerWonLastMission = playerWon;

        if (playerWonLastMission)
        {
            company.AddCash(currentMission.completionReward);
        }
    }

    /// <summary>
    /// Load AAR screen after the menu scene has loaded.
    /// </summary>
    private void LoadAARScreen()
    {
        SceneLoader.OnSceneLoadComplete.RemoveListener(LoadAARScreen);
        MenuScreenManager.Instance.DisplayAAR();
    }

    #endregion
}
