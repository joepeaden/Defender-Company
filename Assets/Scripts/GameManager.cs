using UnityEngine;

/// <summary>
/// Handles transitions between Mission and Menu.
/// </summary>
/// <remarks>
/// Try to keep this only with information or functionality which is always necessary to have on hand.
/// </remarks>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // will eventually move these data into a seperate class.
    private int PlayerCash => playerCash;
    private int playerCash;

    public MissionData CurrentMission => currentMission;
    private MissionData currentMission;

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
