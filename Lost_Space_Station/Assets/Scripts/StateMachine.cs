using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public class State
    {
        public string Name;
        public System.Action onFrame; //default action or state handler (when state is active)
        public System.Action onEnter; //what happens when this state is entered 
        public System.Action onExit; //when state is exited 

        public override string ToString()
        {
            return Name;
        }
    }

    //
    public Dictionary<string, State> states = new Dictionary<string, State>();

    public State currentState { get; private set; }

    public State initialState;

    public State CreateState(string name)
    {
        State state = new State();
        state.Name = name;

        if (states.Count == 0)
        {
            initialState = state;
        }

        states[name] = state;

        return state;
    }

    public void Update()
    {
        //No states yet
        if (states.Count == 0)
        {
            Debug.LogError("*** State machine has no states!");
            return;
        }

        //no current state yet 
        if (currentState == null)
        {
            ChangeState(initialState)
;
        }
        if (currentState.onFrame != null)
        {
            currentState.onFrame();
        }
    }

    public void ChangeState(State newState)
    {
        //check if newState is null 
        if (newState == null)
        {
            Debug.LogError("*** Can't change to a null state!");
            return;
        }
        //do onExit of current state
        if (currentState != null && currentState.onExit != null)
        {
            currentState.onExit();
        }

        //change to newState 
        currentState = newState;

        //do onEnter of newState
        if (currentState.onEnter != null)
        {
            currentState.onEnter();
        }
    }
    public void ChangeState(string newStateName)
    {
        if (states.ContainsKey(newStateName))
        {
            ChangeState(states[newStateName]);
        }
        else
        {
            Debug.LogErrorFormat("*** State machine doesn't have the state {0}", newStateName);
            return;
        }
    }
}
