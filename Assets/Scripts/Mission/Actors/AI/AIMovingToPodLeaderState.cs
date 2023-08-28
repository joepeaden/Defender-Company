/// <summary>
/// The actor is moving to its pod leader
/// </summary>
public class AIMovingToPodLeaderState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        if (input.distFromPodLeader < _controller.GetAIData().maxPodSeparation)
        {
            //return new AIHoldingPositionState();
        }

        return this;
    }

    protected override void _StateUpdate()
    {
        //_controller.GetActor().Move(_controller.pod.leader.transform.position);
    }
}
