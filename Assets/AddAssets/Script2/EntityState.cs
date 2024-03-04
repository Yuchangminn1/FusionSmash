using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState 
{
    protected int currentStateNum;
    public EntityState currentState { get;protected set; }
    protected float stateTimer;
    protected float startTime;
    protected bool endMotionChange = false;    
    protected bool isAbleFly = false;
    protected bool isState2 = false;
    public bool isAbleAttack { get; protected set; } = true;
    protected bool isAbleDodge = true;
    public bool isAbleJump { get; protected set; } = true;

    public virtual void Enter()
    {
        startTime = Time.time;
    }
    public virtual bool Update()
    {
        stateTimer = Time.time;
        return false;
    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void LateUpdate()
    {

    }

    public virtual void Exit()
    {

    }
    


    



}
