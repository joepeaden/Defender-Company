using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkThroughWall : MonoBehaviour
{
    public Collider wallCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>())
        {
            Physics.IgnoreCollision(other, wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Actor>())
        {
            Physics.IgnoreCollision(other, wallCollider, false);
        }
    }
}
