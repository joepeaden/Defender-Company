using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    private static BuildingManager _instance;
    public static BuildingManager Instance => _instance;

    [SerializeField] private int gridSize;
    [SerializeField] private GameObject wallBuildingPrefab;
    [SerializeField] private GameObject minesBuildingPrefab;
    [SerializeField] private GameObject buildingPlaceholderPrefab;

    private Dictionary<Vector3, (int, int)> posToGridCoord = new Dictionary<Vector3, (int, int)>();
    private GameObject placeHolderObject = null;
    private GameObject buildingToInstantiate;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        float xOffset = 0f, zOffset = 0f;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                posToGridCoord.Add(new Vector3(xOffset, 0f, zOffset), (i, j));
                xOffset += 1;
            }
            zOffset += 1;
        }
    }

    private void Update()
    {
        // left click to place
        if (Input.GetButtonDown("Fire1") && placeHolderObject != null)
        {
            Instantiate(buildingToInstantiate, placeHolderObject.transform.position, Quaternion.identity);
        }

        // right click to not place tings anymore
        if (Input.GetButtonDown("Fire2") && placeHolderObject != null)
        {
            Destroy(placeHolderObject);
            placeHolderObject = null;
        }
    }

    public Vector3 GetClosestGridPos(Vector3 position)
    {
        float roundedX = Mathf.Round(position.x);
        float roundedZ = Mathf.Round(position.z);
        Vector3 roundedPos = new Vector3(roundedX, 0, roundedZ);
     
        return roundedPos; 
    }

    public void AttachWallBuildingToMouse()
    {
        placeHolderObject = Instantiate(buildingPlaceholderPrefab);
        buildingToInstantiate = wallBuildingPrefab;
    }

    // i am a villan... A FUCKING VILLAN!
    // BANG YOUR FUCKING HEAD!
    // UNTIL YOUR FUCKING DEAD!

}
