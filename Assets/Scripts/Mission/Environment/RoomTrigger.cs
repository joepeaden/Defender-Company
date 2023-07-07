using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Has events for player entering room or exiting room.
/// </summary>
public class RoomTrigger : MonoBehaviour
{
    public UnityEvent OnRoomEnter = new UnityEvent();
    public UnityEvent OnRoomExit = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OnRoomEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            OnRoomExit.Invoke();
        }
    }
}
