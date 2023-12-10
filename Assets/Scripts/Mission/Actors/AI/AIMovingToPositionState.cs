/// <summary>
/// The actor is moving to an indicated position
/// </summary>
public class AIMovingToPositionState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if ((_controller.GetActor().transform.position - _controller.MovePosition).magnitude < .1f)
        {
            AIState newState = new AIHoldingPositionCombatState();
            newState.EnterState(_controller, this);
            return newState;
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        ;
        //_controller.Move(_controller.MovePosition);
    }
}
