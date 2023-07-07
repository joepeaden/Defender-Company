using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Shatter : MonoBehaviour
{
    public GameObject shatterObject;
    public UnityEvent OnShatter = new UnityEvent();
    public void ShatterObject()
    {
        Instantiate(shatterObject, transform.position, transform.rotation);
        OnShatter.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy() {
        OnShatter.RemoveAllListeners();
    }
}
