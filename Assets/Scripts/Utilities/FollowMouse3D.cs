using UnityEngine;

/// <summary>
/// Attatched to an object will make the object follow where the mouse points in 3D.
/// </summary>
/// <author>
/// Joe
/// </author>
public class FollowMouse3D : MonoBehaviour
{
    [SerializeField] private GameObject raycastPlane;
    public LayerMask mask;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] rayCast = Physics.RaycastAll(ray, mask);

        foreach (RaycastHit hit in rayCast)
        {
            if (rayCast.Length != 0)
            {
                if (hit.transform.gameObject == raycastPlane)
                {
                    transform.position = hit.point;
                }
            }
        }
    }
}
