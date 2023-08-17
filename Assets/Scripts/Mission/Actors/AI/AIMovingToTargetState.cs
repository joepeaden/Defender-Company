using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actor is moving towards its target.
/// </summary>
public class AIMovingToTargetState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if(input.targetInOptimalRange && input.targetInLOS)
        {
            return new AIHoldingPositionState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        _controller.GetActor().Move(_controller.GetTarget().transform.position);
    }
}
