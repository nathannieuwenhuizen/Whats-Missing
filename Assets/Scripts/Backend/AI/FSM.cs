using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ILiveStateDelegate(IState _state);

///<summary>
/// Interface for a state that is used in the hirarirchal state machine.
///</summary>
public interface IState
{
    ///<summary>
    /// Delegate that fires when the state get switched for another
    ///</summary>
    ILiveStateDelegate OnStateSwitch { get; set; }
    ///<summary>
    /// Called at the first frame of the state when it is active.
    ///</summary>
    void Start();
    ///<summary>
    /// Gets called every frame the state is active.
    ///</summary>
    void Run();
    ///<summary>
    /// Called when the state gets deactivated or switches to another state.
    ///</summary>
    void Exit();
    ///<summary>
    /// Debugs the state
    ///</summary>
    void DrawDebug();
}

///<summary>
/// The ifnite state machine. 
///</summary>
public class FSM
{
    private IState cState;
    public FSM (IState _startState)
    {
        SwitchState(_startState);
    }

    public void Update()
    {
        cState?.Run();
    }

    public void Debug() {
        cState?.DrawDebug();
    }

    public void SwitchState( IState _newState)
    {
        if (cState != null)
        {
            cState.OnStateSwitch -= SwitchState;
            cState.Exit();
        }
        cState = _newState;

        cState.Start();
        cState.OnStateSwitch += SwitchState;

    }
}
