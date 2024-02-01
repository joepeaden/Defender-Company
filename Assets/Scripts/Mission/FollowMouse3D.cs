using UnityEngine;

public class FollowMouse3D : MonoBehaviour
{
    public static Transform CursorTransform;

    private void Awake()
    {
        Cursor.visible = false;
        CursorTransform = transform;
    }

    void Update()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0f;
        transform.position = newPosition;    
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}
