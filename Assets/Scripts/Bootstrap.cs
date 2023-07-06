using UnityEngine;

/// <summary>
/// Sets up startup stuff. Don't really know if necessary.
/// </summary>
public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        // not initially present because may reload scene and don't want to re initialize - so just setting up
        //GameObject sceneLoader = Instantiate(new GameObject(), transform);
        //sceneLoader.name = "Scene Loader";
        //sceneLoader.AddComponent
    }
}
