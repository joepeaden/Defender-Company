/// <summary>
/// The actor is moving towards its target.
/// </summary>
public class AIMovingToPositionState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        //if(input.targetInOptimalRange && input.targetInLOS)
        //{
        //    return new AIHoldingPositionCombatState();
        //}

        return this;
    }

    protected override void _StateUpdate()
    {
        _controller.GetActor().Move(_controller.movePosition);
    }
}
