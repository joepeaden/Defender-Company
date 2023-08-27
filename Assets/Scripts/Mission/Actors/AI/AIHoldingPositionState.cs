/// <summary>
/// Actor is holding position (not in combat) and paying attention to detection radius / LOS.
/// </summary>
public class AIHoldingPositionState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if (input.podAlerted)
        {
            return new AIHoldingPositionCombatState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        _controller.GetActor().Move(_controller.transform.position);
    }
}
