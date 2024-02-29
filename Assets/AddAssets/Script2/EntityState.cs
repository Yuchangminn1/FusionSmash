using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState 
{
    protected int currentStateNum;
    protected float stateTimer;
    protected float startTime;
    protected bool endMotionChange = false;    //�ִϸ��̼� Ʈ���� �ִ���
    protected bool isAbleFly = false;         
    protected bool isAbleAttack = true;
    protected bool isAbleDodge = true;
    protected bool isAbleJump = true;
    protected bool isState2=false;

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
