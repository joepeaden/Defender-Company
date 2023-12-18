using UnityEngine;
/// <summary>
/// Actor is not moving and will return to previous state after a duration.
/// </summary>
public class AIDazedState : AIState
{
    private Transform _oldFollowTarget;
    private Vector3 _oldMovePosition;

    protected override void _EnterState()
    {
        if (_prevAIState as AIMovingToPositionState != null)
        {
            _oldMovePosition = _controller.MovePosition;
            _controller.StopGoingToPosition();
        }
        else if (_prevAIState as AIFollowTargetState != null)
        {
            _oldFollowTarget = _controller.FollowTarget;
            _controller.StopFollowingSomething();
        }

        _controller.GetActor().Move(_controller.transform.position);
    }

    protected override AIState _StateUpdate()
    {
        if (TimeInState > .25f)
        {
            return _prevAIState;
        }

        return this;
    }

    protected override void _ExitState()
    {
        base._ExitState();
        _controller.StopBeingDazed();

        if (_prevAIState as AIMovingToPositionState != null)
        {
            _controller.GoToPosition(_oldMovePosition);
        }
        else if (_prevAIState as AIFollowTargetState != null)
        {
            _controller.FollowThisThing(_oldFollowTarget);
        }
    }
}
