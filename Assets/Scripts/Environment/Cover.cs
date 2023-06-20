using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Interactable
{
    public enum CoverType
    {
        Wall,
        Floor
    }

    public CoverType coverType;

    /// <summary>
    /// Will implement later. Maybe use this to implement some visual or audio feedback
    /// </summary>
    public override void Interact(Actor a)
    {
        base.Interact(a);
    }

    public Vector3 GetActorCoverPosition(Vector3 actorPos)
    {
        // Collider[] colliders = GetComponents<Collider>();
        // Vector3 closestPoint = actorPos;
        // foreach(Collider c in colliders)
        // {
        //     Vector3 closestForThisCollider = c.ClosestPointOnBounds(actorPos);

        //     if (closestForThisCollider.magnitude < closestPoint.magnitude)
        //     {
        //         closestPoint = closestForThisCollider;
        //     }
        // } 
        Vector3 targetPos = GetComponent<Collider>().ClosestPointOnBounds(actorPos);
     
        return targetPos;
    }

    /// <summary>
    /// Get the position for the actor to transfer (vault) to.
    /// </summary>
    /// <param name="actorPos">Position of the calling actor.</param>
    /// <returns>Position for the actor to transfer to.</returns>
    public Vector3 GetActorFlipPosition(Vector3 actorPos)
    {
        // not implemented rn. need to adjust because I got rid of the actor cover positions

        // get furthest target position, that's where actor will go
        Vector3 targetPos = actorPos;
        //float minDist = int.MaxValue;
        //foreach (GameObject targetPosGo in actorTargetPositionGOList)
        //{
        //    float dist = Mathf.Abs((targetPosGo.transform.position - actorPos).magnitude);
        //    if (dist > minDist)
        //    {
        //        minDist = dist;
        //        targetPos = targetPosGo.transform.position;
        //    }
        //}

        //// nullify Y value (don't want actor moving up)
        //targetPos.y = 0f;

        return targetPos;
    }
}
