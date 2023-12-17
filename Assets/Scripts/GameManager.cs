using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// Handles transitions between Mission and Menu.
/// </summary>
/// <remarks>
/// Try to keep this only with information or functionality which is always necessary to have on hand.
/// </remarks>
public class GameManager : MonoBehaviour
{   
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // the idea here is just to allow loading the mission scene standalone
                // make a temporary game manager
                GameObject tempGameManager = Instantiate(new GameObject());
                tempGameManager.name = "Temporary Game Manager";
                _instance = tempGameManager.AddComponent<GameManager>();
                // just assign an example mission
                Addressables.LoadAssetAsync<MissionData>("MedMission").Completed += _instance.OnLoadMissionDone;
            }
            return _instance;
        }
    }

    public MissionData CurrentMission => currentMission;
    private MissionData currentMission;

    public bool PlayerWonLastMission => playerWonLastMission;
    private bool playerWonLastMission;

    public PlayerCompany Company => company;
    private PlayerCompany company;

    private InventoryItemDataStorage dataStore;

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

        // should get rid of this object and just load the stuff later. I don't know. You know aht I mean. Use the addressables the way they should be. 
        Addressables.LoadAssetAsync<GameObject>("InventoryItemDataStorage").Completed += OnLoadDataDone;

        MissionManager.OnMissionEnd.AddListener(HandleMissionEnd);
    }

    // for when temporary game managers are used (i.e. running Mission scene by itself)
    public bool IsInitialized()
    {
        return company != null && dataStore != null && currentMission != null;
    }

    private void OnLoadDataDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        // should add exception handling to catch scenarios such as a null result.
        dataStore = obj.Result.GetComponent<InventoryItemDataStorage>();
        company = new PlayerCompany(dataStore);
    }

    public InventoryItemDataStorage GetDataStore()
    {
        return dataStore;
    }

    private void OnLoadMissionDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<MissionData> obj)
    {
        SetCurrentMission(obj.Result);
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
