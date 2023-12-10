using UnityEngine;

public class AIDeliveringBombState : AIState
{
    bool hasPlantedBomb = false;

    protected override AIState _HandleInput(AIInput input)
    {
        return this;
    }

    protected override void _StateUpdate()
    {
        if (!hasPlantedBomb)
        {
            if ((_controller.FollowTarget.position - _controller.transform.position).magnitude < 6f)
            {
                _controller.PlaceBomb();
                hasPlantedBomb = true;
            }
            else
            {
                _controller.GetActor().Move(_controller.MovePosition);

            }
        }
    }
}
