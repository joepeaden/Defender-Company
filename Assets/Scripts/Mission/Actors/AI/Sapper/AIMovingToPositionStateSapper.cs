using UnityEngine;
/// <summary>
/// The actor is moving to an indicated position
/// </summary>
public class AIMovingToPositionStateSapper : AIMovingToPositionState
{
    bool hasPlantedBomb = false;

    //protected override AIState _HandleInput(AIInput input)
    //{
    //    //if ((_controller.GetActor().transform.position - _controller.MovePosition).magnitude < .1f)
    //    //{
    //    //    return new AIHoldingPositionCombatState();
    //    //}

    //    return this;
    //}

    //public override void EnterState(AIActorController controller, AIState prevAIState)
    //{
    //    if (prevAIState == this)
    //    {
    //        return;
    //    }

    //    base.EnterState(controller, prevAIState);

    //    bool foundWall = false;
    //    foreach (GameObject wallGameObject in GameObject.FindGameObjectsWithTag("WallBuilding"))
    //    {
    //        if (!wallGameObject.GetComponent<Building>().isTargeted)
    //        {
    //            wallGameObject.GetComponent<Building>().isTargeted = true;
    //            controller.MoveToPosition(wallGameObject.transform.position);
    //            foundWall = true;
    //            break;
    //        }
    //    }

    //    if (!foundWall)
    //    {
    //        controller.SetFollowTarget(MissionManager.Instance.GetPlayerGO().transform);
    //    }
    //}

    //protected override AIState _StateUpdate()
    //{
    //    if (!hasPlantedBomb)
    //    {
    //        //if (_controller.bumpedIntoWall)//
    //        if((_controller.MovePosition - _controller.transform.position).magnitude < 10f)
    //        {
    //            _controller.PlaceBomb();
    //            hasPlantedBomb = true;
    //        }
    //        //else
    //        //{
    //        //    _controller.GetActor().Move(_controller.MovePosition);

    //        //}
    //    }

    //    return this;
    //}
}
