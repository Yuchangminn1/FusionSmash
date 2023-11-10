using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState 
{
    protected int currentStateNum;
    protected float stateTimer;
    protected float startTime;
    protected bool endMotionChange = true;
    protected bool isAbleFly = false;
    protected bool isAbleAttack = true;
    protected bool isAbleDodge = true;


    public virtual void Enter()
    {
        startTime = Time.time;
    }
    public virtual void Update()
    {
        stateTimer = Time.time;
    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void Exit()
    {

    }
    


    



}
