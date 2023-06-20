using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// For detecting interactable objects such as Cover or Doors.
/// </summary>
public class ActorInteractSensor : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnInteractableNearby;

    private Interactable interactable;

    private HashSet<Collider> colliders = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i)
        {
            interactable = i;

            colliders.Add(other);

            OnInteractableNearby.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i)
        {
            colliders.Remove(other);

            if (colliders.Count < 1)
            {
                interactable = null;
            }
        }
    }

    public Interactable GetInteractable()
    {
        return interactable;
    }

    public Collider GetInteractableCollider()
    {
        return interactable.GetComponent<Collider>();
    }
}
