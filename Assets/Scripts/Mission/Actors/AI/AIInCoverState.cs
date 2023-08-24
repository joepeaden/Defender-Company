using UnityEngine;

/// <summary>
/// The actor is hiding in cover.
/// </summary>
public class AIInCoverState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        //if (!input.targetInRange)
        //{
        //    return new AIMovingToTargetState();
        //}



        // if we're done hiding in cover, pop out.
        if (TimeInState > Random.Range(_controller.GetAIData().minBehindCoverTime, _controller.GetAIData().maxBehindCoverTime))
        {
            return new AIPoppingOutOfCoverState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        // give the actor a bit to get a little further behind cover
        //if (TimeInState > .5f)
        
            _controller.GetActor().Move(_controller.transform.position);
        
    }
}
