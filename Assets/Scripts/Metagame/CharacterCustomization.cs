using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class CharacterCustomization
{
    public UnityEvent OnCustomizationInitialized = new UnityEvent();
    private List<Sprite> faces = new List<Sprite>();

    public CharacterCustomization()
    {
        Addressables.LoadAssetAsync<Sprite>("Face1").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face2").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face3").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face4").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face5").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face6").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face7").Completed += OnLoadAssetDone;
        Addressables.LoadAssetAsync<Sprite>("Face8").Completed += OnLoadFinalAssetDone;
    }

    private void OnLoadAssetDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> obj)
    {
        faces.Add(obj.Result);
    }

    private void OnLoadFinalAssetDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> obj)
    {
        faces.Add(obj.Result);
        OnCustomizationInitialized.Invoke();
    }

    public Sprite GetRandomFace()
    {
        return faces[Random.Range(0, faces.Count)];
    }
}
