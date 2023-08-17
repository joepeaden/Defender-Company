using UnityEngine;

/// <summary>
/// The actor is popping out of cover to shoot at the target.
/// </summary>
public class AIPoppingOutOfCoverState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if (!input.targetInRange)
        {
            return new AIMovingToTargetState();
        }

        // if we're done popping out, duck back into cover.
        // for now just using the same values. Should be fine.
        // probably want it to go off bursts in the future.
        if (TimeInState > Random.Range(_controller.GetAIData().minBehindCoverTime, _controller.GetAIData().maxBehindCoverTime))
        {
            return new AITakingCoverState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        if (!_controller.TargetInLOS)
        {
            _controller.GetActor().Move(_controller.GetTarget().transform.position);
        }
        else
        {
            _controller.GetActor().Move(_controller.transform.position);
        }
    }
}