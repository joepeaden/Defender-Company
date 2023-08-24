using UnityEngine;

/// <summary>
/// The actor is popping out of cover to shoot at the target.
/// </summary>
public class AIPoppingOutOfCoverState : AIState
{
    /// <summary>
    /// How long has it been since we've actually got out of cover and see the target?
    /// </summary>
    private float timeSincePoppedOutOfCover;

    protected override AIState _HandleInput(AIInput input)
    {
        //if (!input.targetInRange)
        //{
        //    return new AIMovingToTargetState();
        //}

        // if we're done popping out, duck back into cover.
        // for now just using the same values. Should be fine.
        // probably want it to go off bursts in the future.

        if (!input.fullyOutOfCover)//input.targetInLOS)
        {
            timeSincePoppedOutOfCover += Time.deltaTime;
        }

        if (timeSincePoppedOutOfCover > Random.Range(_controller.GetAIData().minBehindCoverTime, _controller.GetAIData().maxBehindCoverTime))
        {
            return new AITakingCoverState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        if (!_controller.fullyOutOfCover)
        {
            _controller.GetActor().Move(_controller.GetTarget().transform.position);
        }
        // give the actor a bit to get out of cover
        else //if (TimeInState > .5f)
        {
            _controller.GetActor().Move(_controller.transform.position);
        }
    }
}