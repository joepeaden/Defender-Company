using UnityEngine;

/// <summary>
/// Sets up startup stuff.
/// </summary>
public class Bootstrap : MonoBehaviour
{
    public GameObject SceneLoader;

    private void Awake()
    {
        // not initially present because may reload scene and don't want to re initialize - so just setting up
        Instantiate(SceneLoader);
    }
}
