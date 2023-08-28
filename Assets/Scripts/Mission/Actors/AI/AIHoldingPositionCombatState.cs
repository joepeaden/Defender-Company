using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actor is holding position.
/// </summary>
public class AIHoldingPositionCombatState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        //if (input.distFromPodLeader > _controller.GetAIData().maxPodSeparation)
        //{
        //    return new AIMovingToPodLeaderState();
        //}
        // if the target moves out of sight or range move towards target
        //if (!input.targetInRange || !input.targetInLOS)
        //{
        //    return new AIMovingToTargetState();
        //}

        // we're not taking cover for now.
        // if we decide to take cover do it
        //else
        //if (input.timeForDecision && _controller.Data.useCoverChance > Random.Range(0f, 1f))
        //{
        //    return new AITakingCoverState();
        //}

        // if neither, in range and LOS and not trying to take cover
        return this;
    }

    protected override void _StateUpdate()
    {
        _controller.GetActor().Move(_controller.transform.position);
    }
}