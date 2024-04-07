using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityMarker : ObjectiveMarker
{
    private int totalHitPoints;
    private Actor actor;
    public GameObject healthBarPip;
    public Transform healthBarPipParent;
    private List<GameObject> healthPips = new List<GameObject>();

    public void SetData(ActorController controller, string newLabel)
    {
        SetData(controller.gameObject.transform, newLabel);

        actor = controller.GetActor();
        actor.OnGetHit.AddListener(HandleActorHit);
        totalHitPoints = actor.HitPoints;

        for (int i = 0; i < actor.HitPoints; i++)
        {
            healthPips.Add(Instantiate(healthBarPip, healthBarPipParent));
            healthPips[i].SetActive(true);
        }
    }

    private void HandleActorHit()
    {
        healthPips[actor.HitPoints].GetComponent<Image>().color = Color.red;
        //int twoThirdsThreshold = totalHitPoints / 3 * 2;
        //int oneThirdThreshold = totalHitPoints / 3;

        //if (hitPointsLeft > twoThirdsThreshold)
        //{
        //    return;
        //}
        //else if (hitPointsLeft > oneThirdThreshold)
        //{
        //    dotImage.color = Color.yellow;
        //    label.color = Color.yellow;
        //}
        //else if (hitPointsLeft > 0)
        //{
        //    dotImage.color = Color.red;
        //    label.color = Color.red;
        //}
        //else
        //{
        //    dotImage.color = Color.grey;
        //    label.color = Color.grey;
        //}
    }


}
