public abstract class AIState
{
    /// <summary>
    /// Previous AIState.
    /// </summary>
    protected AIState _prevAIState;
    /// <summary>
    /// Ref to the AI controller.
    /// </summary>
    protected AIActorController _controller;
    /// <summary>
    /// How long the actor has been in this state.
    /// </summary>
    public float TimeInState => timeInState;
    private float timeInState;

    public void EnterState(AIActorController controller, AIState prevAIState)
    {
        if (prevAIState == this)
        {
            return;
        }

        _controller = controller;
        _prevAIState = prevAIState;

        _EnterState();
    }

    public AIState StateUpdate(AIActorController controller, AIState prevAIState)
    {
        //Debug.Log(this.GetType());

        //_controller = controller;
        //_prevAIState = prevAIState;
        AIState newState = _StateUpdate();
        if (newState != this)
        {
            _ExitState();
        }

        return newState;
    }

    protected virtual void _ExitState()
    {

    }

    protected abstract AIState _StateUpdate();
    protected abstract void _EnterState();
}
