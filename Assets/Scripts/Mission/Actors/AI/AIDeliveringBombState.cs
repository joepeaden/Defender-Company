using UnityEngine;

public class AIDeliveringBombState : AIState
{
    protected override AIState _HandleInput(AIInput input)
    {
        //if(input.targetInOptimalRange && input.targetInLOS)
        //{
        //    return new AIHoldingPositionCombatState();
        //}

        return this;
    }


    bool hasPlantedBomb = false;
    //public Vector3 followOffset;

    protected override void _StateUpdate()
    {
        //if ((_controller.transform.position - _controller.MoveTarget.position).magnitude > 5f)
        //{

        //if (_controller.MoveTarget.position)
        //}

        if (!hasPlantedBomb)
        {
            if ((_controller.MoveTarget.position - _controller.transform.position).magnitude < 6f)
            {
                _controller.PlaceBomb();
                hasPlantedBomb = true;
            }
            else
            {
                _controller.GetActor().Move(_controller.MoveTarget.position);// + followOffset);

            }
        }
    }
}
