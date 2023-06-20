using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpot : MonoBehaviour
{
    private Actor actor;
    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
        actor = GetComponentInParent<Actor>();
        actor.OnCrouch.AddListener(HandleCrouch);
        actor.OnStand.AddListener(HandleStand);
    }

    private void HandleCrouch()
    {
        transform.localPosition = new Vector3(originalLocalPosition.x, 0.3f, originalLocalPosition.z);
    }

    private void HandleStand()
    {
        transform.localPosition = originalLocalPosition;
    }
}
