using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButtonSupport : MonoBehaviour
{
    public SceneLoader.SceneName sceneToLoad;
    public bool loadSceneAdditive;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { SceneLoader.Instance.LoadScene(sceneToLoad, loadSceneAdditive); } );
    }
}
