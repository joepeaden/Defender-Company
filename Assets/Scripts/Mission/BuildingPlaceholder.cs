using UnityEngine;

public class BuildingPlaceholder : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = BuildingManager.Instance.GetClosestGridPos(mousePos);
    }
}
