using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    public SceneLoader.SceneList sceneToLoad;
    public bool loadSceneAdditive;

    private void Start()
    {
        Button b = GetComponent<Button>();
        b.onClick.AddListener(delegate { SceneLoader.Instance.LoadScene(sceneToLoad, loadSceneAdditive); } );
    }
}
