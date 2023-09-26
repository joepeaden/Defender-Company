using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetectionTrigger : MonoBehaviour
{
    public AIActorController actorController;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HitBox") && other.GetComponent<HitBox>().GetActor().team != actorController.GetActor().team)
        {
            if (actorController.GetActor().IsAlive && actorController.AttackTarget == null)
            {
                actorController.SetAttackTarget(other.GetComponent<HitBox>().GetActor().gameObject);
            }
        }
    }
}
