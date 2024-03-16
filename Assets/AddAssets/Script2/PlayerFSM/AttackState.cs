
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    int counter;
    Queue<bool> Combo;
    float attackDelay = 0.4f;
    public AttackState(PlayerStateHandler _player,  int _currentStateNum) : base(_player,  _currentStateNum)
    {
        endMotionChange = true;
        isAbleFly = true;
        isAbleAttack = true;
        isAbleJump = false;
        isState2 = true;
    }

    public override void Enter()
    {
        base.Enter();
        player.AttackEnter();
    }
    public override bool Update()
    {
        if (player.attackCount == player.maxAttackCount)
        {
            attackDelay = 0.8f;
        }
        else
        {
            attackDelay = 0.4f;
        }
        if(startTime + attackDelay > Time.time)
        {
            return false;
        }
        //if (!player.Isvisi())
        //{
        //    if (player.isFireButtonPressed)
        //    {
        //        //player.AnimationTrigger = false;
        //        player.nextState = player.attackState;
        //        player.ChangeState();
        //        return true;
        //    }
        //}
        return base.Update();
        
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.AttackExit();
    }
    private void AnimationTriggerErrorCheck()
    {
        if (startTime + 5f < Time.time)
        {
            //player.AnimationTrigger = false;
            
            Debug.Log("AnimationTrigger Error State =  " + player.GetCurrentState().currentState);
        }
        
    }
}
