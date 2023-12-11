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

        Vector3 zeroedTargetPos = _controller.GetActor().transform.position;
        zeroedTargetPos.y = 0f;
        Vector3 zeroedCurrentPos = _controller.MovePosition;
        zeroedCurrentPos.y = 0f;

        bool hasReachedDestination = (zeroedTargetPos - zeroedCurrentPos).magnitude < .5f;

        if (hasReachedDestination || !_controller.ShouldGoToPosition())
        {
            return new AIHoldingPositionCombatState();
        }

        if (_controller.ShouldFollowSomething())
        {
            return new AIFollowTargetState();
        }

        return this;
    }

    protected override void _ExitState()
    {
        base._ExitState();
        _controller.StopGoingToPosition();
    }
}
