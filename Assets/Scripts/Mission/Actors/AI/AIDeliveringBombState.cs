using UnityEngine;

public class AIDeliveringBombState : AIMovingToPositionState
{
    protected override void _ExitState()
    {
        base._ExitState();

        _controller.PlaceBomb();
    }
}
