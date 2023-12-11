using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int hitPoints = 10;
    public bool isBeingPlaced = false;
    public BuildingType buildingType;
    public bool isTargeted;

    public enum BuildingType
    {
        Wall,
        Sandbags,
        Stairs
    }

    private void Update()
    {
        if (hitPoints <= 0)
        {
            GetComponentInChildren<Collider>().enabled = false;

            BuildingManager.Instance.RebuildNav();
            Destroy(gameObject);

            //StartCoroutine(TriggerRebuildAfterAFrame());
        }

        if (isBeingPlaced)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = BuildingManager.Instance.GetClosestSnapPos(mousePos);
        }
    }

    //private IEnumerator TriggerRebuildAfterAFrame()
    //{
    //    yield return null;

    //    BuildingManager.Instance.RebuildNav();
    //    Destroy(gameObject);
    //}
}