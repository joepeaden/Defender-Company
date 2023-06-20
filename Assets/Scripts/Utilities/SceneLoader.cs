using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
    //public static UnityEvent OnGameOverSceneLoaded = new UnityEvent();

    public static SceneLoader Instance { get { return _instance; } }
    private static SceneLoader _instance;

    public enum SceneList
    {
        StartMenu,
        House,
        FailMenu,
        Apartment,
        Survival
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one SceneLoader, deleting one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        //    SceneManager.sceneLoaded += HandleSceneLoaded;
        //}

        //private void OnDestroy()
        //{
        //    SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    public void LoadScene(SceneList sceneToLoad, bool additive)
    {
        SceneManager.LoadScene(sceneToLoad.ToString(), additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

    //private void HandleSceneLoaded(Scene s, LoadSceneMode m)
    //{
    //    if (s.name == "FailMenu")
    //    {
    //        OnGameOverSceneLoaded.Invoke();
    //    }
    //}
}
