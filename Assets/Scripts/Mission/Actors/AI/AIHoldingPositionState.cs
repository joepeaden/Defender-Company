using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actor is holding position.
/// </summary>
public class AIHoldingPositionState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if (!input.targetInRange || !input.targetInLOS)
        {
            return new AIMovingToTargetState();
        }
        else if (input.timeForDecision && _controller.Data.useCoverChance > Random.Range(0f, 1f))
        {
            return new AITakingCoverState();
        }
        else
        {
            return this;
        }
    }

    protected override void _StateUpdate()
    {
        _controller.GetActor().Move(_controller.transform.position);
    }
}
