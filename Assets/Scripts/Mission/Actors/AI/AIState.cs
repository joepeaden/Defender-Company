using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public abstract class AIState
{
    /// <summary>
    /// Previous AIState.
    /// </summary>
    protected AIState _prevAIState;
    /// <summary>
    /// Ref to the AI controller.
    /// </summary>
    protected Enemy _controller;
    /// <summary>
    /// How long the actor has been in this state.
    /// </summary>
    public float TimeInState => timeInState;
    private float timeInState;

    //protected AIInput LatestInput => latestInput;
    //private AIInput latestInput;

    // can add Enter if needed later.
    //public void Enter();

    // going to change these "Enemy" refs to "AIController" later.
    public AIState HandleInput(AIInput input)
    {
        //latestInput = input;

        // get how long we've been in this state.
        if (_prevAIState != null && _prevAIState.GetType() == this.GetType())
        {
            timeInState = Time.deltaTime + _prevAIState.TimeInState;
        }

        return _HandleInput(input);
    }

    public void StateUpdate(Enemy controller, AIState prevAIState)
    {
        Debug.Log(this.GetType());

        _controller = controller;
        _prevAIState = prevAIState;
        _StateUpdate();
    }

    protected abstract AIState _HandleInput(AIInput input);
    protected abstract void _StateUpdate();
}
