using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class StateMachine 
{
    
    private EntityState currentState;

    
    public void ChangeState(EntityState _newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = _newState;
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
    public void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }
}
