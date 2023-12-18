using UnityEngine;

/// <summary>
/// The actor is following something.
/// </summary>
public class AIFollowTargetState : AIState
{
    protected override void _EnterState()
    {
        ;
    }

    protected override AIState _StateUpdate()
    {
        _controller.GetActor().Move(_controller.FollowTarget.position);

        if (_controller.ShouldGoToPosition())
        {
            return new AIMovingToPositionState();
        }

        if (_controller.IsDazed())
        {
            return new AIDazedState();
        }
        
        return this;
    }

    protected override void _ExitState()
    {
        base._ExitState();
        _controller.StopFollowingSomething();
    }
}
