using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectionTrigger : MonoBehaviour
{
    public AIActorController actorController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitBox") && other.GetComponent<HitBox>().GetActor().team != actorController.GetActor().team)
        {
            if (actorController.GetActor().IsAlive)
            {
                actorController.AddPossibleTarget(other.GetComponent<HitBox>().GetActor());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HitBox") && other.GetComponent<HitBox>().GetActor().team != actorController.GetActor().team)
        {
            if (actorController.GetActor().IsAlive)
            {
                actorController.RemovePossibleTarget(other.GetComponent<HitBox>().GetActor());
            }
        }
    }
}
