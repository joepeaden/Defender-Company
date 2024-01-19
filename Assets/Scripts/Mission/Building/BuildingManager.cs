using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Events;

public class BuildingManager : MonoBehaviour
{
    private static BuildingManager _instance;
    public static BuildingManager Instance => _instance;

    /// <summary>
    /// Called wbenever a building is destroyed, placed, or removed.
    /// </summary>
    public UnityEvent OnBuildingModified = new UnityEvent();

    [SerializeField] private int gridSize;
    [SerializeField] private GameObject wallBuildingPrefab;
    [SerializeField] private GameObject stairsBuildingPrefab;
    [SerializeField] private GameObject barricadesPrefab;
    [SerializeField] private GameObject minesBuildingPrefab;
    [SerializeField] private AstarPath pathfindingGrid;

    Dictionary<Vector3, GameObject> occupiedPositions = new Dictionary<Vector3, GameObject>();
    private GameObject placeHolderObject = null;
    private GameObject buildingToInstantiate;

    [SerializeField] private int maxTimeUnits;
    private int timeUnitsRemaining;
    public int buildingTUCost;
    private bool canEditBuildings;
    private float buildingYRotation;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        MissionManager.OnAttackStart.AddListener(HandleAttackStart);
        MissionManager.OnNewTurn.AddListener(HandleNewTurn);
        HandleNewTurn();
    }

    private void Update()
    {
        if (canEditBuildings)
        {
            if (Input.GetKey(KeyCode.W))
            {

            }

            // need to click the button to select first
            if (placeHolderObject != null)
            {
                // left click to place building 
                if (Input.GetButton("Fire1"))
                {
                    // don't place if something is already there
                    if (!occupiedPositions.ContainsKey(placeHolderObject.transform.position) || occupiedPositions.ContainsKey(placeHolderObject.transform.position) && placeHolderObject.GetComponent<Building>().buildingType == Building.BuildingType.Barricades && occupiedPositions[placeHolderObject.transform.position].GetComponent<Building>().buildingType == Building.BuildingType.Wall)
                    {
                        // place otherwise and reduce time units
                        timeUnitsRemaining -= buildingTUCost;
                        MissionUI.Instance.UpdateRemainingTU(timeUnitsRemaining);
                        GameObject newBuilding = Instantiate(buildingToInstantiate, placeHolderObject.transform.position, buildingToInstantiate.transform.rotation);
                        newBuilding.transform.Rotate(new Vector3(0, buildingYRotation, 0));
                        occupiedPositions[placeHolderObject.transform.position] = newBuilding;

                        if (placeHolderObject.GetComponent<Building>().buildingType == Building.BuildingType.Wall)
                        {
                            OnBuildingModified.Invoke();
                            newBuilding.GetComponent<Building>().HandleWallPlaced();
                            newBuilding.GetComponent<Building>().OnBuildingDestroyed.AddListener(HandleBuildingDestroyed);
                        }
                    }
                    else
                    {
                        // add some visual feedback
                        ;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    placeHolderObject.transform.Rotate(new Vector3(0, 0, 90));
                    buildingYRotation = placeHolderObject.transform.rotation.eulerAngles.z;
                }

                if (timeUnitsRemaining < buildingTUCost || Input.GetButtonDown("Fire2")) // right click to not place tings anymore 
                {
                    DisablePlacementMode();
                }
            }
            // if no placeholder currently, delete something if it's there and refund the building cost
            else if (Input.GetButton("Fire2"))
            {
                Vector3 snapPos = GetClosestSnapPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (occupiedPositions.ContainsKey(snapPos))
                {
                    Destroy(occupiedPositions[snapPos]);
                    occupiedPositions.Remove(snapPos);
                    timeUnitsRemaining += buildingTUCost;
                    MissionUI.Instance.UpdateRemainingTU(timeUnitsRemaining);

                    OnBuildingModified.Invoke();
                }
            }
        }
    }

    private void OnDestroy()
    {
        MissionManager.OnAttackStart.RemoveListener(HandleAttackStart);
        MissionManager.OnNewTurn.RemoveListener(HandleNewTurn);
        OnBuildingModified.RemoveAllListeners();
    }

    private void HandleNewTurn()
    {
        canEditBuildings = true;
        timeUnitsRemaining = maxTimeUnits;
        MissionUI.Instance.UpdateRemainingTU(timeUnitsRemaining);
    }

    private void HandleAttackStart()
    {
        DisablePlacementMode();
        canEditBuildings = false;
        RebuildPathfindingGrid();
    }

    public void HandleBuildingDestroyed(Building building)
    {
        occupiedPositions.Remove(building.transform.position);

        RebuildPathfindingGrid();
        OnBuildingModified.Invoke();
    }

    public void RebuildPathfindingGrid()
    {
        pathfindingGrid.Scan();
    }

    private void DisablePlacementMode()
    {
        Destroy(placeHolderObject);
        buildingToInstantiate = null;
    }

    public Vector3 GetClosestSnapPos(Vector3 position)
    {
        int roundedX = (int)Mathf.Ceil(position.x / 1) * 1;
        int roundedY = (int)Mathf.Ceil(position.y / 1) * 1;
        Vector3 roundedPos = new Vector3(roundedX, roundedY, 0);
     
        return roundedPos; 
    }

    public bool IsSpaceOccupiedBy(Vector3 positionToCheck, Building.BuildingType buildingType)
    {
        if (occupiedPositions.ContainsKey(positionToCheck))
        {
            return occupiedPositions[positionToCheck].GetComponent<Building>().buildingType == buildingType; 
        }
        else
        {
            return false;
        }
    }

    public void AttachWallBuildingToMouse()
    {
        if (timeUnitsRemaining - buildingTUCost < 0)
        {
            return;
        }

        if (placeHolderObject != null)
        {
            Destroy(placeHolderObject);
        }

        placeHolderObject = Instantiate(wallBuildingPrefab);
        placeHolderObject.GetComponent<Building>().isBeingPlaced = true;
        buildingToInstantiate = wallBuildingPrefab;
    }

    public void AttachStairsBuildingToMouse()
    {
        if (timeUnitsRemaining - buildingTUCost < 0)
        {
            return;
        }

        if (placeHolderObject != null)
        {
            Destroy(placeHolderObject);
        }

        placeHolderObject = Instantiate(stairsBuildingPrefab);
        placeHolderObject.GetComponent<Building>().isBeingPlaced = true;
        buildingToInstantiate = stairsBuildingPrefab;
    }

    public void AttachBarricadesToMouse()
    {
        if (timeUnitsRemaining - buildingTUCost < 0)
        {
            return;
        }

        if (placeHolderObject != null)
        {
            Destroy(placeHolderObject);
        }

        placeHolderObject = Instantiate(barricadesPrefab);
        placeHolderObject.GetComponent<Building>().isBeingPlaced = true;
        buildingToInstantiate = barricadesPrefab;
    }
}
