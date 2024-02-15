using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [Header("Projectiles")]
    public GameObject projectilePrefab;
    public int amountProjToPool;

    [Header("Decals")]
    public GameObject spriteDecalPrefab;
    public int amountDeadBodyToPool;

    private Transform objectPoolParent;
    private List<GameObject> projectiles;
    private List<GameObject> decals;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // just for organization
        objectPoolParent = Instantiate(new GameObject()).GetComponent<Transform>();
        objectPoolParent.name = "ObjectPool";

        projectiles = CreatePool(projectilePrefab, projectiles, amountProjToPool);
        decals = CreatePool(spriteDecalPrefab, decals, amountProjToPool);
    }

    private List<GameObject> CreatePool(GameObject prefab, List<GameObject> listToAssign, int count)
    {
        listToAssign = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < count; i++)
        {
            tmp = Instantiate(prefab, objectPoolParent);
            tmp.SetActive(false);
            listToAssign.Add(tmp);
        }

        return listToAssign;
    }

    private GameObject GetPooledObject(List<GameObject> theList, GameObject prefab)
    {
        for (int i = 0; i < theList.Count; i++)
        {
            if (!theList[i].activeInHierarchy)
            {
                return theList[i];
            }
        }

        GameObject newObject = Instantiate(prefab);
        theList.Add(newObject);
        return newObject;
    }

    public GameObject GetProjectile()
    {
        return GetPooledObject(projectiles, projectilePrefab);
    }

    public GameObject GetDecal()
    {
        return GetPooledObject(decals, spriteDecalPrefab);
    }
}
