using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateEngine<T> where T : MonoBehaviour
{
    private T _owner;
    public T owner { get { return _owner; } }

    private StateBase<T> _currState;
    public StateBase<T> currState { get { return _currState; } }

    private StateBase<T> _prevState;
    public StateBase<T> prevState { get { return _prevState; } }

    private StateBase<T> _globalState;
    public StateBase<T> globalState { get { return _globalState; } }

    public bool debug = false;

    public void Init(T owner, StateBase<T> initState)
    {
        _owner = owner;
        ChangeState(initState);
    }
	
	public void Execute ()
    {
        if (_currState != null)
            _currState.Execute(_owner);
        if (_globalState != null)
            _globalState.Execute(_owner);
	}

    public void ChangeState(StateBase<T> nextState)
    {
        _prevState = _currState;
        if (_currState != null)
            _currState.Exit(_owner);
        _currState = nextState;
        if (nextState != null)
            nextState.Enter(_owner);

        if (debug)
            Debug.Log(string.Format("Enter State: {0}", nextState.GetType().ToString()));
    }

    public void RevertToPrevState()
    {
        if (_prevState != null)
            ChangeState(_prevState);
    }
}
