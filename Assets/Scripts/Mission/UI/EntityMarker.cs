using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMarker : ObjectiveMarker
{
    private int totalHitPoints;
    private Actor actor;

    public void SetData(ActorController controller, string newLabel)
    {
        SetData(controller.gameObject.transform, newLabel);

        actor = controller.GetActor();
        actor.OnGetHit.AddListener(HandleActorHit);
        totalHitPoints = actor.HitPoints;
    }

    private void HandleActorHit()
    {
        int hitPointsLeft = actor.HitPoints;
        int twoThirdsThreshold = totalHitPoints / 3 * 2;
        int oneThirdThreshold = totalHitPoints / 3;

        if (hitPointsLeft > twoThirdsThreshold)
        {
            return;
        }
        else if (hitPointsLeft > oneThirdThreshold)
        {
            dotImage.color = Color.yellow;
            label.color = Color.yellow;
        }
        else if (hitPointsLeft > 0)
        {
            dotImage.color = Color.red;
            label.color = Color.red;
        }
        else
        {
            dotImage.color = Color.grey;
            label.color = Color.grey;
        }
    }


}
