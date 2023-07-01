using UnityEngine;

/// <summary>
/// Handles transitions between Mission and Menu.
/// </summary>
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        MissionManager.OnMissionEnd.AddListener(HandleMissionEnd);
    }

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
}
