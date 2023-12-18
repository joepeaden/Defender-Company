using UnityEngine;

/// <summary>
/// The actor is holding position.
/// </summary>
public class AIHoldingPositionCombatState : AIState
{
    protected override void _EnterState()
    {
        _controller.GoToPosition(Vector3.zero);
    }

    protected override AIState _StateUpdate()
    {
        if (_controller.ShouldGoToPosition())
        {
            return new AIMovingToPositionState();
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
}