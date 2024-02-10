using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Just a WASD moving target for the camera to follow for "free camera movement"
/// </summary>
public class FreeCameraMovingTarget : MonoBehaviour
{
    public float moveSpeed;
    public bool isBeingFollowed;

    private void Update()
    {
        if (isBeingFollowed)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector2.up * Time.unscaledDeltaTime * moveSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(-Vector2.up * Time.unscaledDeltaTime * moveSpeed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-Vector2.right * Time.unscaledDeltaTime * moveSpeed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector2.right * Time.unscaledDeltaTime * moveSpeed);
            }
        }
        else if (MissionManager.Instance.Player.ControlledActor != null)
        {
            transform.position = MissionManager.Instance.Player.ControlledActor.transform.position;
        }
    }
}
