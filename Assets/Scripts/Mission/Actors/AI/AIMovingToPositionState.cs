using UnityEngine;

/// <summary>
/// The actor is moving to a position
/// </summary>
public class AIMovingToPositionState : AIState
{
    protected override void _EnterState()
    {
        ;
    }

    protected override AIState _StateUpdate()
    {
        _controller.GetActor().Move(_controller.movePosition);

        bool hasReachedDestination = _controller.GetActor().Pathfinder.reachedDestination;
        if (hasReachedDestination || !_controller.ShouldGoToPosition())
        {
            return new AIHoldingPositionCombatState();
        }

        if (_controller.ShouldFollowSomething())
        {
            return new AIFollowTargetState();
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
        _controller.StopGoingToPosition();
    }
}
