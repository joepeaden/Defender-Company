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

        //Vector3 zeroedTargetPos = _controller.GetActor().transform.position;
        //Vector3 zeroedCurrentPos = _controller.MovePosition;
        //zeroedTargetPos.y = 0f;
        //zeroedCurrentPos.y = 0f;

        bool hasReachedDestination = _controller.GetActor().Pathfinder.reachedDestination;//(zeroedTargetPos - zeroedCurrentPos).magnitude == 0f;//<= (this as AIDeliveringBombState == null ? 0f : 1.5f);

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
