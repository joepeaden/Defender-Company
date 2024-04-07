using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButtonSupport : MonoBehaviour
{
    public bool loadSceneAdditive;
    public SceneLoader.SceneName sceneToLoad;
    public bool unloadScene;
    public SceneLoader.SceneName sceneToUnload;

    private void Start()
    {
        SceneLoader.SceneName? _sceneToUnload = unloadScene ? sceneToUnload : null;
        GetComponent<Button>().onClick.AddListener(delegate { SceneLoader.Instance.ChangeScene(sceneToLoad, _sceneToUnload, loadSceneAdditive); } );
    }
}
