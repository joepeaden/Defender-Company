/// <summary>
/// The actor is moving towards its target.
/// </summary>
public class AIMovingToTargetState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if(input.targetInOptimalRange && input.targetInLOS)
        {
            return new AIHoldingPositionCombatState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        _controller.GetActor().Move(_controller.GetTarget().transform.position);
    }
}
